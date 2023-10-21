using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Assets.Scripts.Grasshopper_IO.Data;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Trainer : Agent
{
    [SerializeField]
    private bool _debug = false;
    [SerializeField]
    private bool _log = true;

    private int _token;

    private string _previousDataFromGh = "";
    private string _lastDataFromGh = "";
    private bool _dataFromGhIsChanged = false;
    private bool _isWaitingForResponse = false;
    private DataIn _ghData;

    private Point3d[] _candidates = new Point3d[6];
    private Point3d _spawnPoint = new Point3d();
    private Point3d _spawnRotation = new Point3d();
    private int _reset;

    // debug
    private int _stepCount = 0;

    void Start()
    {
        // start coroutine to get data from grasshopper
        StartCoroutine(WaitForGrasshopperDataAndUpdate());
        Init();
    }

    private void Init()
    {
        // initialize candidates
        for (int i = 0; i < _candidates.Length; i++)
        {
            _candidates[i] = new Point3d();  // This is the key step

            _candidates[i].X = 0;
            _candidates[i].Y = 0;
            _candidates[i].Z = 0;
        }
    }

    //void Update()
    //{
    //    print(_ghData.candidates[0].X);
    //}

    private IEnumerator WaitForGrasshopperDataAndUpdate()
    {
        while (true)
        {
            // get from grasshopper
            string dataFromGh = gameObject.GetComponent<Gh_IO>().msgFromGh;

            // check for message changes
            _previousDataFromGh = _lastDataFromGh;
            _lastDataFromGh = dataFromGh;
            if (_lastDataFromGh != _previousDataFromGh)
            {
                _dataFromGhIsChanged = true;
            }

            if (_dataFromGhIsChanged)
            {
                _ghData = JsonUtility.FromJson<DataIn>(dataFromGh);
                Academy.Instance.EnvironmentStep();
                _dataFromGhIsChanged = false;
                _isWaitingForResponse = false; // no longer waiting after processing data

                RequestDecision(); // Manually request a decision (and thus a step, required to disable decision requester)
            }

            yield return null;
        }
    }

    public override void OnEpisodeBegin()
    {
        _token = 5;

        _stepCount = 0;

        // reset parameters
        _spawnPoint.X = 0;
        _spawnPoint.Y = 0;
        _spawnPoint.Z = 0;
        _spawnRotation.X = 0;
        _spawnRotation.Y = 0;
        _reset = 1;

        // populate candidates
        foreach (Point3d candidate in _candidates)
        {
            candidate.X = 0;
            candidate.Y = 0;
            candidate.Z = 0;
        }
        DataOut dataOut = new DataOut(0,0,0,0,0,_reset);
        string msg = dataOut.Serialize();
        // send to grasshopper
        gameObject.GetComponent<Gh_IO>().msgToGh = msg;

        if (_debug || _log) print($"reset, msg: {msg}");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (_ghData == null) return;
        Observation obs = _ghData.observation;

        // bounding box dimension (3)
        sensor.AddObservation(obs.bBox.X);
        sensor.AddObservation(obs.bBox.Y);
        sensor.AddObservation(obs.bBox.Z);

        // other parameters (4)
        sensor.AddObservation(obs.aggregationDiff);
        sensor.AddObservation(obs.density);
        sensor.AddObservation(obs.maxStress);
        sensor.AddObservation(obs.maxDisplacement);

        // near distance (6)
        sensor.AddObservation(obs.nearDistance[0]);
        sensor.AddObservation(obs.nearDistance[1]);
        sensor.AddObservation(obs.nearDistance[2]);
        sensor.AddObservation(obs.nearDistance[3]);
        sensor.AddObservation(obs.nearDistance[4]);
        sensor.AddObservation(obs.nearDistance[5]);

        // candidate map (6)
        sensor.AddObservation(obs.candidatesMap[0]);
        sensor.AddObservation(obs.candidatesMap[1]);
        sensor.AddObservation(obs.candidatesMap[2]);
        sensor.AddObservation(obs.candidatesMap[3]);
        sensor.AddObservation(obs.candidatesMap[4]);
        sensor.AddObservation(obs.candidatesMap[5]);

        if (_debug)
        {
            // debug
            string msg = $"bbox({obs.bBox.X},{obs.bBox.Y},{obs.bBox.Z}), \n" +
                         $"near({obs.nearDistance[0]},{obs.nearDistance[1]},{obs.nearDistance[2]},{obs.nearDistance[3]},{obs.nearDistance[4]},{obs.nearDistance[5]}), \n" +
                         $"map({obs.candidatesMap[0]},{obs.candidatesMap[1]},{obs.candidatesMap[2]},{obs.candidatesMap[3]},{obs.candidatesMap[4]},{obs.candidatesMap[5]}), \n" +
                         $"agg({obs.aggregationDiff}),density({obs.density}),stress({obs.maxStress}),displace({obs.maxDisplacement})";

            print($"OBSERVE [step: {_stepCount}, msg: {msg}]");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isWaitingForResponse) return;
        _reset = 0;
        // remap rotation to 0-360
        _spawnRotation.X = actions.ContinuousActions[0] * 180;
        _spawnRotation.Y = actions.ContinuousActions[1] * 180;
        if (_ghData != null)
        {
            // set candidates
            for (int i = 0; i < _candidates.Length; i++)
            {
                _candidates[i].X = _ghData.candidates[i].X;
                _candidates[i].Y = _ghData.candidates[i].Y;
                _candidates[i].Z = _ghData.candidates[i].Z;
            }
        }
        else
        {
            // set candidates
            foreach (var t in _candidates)
            {
                t.X = 0;
                t.Y = 0;
                t.Z = 0;
            }
        }
        

        // get candidates
        switch (actions.DiscreteActions[0])
        {
            case 0:
                _spawnPoint.X = _candidates[0].X;
                _spawnPoint.Y = _candidates[0].Y;
                _spawnPoint.Z = _candidates[0].Z;
                break;
            case 1:
                _spawnPoint.X = _candidates[1].X;
                _spawnPoint.Y = _candidates[1].Y;
                _spawnPoint.Z = _candidates[1].Z;
                break;
            case 2:
                _spawnPoint.X = _candidates[2].X;
                _spawnPoint.Y = _candidates[2].Y;
                _spawnPoint.Z = _candidates[2].Z;
                break;
            case 3:
                _spawnPoint.X = _candidates[3].X;
                _spawnPoint.Y = _candidates[3].Y;
                _spawnPoint.Z = _candidates[3].Z;
                break;
            case 4:
                _spawnPoint.X = _candidates[4].X;
                _spawnPoint.Y = _candidates[4].Y;
                _spawnPoint.Z = _candidates[4].Z;
                break;
            case 5:
                _spawnPoint.X = _candidates[5].X;
                _spawnPoint.Y = _candidates[5].Y;
                _spawnPoint.Z = _candidates[5].Z;
                break;
            default:
                _spawnPoint.X = 0;
                _spawnPoint.Y = 0;
                _spawnPoint.Z = 0;
                break;
        }
        
        DataOut dataOut = new DataOut(_spawnPoint, _spawnRotation, _reset);
        string msg = dataOut.Serialize();

        _isWaitingForResponse = true;
        // send to grasshopper
        gameObject.GetComponent<Gh_IO>().msgToGh = msg;
        if (_ghData != null) AddReward(_ghData.score);

        if (_token <= 0)
        {
            EndEpisode();
        }
        _token--;
        _stepCount++;
        if (_debug || _log)
        {
            print($"ACTION [Step: {_stepCount}, msg: {msg}]");
        }
    }

    // debug
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
    }
}

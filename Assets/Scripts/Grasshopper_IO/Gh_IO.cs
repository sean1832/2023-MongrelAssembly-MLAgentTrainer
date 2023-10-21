using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using UnityEditor.PackageManager;
using System.Text;
using System;

public class Gh_IO : MonoBehaviour
{
    //[SerializeField] private bool send;
    [SerializeField] private string IP = "127.0.0.1";
    [SerializeField] private int sendPort = 6000;
    [SerializeField] private int receivePort = 6001;

    private IPEndPoint _endPoint;
    private UdpClient _client;


    // receiver fields
    private bool _messageChanged = false;
    private Thread receiveThread;
    private string lastReceivedMessage = "";
    private string previousMessage = "";


    // public fields
    public string msgToGh;
    public string msgFromGh;



    // Start is called before the first frame update
    void Start()
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(IP), sendPort);
        _client = new UdpClient();

        // UDP sender status
        print("Sending to " + IP + " : " + sendPort);

        // create a thread for receiving UDP message
        receiveThread = new Thread(new ThreadStart(Receiver));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // UDP receiver status
        print("receiving from " + IP + " : " + receivePort);
    }

    // Update is called once per frame
    void Update()
    {
        UdpServer.Sender(msgToGh, _endPoint, _client);


        if (_messageChanged)
        {
            //print($"Receiving message: {lastReceivedMessage}");
            msgFromGh = lastReceivedMessage;
            _messageChanged = false;
        }
    }

    // Exit thread when stop playing
    void OnApplicationQuit()
    {
        stopThread();
    }

    private void stopThread()
    {
        if (receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        _client.Close();
    }

    public void Receiver()
    {
        UdpClient client = new UdpClient(receivePort);
        
        while (true)
        {
            try
            {
                // receive bytes
                IPEndPoint IP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref IP);

                // decode
                string message = Encoding.UTF8.GetString(data);

                // manage changes, only update if change happens, else do nothing
                previousMessage = lastReceivedMessage;
                lastReceivedMessage = message;
                if (lastReceivedMessage != previousMessage)
                {
                    _messageChanged = true;

                    //// show message
                    //print($"received message: [{message}]");
                }
            }
            catch (Exception ex)
            {
                print($"UDP Receiver error: {ex.ToString()}");
            }
        }
    }

}

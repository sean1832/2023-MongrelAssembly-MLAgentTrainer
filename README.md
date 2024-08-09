# 2023-MongrelAssembly-MLAgentTrainer

## Pre-requisites
- Python 3.9.13 (Must be exactly 3.9.x)
- Unity3D 2022.3.40f1 LTS
  - ML-Agents 2.0.1

## Installation

```bash
python -m venv venv
./venv/Scripts/activate
python -m pip install --upgrade pip
pip install mlagents
pip install torch torchvision torchaudio
pip install protobuf==3.20.3
```

To test the installation, run the following command:
```bash
mlagents-learn --help
```

To Start training, run:
```bash
mlagents-learn --run-id test01
```

## Tutorials
- [Machine Learning AI in Unity | Code Monkey | Youtube](https://www.youtube.com/watch?v=zPFU30tbyKs&list=PLzDRvYVwl53vehwiN_odYJkPBzcqFw110)

## ML-Agents Documentation
- [ML-Agents-Toolkit-Documentation | Github Page](https://unity-technologies.github.io/ml-agents/ML-Agents-Toolkit-Documentation/)

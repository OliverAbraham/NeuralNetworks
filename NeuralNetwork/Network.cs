﻿using Newtonsoft.Json;

namespace NeuralNetwork
{
    public class Network
    {
        #region ------------- Internal types ------------------------------------------------------
        public delegate void TrainingProgressHandler(float[] output, string statusText, byte[] currentTrainingImage, int currentTrainingImageIndex, int totalAccuracy);
        public delegate void TrainingFinishedHandler(string statusText);

        private class Callbacks
        {
            public TrainingProgressHandler OnProgress;
            public TrainingFinishedHandler OnFinished;

            public Callbacks(TrainingProgressHandler onProgress, TrainingFinishedHandler onFinished)
            {
                OnProgress = onProgress;
                OnFinished = onFinished;
            }
        }
        #endregion



        #region ------------- Properties ----------------------------------------------------------
        // training data
        public int      TrainingImageSize      { get; private set; } = 0;
        public int      TrainingImageCount     { get; private set; } = 0;
                                               
        // Network structure                   
        public int      NeuronsInInputLayer    => _structure.NeuronsInInputLayer;  
        public int      HiddenLayersCount      => _structure.HiddenLayersCount;    
        public int      NeuronsInHiddenLayers  => _structure.NeuronsInHiddenLayers;
        public int      NeuronsInOutputLayers  => _structure.NeuronsInOutputLayers; 
                                               
        // Training                            
        public float    TrainingSpeed          { get; set; } = 0.0005F;
        public int      StopAfterIterations    { get; set; }
        public int      StopAfterAccurracy     { get; set; }
        public int      TotalTrainingIterations => _structure.TotalTrainingIterations;
        public bool     IsInitialized => _structure is not null;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Structure _structure;
        
        // training data
        private byte[][] _trainingImages;
        private byte[]   _trainingLabels;
        
        private bool     _trainingInProgress;
        private int      _numberOfGoodOutputs = 0;// kleine Statistik um die Akkuratheit zu messen
        private Thread   _trainingThread;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public bool StartLoadingTrainingData(string trainingDataDirectory, Action onLoadFinished, ref string messages)
        {
            var trainingFiles = new string[4];
            trainingFiles[0] = @"train-images.idx3-ubyte";
            trainingFiles[1] = @"train-labels.idx1-ubyte";
            trainingFiles[2] = @"t10k-images.idx3-ubyte";
            trainingFiles[3] = @"t10k-labels.idx1-ubyte";

            messages = "";
            foreach (var file in trainingFiles)
            {
                var fullFilename = Path.Combine(trainingDataDirectory, @"train-images.idx3-ubyte");
                if (!File.Exists(fullFilename))
                {
                    messages += $"Cannot find {fullFilename}\n";
                    return false;
                }
            }

            Thread loadThread = new Thread(new ThreadStart(loadData));
            loadThread.Start();

            //_loadingInProgress = true;

            void loadData()
            {
                //if (_loadingInProgress)
                //{
                    TrainingImageCount = 60000;
                    TrainingImageSize = 28;
                    _trainingImages = MnistTrainingDataLoader.LoadImageFile(Path.Combine(trainingDataDirectory, trainingFiles[0]), TrainingImageCount);
                    _trainingLabels = MnistTrainingDataLoader.LoadLabelFile(Path.Combine(trainingDataDirectory, trainingFiles[1]), TrainingImageCount);
                //}
                //else
                //{
                //    _trainingIterations = 10000;
                //    _trainingImages = MnistTrainingDataLoader.LoadImageFile(Path.Combine(_trainingDataDirectory, trainingFiles[2]), _trainingIterations);
                //    _trainingLabels = MnistTrainingDataLoader.LoadLabelFile(Path.Combine(_trainingDataDirectory, trainingFiles[3]), _trainingIterations);
                //}

                onLoadFinished();
            }

            return true;
        }

        public byte[] GetTrainingImageById(int id)
        {
            return _trainingImages[id % TrainingImageCount];
        }

        public void Initialize(int neuronsInInputLayer, int hiddenLayersCount, int neuronsInHiddenLayers, int neuronsInOutputLayer)
        {
            _structure = new Structure(neuronsInInputLayer, hiddenLayersCount, neuronsInHiddenLayers, neuronsInOutputLayer,
                -0.5F, 0.5F, false, Neuron.ReLU);
        }

        public void StartTraining(TrainingProgressHandler onProgress, TrainingFinishedHandler onFinished)
        {
            var callbacks = new Callbacks(onProgress, onFinished);

            if (_trainingThread is null)
                _trainingThread = new Thread(new ParameterizedThreadStart(Training));
            _trainingThread.Start(callbacks);
        }

        public void CancelTraining()
        {
            _trainingInProgress = false;
        }

        public void SaveNetwork(string dataDirectory)
        {
            var path = Path.Combine(dataDirectory, "NetworkStructure.json");
            var serializedStructure = JsonConvert.SerializeObject(_structure);
            File.WriteAllText(path, serializedStructure);
        }

        public void LoadNetwork(string dataDirectory)
        {
            var fullFilename = Path.Combine(dataDirectory, "NetworkStructure.json");
            var serializedStructure = File.ReadAllText(fullFilename);
            var data = (Structure)JsonConvert.DeserializeObject<Structure>(serializedStructure);
            
            _structure = new Structure(data.NeuronsInInputLayer, data.HiddenLayersCount, data.NeuronsInHiddenLayers, data.NeuronsInOutputLayers,
                -0.5F, 0.5F, false, Neuron.ReLU);

            _structure.CopyWeightsAndBiasesFrom(data);
        }

        public int GetOutputClassification(float[] networkOutputs)
        {
            int num = 0;
            float highestFloat = 0.0F;

            for (int i = 0; i < networkOutputs.Length; i++)
            {
                if (networkOutputs[i] > highestFloat)
                {
                    highestFloat = networkOutputs[i];
                    num = i;
                }
            }

            return num;
        }

        public float[] Think(byte[] image)
        {
            if (_structure is null)
                throw new Exception("brain is not initialized");
            return _structure.Think(MnistTrainingDataLoader.ByteToFloat(image));
        }

        public float[] Think(float[] inputValues)
        {
            if (_structure is null)
                throw new Exception("brain is not initialized");
            return _structure.Think(inputValues);
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private void Training(object data)
        {
            TrainingProgressHandler onProgress = ((Callbacks)data).OnProgress;
            TrainingFinishedHandler onFinished = ((Callbacks)data).OnFinished;
            var iteration = 0;
            var totalSuccess = 0;
            int totalAccuracy = 0;
            int calculateAccuracyEvery = 100;

            _trainingInProgress = true;
            while (_trainingInProgress)
            {
                var currentTrainingImageIndex = iteration % TrainingImageCount;
                var currentTrainingImage = _trainingImages[currentTrainingImageIndex];
                var expectedOutput       = _trainingLabels[currentTrainingImageIndex];

                float[] output = _structure.Think(MnistTrainingDataLoader.ByteToFloat(currentTrainingImage));
                var currentOutput = GetOutputClassification(output);
                float cost = CalculateCost(output, expectedOutput);

                _structure.Backpropagate(TrainingSpeed, currentOutput, expectedOutput);

                // Normalerweise trennt man die Prüfdaten von den Trainingsdaten,
                // um zu gucken ob das Netz nicht nur auswendig lernt, sondern auch generalisiert
                if (iteration % calculateAccuracyEvery == 0)
                    _numberOfGoodOutputs = 0; //reset statistic every "calculateAccuracyEvery" steps

                if (expectedOutput == currentOutput)
                {
                    totalSuccess++;
                    _numberOfGoodOutputs++;
                }

                double accuracy = _numberOfGoodOutputs / (iteration % calculateAccuracyEvery + 1.0);
                int percent = (int)(100 * accuracy);
                totalAccuracy = Convert.ToInt32(totalSuccess * 100 / (iteration + 1));

                _structure.TotalTrainingIterations++;
                iteration++;

                if ((iteration % 100) == 0)
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration, currentTrainingImage, currentTrainingImageIndex);

                if (StopAfterIterations > 0 && iteration >= StopAfterIterations-1)
                {
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration, currentTrainingImage, currentTrainingImageIndex);
                    break;
                }
                if (StopAfterAccurracy > 0 && iteration >= calculateAccuracyEvery & totalAccuracy >= StopAfterAccurracy)
                {
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration, currentTrainingImage, currentTrainingImageIndex);
                    break;
                }
            }

            totalSuccess = 0;

            var currentStatus = $"Training finished after {iteration,6} iterations. Total accuracy: {totalAccuracy,4}%        ";
            onFinished(currentStatus);

            _trainingInProgress = false;
            _trainingThread = null;
        }

        private void UpdateUIWhileTraining(TrainingProgressHandler onProgress, float[] currentOutput, float cost, int percent, int totalAccuracy, int count, byte[] currentTrainingImage, int currentTrainingImageIndex)
        {
            var currentStatus = $"accuracy: {percent,4}%   cost: {Math.Round(cost, 1),4}   \ntotal accuracy: {totalAccuracy,4}%        ";
            onProgress(currentOutput, currentStatus, currentTrainingImage, currentTrainingImageIndex, totalAccuracy);
        }

        private float CalculateCost(float[] networkOutputs, int targetOutput)
        {
            float cost = 0.0F;

            float[] targetOutputs = new float[networkOutputs.Length];
            targetOutputs[targetOutput] = 1.0F;

            for (int i = 0; i < networkOutputs.Length; i++)
            {
                cost += Math.Max(0, networkOutputs[i]) - targetOutputs[i];
            }

            return cost;
        }

        //private float[][][] GetSumOfChanges(float[][][] total, float[][][] current)
        //{
        //    float[][][] sum = new float[total.Length][][];
        //
        //    for (int layerNum = 0; layerNum < total.Length; layerNum++)
        //    {
        //        sum[layerNum] = new float[total[layerNum].Length][];
        //
        //        for (int neuronNum = 0; neuronNum < total[layerNum].Length; neuronNum++)
        //        {
        //            sum[layerNum][neuronNum] = new float[total[layerNum][neuronNum].Length];
        //
        //            for (int weightNum = 0; weightNum < total[layerNum][neuronNum].Length; weightNum++)
        //            {
        //                sum[layerNum][neuronNum][weightNum] = total[layerNum][neuronNum][weightNum] + current[layerNum][neuronNum][weightNum];
        //            }
        //        }
        //    }
        //
        //    return sum;
        //}
        //
        //private void ApplyChanges(Brain brain, float[][][][] changes)
        //{
        //    Neuron[][] layers = new Neuron[brain.HiddenLayers.Length + 1][];
        //    Array.Copy(brain.AllLayers, 1, layers, 0, brain.AllLayers.Length - 1);
        //
        //    for (int layerNum = 0; layerNum < layers.Length; layerNum++)
        //    {
        //        Neuron[] layer = layers[layerNum];
        //
        //        for (int neuronNum = 0; neuronNum < layer.Length; neuronNum++)
        //        {
        //            Neuron neuron = layer[neuronNum];
        //
        //            for (int weightNum = 0; weightNum < neuron.Weights.Length; weightNum++)
        //            {
        //                float change = 0.0F;
        //
        //                for (int i = 0; i < changes.Length; i++)
        //                {
        //                    change += changes[i][layerNum][neuronNum][weightNum];
        //                }
        //                change = change / changes.Length;
        //
        //                neuron.Weights[weightNum] += change;
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
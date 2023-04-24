using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace NeuralNetwork
{
    public class Network
    {
        #region ------------- Internal types ------------------------------------------------------
        public delegate void TrainingProgressHandler(float[] output, string statusText, byte[] currentTrainingImage, int currentTrainingImageIndex, int totalAccuracy);
        public delegate void TrainingFinishedHandler(string statusText);

        private class Callbacks
        {
            public ITrainingDataManager TrainingDataManager;
            public TrainingProgressHandler OnProgress;
            public TrainingFinishedHandler OnFinished;

            public Callbacks(ITrainingDataManager trainingDataManager, TrainingProgressHandler onProgress, TrainingFinishedHandler onFinished)
            {
                TrainingDataManager = trainingDataManager;
                OnProgress = onProgress;
                OnFinished = onFinished;
            }
        }
        #endregion



        #region ------------- Properties ----------------------------------------------------------
                                              
        // Network structure                   
        public bool     IsInitialized          => _structure is not null;
        public int      NeuronsInInputLayer    => _structure.NeuronsInInputLayer;  
        public int      HiddenLayersCount      => _structure.HiddenLayersCount;    
        public int      NeuronsInHiddenLayers  => _structure.NeuronsInHiddenLayers;
        public int      NeuronsInOutputLayers  => _structure.NeuronsInOutputLayers; 
                                               
        // Training                            
        public float    TrainingSpeed          { get; set; } = 0.0005F;
        public int      StopAfterIterations    { get; set; }
        public int      StopAfterAccurracy     { get; set; }
        public int      TotalTrainingIterations => _structure.TotalTrainingIterations;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Structure _structure;
        private bool     _trainingInProgress;
        private int      _numberOfGoodOutputs = 0;// kleine Statistik um die Akkuratheit zu messen
        private Thread   _trainingThread;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public void Initialize(int neuronsInInputLayer, int hiddenLayersCount, int neuronsInHiddenLayers, int neuronsInOutputLayer)
        {
            _structure = new Structure(neuronsInInputLayer, hiddenLayersCount, neuronsInHiddenLayers, neuronsInOutputLayer,
                -0.5F, 0.5F, false, Neuron.ReLU);
        }

        public void StartTraining(ITrainingDataManager trainingDataManager, TrainingProgressHandler onProgress, TrainingFinishedHandler onFinished)
        {
            var callbacks = new Callbacks(trainingDataManager, onProgress, onFinished);

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

        public int GetClassification(float[] networkOutputs)
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
            ITrainingDataManager    trainingDataManager = ((Callbacks)data).TrainingDataManager;
            TrainingProgressHandler onProgress          = ((Callbacks)data).OnProgress;
            TrainingFinishedHandler onFinished          = ((Callbacks)data).OnFinished;
            var iteration = 0;
            var totalSuccess = 0;
            int totalAccuracy = 0;
            int calculateAccuracyEvery = 100;

            var stopwatch = Stopwatch.StartNew();
            _trainingInProgress = true;
            while (_trainingInProgress)
            {
                (var currentTrainingImage, var expectedOutput) = trainingDataManager.GetRandomTrainingImageAndLabel();

                var outputs = _structure.Think(MnistTrainingDataLoader.ByteToFloat(currentTrainingImage));
                var currentOutput = GetClassification(outputs);
                
                var costs = CalculateCostVector(outputs, expectedOutput);
                var cost = CalculateTotalCost(costs);

                if (cost > 0.2f)
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
                {
                    var elapsed = stopwatch.ElapsedMilliseconds;
                    UpdateUIWhileTraining(onProgress, outputs, cost, percent, totalAccuracy, iteration, currentTrainingImage, 0);
                    stopwatch.Restart();
                }

                if (StopAfterIterations > 0 && iteration >= StopAfterIterations-1)
                {
                    UpdateUIWhileTraining(onProgress, outputs, cost, percent, totalAccuracy, iteration, currentTrainingImage, 0);
                    break;
                }
                if (StopAfterAccurracy > 0 && iteration >= calculateAccuracyEvery & totalAccuracy >= StopAfterAccurracy)
                {
                    UpdateUIWhileTraining(onProgress, outputs, cost, percent, totalAccuracy, iteration, currentTrainingImage, 0);
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

        private float[] CalculateCostVector(float[] currentOutputs, int expectedOutputClassification)
        {
            var costs = new float[currentOutputs.Length];

            var expectedOutputs = new float[currentOutputs.Length];
            expectedOutputs[expectedOutputClassification] = 1.0F;

            for (int i = 0; i < currentOutputs.Length; i++)
            {
                if (currentOutputs[i] < 1f)
                    costs[i] += expectedOutputs[i] - currentOutputs[i];
            }

            return costs;
        }

        private float CalculateTotalCost(float[] costs)
        {
            return costs.Select(x => Math.Abs(x)).Sum();
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
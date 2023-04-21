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
        public int      NeuronsInInputLayer    { get; set; } = 28 * 28;
        public int      HiddenLayersCount      { get; set; } = 3;
        public int      NeuronsInHiddenLayers  { get; set; } = 16;
        public int      NeuronsInOutputLayer   { get; set; } = 10;
                                               
        // Training                            
        public float    TrainingSpeed          { get; set; } = 0.0005F;
        public int      StopAfterIterations    { get; set; }
        public int      StopAfterAccurracy     { get; set; }
        public int      TotalTrainingIterations => _brain.TotalTrainingIterations;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Brain    _brain;
        
        // training data
        private byte[][] _trainingImages;
        private byte[]   _trainingLabels;
        
        private bool     _trainingInProgress;
        private bool     _trainingDataLoadInProgress;
        private int      _trainingStateIsGood = 0;// kleine Statistik um die Akkuratheit zu messen
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

            _trainingDataLoadInProgress = true;
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

                _trainingDataLoadInProgress = false;
                onLoadFinished();
            }

            return true;
        }

        public byte[] GetTrainingImageById(int id)
        {
            return _trainingImages[id % TrainingImageCount];
        }

        public void Initialize()
        {
            _brain = new Brain(NeuronsInInputLayer, NeuronsInOutputLayer, HiddenLayersCount, NeuronsInHiddenLayers, 
                -0.3F, 0.3F, false, Neuron.ReLU);
        }

        public void StartTraining(TrainingProgressHandler onProgress, TrainingFinishedHandler onFinished)
        {
            if (_brain is null)
                Initialize();

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
            var path = Path.Combine(dataDirectory, "NetworkStructure.bin");
            _brain.SaveNetworkToFile(path);

            //path = Path.Combine(dataDirectory, @"save.brain");
            //File.Create(path).Close();
            //File.WriteAllText(path, _brain.SerializeNetwork());
        }

        public void LoadNetwork(string dataDirectory)
        {
            var fullFilename = Path.Combine(dataDirectory, "NetworkStructure.bin");
            LoadNetworkFromFile(fullFilename);
        }

        private void LoadNetworkFromFile(string dataDirectory)
        {
            var fullFilename = Path.Combine(dataDirectory, "NetworkStructure.bin");
            _brain = new Brain(fullFilename);
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

        public float[] Think(float[] inputValues)
        {
            return _brain.Think(inputValues, Neuron.ReLU);
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

            _trainingInProgress = true;
            while (_trainingInProgress)
            {
                var currentTrainingImageIndex = iteration % TrainingImageCount;
                var currentTrainingImage = _trainingImages[currentTrainingImageIndex];
                var currentTrainingLabel = _trainingLabels[currentTrainingImageIndex];

                float[] output = _brain.Think(MnistTrainingDataLoader.ByteToFloat(currentTrainingImage), Neuron.ReLU);
                var outputNum = GetOutputClassification(output);
                float cost = CalculateCost(output, currentTrainingLabel);
                Backpropagate(_brain, TrainingSpeed, outputNum, currentTrainingLabel);
                int expected = currentTrainingLabel;

                // Normalerweise trennt man die Prüfdaten von den Trainingsdaten,
                // um zu gucken ob das Netz nicht nur auswendig lernt, sondern auch generalisiert
                if (iteration % 1000 == 0)
                    _trainingStateIsGood = 0; //reset statistic every 1000 steps
                if (expected == outputNum)
                {
                    totalSuccess++;
                    _trainingStateIsGood++;
                }

                double accuracy = _trainingStateIsGood / (iteration % 1000 + 1.0);
                int percent = (int)(100 * accuracy);
                totalAccuracy = Convert.ToInt32(totalSuccess * 100 / (iteration + 1));

                _brain.TotalTrainingIterations++;
                iteration++;

                if ((iteration % 100) == 0)
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration, currentTrainingImage, currentTrainingImageIndex);

                if (StopAfterIterations > 0 && iteration >= StopAfterIterations-1)
                {
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration, currentTrainingImage, currentTrainingImageIndex);
                    break;
                }
                if (StopAfterAccurracy > 0 && iteration >= 1000 & totalAccuracy >= StopAfterAccurracy)
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

        private float[][][] GetSumOfChanges(float[][][] total, float[][][] current)
        {
            float[][][] sum = new float[total.Length][][];

            for (int layerNum = 0; layerNum < total.Length; layerNum++)
            {
                sum[layerNum] = new float[total[layerNum].Length][];

                for (int neuronNum = 0; neuronNum < total[layerNum].Length; neuronNum++)
                {
                    sum[layerNum][neuronNum] = new float[total[layerNum][neuronNum].Length];

                    for (int weightNum = 0; weightNum < total[layerNum][neuronNum].Length; weightNum++)
                    {
                        sum[layerNum][neuronNum][weightNum] = total[layerNum][neuronNum][weightNum] + current[layerNum][neuronNum][weightNum];
                    }
                }
            }

            return sum;
        }

        private void ApplyChanges(Brain brain, float[][][][] changes)
        {
            Neuron[][] layers = new Neuron[brain.HiddenLayers.Length + 1][];
            Array.Copy(brain.AllLayers, 1, layers, 0, brain.AllLayers.Length - 1);

            for (int layerNum = 0; layerNum < layers.Length; layerNum++)
            {
                Neuron[] layer = layers[layerNum];

                for (int neuronNum = 0; neuronNum < layer.Length; neuronNum++)
                {
                    Neuron neuron = layer[neuronNum];

                    for (int weightNum = 0; weightNum < neuron.Weight.Length; weightNum++)
                    {
                        float change = 0.0F;

                        for (int i = 0; i < changes.Length; i++)
                        {
                            change += changes[i][layerNum][neuronNum][weightNum];
                        }
                        change = change / changes.Length;

                        neuron.Weight[weightNum] += change;
                    }
                }
            }
        }

        private float[][][] Backpropagate(Brain brain, float TweakAmount, int outputNum, int expectedNum)
        {
            Neuron[][] layers = new Neuron[brain.HiddenLayers.Length + 1][];
            Array.Copy(brain.AllLayers, 1, layers, 0, brain.AllLayers.Length - 1);
            float[] targetOutput = new float[10];
            targetOutput[expectedNum] = 1.0F;

            float[][][] allChanges = new float[layers.Length][][];

            for (int layerNum = layers.Length - 1; layerNum >= 0; layerNum--)
            {
                Neuron[] layer = layers[layerNum];
                float[][] neuronChanges = new float[layer.Length][];

                for (int neuronNum = 0; neuronNum < layer.Length; neuronNum++)
                {
                    Neuron neuron = layer[neuronNum];
                    float[] weightChanges = new float[neuron.Weight.Length];

                    for (int i = 0; i < neuron.Weight.Length; i++)
                    {
                        float deltaW = 0.0F;
                        if (neuron.Type == Neuron.NeuronType.HiddenNeuron)
                        {
                            float delta_i = Delta_i(layers, layerNum, neuronNum, targetOutput);
                            float activation_j = brain.AllLayers[layerNum][i].Activation;

                            deltaW = DeltaW(TweakAmount, delta_i, activation_j);
                        }
                        else
                        {
                            float delta_i = Delta_i(neuron, targetOutput[neuronNum], Neuron.ReLU(neuron.Activation));
                            float activation_j = brain.AllLayers[layerNum][i].Activation;
                            deltaW = DeltaW(TweakAmount, delta_i, activation_j);
                        }

                        if (!float.IsNaN(deltaW))
                        {
                            weightChanges[i] = deltaW;
                            neuron.Weight[i] += deltaW;
                        }
                    }
                    neuronChanges[neuronNum] = weightChanges;
                }
                allChanges[layerNum] = neuronChanges;
            }

            return allChanges;
        }

        private float DeltaW(float tweakAmount, float delta_i, float activation_j)
        {
            return tweakAmount * delta_i * activation_j;
        }

        private float Delta_i(Neuron neuron, float targetOutput, float currentOutput)
        {
            neuron.Delta_i = derivative_ReLU(neuron.Activation) * (targetOutput - currentOutput);
            return neuron.Delta_i;
        }

        private float Delta_i(Neuron[][] layers, int targetLayer, int targetNeuron, float[] targetOutput)
        {
            Neuron neuron = layers[targetLayer][targetNeuron];
            int prevLayer = targetLayer + 1;

            float sum = 0.0F;
            for (int i = 0; i < layers[prevLayer].Length; i++)
            {
                Neuron prevNeuron = layers[prevLayer][i];
                sum += prevNeuron.Delta_i * prevNeuron.Weight[targetNeuron];
            }
            neuron.Delta_i = derivative_ReLU(neuron.Activation) * sum;

            return neuron.Delta_i;
        }

        private float derivative_tanh(float x)
        {
            return (float)((1 - Math.Tanh(x)));
        }

        private float derivative_ReLU(double x)
        {
            if(x > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
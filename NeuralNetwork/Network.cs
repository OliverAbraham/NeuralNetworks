namespace NeuralNetwork
{
    public class Network
    {
        #region ------------- Internal types ------------------------------------------------------
        internal class Callbacks
        {
            public Action OnProgress;
            public Action OnFinished;

            public Callbacks(Action onProgress, Action onFinished)
            {
                OnProgress = onProgress;
                OnFinished = onFinished;
            }
        }
        #endregion



        #region ------------- Properties ----------------------------------------------------------
        public int      _imageSize             { get; private set; } = 28;
        public int      _neuronsInInputLayer   { get; private set; } = 28 * 28;
        public int      _hiddenLayersCount     { get; set; } = 3;
        public int      _neuronsInHiddenLayers { get; set; } = 16;
        public int      _neuronsInOutputLayer  { get; private set; } = 10;
        public float    _trainingSpeed         { get; set; } = 0.0005F;

        public Brain    _brain;
        public byte[][] _trainingImages;
        public byte[]   _trainingLabels;
        public bool     _trainingDataLoadInProgress;
        public bool     _trainingInProgress;
        public int      _trainingStateIsGood = 0;// kleine Statistik um die Akkuratheit zu messen
        public int      _stopAfterIterations;
        public int      _stopAfterAccurracy;
        public int      _trainingIterations;
        public int      _totalTrainingIterations;
        public float[]  _currentOutput;
        public string   _currentStatus;
        public byte[]   _currentImage;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
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
            _brain = new Brain(_neuronsInInputLayer, _neuronsInOutputLayer, _hiddenLayersCount, _neuronsInHiddenLayers, -0.5F, 0.5F, false, Neuron.ReLU);

            Thread loadThread = new Thread(new ThreadStart(loadData));
            loadThread.Start();

            //_loadingInProgress = true;

            void loadData()
            {
                //if (_loadingInProgress)
                //{
                    _trainingIterations = 60000;
                    _trainingImages = MnistTrainingDataLoader.LoadImageFile(Path.Combine(trainingDataDirectory, trainingFiles[0]), _trainingIterations);
                    _trainingLabels = MnistTrainingDataLoader.LoadLabelFile(Path.Combine(trainingDataDirectory, trainingFiles[1]), _trainingIterations);
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

        public void StartTraining(Action onProgress, Action onFinished)
        {
            var callbacks = new Callbacks(onProgress, onFinished);

            if (_trainingThread is null)
                _trainingThread = new Thread(new ParameterizedThreadStart(Training));
            _trainingThread.Start(callbacks);
        }

        private void Training(object data)
        {
            Action onProgress = ((Callbacks)data).OnProgress;
            Action onFinished = ((Callbacks)data).OnFinished;
            var iteration = 0;
            var totalSuccess = 0;
            int totalAccuracy = 0;

            _trainingInProgress = true;
            while (_trainingInProgress)
            {
                float[] output = _brain.Think(MnistTrainingDataLoader.ByteToFloat(_trainingImages[iteration]), Neuron.ReLU);
                var outputNum = GetOutputClassifcation(output);
                float cost = CalculateCost(output, _trainingLabels[iteration]);
                Backpropagate(_brain, _trainingSpeed, outputNum, _trainingLabels[iteration]);
                int expected = _trainingLabels[iteration];

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

                if ((iteration % 100) == 0)
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration);

                iteration++;
                if (_stopAfterIterations > 0 && iteration >= _stopAfterIterations-1)
                {
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration);
                    break;
                }
                if (_stopAfterAccurracy > 0 && iteration >= 1000 & totalAccuracy >= _stopAfterAccurracy)
                {
                    UpdateUIWhileTraining(onProgress, output, cost, percent, totalAccuracy, iteration);
                    break;
                }
            }

            _totalTrainingIterations = iteration;
            totalSuccess = 0;

            _currentStatus = $"Training finished after {iteration,6} iterations. Total accuracy: {totalAccuracy,4}%        ";
            onFinished();

            _trainingInProgress = false;
            _trainingThread = null;
        }

        private void UpdateUIWhileTraining(Action onProgress, float[] output, float cost, int percent, int totalAccuracy, int count)
        {
            _currentOutput = output;
            _currentStatus = $"accuracy: {percent,4}%   cost: {Math.Round(cost, 1),4}   total accuracy: {totalAccuracy,4}%   iterations: {count,6} of {_stopAfterIterations,6}        ";
            _currentImage = _trainingImages[count];
            onProgress();
        }

        public void CancelTraining()
        {
            _trainingInProgress = false;
        }

        public void SaveBrainStructure(string dataDirectory)
        {
            string path = Path.Combine(dataDirectory, @"save.brainStream");
            FileStream fstream = new FileStream(path, FileMode.Create);
            MemoryStream stream = _brain.BuildStructureStream();
            stream.WriteTo(fstream);
            fstream.Close();

            path = Path.Combine(dataDirectory, @"save.brain");
            File.Create(path).Close();
            File.WriteAllText(path, _brain.BrainStructureString);
        }

        public void LoadBrainStructure(string dataDirectory)
        {
            string path = Path.Combine(dataDirectory, @"save.brainStream");
            _brain = new Brain(new FileStream(path, FileMode.Open), Neuron.ReLU);
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        public int GetOutputClassifcation(float[] networkOutputs)
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

        public static float CalculateCost(float[] networkOutputs, int targetOutput)
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

        public static float[][][] SumChanges(float[][][] total, float[][][] current)
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

        public static void ApplyChanges(Brain brain, float[][][][] changes)
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

        static public float[][][] Backpropagate(Brain brain, float TweakAmount, int outputNum, int expectedNum)
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

        private static float DeltaW(float tweakAmount, float delta_i, float activation_j)
        {
            return tweakAmount * delta_i * activation_j;
        }

        private static float Delta_i(Neuron neuron, float targetOutput, float currentOutput)
        {
            neuron.Delta_i = derivative_ReLU(neuron.Activation) * (targetOutput - currentOutput);
            return neuron.Delta_i;
        }

        private static float Delta_i(Neuron[][] layers, int targetLayer, int targetNeuron, float[] targetOutput)
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

        private static float derivative_tanh(float x)
        {
            return (float)((1 - Math.Tanh(x)));
        }

        private static float derivative_ReLU(double x)
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
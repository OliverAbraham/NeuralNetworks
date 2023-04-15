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
        public Brain    _brain;
        public byte[][] _trainingImages;
        public byte[]   _trainingLabels;
        public bool     _trainingDataLoadInProgress;
        public int      _imageSize = 28;
        public float    _trainingSpeed = 0.0005F;
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
            _brain = new Brain(_imageSize * _imageSize, 10, 3, 16, -0.5F, 0.5F, false, Neuron.ReLU);
            //Brain = new Brain(new FileStream(@"network.brainStream", FileMode.Open), Neuron.ReLU);//new Brain(File.ReadAllText(@"network.brain"),28 * 28, 10, 3, 16);
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
                var outputNum = NeuralNetwork.Training.OutputNumber(output);
                float cost = NeuralNetwork.Training.CalculateCost(output, _trainingLabels[iteration]);
                NeuralNetwork.Training.Backpropagate(_brain, _trainingSpeed, outputNum, _trainingLabels[iteration]);
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
                if (_stopAfterIterations > 0 && iteration >= _stopAfterIterations)
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
        #endregion
    }
}
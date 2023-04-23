namespace NeuralNetwork
{
    public class TrainingDataManager : ITrainingDataManager
    {
        #region ------------- Properties ----------------------------------------------------------
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private byte[][] _trainingImages;
        private byte[]   _trainingLabels;
        private int      TrainingImageSize;
        private int      TrainingImageCount;
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
                var fullFilename = Path.Combine(trainingDataDirectory, file);
                if (!File.Exists(fullFilename))
                {
                    messages += $"Cannot find {fullFilename}\n";
                    return false;
                }
            }

            Thread loadThread = new Thread(new ThreadStart(loadData));
            loadThread.Start();

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

        public int GetImageSize() => TrainingImageSize;

        public int GetImageCount() => TrainingImageCount;

        public (byte[], byte) GetTrainingImageAndLabel(int id)
        {
            var index = id % TrainingImageCount;
            return (_trainingImages[index], _trainingLabels[index]);
        }

        public (byte[], byte) GetRandomTrainingImageAndLabel()
        {
            var index = RandomNumberGenerator.Between(0, TrainingImageCount-1);
            return (_trainingImages[index], _trainingLabels[index]);
        }
        #endregion
    }
}

namespace NeuralNetwork
{
    /// <summary>
    /// Loads MNIST training data files. 
    /// Loads a couple of files, one with the training images, one with the classification labels for each image
    /// Loading is accomplished with a background thread.
    /// </summary>
    public interface ITrainingDataManager
    {
        bool StartLoadingTrainingData(string trainingDataDirectory, Action onLoadFinished, ref string messages);
        int GetImageSize();
        int GetImageCount();
        (byte[], byte) GetTrainingImageAndLabel(int id);
        (byte[], byte) GetRandomTrainingImageAndLabel();
    }
}

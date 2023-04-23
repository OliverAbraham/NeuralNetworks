namespace NeuralNetwork
{
    public interface ITrainingDataManager
    {
        bool StartLoadingTrainingData(string trainingDataDirectory, Action onLoadFinished, ref string messages);
        int GetImageSize();
        int GetImageCount();
        (byte[], byte) GetTrainingImageAndLabel(int id);
        (byte[], byte) GetRandomTrainingImageAndLabel();
    }
}

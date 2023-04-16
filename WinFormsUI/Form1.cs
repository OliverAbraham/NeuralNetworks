using HandwritingRecognition;
using NeuralNetwork;
using static NeuralNetwork.Network;
using Network = NeuralNetwork.Network;

namespace WinFormsUI
{
    public partial class Form1 : Form
    {
        #region ------------- Fields --------------------------------------------------------------
        private string _trainingDataDirectory = "./TrainingData";
        private Network _network;
        private Bitmap _currentBitmap = null;
        private DrawPad _drawPad;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            InitDrawPad();
            _network = new Network();
            textBoxNeuronsInInputLayer.Text   = _network.NeuronsInInputLayer.ToString();
            textBoxHiddenLayers.Text          = _network.HiddenLayersCount.ToString();
            textBoxNeuronsInHiddenLayers.Text = _network.NeuronsInHiddenLayers.ToString();
            textBoxNeuronsInOutputLayer.Text  = _network.NeuronsInOutputLayer.ToString();
            textBoxTrainingSpeed.Text         = _network.TrainingSpeed.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetInitialButtonStates();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _network.CancelTraining();
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        #region ------------- Init ----------------------------------------------------------------
        private void InitDrawPad()
        {
            _drawPad = new DrawPad();
            _drawPad.Location = new Point(600, 50);
            _drawPad.Size = new Size(600, 600);
            _drawPad.ImageSize = new Size(600, 600);
            _drawPad.LineWidth = 25;
            _drawPad.BackColor = Color.Black;
            Controls.Add(_drawPad);
        }

        private void SetInitialButtonStates()
        {
            buttonStartTraining.Enabled = false;
            save_button.Enabled = false;
            buttonCancelTraining.Enabled = false;
            _drawPad.Enabled = false;
        }
        #endregion
        #region ------------- Training image update -----------------------------------------------
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            DrawCurrentTrainingImage(e.Graphics);
        }

        private void DrawCurrentTrainingImage(Graphics graphics)
        {
            if (_currentBitmap is null)
                return;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            Rectangle rec = new Rectangle(50, 50, 200, 200);
            graphics.DrawImage(_currentBitmap, rec);
            _currentBitmap.Dispose();
            _currentBitmap = null;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_drawPad is null)
                return;
            int padsize = Height;
            _drawPad.Size = new Size(padsize, padsize);

            if (WindowState == FormWindowState.Minimized)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }
        #endregion
        #region ------------- Training data -------------------------------------------------------
        private void buttonLoadTrainingData_Click(object sender, EventArgs e)
        {
            string messages = "";
            var success = _network.StartLoadingTrainingData(_trainingDataDirectory, OnLoadFinished, ref messages);
            if (!success)
            {
                MessageBox.Show(messages);
                return;
            }
            labelStatus.Text = "loading training data...";
            Refresh();
        }

        private void OnLoadFinished()
        {
            Invoke(new MethodInvoker(
                delegate ()
                {
                    labelStatus.Text = "Training data loaded.";
                    textBoxStopAfterIterations.Text = "60000";
                    buttonStartTraining.Enabled = true;
                    save_button.Enabled = true;
                }));
        }
        #endregion
        #region ------------- Training ------------------------------------------------------------
        private void buttonStartTraining_Click(object sender, EventArgs e)
        {
            _network.StopAfterIterations = 0;
            _network.StopAfterAccurracy = 0;

            var hiddenLayersEntered = false;
            if (int.TryParse(textBoxHiddenLayers.Text, out int hiddenLayers))
                hiddenLayersEntered = true;

            var neuronsInHiddenLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInHiddenLayers.Text, out int neuronsInHiddenLayers))
                neuronsInHiddenLayersEntered = true;

            var stopAfterIterationsEntered = false;
            if (int.TryParse(textBoxStopAfterIterations.Text, out int iterations))
                stopAfterIterationsEntered = true;

            var stopAfterAccurracyEntered = false;
            if (int.TryParse(textBoxStopIfAccuracyIs.Text, out int accuracy))
                stopAfterAccurracyEntered = true;

            var trainingSpeedEntered = false;
            if (float.TryParse(textBoxTrainingSpeed.Text, out float trainingSpeed))
                trainingSpeedEntered = true;

            if (!ValidateEntries(
                hiddenLayersEntered, hiddenLayers,
                neuronsInHiddenLayersEntered, neuronsInHiddenLayers,
                stopAfterIterationsEntered, iterations,
                stopAfterAccurracyEntered, accuracy, 
                trainingSpeedEntered, trainingSpeed, 
                out string messages))
            {
                MessageBox.Show(messages);
                return;
            }

            if (hiddenLayersEntered)          _network.HiddenLayersCount     = hiddenLayers;
            if (neuronsInHiddenLayersEntered) _network.NeuronsInHiddenLayers = neuronsInHiddenLayers;
            if (stopAfterIterationsEntered)   _network.StopAfterIterations   = iterations;
            if (stopAfterAccurracyEntered)    _network.StopAfterAccurracy    = accuracy;
            if (trainingSpeedEntered)         _network.TrainingSpeed         = trainingSpeed;

            SetButtonsForTrainingProgress();

            _network.StartTraining(onTrainingProgress, onTrainingFinished);
        }

        private bool ValidateEntries(
            bool hiddenLayersEntered, int hiddenLayers,
            bool neuronsInHiddenLayersEntered, int neuronsInHiddenLayers,
            bool iterationsEntered, int iterations,
            bool accurracyEntered, int accuracy,
            bool trainingSpeedEntered, float trainingSpeed,
            out string messages)
        {
            messages = "";
            if (!iterationsEntered && !accurracyEntered)
            {
                messages = "please enter either iterations or accuracy, to set when training should end!";
                return false;
            }
            if (iterationsEntered && accurracyEntered)
            {
                messages = "please enter either iterations or accuracy, not both!";
                return false;
            }

            if (hiddenLayersEntered && hiddenLayers <= 0)
            {
                messages = "please enter the number of hidden layers as a positive number!";
                return false;
            }
            if (neuronsInHiddenLayersEntered && neuronsInHiddenLayers <= 0)
            {
                messages = "please enter the number of hidden layers as a positive number!";
                return false;
            }
            if (iterationsEntered && iterations <= 0)
            {
                messages = "please enter the number of iterations as a positive number!";
                return false;
            }
            if (accurracyEntered && (accuracy < 10 || accuracy > 100))
            {
                messages = "please enter accuracy threshold as a percentage between 10 and 100!";
                return false;
            }
            if (trainingSpeedEntered && (trainingSpeed <= 0 || trainingSpeed > 1.0))
            {
                messages = "please enter training speed as a number between 0 and 1!";
                return false;
            }
            return true;
        }

        private void onTrainingProgress(float[] currentOutput, string statusText, byte[] currentTrainingImage)
        {
            Invoke(new MethodInvoker(
                delegate ()
                {
                    _currentBitmap = BitmapConverter.ByteToBitmap(currentTrainingImage, _network.ImageSize, _network.ImageSize);
                    labelStatus.Text = statusText;
                    ShowCurrentImageClassification(currentOutput);
                    Refresh();
                }));
        }

        public void ShowCurrentImageClassification(float[] output)
        {
            var result = "";
            for (int i = 0; i < output.Length; i++)
            {
                result += $"{i}: {output[i].ToString("0.00")}\n";
            }
            labelTrainingClassification.Text = result;
        }

        private void onTrainingFinished(string statusText)
        {
            Invoke(new MethodInvoker(
                delegate ()
                {
                    labelStatus.Text = statusText;
                    SetButtonsAfterTraining();
                }));
        }

        private void buttonCancelTraining_Click(object sender, EventArgs e)
        {
            _network.CancelTraining();
            labelStatus.Text = "training cancelled";
            SetButtonsAfterTraining();
        }

        private void SetButtonsForTrainingProgress()
        {
            textBoxNeuronsInHiddenLayers.Enabled = false;
            textBoxHiddenLayers         .Enabled = false;
            textBoxStopAfterIterations  .Enabled = false;
            textBoxStopIfAccuracyIs     .Enabled = false;
            textBoxTrainingSpeed        .Enabled = false;
            buttonCancelTraining        .Enabled = true;
            buttonStartTraining         .Enabled = false;
            buttonLoadTrainingData      .Enabled = false;
            labelStatus.Text = "Training ...";
        }

        private void SetButtonsAfterTraining()
        {
            _drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
            _drawPad                    .Enabled = true;
            //textBoxNeuronsInHiddenLayers.Enabled = true;
            //textBoxHiddenLayers         .Enabled = true;
            textBoxStopAfterIterations  .Enabled = true;
            textBoxStopIfAccuracyIs     .Enabled = true;
            textBoxTrainingSpeed        .Enabled = true;
            buttonCancelTraining        .Enabled = false;
            buttonStartTraining         .Enabled = true;
            buttonLoadTrainingData      .Enabled = true;
        }
        #endregion
        #region ------------- Loading/saving ------------------------------------------------------
        private void buttonSaveNetwork_Click(object sender, EventArgs e)
        {
            _network.SaveNetwork(_trainingDataDirectory);
            labelStatus.Text = "Network saved.";
            _drawPad.Enabled = true;
        }

        private void buttonLoadNetwork_Click(object sender, EventArgs e)
        {
            _network.LoadNetwork(_trainingDataDirectory);
            labelStatus.Text = "Network loaded.";
            _drawPad.Enabled = true;
            _drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
        }
        #endregion
        #region ------------- Checking handwritten image ------------------------------------------
        private void OnUserHasDrawnAnImage()
        {
            buttonCheck_Click(null, null);
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            var inputValues = GetHandWrittenImage();
            if (inputValues is null)
                return;

            _currentBitmap = Bitmap.FromFile("img.bmp") as Bitmap;
            Refresh();

            var outputNeurons = _network.Think(inputValues);

            DisplayClassification(outputNeurons);
        }

        private float[] GetHandWrittenImage()
        {
            _drawPad.RescaleImage(_network.ImageSize, _network.ImageSize);
            _drawPad.CenterImage();
            _drawPad.Image.Save(@"img.png", System.Drawing.Imaging.ImageFormat.Png);
            _drawPad.Image.Save(@"img.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            float[] handwrittenImage = _drawPad.ImageToFloat();
            return handwrittenImage;
        }

        private void DisplayClassification(float[] outputNeurons)
        {
            var results = "";
            for (int i = 0; i < outputNeurons.Length; i++)
                results += i + ": " + outputNeurons[i].ToString("0.00") + "\n";

            labelClassification.Text = _network.GetOutputClassification(outputNeurons).ToString();
            labelClassificationResults.Text = results;
            _drawPad.ResetImage();
        }
        #endregion
        #endregion
    }
}
using HandwritingRecognition;
using NeuralNetwork;
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
            textBoxTrainingSpeed.Text = _network._trainingSpeed.ToString();
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
                    textBoxStopAfterIterations.Text = _network._trainingIterations.ToString();
                    buttonStartTraining.Enabled = true;
                    save_button.Enabled = true;
                }));
        }
        #endregion
        #region ------------- Training ------------------------------------------------------------
        private void buttonStartTraining_Click(object sender, EventArgs e)
        {
            _network._stopAfterIterations = 0;
            _network._stopAfterAccurracy = 0;

            var iterationsEntered = false;
            if (int.TryParse(textBoxStopAfterIterations.Text, out int iterations))
                iterationsEntered = true;

            var accurracyEntered = false;
            if (int.TryParse(textBoxStopIfAccuracyIs.Text, out int accuracy))
                accurracyEntered = true;

            var trainingSpeedEntered = false;
            if (float.TryParse(textBoxTrainingSpeed.Text, out float trainingSpeed))
                trainingSpeedEntered = true;

            if (!ValidateEntries(iterationsEntered, iterations, accurracyEntered, accuracy, trainingSpeedEntered, trainingSpeed, out string messages))
            {
                MessageBox.Show(messages);
                return;
            }

            _network._stopAfterIterations = iterations;
            _network._stopAfterAccurracy = accuracy;
            if (trainingSpeedEntered) _network._trainingSpeed = trainingSpeed;

            SetButtonsForTrainingProgress();

            _network.StartTraining(onTrainingProgress, onTrainingFinished);
        }

        private bool ValidateEntries(
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

        private void SetButtonsForTrainingProgress()
        {
            textBoxStopAfterIterations.Enabled = false;
            textBoxStopIfAccuracyIs.Enabled = false;
            textBoxTrainingSpeed.Enabled = false;
            buttonCancelTraining.Enabled = true;
            buttonStartTraining.Enabled = false;
            buttonLoadTrainingData.Enabled = false;
            labelStatus.Text = "Training ...";
        }

        private void onTrainingProgress()
        {
            Invoke(new MethodInvoker(
                delegate ()
                {
                    _currentBitmap = BitmapConverter.ByteToBitmap(_network._currentImage, _network._imageSize, _network._imageSize);
                    Invoke(new MethodInvoker(TrainingProgressUiUpdate));
                    labelStatus.Text = _network._currentStatus;
                }));
        }

        private void TrainingProgressUiUpdate()
        {
            labelStatus.Text = _network._currentStatus;
            ShowCurrentImageClassification(_network._currentOutput);
            Refresh();
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

        private void onTrainingFinished()
        {
            Invoke(new MethodInvoker(
                delegate ()
                {
                    _currentBitmap = BitmapConverter.ByteToBitmap(_network._currentImage, _network._imageSize, _network._imageSize);
                    labelStatus.Text = _network._currentStatus;
                    SetButtonsAfterTraining();
                }));
        }

        private void buttonCancelTraining_Click(object sender, EventArgs e)
        {
            _network.CancelTraining();
            labelStatus.Text = _network._currentStatus;
            SetButtonsAfterTraining();
        }

        private void SetButtonsAfterTraining()
        {
            _drawPad.Enabled = true;
            _drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
            textBoxStopAfterIterations.Enabled = true;
            textBoxStopIfAccuracyIs.Enabled = true;
            textBoxTrainingSpeed.Enabled = true;
            buttonCancelTraining.Enabled = false;
            buttonStartTraining.Enabled = true;
            buttonLoadTrainingData.Enabled = true;
        }
        #endregion
        #region ------------- Loading/saving ------------------------------------------------------
        private void buttonSaveNetwork_Click(object sender, EventArgs e)
        {
            _network.SaveBrainStructure(_trainingDataDirectory);
            labelStatus.Text = "Network saved.";
            _drawPad.Enabled = true;
        }

        private void buttonLoadNetwork_Click(object sender, EventArgs e)
        {
            _network.LoadBrainStructure(_trainingDataDirectory);
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
            var handwrittenImage = GetHandWrittenImage();
            if (handwrittenImage is null)
                return;

            _currentBitmap = Bitmap.FromFile("img.bmp") as Bitmap;
            Refresh();

            var classification = _network._brain.Think(handwrittenImage, Neuron.ReLU);

            DisplayClassification(classification);
        }

        private float[] GetHandWrittenImage()
        {
            _drawPad.RescaleImage(_network._imageSize, _network._imageSize);
            _drawPad.CenterImage();
            _drawPad.Image.Save(@"img.png", System.Drawing.Imaging.ImageFormat.Png);
            _drawPad.Image.Save(@"img.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            float[] handwrittenImage = _drawPad.ImageToFloat();
            return handwrittenImage;
        }

        private void DisplayClassification(float[] classification)
        {
            var results = "";
            for (int i = 0; i < classification.Length; i++)
                results += i + ": " + classification[i].ToString("0.00") + "\n";

            labelClassification.Text = NeuralNetwork.Training.OutputNumber(classification).ToString();
            labelClassificationResults.Text = results;
            _drawPad.ResetImage();
        }
        #endregion
        #endregion
    }
}
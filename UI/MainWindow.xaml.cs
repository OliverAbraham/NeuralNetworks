using Abraham.WPFWindowLayoutManager;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// User interface for the Neural network explorer
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region ------------- Fields --------------------------------------------------------------
        private WindowLayoutManager _layoutManager;

        #region Training data
        private string _trainingDataDirectory = "./TrainingData";
        private ITrainingDataManager _trainingDataManager;
        #endregion

        #region Network
        private Network _network;
        #endregion

        #region Structure
        private double _margin;
        private double _totalHeight;
        private List<Point> _outputNeuronPositions;
        private List<Point> _inputNeuronPositions;
        private List<List<Point>> _hiddenNeuronPositions;
        #endregion

        #region Training
        private LineChartControl _accuracyChart;
        #endregion

        #region Test
        private DrawPad _drawPad;
        #endregion
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public MainWindow()
        {
            _layoutManager = new WindowLayoutManager(window: this, key: "MainWindow");
            InitializeComponent();
            _drawPad = new DrawPad(HandwritingCanvas);
            InitStructureDisplay();
            _network = new Network();
            TrainingDataInit();
            TrainingInit();
            StructureInit();
            TestInit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialButtonStates();
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            _network.CancelTraining();
			_layoutManager.Save();
		}
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        #region ------------- Init ----------------------------------------------------------------
        private void InitStructureDisplay()
        {
        }

        private void SetInitialButtonStates()
        {
            buttonStartTraining.IsEnabled = false;
            buttonSaveNetwork.IsEnabled = false;
            buttonStopTraining.IsEnabled = false;
            //_drawPad.IsEnabled = false;
        }
        #endregion
        #region ------------- Training data -------------------------------------------------------
        private void TrainingDataInit()
        {
            _trainingDataManager = new TrainingDataManager();
        }

        private void buttonLoadTrainingData_Click(object sender, RoutedEventArgs e)
        {
            string messages = "";
            var success = _trainingDataManager.StartLoadingTrainingData(_trainingDataDirectory, OnLoadFinished, ref messages);
            if (!success)
            {
                MessageBox.Show(messages);
                return;
            }
            SetStatusBeforeLoading();
        }

        private void OnLoadFinished()
        {
            Dispatcher.Invoke(() =>
            {
                SetFieldsInTrainingDataBoxAfterLoad();
                SetStatusAfterLoad();
                InitNetworkStructureAfterLoad();
                InitTrainingAfterLoad();
            });
        }

        private void SetFieldsInTrainingDataBoxAfterLoad()
        {
            textBoxImageSize.Text = $"{_trainingDataManager.GetImageSize()} x {_trainingDataManager.GetImageSize()}";
            textBoxImageSize.IsEnabled = false;
            textBoxImageCount.Text = $"{_trainingDataManager.GetImageSize()}";
            textBoxImageCount.IsEnabled = false;

            sliderTrainingImage.Minimum = 0;
            sliderTrainingImage.Maximum = _trainingDataManager.GetImageCount() - 1;
            sliderTrainingImage.Value = 0;
            sliderTrainingImage_ValueChanged(null,null);
        }

        private void SetStatusBeforeLoading()
        {
            labelStatus.Content = "loading training data...";
        }

        private void SetStatusAfterLoad()
        {
            labelStatus.Content = "Training data loaded.";
        }

        private void sliderTrainingImage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int imageIndex = Convert.ToInt32(sliderTrainingImage.Value);
            (var image, var label) = _trainingDataManager.GetTrainingImageAndLabel(imageIndex);
            CurrentTrainingImage.Source = BitmapLibrary.CreateImageFromBytes(image);
            CurrentTrainingImageNumber.Content = imageIndex.ToString();
        }
        #endregion
        #region ------------- Structure -----------------------------------------------------------
        private void StructureInit()
        {
        }

        private void InitNetworkStructureAfterLoad()
        {
            _network.Initialize(_trainingDataManager.GetImageSize() * _trainingDataManager.GetImageSize(), 6, 16, 10);

            textBoxNeuronsInInputLayer  .Text = _network.NeuronsInInputLayer  .ToString();
            textBoxHiddenLayers         .Text = _network.HiddenLayersCount    .ToString();
            textBoxNeuronsInHiddenLayers.Text = _network.NeuronsInHiddenLayers.ToString();
            textBoxNeuronsInOutputLayer .Text = _network.NeuronsInOutputLayers .ToString();

            DrawStructure();
        }

        private void buttonResetNetwork_Click(object sender, EventArgs e)
        {
            var inputLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInInputLayer.Text, out int neuronsInInputLayer))
                inputLayersEntered  = true;

            var hiddenLayersEntered = false;
            if (int.TryParse(textBoxHiddenLayers.Text, out int hiddenLayers))
                hiddenLayersEntered = true;

            var neuronsInHiddenLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInHiddenLayers.Text, out int neuronsInHiddenLayers))
                neuronsInHiddenLayersEntered = true;

            var neuronsInOutputLayerEntered = false;
            if (int.TryParse(textBoxNeuronsInOutputLayer.Text, out int neuronsInOutputLayer))
                neuronsInOutputLayerEntered = true;

            _network.Initialize(neuronsInInputLayer, hiddenLayers, neuronsInHiddenLayers, neuronsInOutputLayer);

            DrawStructure();
            _accuracyChart.Clear();
        }

        private void DrawStructure()
        {
            _margin = 0.1;
            _totalHeight = canvasStructure.Height * 0.8;

            canvasStructure.Children.Clear();
            DrawInputNeurons();
            DrawHiddenNeurons();
            DrawOutputNeurons();
        }

        private void DrawInputNeurons()
        {
            var x = canvasStructure.Width * _margin;
            var y = (canvasStructure.Height/2) - (_totalHeight/2);
            _inputNeuronPositions = DrawNCirclesVertically(x, y, _totalHeight, _network.NeuronsInInputLayer, Brushes.Black);
        }

        private void DrawHiddenNeurons()
        {
            var x = canvasStructure.Width * 3 * _margin;
            if (_network.HiddenLayersCount == 1)
                x = canvasStructure.Width / 2;

            var y = (canvasStructure.Height/2) - (_totalHeight/2);

            var totalWidthForHiddenNeurons = canvasStructure.Width - (4 * _margin * canvasStructure.Width);
            var horizontalIncrement = totalWidthForHiddenNeurons / _network.HiddenLayersCount;

            _hiddenNeuronPositions = new List<List<Point>>();

            for (int h=0; h < _network.HiddenLayersCount; h++)
            {
                var pointList = DrawNCirclesVertically(x + h*horizontalIncrement, y, _totalHeight, _network.NeuronsInHiddenLayers, Brushes.Black);
                _hiddenNeuronPositions.Add(pointList);

                if (h == 0)
                    DrawNLines(_inputNeuronPositions, pointList, Brushes.Black);
                else
                    DrawNLines(_hiddenNeuronPositions[h-1], _hiddenNeuronPositions[h], Brushes.Black);
            }
        }

        private void DrawOutputNeurons()
        {
            var x = (1 - _margin) * canvasStructure.Width;
            var y = (canvasStructure.Height / 2) - (_totalHeight / 2);
            _outputNeuronPositions = DrawNCirclesVertically(x, y, _totalHeight, _network.NeuronsInOutputLayers, Brushes.Black);

            DrawNLines(_hiddenNeuronPositions[_network.HiddenLayersCount-1], _outputNeuronPositions, Brushes.Black);
        }

        private void DrawNLines(List<Point> from, List<Point> to, Brush brush)
        {
            foreach(var p2 in to)
                foreach(var p1 in from)
                     CanvasGraphicsLibrary.AddLine(canvasStructure, p1.X, p1.Y, p2.X, p2.Y, brush, 1);
        }

        private List<Point> DrawNCirclesVertically(double x, double y, double totalHeight, int count, Brush brush)
        {
            if (count == 1)
                y = y + (totalHeight / 2);

            var points = new List<Point>();
            var increment = (count > 1) ? totalHeight / (count - 1) : 0;
            for (int i = 0; i < count; i++)
            {
                var ypos = y + (i * increment);
                CanvasGraphicsLibrary.AddCircleAt(canvasStructure, x, ypos, 10, brush);
                points.Add(new Point(x+5, ypos+5));
            }
            return points;
        }

        private void buttonLoadNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _network.LoadNetwork(_trainingDataDirectory);
                labelStatus.Content = "Network loaded.";
                textBoxNeuronsInInputLayer  .Text    = _network.NeuronsInInputLayer  .ToString();
                textBoxHiddenLayers         .Text    = _network.HiddenLayersCount    .ToString();
                textBoxNeuronsInHiddenLayers.Text    = _network.NeuronsInHiddenLayers.ToString();
                textBoxNeuronsInOutputLayer .Text    = _network.NeuronsInOutputLayers.ToString();
                labelTotalTrainingIterations.Content = _network.TotalTrainingIterations;
                DrawStructure();
                _accuracyChart.Clear();
                Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSaveNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _network.SaveNetwork(_trainingDataDirectory);
                labelStatus.Content = "Network saved.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region ------------- Training ------------------------------------------------------------
        private void InitTrainingAfterLoad()
        {
            textBoxNeuronsInInputLayer.Text = _network.NeuronsInInputLayer.ToString();
            textBoxStopAfterIterations.Text = "1000000";
            labelTotalTrainingIterations.Content = _network.TotalTrainingIterations;
            buttonStartTraining.IsEnabled = true;
            buttonSaveNetwork.IsEnabled = true;
            _accuracyChart.Refresh();
        }

        private void TrainingInit()
        {
            _accuracyChart = new LineChartControl(canvasAccuracy);
            textBoxTrainingSpeed.Text = _network.TrainingSpeed.ToString();
        }

        private void buttonStartTraining_Click(object sender, EventArgs e)
        {
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

            _network.StopAfterIterations = 0;
            _network.StopAfterAccurracy = 0;
            if (stopAfterIterationsEntered  ) _network.StopAfterIterations   = iterations;
            if (stopAfterAccurracyEntered   ) _network.StopAfterAccurracy    = accuracy;
            if (trainingSpeedEntered        ) _network.TrainingSpeed         = trainingSpeed;

            SetButtonsForTrainingProgress();
            _network.StartTraining(_trainingDataManager, onTrainingProgress, onTrainingFinished);
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

        private void onTrainingProgress(float[] currentOutput, string statusText, byte[] currentTrainingImage, int currentTrainingImageIndex, int totalAccuracy)
        {
            Dispatcher.Invoke(() =>
            {
                CurrentTrainingImage.Source = BitmapLibrary.CreateImageFromBytes(currentTrainingImage);
                labelStatus.Content = statusText;
                labelTotalTrainingIterations.Content = _network.TotalTrainingIterations;
                ShowCurrentImageClassification(currentOutput);
                AddValueToAccuracyDiagram(totalAccuracy);
            });
        }

        public void ShowCurrentImageClassification(float[] output)
        {
            var result = "";
            for (int i = 0; i < output.Length; i++)
            {
                result += $"{i}: {output[i].ToString("0.00")}\n";
            }
            labelTrainingClassification.Content = result;
        }

        private void onTrainingFinished(string statusText)
        {
            Dispatcher.Invoke(() =>
            {
                labelStatus.Content = statusText;
                SetButtonsAfterTraining();
            });
        }

        private void buttonStopTraining_Click(object sender, RoutedEventArgs e)
        {
            _network.CancelTraining();
            labelStatus.Content = "training cancelled";
            SetButtonsAfterTraining();
        }

        private void SetButtonsForTrainingProgress()
        {
            textBoxNeuronsInInputLayer  .IsEnabled = false;
            //textBoxNeuronsInHiddenLayers.IsEnabled = false;
            //textBoxHiddenLayers         .IsEnabled = false;
            textBoxNeuronsInOutputLayer .IsEnabled = false;
            textBoxStopAfterIterations  .IsEnabled = false;
            textBoxStopIfAccuracyIs     .IsEnabled = false;
            textBoxTrainingSpeed        .IsEnabled = false;
            buttonStopTraining          .IsEnabled = true;
            buttonStartTraining         .IsEnabled = false;
            buttonLoadTrainingData      .IsEnabled = false;
            labelStatus                 .Content = "Training ...";
        }

        private void SetButtonsAfterTraining()
        {
            //_drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
            //_drawPad.IsEnabled = true;
            //textBoxNeuronsInHiddenLayers.IsEnabled = true;
            //textBoxHiddenLayers         .IsEnabled = true;
            textBoxStopAfterIterations.IsEnabled = true;
            textBoxStopIfAccuracyIs.IsEnabled = true;
            textBoxTrainingSpeed.IsEnabled = true;
            buttonStopTraining.IsEnabled = false;
            buttonStartTraining.IsEnabled = true;
            buttonLoadTrainingData.IsEnabled = true;
        }

        private void AddValueToAccuracyDiagram(int totalAccuracy)
        {
            _accuracyChart.AddValue(totalAccuracy);
            NotifyPropertyChanged(nameof(canvasAccuracy));
        }
        #endregion
        #region ------------- Test ----------------------------------------------------------------
        private void TestInit()
        {
        }

        private void HandwritingCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            HandwritingCanvas.Children.Clear();
        }

        private void Handwriting_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_network.IsInitialized)
                return;

            var renderTargetBitmap = BitmapLibrary.GetBitmapFromCanvas(HandwritingCanvas, 28, 28);
            CurrentTrainingImage.Source = renderTargetBitmap;

            System.Drawing.Bitmap drawingBitmap = BitmapLibrary.ConvertRenderTargetBitmapToSystemDrawingBitmap(renderTargetBitmap);
            var imageBytes = BitmapLibrary.ConvertColorBitmapTo1dimensionalBytesArray(drawingBitmap);
            var outputNeurons = _network.Think(imageBytes);
            DisplayClassification(outputNeurons);
        }

        private void buttonTestCurrentTrainingImage_Click(object sender, RoutedEventArgs e)
        {
            if (!_network.IsInitialized)
                return;
            var bitmap = CurrentTrainingImage.Source as BitmapSource;
            var imageBytes = BitmapLibrary.CreateBytesFromImage(bitmap);
            var outputNeurons = _network.Think(imageBytes);
            DisplayClassification(outputNeurons);
        }

        private void DisplayClassification(float[] outputNeurons)
        {
            string outputNeuronsAsString = ConvertArrayToString(outputNeurons);

            labelClassicationResults.Content = outputNeuronsAsString;
            labelClassification.Content = _network.GetClassification(outputNeurons).ToString();
            _drawPad.ResetImage();
        }

        private static string ConvertArrayToString(float[] outputNeurons)
        {
            var results = "";

            for (int i = 0; i < outputNeurons.Length; i++)
                results += i + ": " + outputNeurons[i].ToString("0.00") + "\n";
            
            return results;
        }
        #endregion
        #endregion



        #region ------------- INotifyPropertyChanged ----------------------------------------------
        // add "INotifyPropertyChanged" to your class
        // add "using System.ComponentModel";
        // add "using System";

        [NonSerialized]
        private PropertyChangedEventHandler? _propertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = _propertyChanged; // avoid race condition
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

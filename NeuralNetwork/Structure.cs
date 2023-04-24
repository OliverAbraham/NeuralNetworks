namespace NeuralNetwork
{
    /// <summary>
    /// Represents the structure of a Neural Network.
    /// The structure contains an input layer, a number of hidden layers and an output layer.
    /// </summary>
    public class Structure
    {
        #region ------------- Properties ----------------------------------------------------------
        public int          NeuronsInInputLayer     { get; set; }
        public int          HiddenLayersCount       { get; set; }
        public int          NeuronsInHiddenLayers   { get; set; }
        public int          NeuronsInOutputLayers   { get; set; }
        public int          TotalTrainingIterations { get; set; }
        public Neuron[][]   AllLayers               { get; set; }
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        [Newtonsoft.Json.JsonIgnore]
        private Neuron[][]   _hiddenLayers = null;
        [Newtonsoft.Json.JsonIgnore]
        private Neuron[]     _inputs       = null;
        [Newtonsoft.Json.JsonIgnore]
        private Neuron[]     _outputs      = null;
        [Newtonsoft.Json.JsonIgnore]
        private Func<float, float> _activationFunction;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public Structure()
        {
        }

        public Structure(int neuronsInInputLayer, int hiddenLayersCount, int neuronsInHiddenLayer, int neuronsInOutputLayer,
            float minWeightVal, float maxWeightVal, bool addDisconnectedWeights, Func<float, float> activationFunc)
        {
            NeuronsInInputLayer   = neuronsInInputLayer;
            HiddenLayersCount     = hiddenLayersCount;
            NeuronsInHiddenLayers = neuronsInHiddenLayer;
            NeuronsInOutputLayers  = neuronsInOutputLayer;
            _activationFunction = activationFunc;
            GenerateNeurons();
            GenerateBrainStructure();
            InitializeWeightsAndBiases(minWeightVal, maxWeightVal, addDisconnectedWeights);
        }
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        /// <summary>
        /// Compute inputs with the network and return outputs
        /// </summary>
        public float[] Think(float[] values)
        {
            // Reset all neuron activations
            for (int layer = 0; layer < AllLayers.Length; layer++)
            {
                for (int i = 0; i < AllLayers[layer].Length; i++)
                {
                    AllLayers[layer][i]._activation = 0.0F;
                    AllLayers[layer][i]._receivedInputs = 0;
                }
            }

            //read in all given inputs and set the value of the input neurons
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i].Activate(_inputs[i], values[i]);
            }

            //get all computed data from output neurons
            var results = new float[_outputs.Length];
            for (int i = 0; i < _outputs.Length; i++)
            {
                results[i] = _activationFunction(_outputs[i]._activation);
            }

            return results;
        }

        /// <summary>
        /// Adjust weights and biases of all neurons using the backpropagation algorithm
        /// </summary>
        public void Backpropagate(float trainingSpeed, int outputNum, int expectedNum)
        {
            float[] targetOutput = new float[10];
            targetOutput[expectedNum] = 1.0F;

            for (int layer = AllLayers.Length - 1; layer > 0; layer--) // all but the input layer
            {
                Neuron[] layerNeurons = AllLayers[layer];

                for (int n = 0; n < layerNeurons.Length; n++)
                {
                    Neuron neuron = layerNeurons[n];

                    for (int w = 0; w < neuron.Weights.Length; w++)
                    {
                        float delta_i = (neuron._type == Neuron.NeuronType.HiddenNeuron)
                            ? Delta_i(AllLayers, layer, n, targetOutput)
                            : Delta_i(neuron, targetOutput[n], Neuron.ReLU(neuron._activation));

                        float activation_j = AllLayers[layer-1][w]._activation;
                        float deltaW = DeltaW(trainingSpeed, delta_i, activation_j);

                        if (!float.IsNaN(deltaW))
                            neuron.Weights[w] += deltaW;
                    }
                }
            }
        }

        public void CopyWeightsAndBiasesFrom(Structure data)
        {
            for (int layer = 0; layer < data.AllLayers.Length; layer++)
            {
                var sourceNeurons = data.AllLayers[layer];
                for (int i = 0; i < sourceNeurons.Length; i++)
                {
                    AllLayers[layer][i].Bias = sourceNeurons[i].Bias;
                    AllLayers[layer][i].Weights = sourceNeurons[i].Weights;
                }
            }
            TotalTrainingIterations = data.TotalTrainingIterations;
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private float DeltaW(float tweakAmount, float delta_i, float activation_j)
        {
            return tweakAmount * delta_i * activation_j;
        }

        private float Delta_i(Neuron neuron, float targetOutput, float currentOutput)
        {
            neuron._delta_i = derivative_ReLU(neuron._activation) * (targetOutput - currentOutput);
            return neuron._delta_i;
        }

        private float Delta_i(Neuron[][] layers, int targetLayer, int targetNeuron, float[] targetOutput)
        {
            Neuron neuron = layers[targetLayer][targetNeuron];
            int prevLayer = targetLayer + 1;

            float sum = 0.0F;
            for (int i = 0; i < layers[prevLayer].Length; i++)
            {
                Neuron prevNeuron = layers[prevLayer][i];
                sum += prevNeuron._delta_i * prevNeuron.Weights[targetNeuron];
            }
            neuron._delta_i = derivative_ReLU(neuron._activation) * sum;

            return neuron._delta_i;
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

        private void GenerateNeurons()
        {
            var layerNo = 0;
            // Generate input neurons
            {
                var neurons = new Neuron[NeuronsInInputLayer];
                for (int i = 0; i < neurons.Length; i++)
                {
                    neurons[i]                     = new Neuron();
                    neurons[i]._type               = Neuron.NeuronType.InputNeuron;
                    neurons[i]._layer              = layerNo;
                    neurons[i]._layerIndex         = i;
                    neurons[i]._inputs             = new Neuron[NeuronsInInputLayer];
                    neurons[i].Weights             = new float[NeuronsInHiddenLayers]; // unnecessary! Input neurons don't have weights and biases
                    neurons[i]._activationFunction = _activationFunction;
                }
                _inputs = neurons;
                layerNo++;
            }

            //Generate hidden neurons
            _hiddenLayers = new Neuron[HiddenLayersCount][];
            for (int layer = 0; layer < _hiddenLayers.Length; layer++)
            {
                var neurons = new Neuron[NeuronsInHiddenLayers];
                for (int i = 0; i < neurons.Length; i++)
                {
                    neurons[i]                     = new Neuron();
                    neurons[i]._type               = Neuron.NeuronType.HiddenNeuron;
                    neurons[i]._layer              = layerNo;
                    neurons[i]._layerIndex         = i;
                    neurons[i]._activationFunction = _activationFunction;

                    // connect the first hidden layer with the input neurons
                    if (layer == 0)
                    {
                        neurons[i]._inputs             = new Neuron[NeuronsInInputLayer];
                        neurons[i].Weights             = new float[NeuronsInInputLayer];
                    }
                    else
                    {
                        neurons[i]._inputs             = new Neuron[NeuronsInHiddenLayers];
                        neurons[i].Weights             = new float[NeuronsInHiddenLayers];
                    }
                }
                _hiddenLayers[layer] = neurons;
                layerNo++;
            }

            //Generate output neurons
            {
                var neurons = new Neuron[NeuronsInOutputLayers];
                for (int i = 0; i < neurons.Length; i++)
                {
                    neurons[i]                     = new Neuron();
                    neurons[i]._type               = Neuron.NeuronType.OutputNeuron;
                    neurons[i]._layer              = layerNo;
                    neurons[i]._layerIndex         = i;
                    neurons[i]._inputs             = new Neuron[NeuronsInHiddenLayers];
                    neurons[i].Weights             = new float[NeuronsInHiddenLayers];
                    neurons[i]._activationFunction = _activationFunction;
                }
                _outputs = neurons;
            }

            AllLayers = new Neuron[_hiddenLayers.Length + 2][];
            AllLayers[0] = _inputs;
            Array.Copy(_hiddenLayers, 0, AllLayers, 1, _hiddenLayers.Length);
            AllLayers[AllLayers.Length - 1] = _outputs;
        }

        private void GenerateBrainStructure()
        {
            for (int layer = 0; layer < AllLayers.Length - 1; layer++)
            {
                for (int neuron = 0; neuron < AllLayers[layer].Length; neuron++)
                {
                    Neuron currentNeuron = AllLayers[layer][neuron]; // neuron, that gets outputs
                    
                    currentNeuron._outputs = new Neuron[AllLayers[layer + 1].Length];

                    // connect all neurons of the next layer with us
                    for (int i = 0; i < AllLayers[layer + 1].Length; i++)
                    {
                        Neuron nextLayerNeuron = AllLayers[layer + 1][i]; // Neuron that will receive input
                        currentNeuron._outputs[i] = nextLayerNeuron;
                        nextLayerNeuron._inputs[nextLayerNeuron._inputCount] = currentNeuron;
                        nextLayerNeuron._inputCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Randomly generates the weights of the neuron
        /// </summary>
        private void InitializeWeightsAndBiases(float minWeightVal, float maxWeightVal, bool addDisconnectedWeights)
        {
            int min = (int)(minWeightVal * 100);
            int max = (int)(maxWeightVal * 100);

            for (int layer = 0; layer < AllLayers.Length; layer++)
            {
                for (int neuron = 0; neuron < AllLayers[layer].Length; neuron++)
                {
                    Neuron currentNeuron = AllLayers[layer][neuron];
                    
                    currentNeuron.Bias = RandomNumberGenerator.Between(-10, 10);

                    if (layer > 0)
                    for (int i = 0; i < currentNeuron.Weights.Length; i++)
                    {
                        currentNeuron.Weights[i] = RandomNumberGenerator.Between(min, max) / 100.0F;
                        if (addDisconnectedWeights)
                            currentNeuron.Weights[i] *= RandomNumberGenerator.Between(-1, 1);
                    }
                }
            }
        }
        #endregion
    }
}
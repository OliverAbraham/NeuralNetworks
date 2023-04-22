﻿namespace NeuralNetwork
{
    public class Brain : ICloneable
    {
        #region ------------- Properties ----------------------------------------------------------
                                               
        // Network structure                   
        public int          NeuronsInInputLayer     { get; set; }
        public int          HiddenLayersCount       { get; set; }
        public int          NeuronsInHiddenLayers   { get; set; }
        public int          NeuronsInOutputLayer    { get; set; }
        public int          TotalTrainingIterations { get; set; }

        public Neuron[][]   AllLayers               { get; set; } = null;
        public Neuron[][]   HiddenLayers            { get; set; } = null;
        public Neuron[]     Inputs                  { get; set; } = null;
        public Neuron[]     Outputs                 { get; set; } = null;
        
        public Func<float, float> ActivationFunction { get; set; } = Neuron.ReLU;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Func<float, float> _activationFunction;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public Brain()
        {
        }

        public Brain(int inputCount, int outputCount, int layerCount, int neuronsPerLayer, float minWeightVal, float maxWeightVal, bool addDisconnectedWeights, Func<float, float> activationFunc)
        {
            _activationFunction = activationFunc;
            GenerateNeurons(inputCount, outputCount, layerCount, neuronsPerLayer);
            GenerateBrainStructure(minWeightVal, maxWeightVal, addDisconnectedWeights);
        }

        public Brain(string filename)
        {
            var structure = new FileStream(filename, FileMode.Open);
            _activationFunction = Neuron.ReLU;
            DeserializeStructure(structure);
        }
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        /// <summary>
        /// Compute inputs with the network and return outputs
        /// </summary>
        public float[] Think(float[] values)
        {
            EraseActivations();

            //read in all given inputs and set the value of the input neurons
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i].Activate(Inputs[i], values[i]);
            }

            //get all computed data from output neurons
            var results = new float[Outputs.Length];
            for (int i = 0; i < Outputs.Length; i++)
            {
                results[i] = ActivationFunction(Outputs[i].Activation);
            }

            return results;
        }

        public void SaveNetworkToFile(string filename)
        {
            FileStream fstream = new FileStream(filename, FileMode.Create);
            MemoryStream stream = SerializeStructure();
            stream.WriteTo(fstream);
            fstream.Close();
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------

        private MemoryStream SerializeStructure()
        {
            MemoryStream structure = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(structure);
            
            binaryWriter.Write(AllLayers.Length); // write layer count

            for (int layer = 0; layer < AllLayers.Length; layer++)
            {
                binaryWriter.Write(AllLayers[layer].Length); // write neuron count per layer
            }

            for (int layer = 1; layer < AllLayers.Length; layer++)
            {
                for (int neuronIndex = 0; neuronIndex < AllLayers[layer].Length; neuronIndex++)
                {
                    var neuron = AllLayers[layer][neuronIndex];

                    for (int i = 0; i < neuron.Weight.Length; i++)
                    {
                        binaryWriter.Write(neuron.Weight[i]);
                    }
                }
            }

            return structure;
        }

        private void DeserializeStructure(Stream structure)
        {
            BinaryReader reader = new BinaryReader(structure);

            int layerAmount = reader.ReadInt32();
            int inputCount = reader.ReadInt32();
            int[] hiddenCount = new int[layerAmount - 2];

            for (int i = 0; i < layerAmount - 2; i++)
            {
                hiddenCount[i] = reader.ReadInt32();
            }

            int outputCount = reader.ReadInt32();

            NeuronsInInputLayer     = inputCount;
            HiddenLayersCount       = layerAmount - 2;
            NeuronsInHiddenLayers   = hiddenCount[0];
            NeuronsInOutputLayer    = outputCount;


            GenerateNeurons(inputCount, outputCount, layerAmount - 2, hiddenCount[0]);

            // Connect the input layer to the first hidden layer
            for (int i = 0; i < inputCount; i++)
            {
                Inputs[i].OutputConnections = HiddenLayers[0];
            }

            // Connect the hidden layers to next hidden layers
            for (int layer = 0; layer < hiddenCount.Length; layer++)
            {
                for (int neuronIndex = 0; neuronIndex < hiddenCount[layer]; neuronIndex++)
                {
                    Neuron targetNeuron = HiddenLayers[layer][neuronIndex];
                    targetNeuron.InputConnections = AllLayers[layer];
                    targetNeuron.OutputConnections = AllLayers[layer + 2];

                    for (int i = 0; i < targetNeuron.Weight.Length; i++)
                    {
                        targetNeuron.Weight[i] = reader.ReadSingle();
                    }
                }
            }

            // Connect the last hidden layer to the output layer
            for (int neuronIndex = 0; neuronIndex < outputCount; neuronIndex++)
            {
                Neuron targetNeuron = Outputs[neuronIndex];
                targetNeuron.InputConnections = HiddenLayers[HiddenLayers.Length - 1];

                for (int i = 0; i < targetNeuron.Weight.Length; i++)
                {
                    targetNeuron.Weight[i] = reader.ReadSingle();
                }
            }

            structure.Seek(0, SeekOrigin.Begin);
        }

        private string SerializeNetwork()
        {
            string results = "";
            for (int layer = 0; layer < AllLayers.Length; layer++)
            {
                results += "\nL" + layer + ":\n";

                for (int neuron = 0; neuron < AllLayers[layer].Length; neuron++)
                {
                    Neuron targetNeuron = AllLayers[layer][neuron]; // neuron, that gets output
                    results += "\n\tN" + neuron + ":\n";

                    for (int i = 0; i < targetNeuron.Weight.Length; i++)
                    {
                        results += "\n\t\tW" + i + ":" + targetNeuron.Weight[i];
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Resets all neuron activations
        /// </summary>
        private void EraseActivations()
        {
            for (int layer = 0; layer < AllLayers.Length; layer++)
            {
                for (int i = 0; i < AllLayers[layer].Length; i++)
                {
                    AllLayers[layer][i].Activation = 0.0F;
                    AllLayers[layer][i].ReceivedInputs = 0;
                }
            }
        }

        private void GenerateNeurons(int inputCount, int outputCount, int layerCount, int neuronsPerLayer)
        {
            // Create container for neurons
            Inputs = new Neuron[inputCount];
            Outputs = new Neuron[outputCount];
            HiddenLayers = new Neuron[layerCount][];

            // Generate input neurons
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i]                    = new Neuron();
                Inputs[i].Type               = Neuron.NeuronType.InputNeuron;
                Inputs[i].ID                 = "Input" + i;
                Inputs[i].LayerIndex         = i;
                Inputs[i].InputConnections   = new Neuron[inputCount];
                Inputs[i].Weight             = new float[neuronsPerLayer];
                Inputs[i].ActivationFunction = _activationFunction;
            }

            //Generate output neurons
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i]                    = new Neuron();
                Outputs[i].Type               = Neuron.NeuronType.OutputNeuron;
                Outputs[i].ID                 = "Output" + i;
                Outputs[i].LayerIndex         = i;
                Outputs[i].InputConnections   = new Neuron[neuronsPerLayer];
                Outputs[i].Weight             = new float[neuronsPerLayer];
                Outputs[i].ActivationFunction = _activationFunction;
            }

            //Generate hidden neurons
            for (int layer = 1; layer < HiddenLayers.Length; layer++)
            {
                HiddenLayers[layer] = new Neuron[neuronsPerLayer];
                for (int i = 0; i < HiddenLayers[layer].Length; i++)
                {
                    HiddenLayers[layer][i]                    = new Neuron();
                    HiddenLayers[layer][i].Type               = Neuron.NeuronType.HiddenNeuron;
                    HiddenLayers[layer][i].ID                 = "Neuron" + i;
                    HiddenLayers[layer][i].LayerIndex         = i;
                    HiddenLayers[layer][i].InputConnections   = new Neuron[neuronsPerLayer];
                    HiddenLayers[layer][i].Weight             = new float[neuronsPerLayer];
                    HiddenLayers[layer][i].ActivationFunction = _activationFunction;
                }
            }

            //Generate input neurons of the first layer, to configure them with the inputs
            HiddenLayers[0] = new Neuron[neuronsPerLayer];
            for (int i = 0; i < HiddenLayers[0].Length; i++)
            {
                HiddenLayers[0][i]                    = new Neuron();
                HiddenLayers[0][i].Type               = Neuron.NeuronType.HiddenNeuron;
                HiddenLayers[0][i].ID                 = "Neuron" + i;
                HiddenLayers[0][i].LayerIndex         = i;
                HiddenLayers[0][i].InputConnections   = new Neuron[inputCount];
                HiddenLayers[0][i].Weight             = new float[inputCount];
                HiddenLayers[0][i].ActivationFunction = _activationFunction;
            }

            AllLayers = new Neuron[HiddenLayers.Length + 2][];
            Array.Copy(HiddenLayers, 0, AllLayers, 1, HiddenLayers.Length);
            AllLayers[0] = Inputs;
            AllLayers[AllLayers.Length - 1] = Outputs;
        }

        private void GenerateBrainStructure(float minWeightVal, float maxWeightVal, bool addDisconnectedWeights) // Randomly generates the weights of the neuron
        {
            int min = (int)(minWeightVal * 100);
            int max = (int)(maxWeightVal * 100);

            for (int layer = 0; layer < AllLayers.Length - 1; layer++)
            {
                for (int neuronIndex = 0; neuronIndex < AllLayers[layer].Length; neuronIndex++)
                {
                    Neuron targetNeuron = AllLayers[layer][neuronIndex]; // neuron, that gets outputs
                    int outputCount = AllLayers[layer + 1].Length; // Amount of outputs
                    targetNeuron.OutputConnections = new Neuron[outputCount];
                    targetNeuron.Bias = RandomNumberGenerator.Between(-10, 10);

                    for (int i = 0; i < AllLayers[layer + 1].Length; i++)
                    {
                        Neuron outputNeuron = AllLayers[layer + 1][i]; // Neuron that will receive input
                        int inputCount = outputNeuron.InputConnectionsCount;

                        targetNeuron.OutputConnections[i] = outputNeuron;

                        outputNeuron.InputConnections[inputCount] = AllLayers[layer][neuronIndex];
                        outputNeuron.Weight[inputCount] = RandomNumberGenerator.Between(min, max) / 100.0F;

                        if (addDisconnectedWeights)
                            outputNeuron.Weight[inputCount] *= RandomNumberGenerator.Between(-1, 1);

                        outputNeuron.InputConnectionsCount++;
                    }
                }
            }
        }

        private void ParseBrainStructurString(string _brainStructure) // Parse a given brainStructure string
        {
            string brainStructure = _brainStructure.Replace("\n", string.Empty).Replace("\t", string.Empty);
            string brainStructureSubstring = brainStructure;

            string[] stringLayers = new string[brainStructure.Count(c => c == 'L')];
            string[][] stringNeurons = new string[stringLayers.Length][];
            float[][][] weights = new float[stringLayers.Length][][];

            for (int i = 0; i < stringLayers.Length; i++) // all Layers
            {
                int start = brainStructureSubstring.IndexOf('L');
                int end = brainStructureSubstring.Substring(start + 1).IndexOf('L');
                if (end < 0)
                    end = brainStructureSubstring.Length - 1;

                string str = brainStructureSubstring.Substring(start, end - start + 1);
                brainStructureSubstring = brainStructureSubstring.Replace(str, string.Empty);
                stringLayers[i] = str;
            }

            for (int iter = 0; iter < stringLayers.Length; iter++) // all Neurons
            {
                stringNeurons[iter] = new string[stringLayers[iter].Count(c => c == 'N')];
                string layerSubstring = stringLayers[iter].Substring(3);

                for (int i = 0; i < stringNeurons[iter].Length; i++)
                {
                    int start = layerSubstring.IndexOf('N');
                    int end = layerSubstring.Substring(start + 1).IndexOf('N');
                    if (end < 0)
                        end = layerSubstring.Length - 1;

                    string str = layerSubstring.Substring(start, end - start + 1);
                    layerSubstring = layerSubstring.Replace(str, string.Empty);
                    stringNeurons[iter][i] = str;
                }
            }

            for (int iteration = 0; iteration < stringLayers.Length; iteration++) // all Weights
            {
                weights[iteration] = new float[stringNeurons[iteration].Length][];
                string layerSubstring = stringLayers[iteration].Substring(3);

                for (int iter = 0; iter < stringNeurons[iteration].Length; iter++)
                {
                    weights[iteration][iter] = new float[stringNeurons[iteration][iter].Count(c => c == 'W')];
                    string neuronSubstring = stringNeurons[iteration][iter].Substring(stringNeurons[iteration][iter].IndexOf(':') + 1);

                    for (int i = 0; i < weights[iteration][iter].Length; i++)
                    {
                        int start = neuronSubstring.IndexOf('W');
                        int end = neuronSubstring.Substring(start + 1).IndexOf('W');
                        if (end < 0)
                            end = neuronSubstring.Length - 1;

                        string str = neuronSubstring.Substring(start, end - start + 1);
                        neuronSubstring = neuronSubstring.Replace(str, string.Empty);
                        weights[iteration][iter][i] = float.Parse(str.Substring(str.IndexOf(':') + 1));
                    }

                    AllLayers[iteration][iter].InputConnections      = AllLayers[Math.Max(0, iteration - 1)];
                    AllLayers[iteration][iter].InputConnectionsCount = AllLayers[Math.Max(0, iteration - 1)].Length - 1;
                    AllLayers[iteration][iter].OutputConnections     = AllLayers[Math.Min(AllLayers.Length - 1, iteration + 1)];
                    AllLayers[iteration][iter].Weight                = weights[iteration][iter];
                }
            }
        }

        public object Clone()
        {
            Brain brain                = new Brain();
            brain.Inputs               = new Neuron[this.Inputs.Length];
            brain.Outputs              = new Neuron[this.Outputs.Length];
            brain.HiddenLayers         = new Neuron[this.HiddenLayers.Length][];
            brain.AllLayers            = new Neuron[this.HiddenLayers.Length + 2][];
            brain._activationFunction   = this._activationFunction;


            for (int i = 0; i < Inputs.Length; i++)
            {
                brain.Inputs[i] = this.Inputs[i].Clone() as Neuron;
            }

            for (int i = 0; i < Outputs.Length; i++)
            {
                brain.Outputs[i] = this.Outputs[i].Clone() as Neuron;
            }

            for (int iter = 0; iter < HiddenLayers.Length; iter++)
            {
                brain.HiddenLayers[iter] = new Neuron[HiddenLayers[iter].Length];

                for (int i = 0; i < HiddenLayers[iter].Length; i++)
                {
                    brain.HiddenLayers[iter][i] = this.HiddenLayers[iter][i].Clone() as Neuron;
                }
            }

            Array.Copy(brain.HiddenLayers, 0, brain.AllLayers, 1, brain.HiddenLayers.Length);
            brain.AllLayers[0] = brain.Inputs;
            brain.AllLayers[AllLayers.Length - 1] = brain.Outputs;

            for (int iter = 0; iter < brain.AllLayers.Length; iter++)
            {
                for (int i = 0; i < brain.AllLayers[iter].Length; i++)
                {
                    Neuron neuron = brain.AllLayers[iter][i];
                    neuron.InputConnections = brain.AllLayers[Math.Max(0, iter - 1)];
                    neuron.OutputConnections = brain.AllLayers[Math.Min(brain.AllLayers.Length - 1, iter + 1)];
                }
            }

            return brain;
        }
        #endregion
    }
}
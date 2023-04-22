namespace NeuralNetwork
{
    public class Neuron : ICloneable
    {
        #region ------------- Types and constants -------------------------------------------------
        public enum NeuronType { HiddenNeuron, InputNeuron, OutputNeuron }
        #endregion



        #region ------------- Properties ----------------------------------------------------------
        public string             ID; // Name
        public int                LayerIndex;
        public NeuronType         Type;

        /// <summary>
        /// Input neurons to us
        /// </summary>
        public Neuron[]           InputConnections;
        public int                InputConnectionsCount = 0;
        public Neuron[]           OutputConnections;
        public float              Activation = 0.0F;
        public float              Bias;
        public float[]            Weight;
        public int                ReceivedInputs = 0;
        public Func<float, float> ActivationFunction;
        public float              Delta_i;
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public void Activate(Neuron sender, float value)
        {
            if (Type == NeuronType.InputNeuron)
            {
                Activation = value;
                SendValue();
            }

            else if (Type == NeuronType.OutputNeuron)
            {
                int i = sender.LayerIndex;
                Activation += value * Weight[i];
            }

            else
            {
                int i = sender.LayerIndex;
                Activation += value * Weight[i];
                ReceivedInputs++;

                if (ReceivedInputs == InputConnections.Length)
                {
                    SendValue();
                }
            }
        }

        public static float Sigmoid(float x) // sigmoid function
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        public static float ReLU(float x)
        {
            return (float)Math.Max(0, x);
        }

        public object Clone()
        {
            Neuron neuron = new Neuron();
            neuron.Type = this.Type;
            neuron.ID = this.ID;
            neuron.LayerIndex = this.LayerIndex;
            neuron.Bias = this.Bias;
            neuron.Weight = new float[this.Weight.Length];
            neuron.ActivationFunction = this.ActivationFunction;

            for (int i = 0; i < Weight.Length; i++)
            {
                neuron.Weight[i] = this.Weight[i];
            }

            return neuron;
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private void SendValue()
        {
            for (int i = 0; i < OutputConnections.Length; i++)
            {
                if (!float.IsNaN(ActivationFunction(Activation)))
                    OutputConnections[i].Activate(this, ActivationFunction(Activation));
                else
                    OutputConnections[i].Activate(this, 0);
            }
        }
        #endregion
    }
}

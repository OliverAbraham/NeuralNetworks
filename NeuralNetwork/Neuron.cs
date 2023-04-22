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
        public Neuron[]           Inputs;
        public int                InputCount = 0;
        public Neuron[]           Outputs;
        public float              Activation = 0.0F;
        public float              Bias;
        public float[]            Weights;
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
                ActivateNextLayer();
            }

            else if (Type == NeuronType.OutputNeuron)
            {
                int i = sender.LayerIndex;
                Activation += value * Weights[i];
            }

            else
            {
                int i = sender.LayerIndex;
                Activation += value * Weights[i];
                ReceivedInputs++;

                if (ReceivedInputs == Inputs.Length)
                {
                    ActivateNextLayer();
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
            Neuron neuron             = new Neuron();
            neuron.Type               = this.Type;
            neuron.ID                 = this.ID;
            neuron.LayerIndex         = this.LayerIndex;
            neuron.Bias               = this.Bias;
            neuron.Weights             = new float[this.Weights.Length];
            neuron.ActivationFunction = this.ActivationFunction;

            for (int i = 0; i < Weights.Length; i++)
            {
                neuron.Weights[i] = this.Weights[i];
            }

            return neuron;
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private void ActivateNextLayer()
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (!float.IsNaN(ActivationFunction(Activation)))
                    Outputs[i].Activate(this, ActivationFunction(Activation));
                else
                    Outputs[i].Activate(this, 0);
            }
        }
        #endregion
    }
}

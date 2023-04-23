namespace NeuralNetwork
{
    public class Neuron : ICloneable
    {
        #region ------------- Types and constants -------------------------------------------------
        public enum NeuronType { InputNeuron, HiddenNeuron, OutputNeuron }
        #endregion



        #region ------------- Properties ----------------------------------------------------------
        public float              Bias    { get; set; }
        public float[]            Weights { get; set; }
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        public int                _layer;
        public int                _layerIndex;
        public NeuronType         _type;

        [Newtonsoft.Json.JsonIgnore]
        public Neuron[]           _inputs;

        [Newtonsoft.Json.JsonIgnore]
        public Neuron[]           _outputs;

        [Newtonsoft.Json.JsonIgnore]
        public Func<float, float> _activationFunction;

        [Newtonsoft.Json.JsonIgnore]
        public int                _inputCount = 0;

        [Newtonsoft.Json.JsonIgnore]
        public float              _activation = 0.0F;

        [Newtonsoft.Json.JsonIgnore]
        public int                _receivedInputs = 0;

        [Newtonsoft.Json.JsonIgnore]
        public float              _delta_i;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public void Activate(Neuron sender, float value)
        {
            if (_type == NeuronType.InputNeuron)
            {
                _activation = value;
                ActivateNextLayer();
            }

            else if (_type == NeuronType.OutputNeuron)
            {
                int i = sender._layerIndex;
                _activation += value * Weights[i];
            }

            else
            {
                int i = sender._layerIndex;
                _activation += value * Weights[i];
                _receivedInputs++;

                if (_receivedInputs == _inputs.Length)
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
            neuron._type               = this._type;
            neuron._layerIndex         = this._layerIndex;
            neuron.Bias               = this.Bias;
            neuron.Weights             = new float[this.Weights.Length];
            neuron._activationFunction = this._activationFunction;

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
            for (int i = 0; i < _outputs.Length; i++)
            {
                if (!float.IsNaN(_activationFunction(_activation)))
                    _outputs[i].Activate(this, _activationFunction(_activation));
                else
                    _outputs[i].Activate(this, 0);
            }
        }
        #endregion
    }
}

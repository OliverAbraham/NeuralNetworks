namespace WinFormsUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            buttonStartTraining = new Button();
            save_button = new Button();
            labelTrainingClassification = new Label();
            label1 = new Label();
            labelHandwritingInput = new Label();
            label3 = new Label();
            labelStatus = new Label();
            buttonLoadTrainingData = new Button();
            label4 = new Label();
            label5 = new Label();
            labelClassificationResults = new Label();
            labelClassification = new Label();
            buttonLoadNetwork = new Button();
            labelIterations = new Label();
            textBoxStopAfterIterations = new TextBox();
            labelStopWhen = new Label();
            textBoxStopIfAccuracyIs = new TextBox();
            buttonStopTraining = new Button();
            label6 = new Label();
            textBoxTrainingSpeed = new TextBox();
            labelNeuronsInOutputLayer = new Label();
            textBoxNeuronsInOutputLayer = new TextBox();
            labelNeuronsInHiddenLayer = new Label();
            textBoxNeuronsInHiddenLayers = new TextBox();
            labelHiddenLayers = new Label();
            textBoxHiddenLayers = new TextBox();
            labelNeuronsInInputLayer = new Label();
            textBoxNeuronsInInputLayer = new TextBox();
            buttonResetNetwork = new Button();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            SuspendLayout();
            // 
            // buttonStartTraining
            // 
            buttonStartTraining.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonStartTraining.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonStartTraining.Location = new Point(378, 984);
            buttonStartTraining.Margin = new Padding(5, 6, 5, 6);
            buttonStartTraining.Name = "buttonStartTraining";
            buttonStartTraining.Size = new Size(214, 56);
            buttonStartTraining.TabIndex = 0;
            buttonStartTraining.Text = "Train network";
            buttonStartTraining.UseVisualStyleBackColor = true;
            buttonStartTraining.Click += buttonStartTraining_Click;
            // 
            // save_button
            // 
            save_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            save_button.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            save_button.Location = new Point(942, 985);
            save_button.Margin = new Padding(5, 6, 5, 6);
            save_button.Name = "save_button";
            save_button.Size = new Size(239, 56);
            save_button.TabIndex = 2;
            save_button.Text = "Save network";
            save_button.UseVisualStyleBackColor = true;
            save_button.Click += buttonSaveNetwork_Click;
            // 
            // labelTrainingClassification
            // 
            labelTrainingClassification.AutoSize = true;
            labelTrainingClassification.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point);
            labelTrainingClassification.Location = new Point(457, 71);
            labelTrainingClassification.Margin = new Padding(5, 0, 5, 0);
            labelTrainingClassification.Name = "labelTrainingClassification";
            labelTrainingClassification.Size = new Size(45, 36);
            labelTrainingClassification.TabIndex = 3;
            labelTrainingClassification.Text = "---";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 11);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(209, 25);
            label1.TabIndex = 5;
            label1.Text = "Current training image:";
            // 
            // labelHandwritingInput
            // 
            labelHandwritingInput.AutoSize = true;
            labelHandwritingInput.Location = new Point(1442, 9);
            labelHandwritingInput.Margin = new Padding(5, 0, 5, 0);
            labelHandwritingInput.Name = "labelHandwritingInput";
            labelHandwritingInput.Size = new Size(167, 25);
            labelHandwritingInput.TabIndex = 6;
            labelHandwritingInput.Text = "Handwriting input:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(457, 11);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(72, 25);
            label3.TabIndex = 7;
            label3.Text = "Result:";
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(308, 893);
            labelStatus.Margin = new Padding(5, 0, 5, 0);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(33, 25);
            labelStatus.TabIndex = 8;
            labelStatus.Text = "---";
            // 
            // buttonLoadTrainingData
            // 
            buttonLoadTrainingData.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonLoadTrainingData.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonLoadTrainingData.Location = new Point(29, 985);
            buttonLoadTrainingData.Margin = new Padding(5, 6, 5, 6);
            buttonLoadTrainingData.Name = "buttonLoadTrainingData";
            buttonLoadTrainingData.Size = new Size(294, 55);
            buttonLoadTrainingData.TabIndex = 9;
            buttonLoadTrainingData.Text = "Load training data";
            buttonLoadTrainingData.UseVisualStyleBackColor = true;
            buttonLoadTrainingData.Click += buttonLoadTrainingData_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(21, 893);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(79, 25);
            label4.TabIndex = 10;
            label4.Text = "Status: ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(2141, 10);
            label5.Margin = new Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new Size(72, 25);
            label5.TabIndex = 12;
            label5.Text = "Result:";
            // 
            // labelClassificationResults
            // 
            labelClassificationResults.AutoSize = true;
            labelClassificationResults.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point);
            labelClassificationResults.Location = new Point(2141, 70);
            labelClassificationResults.Margin = new Padding(5, 0, 5, 0);
            labelClassificationResults.Name = "labelClassificationResults";
            labelClassificationResults.Size = new Size(45, 36);
            labelClassificationResults.TabIndex = 11;
            labelClassificationResults.Text = "---";
            // 
            // labelClassification
            // 
            labelClassification.AutoSize = true;
            labelClassification.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Regular, GraphicsUnit.Point);
            labelClassification.Location = new Point(2132, 512);
            labelClassification.Margin = new Padding(5, 0, 5, 0);
            labelClassification.Name = "labelClassification";
            labelClassification.Size = new Size(107, 82);
            labelClassification.TabIndex = 13;
            labelClassification.Text = "---";
            // 
            // buttonLoadNetwork
            // 
            buttonLoadNetwork.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonLoadNetwork.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonLoadNetwork.Location = new Point(1219, 985);
            buttonLoadNetwork.Margin = new Padding(5, 6, 5, 6);
            buttonLoadNetwork.Name = "buttonLoadNetwork";
            buttonLoadNetwork.Size = new Size(249, 56);
            buttonLoadNetwork.TabIndex = 14;
            buttonLoadNetwork.Text = "Load network";
            buttonLoadNetwork.UseVisualStyleBackColor = true;
            buttonLoadNetwork.Click += buttonLoadNetwork_Click;
            // 
            // labelIterations
            // 
            labelIterations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelIterations.AutoSize = true;
            labelIterations.Location = new Point(21, 776);
            labelIterations.Margin = new Padding(5, 0, 5, 0);
            labelIterations.Name = "labelIterations";
            labelIterations.Size = new Size(185, 25);
            labelIterations.TabIndex = 16;
            labelIterations.Text = "Stop after iterations:";
            // 
            // textBoxStopAfterIterations
            // 
            textBoxStopAfterIterations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxStopAfterIterations.Location = new Point(308, 773);
            textBoxStopAfterIterations.Margin = new Padding(4);
            textBoxStopAfterIterations.Name = "textBoxStopAfterIterations";
            textBoxStopAfterIterations.Size = new Size(132, 30);
            textBoxStopAfterIterations.TabIndex = 17;
            // 
            // labelStopWhen
            // 
            labelStopWhen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStopWhen.AutoSize = true;
            labelStopWhen.Location = new Point(21, 815);
            labelStopWhen.Margin = new Padding(5, 0, 5, 0);
            labelStopWhen.Name = "labelStopWhen";
            labelStopWhen.Size = new Size(255, 25);
            labelStopWhen.TabIndex = 18;
            labelStopWhen.Text = "Stop when total accuracy is:";
            // 
            // textBoxStopIfAccuracyIs
            // 
            textBoxStopIfAccuracyIs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxStopIfAccuracyIs.Location = new Point(308, 812);
            textBoxStopIfAccuracyIs.Margin = new Padding(4);
            textBoxStopIfAccuracyIs.Name = "textBoxStopIfAccuracyIs";
            textBoxStopIfAccuracyIs.Size = new Size(132, 30);
            textBoxStopIfAccuracyIs.TabIndex = 19;
            // 
            // buttonStopTraining
            // 
            buttonStopTraining.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonStopTraining.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonStopTraining.Location = new Point(615, 985);
            buttonStopTraining.Margin = new Padding(5, 6, 5, 6);
            buttonStopTraining.Name = "buttonStopTraining";
            buttonStopTraining.Size = new Size(124, 56);
            buttonStopTraining.TabIndex = 20;
            buttonStopTraining.Text = "Stop";
            buttonStopTraining.UseVisualStyleBackColor = true;
            buttonStopTraining.Click += buttonCancelTraining_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(21, 854);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(148, 25);
            label6.TabIndex = 21;
            label6.Text = "Training speed:";
            // 
            // textBoxTrainingSpeed
            // 
            textBoxTrainingSpeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxTrainingSpeed.Location = new Point(308, 851);
            textBoxTrainingSpeed.Margin = new Padding(4);
            textBoxTrainingSpeed.Name = "textBoxTrainingSpeed";
            textBoxTrainingSpeed.Size = new Size(132, 30);
            textBoxTrainingSpeed.TabIndex = 22;
            // 
            // labelNeuronsInOutputLayer
            // 
            labelNeuronsInOutputLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelNeuronsInOutputLayer.AutoSize = true;
            labelNeuronsInOutputLayer.Location = new Point(21, 636);
            labelNeuronsInOutputLayer.Margin = new Padding(5, 0, 5, 0);
            labelNeuronsInOutputLayer.Name = "labelNeuronsInOutputLayer";
            labelNeuronsInOutputLayer.Size = new Size(218, 25);
            labelNeuronsInOutputLayer.TabIndex = 23;
            labelNeuronsInOutputLayer.Text = "Neurons in output layer:";
            // 
            // textBoxNeuronsInOutputLayer
            // 
            textBoxNeuronsInOutputLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxNeuronsInOutputLayer.Enabled = false;
            textBoxNeuronsInOutputLayer.Location = new Point(308, 633);
            textBoxNeuronsInOutputLayer.Margin = new Padding(4);
            textBoxNeuronsInOutputLayer.Name = "textBoxNeuronsInOutputLayer";
            textBoxNeuronsInOutputLayer.Size = new Size(132, 30);
            textBoxNeuronsInOutputLayer.TabIndex = 24;
            textBoxNeuronsInOutputLayer.Text = "10";
            // 
            // labelNeuronsInHiddenLayer
            // 
            labelNeuronsInHiddenLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelNeuronsInHiddenLayer.AutoSize = true;
            labelNeuronsInHiddenLayer.Location = new Point(21, 597);
            labelNeuronsInHiddenLayer.Margin = new Padding(5, 0, 5, 0);
            labelNeuronsInHiddenLayer.Name = "labelNeuronsInHiddenLayer";
            labelNeuronsInHiddenLayer.Size = new Size(223, 25);
            labelNeuronsInHiddenLayer.TabIndex = 25;
            labelNeuronsInHiddenLayer.Text = "Neurons in hidden layer:";
            // 
            // textBoxNeuronsInHiddenLayers
            // 
            textBoxNeuronsInHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxNeuronsInHiddenLayers.Location = new Point(308, 594);
            textBoxNeuronsInHiddenLayers.Margin = new Padding(4);
            textBoxNeuronsInHiddenLayers.Name = "textBoxNeuronsInHiddenLayers";
            textBoxNeuronsInHiddenLayers.Size = new Size(132, 30);
            textBoxNeuronsInHiddenLayers.TabIndex = 26;
            textBoxNeuronsInHiddenLayers.Text = "16";
            // 
            // labelHiddenLayers
            // 
            labelHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelHiddenLayers.AutoSize = true;
            labelHiddenLayers.Location = new Point(21, 558);
            labelHiddenLayers.Margin = new Padding(5, 0, 5, 0);
            labelHiddenLayers.Name = "labelHiddenLayers";
            labelHiddenLayers.Size = new Size(137, 25);
            labelHiddenLayers.TabIndex = 27;
            labelHiddenLayers.Text = "Hidden layers:";
            // 
            // textBoxHiddenLayers
            // 
            textBoxHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxHiddenLayers.Location = new Point(308, 555);
            textBoxHiddenLayers.Margin = new Padding(4);
            textBoxHiddenLayers.Name = "textBoxHiddenLayers";
            textBoxHiddenLayers.Size = new Size(132, 30);
            textBoxHiddenLayers.TabIndex = 28;
            textBoxHiddenLayers.Text = "2";
            // 
            // labelNeuronsInInputLayer
            // 
            labelNeuronsInInputLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelNeuronsInInputLayer.AutoSize = true;
            labelNeuronsInInputLayer.Location = new Point(21, 519);
            labelNeuronsInInputLayer.Margin = new Padding(5, 0, 5, 0);
            labelNeuronsInInputLayer.Name = "labelNeuronsInInputLayer";
            labelNeuronsInInputLayer.Size = new Size(206, 25);
            labelNeuronsInInputLayer.TabIndex = 29;
            labelNeuronsInInputLayer.Text = "Neurons in input layer:";
            // 
            // textBoxNeuronsInInputLayer
            // 
            textBoxNeuronsInInputLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxNeuronsInInputLayer.Enabled = false;
            textBoxNeuronsInInputLayer.Location = new Point(308, 516);
            textBoxNeuronsInInputLayer.Margin = new Padding(4);
            textBoxNeuronsInInputLayer.Name = "textBoxNeuronsInInputLayer";
            textBoxNeuronsInInputLayer.Size = new Size(132, 30);
            textBoxNeuronsInInputLayer.TabIndex = 30;
            textBoxNeuronsInInputLayer.Text = "784";
            // 
            // buttonResetNetwork
            // 
            buttonResetNetwork.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonResetNetwork.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            buttonResetNetwork.Location = new Point(21, 667);
            buttonResetNetwork.Margin = new Padding(5, 6, 5, 6);
            buttonResetNetwork.Name = "buttonResetNetwork";
            buttonResetNetwork.Size = new Size(167, 32);
            buttonResetNetwork.TabIndex = 31;
            buttonResetNetwork.Text = "Reset network";
            buttonResetNetwork.UseVisualStyleBackColor = true;
            buttonResetNetwork.Click += buttonResetNetwork_Click;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label7.Location = new Point(21, 481);
            label7.Margin = new Padding(5, 0, 5, 0);
            label7.Name = "label7";
            label7.Size = new Size(204, 25);
            label7.TabIndex = 32;
            label7.Text = "Network parameters";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label8.Location = new Point(21, 739);
            label8.Margin = new Padding(5, 0, 5, 0);
            label8.Name = "label8";
            label8.Size = new Size(91, 25);
            label8.TabIndex = 33;
            label8.Text = "Training";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(660, 9);
            label9.Margin = new Padding(5, 0, 5, 0);
            label9.Name = "label9";
            label9.Size = new Size(89, 25);
            label9.TabIndex = 34;
            label9.Text = "Network:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2251, 1079);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(buttonResetNetwork);
            Controls.Add(textBoxNeuronsInInputLayer);
            Controls.Add(labelNeuronsInInputLayer);
            Controls.Add(textBoxHiddenLayers);
            Controls.Add(labelHiddenLayers);
            Controls.Add(textBoxNeuronsInHiddenLayers);
            Controls.Add(labelNeuronsInHiddenLayer);
            Controls.Add(textBoxNeuronsInOutputLayer);
            Controls.Add(labelNeuronsInOutputLayer);
            Controls.Add(textBoxTrainingSpeed);
            Controls.Add(label6);
            Controls.Add(buttonStopTraining);
            Controls.Add(textBoxStopIfAccuracyIs);
            Controls.Add(labelStopWhen);
            Controls.Add(textBoxStopAfterIterations);
            Controls.Add(labelIterations);
            Controls.Add(buttonLoadNetwork);
            Controls.Add(labelClassification);
            Controls.Add(label5);
            Controls.Add(labelClassificationResults);
            Controls.Add(label4);
            Controls.Add(buttonLoadTrainingData);
            Controls.Add(labelStatus);
            Controls.Add(label3);
            Controls.Add(labelHandwritingInput);
            Controls.Add(label1);
            Controls.Add(labelTrainingClassification);
            Controls.Add(save_button);
            Controls.Add(buttonStartTraining);
            DoubleBuffered = true;
            Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(5, 6, 5, 6);
            Name = "Form1";
            Text = "Handwriting recognition with a Neural network";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Paint += MainForm_Paint;
            Resize += MainForm_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonStartTraining;
        private Button save_button;
        private Label labelTrainingClassification;
        private Label label1;
        private Label labelHandwritingInput;
        private Label label3;
        private Label labelStatus;
        private Button buttonLoadTrainingData;
        private Label label4;
        private Label label5;
        private Label labelClassificationResults;
        private Label labelClassification;
        private Button buttonLoadNetwork;
        private Label labelIterations;
        private TextBox textBoxStopAfterIterations;
        private Label labelStopWhen;
        private TextBox textBoxStopIfAccuracyIs;
        private Button buttonStopTraining;
        private Label label6;
        private TextBox textBoxTrainingSpeed;
        private Label labelNeuronsInOutputLayer;
        private TextBox textBoxNeuronsInOutputLayer;
        private Label labelNeuronsInHiddenLayer;
        private TextBox textBoxNeuronsInHiddenLayers;
        private Label labelHiddenLayers;
        private TextBox textBoxHiddenLayers;
        private Label labelNeuronsInInputLayer;
        private TextBox textBoxNeuronsInInputLayer;
        private Button buttonResetNetwork;
        private Label label7;
        private Label label8;
        private Label label9;
    }
}
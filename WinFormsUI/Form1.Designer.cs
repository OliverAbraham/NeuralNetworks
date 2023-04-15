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
            label2 = new Label();
            label3 = new Label();
            labelStatus = new Label();
            buttonLoadTrainingData = new Button();
            label4 = new Label();
            label5 = new Label();
            labelClassificationResults = new Label();
            labelClassification = new Label();
            buttonLoadNetwork = new Button();
            button1 = new Button();
            labelIterations = new Label();
            textBoxStopAfterIterations = new TextBox();
            labelStopWhen = new Label();
            textBoxStopIfAccuracyIs = new TextBox();
            buttonCancelTraining = new Button();
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
            SuspendLayout();
            // 
            // buttonStartTraining
            // 
            buttonStartTraining.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonStartTraining.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonStartTraining.Location = new Point(378, 984);
            buttonStartTraining.Margin = new Padding(5, 6, 5, 6);
            buttonStartTraining.Name = "buttonStartTraining";
            buttonStartTraining.Size = new Size(244, 56);
            buttonStartTraining.TabIndex = 0;
            buttonStartTraining.Text = "Train network";
            buttonStartTraining.UseVisualStyleBackColor = true;
            buttonStartTraining.Click += buttonStartTraining_Click;
            // 
            // save_button
            // 
            save_button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            save_button.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            save_button.Location = new Point(999, 985);
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
            label1.Size = new Size(147, 25);
            label1.TabIndex = 5;
            label1.Text = "Training image:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(800, 12);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(167, 25);
            label2.TabIndex = 6;
            label2.Text = "Handwriting input:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(457, 11);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(82, 25);
            label3.TabIndex = 7;
            label3.Text = "Results:";
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(308, 901);
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
            label4.Location = new Point(21, 901);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(79, 25);
            label4.TabIndex = 10;
            label4.Text = "Status: ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1699, 12);
            label5.Margin = new Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new Size(82, 25);
            label5.TabIndex = 12;
            label5.Text = "Results:";
            // 
            // labelClassificationResults
            // 
            labelClassificationResults.AutoSize = true;
            labelClassificationResults.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point);
            labelClassificationResults.Location = new Point(1699, 72);
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
            labelClassification.Location = new Point(1690, 514);
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
            buttonLoadNetwork.Location = new Point(1276, 985);
            buttonLoadNetwork.Margin = new Padding(5, 6, 5, 6);
            buttonLoadNetwork.Name = "buttonLoadNetwork";
            buttonLoadNetwork.Size = new Size(249, 56);
            buttonLoadNetwork.TabIndex = 14;
            buttonLoadNetwork.Text = "Load network";
            buttonLoadNetwork.UseVisualStyleBackColor = true;
            buttonLoadNetwork.Click += buttonLoadNetwork_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(1622, 985);
            button1.Margin = new Padding(5, 6, 5, 6);
            button1.Name = "button1";
            button1.Size = new Size(137, 56);
            button1.TabIndex = 15;
            button1.Text = "Check";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonCheck_Click;
            // 
            // labelIterations
            // 
            labelIterations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelIterations.AutoSize = true;
            labelIterations.Location = new Point(21, 776);
            labelIterations.Margin = new Padding(5, 0, 5, 0);
            labelIterations.Name = "labelIterations";
            labelIterations.Size = new Size(172, 25);
            labelIterations.TabIndex = 16;
            labelIterations.Text = "Training iterations:";
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
            labelStopWhen.Location = new Point(21, 818);
            labelStopWhen.Margin = new Padding(5, 0, 5, 0);
            labelStopWhen.Name = "labelStopWhen";
            labelStopWhen.Size = new Size(255, 25);
            labelStopWhen.TabIndex = 18;
            labelStopWhen.Text = "Stop when total accuracy is:";
            // 
            // textBoxStopIfAccuracyIs
            // 
            textBoxStopIfAccuracyIs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxStopIfAccuracyIs.Location = new Point(308, 815);
            textBoxStopIfAccuracyIs.Margin = new Padding(4);
            textBoxStopIfAccuracyIs.Name = "textBoxStopIfAccuracyIs";
            textBoxStopIfAccuracyIs.Size = new Size(132, 30);
            textBoxStopIfAccuracyIs.TabIndex = 19;
            // 
            // buttonCancelTraining
            // 
            buttonCancelTraining.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonCancelTraining.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCancelTraining.Location = new Point(656, 985);
            buttonCancelTraining.Margin = new Padding(5, 6, 5, 6);
            buttonCancelTraining.Name = "buttonCancelTraining";
            buttonCancelTraining.Size = new Size(154, 56);
            buttonCancelTraining.TabIndex = 20;
            buttonCancelTraining.Text = "Cancel";
            buttonCancelTraining.UseVisualStyleBackColor = true;
            buttonCancelTraining.Click += buttonCancelTraining_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(21, 859);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(148, 25);
            label6.TabIndex = 21;
            label6.Text = "Training speed:";
            // 
            // textBoxTrainingSpeed
            // 
            textBoxTrainingSpeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxTrainingSpeed.Location = new Point(308, 856);
            textBoxTrainingSpeed.Margin = new Padding(4);
            textBoxTrainingSpeed.Name = "textBoxTrainingSpeed";
            textBoxTrainingSpeed.Size = new Size(132, 30);
            textBoxTrainingSpeed.TabIndex = 22;
            // 
            // labelNeuronsInOutputLayer
            // 
            labelNeuronsInOutputLayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelNeuronsInOutputLayer.AutoSize = true;
            labelNeuronsInOutputLayer.Location = new Point(21, 735);
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
            textBoxNeuronsInOutputLayer.Location = new Point(308, 732);
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
            labelNeuronsInHiddenLayer.Location = new Point(21, 696);
            labelNeuronsInHiddenLayer.Margin = new Padding(5, 0, 5, 0);
            labelNeuronsInHiddenLayer.Name = "labelNeuronsInHiddenLayer";
            labelNeuronsInHiddenLayer.Size = new Size(223, 25);
            labelNeuronsInHiddenLayer.TabIndex = 25;
            labelNeuronsInHiddenLayer.Text = "Neurons in hidden layer:";
            // 
            // textBoxNeuronsInHiddenLayer
            // 
            textBoxNeuronsInHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxNeuronsInHiddenLayers.Location = new Point(308, 693);
            textBoxNeuronsInHiddenLayers.Margin = new Padding(4);
            textBoxNeuronsInHiddenLayers.Name = "textBoxNeuronsInHiddenLayer";
            textBoxNeuronsInHiddenLayers.Size = new Size(132, 30);
            textBoxNeuronsInHiddenLayers.TabIndex = 26;
            textBoxNeuronsInHiddenLayers.Text = "16";
            // 
            // labelHiddenLayers
            // 
            labelHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelHiddenLayers.AutoSize = true;
            labelHiddenLayers.Location = new Point(21, 657);
            labelHiddenLayers.Margin = new Padding(5, 0, 5, 0);
            labelHiddenLayers.Name = "labelHiddenLayers";
            labelHiddenLayers.Size = new Size(137, 25);
            labelHiddenLayers.TabIndex = 27;
            labelHiddenLayers.Text = "Hidden layers:";
            // 
            // textBoxHiddenLayers
            // 
            textBoxHiddenLayers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxHiddenLayers.Location = new Point(308, 654);
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
            labelNeuronsInInputLayer.Location = new Point(21, 618);
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
            textBoxNeuronsInInputLayer.Location = new Point(308, 615);
            textBoxNeuronsInInputLayer.Margin = new Padding(4);
            textBoxNeuronsInInputLayer.Name = "textBoxNeuronsInInputLayer";
            textBoxNeuronsInInputLayer.Size = new Size(132, 30);
            textBoxNeuronsInInputLayer.TabIndex = 30;
            textBoxNeuronsInInputLayer.Text = "784";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1825, 1079);
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
            Controls.Add(buttonCancelTraining);
            Controls.Add(textBoxStopIfAccuracyIs);
            Controls.Add(labelStopWhen);
            Controls.Add(textBoxStopAfterIterations);
            Controls.Add(labelIterations);
            Controls.Add(button1);
            Controls.Add(buttonLoadNetwork);
            Controls.Add(labelClassification);
            Controls.Add(label5);
            Controls.Add(labelClassificationResults);
            Controls.Add(label4);
            Controls.Add(buttonLoadTrainingData);
            Controls.Add(labelStatus);
            Controls.Add(label3);
            Controls.Add(label2);
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
        private Label label2;
        private Label label3;
        private Label labelStatus;
        private Button buttonLoadTrainingData;
        private Label label4;
        private Label label5;
        private Label labelClassificationResults;
        private Label labelClassification;
        private Button buttonLoadNetwork;
        private Button button1;
        private Label labelIterations;
        private TextBox textBoxStopAfterIterations;
        private Label labelStopWhen;
        private TextBox textBoxStopIfAccuracyIs;
        private Button buttonCancelTraining;
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
    }
}
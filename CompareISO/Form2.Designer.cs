namespace CompareISO
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            instructionsLabel = new Label();
            yesButton = new Button();
            noButton = new Button();
            SuspendLayout();
            // 
            // instructionsLabel
            // 
            instructionsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            instructionsLabel.Location = new Point(17, 13);
            instructionsLabel.Margin = new Padding(5, 0, 5, 0);
            instructionsLabel.Name = "instructionsLabel";
            instructionsLabel.Size = new Size(499, 69);
            instructionsLabel.TabIndex = 3;
            instructionsLabel.Text = "The directory \"############\" is not empty; do you want to empty it first?";
            instructionsLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // yesButton
            // 
            yesButton.Location = new Point(70, 94);
            yesButton.Margin = new Padding(4);
            yesButton.Name = "yesButton";
            yesButton.Size = new Size(161, 73);
            yesButton.TabIndex = 7;
            yesButton.Text = "Yes";
            yesButton.UseVisualStyleBackColor = true;
            yesButton.Click += yesButton_Click;
            // 
            // noButton
            // 
            noButton.Location = new Point(300, 94);
            noButton.Margin = new Padding(4);
            noButton.Name = "noButton";
            noButton.Size = new Size(161, 73);
            noButton.TabIndex = 8;
            noButton.Text = "No";
            noButton.UseVisualStyleBackColor = true;
            noButton.Click += noButton_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(530, 179);
            ControlBox = false;
            Controls.Add(noButton);
            Controls.Add(yesButton);
            Controls.Add(instructionsLabel);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form2";
            Text = "Directory not empty";
            ResumeLayout(false);
        }

        #endregion

        private Label instructionsLabel;
        private Button yesButton;
        private Button noButton;
    }
}
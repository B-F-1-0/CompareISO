namespace CompareISO
{
    partial class Form3
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
            tableRichTextBox = new RichTextBox();
            exitButton = new Button();
            SuspendLayout();
            // 
            // instructionsLabel
            // 
            instructionsLabel.Location = new Point(12, 20);
            instructionsLabel.Name = "instructionsLabel";
            instructionsLabel.Size = new Size(552, 52);
            instructionsLabel.TabIndex = 0;
            instructionsLabel.Text = "The table has been generated below. You may copy the contents of this table and paste its contents on a document page on a MediaWiki-based wiki.";
            instructionsLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // tableRichTextBox
            // 
            tableRichTextBox.Location = new Point(12, 75);
            tableRichTextBox.Name = "tableRichTextBox";
            tableRichTextBox.ReadOnly = true;
            tableRichTextBox.Size = new Size(552, 378);
            tableRichTextBox.TabIndex = 1;
            tableRichTextBox.Text = "";
            // 
            // exitButton
            // 
            exitButton.Location = new Point(227, 459);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(125, 52);
            exitButton.TabIndex = 6;
            exitButton.Text = "Exit";
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += exitButton_Click;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(576, 521);
            Controls.Add(exitButton);
            Controls.Add(tableRichTextBox);
            Controls.Add(instructionsLabel);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form3";
            Text = "Wiki Table Generator";
            ResumeLayout(false);
        }

        #endregion

        private Label instructionsLabel;
        private RichTextBox tableRichTextBox;
        private Button exitButton;
    }
}
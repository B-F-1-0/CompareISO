namespace CompareISO
{
    partial class Form4
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
            groupBox1 = new GroupBox();
            okButton = new Button();
            groupBox2 = new GroupBox();
            iso1IndexList = new ListBox();
            iso2IndexList = new ListBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // instructionsLabel
            // 
            instructionsLabel.Location = new Point(22, 9);
            instructionsLabel.Name = "instructionsLabel";
            instructionsLabel.Size = new Size(632, 62);
            instructionsLabel.TabIndex = 0;
            instructionsLabel.Text = "The following images were found on the INSTALL.WIMs. Please select the images you like to compare then press OK.";
            instructionsLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(iso1IndexList);
            groupBox1.Location = new Point(22, 94);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(310, 231);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "ISO #1's INSTALL.WIM";
            // 
            // okButton
            // 
            okButton.Location = new Point(276, 341);
            okButton.Name = "okButton";
            okButton.Size = new Size(125, 52);
            okButton.TabIndex = 7;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(iso2IndexList);
            groupBox2.Location = new Point(344, 94);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(310, 231);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "ISO #2's INSTALL.WIM";
            // 
            // iso1IndexList
            // 
            iso1IndexList.FormattingEnabled = true;
            iso1IndexList.ItemHeight = 21;
            iso1IndexList.Location = new Point(6, 28);
            iso1IndexList.Name = "iso1IndexList";
            iso1IndexList.Size = new Size(298, 193);
            iso1IndexList.TabIndex = 8;
            // 
            // iso2IndexList
            // 
            iso2IndexList.FormattingEnabled = true;
            iso2IndexList.ItemHeight = 21;
            iso2IndexList.Location = new Point(6, 28);
            iso2IndexList.Name = "iso2IndexList";
            iso2IndexList.Size = new Size(298, 193);
            iso2IndexList.TabIndex = 9;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(676, 405);
            ControlBox = false;
            Controls.Add(groupBox2);
            Controls.Add(okButton);
            Controls.Add(groupBox1);
            Controls.Add(instructionsLabel);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form4";
            Text = "Select Images";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label instructionsLabel;
        private GroupBox groupBox1;
        private Button okButton;
        private GroupBox groupBox2;
        private ListBox iso1IndexList;
        private ListBox iso2IndexList;
    }
}
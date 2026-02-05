namespace CompareISO
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ISO1button = new Button();
            ISO2button = new Button();
            instructionsLabel = new Label();
            groupBox1 = new GroupBox();
            addedFilesRichTextBox = new RichTextBox();
            groupBox2 = new GroupBox();
            removedFilesRichTextBox = new RichTextBox();
            exitButton = new Button();
            compareButton = new Button();
            wikiTableButton = new Button();
            openISO1FileDialog = new OpenFileDialog();
            iso1FileNameTextBox = new TextBox();
            iso2FileNameTextBox = new TextBox();
            extractProgressBar = new ProgressBar();
            progressLabel = new Label();
            tempFolderButton = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // ISO1button
            // 
            ISO1button.BackColor = Color.White;
            ISO1button.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            ISO1button.Location = new Point(50, 83);
            ISO1button.Margin = new Padding(4);
            ISO1button.Name = "ISO1button";
            ISO1button.Size = new Size(193, 73);
            ISO1button.TabIndex = 0;
            ISO1button.Text = "Open ISO #1";
            ISO1button.UseVisualStyleBackColor = false;
            ISO1button.Click += ISO1button_Click;
            // 
            // ISO2button
            // 
            ISO2button.BackColor = Color.White;
            ISO2button.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            ISO2button.Location = new Point(346, 83);
            ISO2button.Margin = new Padding(4);
            ISO2button.Name = "ISO2button";
            ISO2button.Size = new Size(193, 73);
            ISO2button.TabIndex = 1;
            ISO2button.Text = "Open ISO #2";
            ISO2button.UseVisualStyleBackColor = false;
            ISO2button.Click += ISO2button_Click;
            // 
            // instructionsLabel
            // 
            instructionsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            instructionsLabel.Location = new Point(24, 17);
            instructionsLabel.Margin = new Padding(4, 0, 4, 0);
            instructionsLabel.Name = "instructionsLabel";
            instructionsLabel.Size = new Size(563, 62);
            instructionsLabel.TabIndex = 2;
            instructionsLabel.Text = "Open two different ISO files to compare what was added or removed, then click compare to get the results.";
            instructionsLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(addedFilesRichTextBox);
            groupBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox1.Location = new Point(315, 207);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(257, 196);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Added Files";
            // 
            // addedFilesRichTextBox
            // 
            addedFilesRichTextBox.BackColor = Color.White;
            addedFilesRichTextBox.Location = new Point(7, 29);
            addedFilesRichTextBox.Name = "addedFilesRichTextBox";
            addedFilesRichTextBox.ReadOnly = true;
            addedFilesRichTextBox.Size = new Size(243, 160);
            addedFilesRichTextBox.TabIndex = 0;
            addedFilesRichTextBox.Text = "";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(removedFilesRichTextBox);
            groupBox2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox2.Location = new Point(24, 207);
            groupBox2.Margin = new Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4);
            groupBox2.Size = new Size(257, 196);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Removed Files";
            // 
            // removedFilesRichTextBox
            // 
            removedFilesRichTextBox.BackColor = Color.White;
            removedFilesRichTextBox.Location = new Point(7, 29);
            removedFilesRichTextBox.Name = "removedFilesRichTextBox";
            removedFilesRichTextBox.ReadOnly = true;
            removedFilesRichTextBox.Size = new Size(243, 160);
            removedFilesRichTextBox.TabIndex = 1;
            removedFilesRichTextBox.Text = "";
            // 
            // exitButton
            // 
            exitButton.Location = new Point(462, 489);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(125, 52);
            exitButton.TabIndex = 5;
            exitButton.Text = "Exit";
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += exitButton_Click;
            // 
            // compareButton
            // 
            compareButton.Location = new Point(12, 490);
            compareButton.Name = "compareButton";
            compareButton.Size = new Size(125, 52);
            compareButton.TabIndex = 6;
            compareButton.Text = "Compare";
            compareButton.UseVisualStyleBackColor = true;
            compareButton.Click += compareButton_Click;
            // 
            // wikiTableButton
            // 
            wikiTableButton.Location = new Point(162, 490);
            wikiTableButton.Name = "wikiTableButton";
            wikiTableButton.Size = new Size(125, 52);
            wikiTableButton.TabIndex = 7;
            wikiTableButton.Text = "Print Wiki Table";
            wikiTableButton.UseVisualStyleBackColor = true;
            wikiTableButton.Click += wikiTableButton_Click;
            // 
            // openISO1FileDialog
            // 
            openISO1FileDialog.DefaultExt = "iso";
            openISO1FileDialog.Filter = "ISO files (*.iso)|*.iso";
            openISO1FileDialog.RestoreDirectory = true;
            openISO1FileDialog.Title = "Open ISO File";
            // 
            // iso1FileNameTextBox
            // 
            iso1FileNameTextBox.BackColor = Color.White;
            iso1FileNameTextBox.Enabled = false;
            iso1FileNameTextBox.ForeColor = Color.Black;
            iso1FileNameTextBox.Location = new Point(50, 163);
            iso1FileNameTextBox.Name = "iso1FileNameTextBox";
            iso1FileNameTextBox.Size = new Size(193, 29);
            iso1FileNameTextBox.TabIndex = 8;
            // 
            // iso2FileNameTextBox
            // 
            iso2FileNameTextBox.BackColor = Color.White;
            iso2FileNameTextBox.Enabled = false;
            iso2FileNameTextBox.ForeColor = Color.Black;
            iso2FileNameTextBox.Location = new Point(346, 163);
            iso2FileNameTextBox.Name = "iso2FileNameTextBox";
            iso2FileNameTextBox.Size = new Size(193, 29);
            iso2FileNameTextBox.TabIndex = 9;
            // 
            // extractProgressBar
            // 
            extractProgressBar.BackColor = Color.Gainsboro;
            extractProgressBar.ForeColor = Color.OliveDrab;
            extractProgressBar.Location = new Point(75, 410);
            extractProgressBar.MarqueeAnimationSpeed = 0;
            extractProgressBar.Name = "extractProgressBar";
            extractProgressBar.Size = new Size(453, 30);
            extractProgressBar.Style = ProgressBarStyle.Continuous;
            extractProgressBar.TabIndex = 10;
            // 
            // progressLabel
            // 
            progressLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            progressLabel.ForeColor = Color.Black;
            progressLabel.Location = new Point(79, 450);
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new Size(449, 27);
            progressLabel.TabIndex = 11;
            progressLabel.Text = "Enter some files and press \"Compare\"";
            progressLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tempFolderButton
            // 
            tempFolderButton.Location = new Point(312, 489);
            tempFolderButton.Name = "tempFolderButton";
            tempFolderButton.Size = new Size(125, 52);
            tempFolderButton.TabIndex = 12;
            tempFolderButton.Text = "Open Program Folder";
            tempFolderButton.UseVisualStyleBackColor = true;
            tempFolderButton.Click += tempFolderButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(603, 554);
            Controls.Add(tempFolderButton);
            Controls.Add(progressLabel);
            Controls.Add(extractProgressBar);
            Controls.Add(groupBox1);
            Controls.Add(iso2FileNameTextBox);
            Controls.Add(iso1FileNameTextBox);
            Controls.Add(wikiTableButton);
            Controls.Add(compareButton);
            Controls.Add(exitButton);
            Controls.Add(groupBox2);
            Controls.Add(instructionsLabel);
            Controls.Add(ISO2button);
            Controls.Add(ISO1button);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "Form1";
            Text = "ISO Compare";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ISO1button;
        private Button ISO2button;
        private Label instructionsLabel;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button exitButton;
        private Button compareButton;
        private Button wikiTableButton;
        private RichTextBox addedFilesRichTextBox;
        private RichTextBox removedFilesRichTextBox;
        private OpenFileDialog openISO1FileDialog;
        private TextBox iso1FileNameTextBox;
        private TextBox iso2FileNameTextBox;
        private ProgressBar extractProgressBar;
        private Label progressLabel;
        private Button tempFolderButton;
    }
}
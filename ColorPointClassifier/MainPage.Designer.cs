namespace ColorPointClassifier
{
    partial class MainPage
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
            this.button1 = new System.Windows.Forms.Button();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.FillRestButton = new System.Windows.Forms.Button();
            this.FillingProgressBar = new System.Windows.Forms.ProgressBar();
            this.DebugTextBox = new System.Windows.Forms.TextBox();
            this.DebugLabel = new System.Windows.Forms.Label();
            this.DrawButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.GenerateLimitedPicker = new System.Windows.Forms.NumericUpDown();
            this.InitialPointsLabel = new System.Windows.Forms.Label();
            this.KNNListBox = new System.Windows.Forms.ComboBox();
            this.SeedPicker = new System.Windows.Forms.NumericUpDown();
            this.SeedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenerateLimitedPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SeedPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generate Points";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Generate_Click);
            // 
            // ImageBox
            // 
            this.ImageBox.Location = new System.Drawing.Point(12, 74);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(500, 500);
            this.ImageBox.TabIndex = 1;
            this.ImageBox.TabStop = false;
            // 
            // FillRestButton
            // 
            this.FillRestButton.Location = new System.Drawing.Point(138, 12);
            this.FillRestButton.Name = "FillRestButton";
            this.FillRestButton.Size = new System.Drawing.Size(100, 23);
            this.FillRestButton.TabIndex = 3;
            this.FillRestButton.Text = "Fill Rest";
            this.FillRestButton.UseVisualStyleBackColor = true;
            this.FillRestButton.Click += new System.EventHandler(this.FillRestButton_Click);
            // 
            // FillingProgressBar
            // 
            this.FillingProgressBar.Location = new System.Drawing.Point(138, 39);
            this.FillingProgressBar.Name = "FillingProgressBar";
            this.FillingProgressBar.Size = new System.Drawing.Size(371, 20);
            this.FillingProgressBar.TabIndex = 4;
            // 
            // DebugTextBox
            // 
            this.DebugTextBox.AcceptsReturn = true;
            this.DebugTextBox.CausesValidation = false;
            this.DebugTextBox.Enabled = false;
            this.DebugTextBox.Location = new System.Drawing.Point(518, 74);
            this.DebugTextBox.Multiline = true;
            this.DebugTextBox.Name = "DebugTextBox";
            this.DebugTextBox.Size = new System.Drawing.Size(238, 499);
            this.DebugTextBox.TabIndex = 6;
            // 
            // DebugLabel
            // 
            this.DebugLabel.AutoSize = true;
            this.DebugLabel.Location = new System.Drawing.Point(515, 58);
            this.DebugLabel.Name = "DebugLabel";
            this.DebugLabel.Size = new System.Drawing.Size(74, 13);
            this.DebugLabel.TabIndex = 7;
            this.DebugLabel.Text = "Debug Output";
            // 
            // DrawButton
            // 
            this.DrawButton.Location = new System.Drawing.Point(244, 12);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new System.Drawing.Size(106, 23);
            this.DrawButton.TabIndex = 8;
            this.DrawButton.Text = "Draw Image";
            this.DrawButton.UseVisualStyleBackColor = true;
            this.DrawButton.Click += new System.EventHandler(this.DrawButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(434, 12);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 9;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // GenerateLimitedPicker
            // 
            this.GenerateLimitedPicker.Location = new System.Drawing.Point(636, 12);
            this.GenerateLimitedPicker.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.GenerateLimitedPicker.Name = "GenerateLimitedPicker";
            this.GenerateLimitedPicker.Size = new System.Drawing.Size(120, 20);
            this.GenerateLimitedPicker.TabIndex = 10;
            this.GenerateLimitedPicker.Value = new decimal(new int[] {
            40000,
            0,
            0,
            0});
            // 
            // InitialPointsLabel
            // 
            this.InitialPointsLabel.AutoSize = true;
            this.InitialPointsLabel.Location = new System.Drawing.Point(569, 15);
            this.InitialPointsLabel.Name = "InitialPointsLabel";
            this.InitialPointsLabel.Size = new System.Drawing.Size(61, 13);
            this.InitialPointsLabel.TabIndex = 11;
            this.InitialPointsLabel.Text = "Inital Points";
            // 
            // KNNListBox
            // 
            this.KNNListBox.FormattingEnabled = true;
            this.KNNListBox.Items.AddRange(new object[] {
            "1",
            "3",
            "7",
            "15"});
            this.KNNListBox.Location = new System.Drawing.Point(12, 38);
            this.KNNListBox.Name = "KNNListBox";
            this.KNNListBox.Size = new System.Drawing.Size(119, 21);
            this.KNNListBox.TabIndex = 12;
            this.KNNListBox.Text = "Select K";
            // 
            // SeedPicker
            // 
            this.SeedPicker.Location = new System.Drawing.Point(636, 39);
            this.SeedPicker.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.SeedPicker.Name = "SeedPicker";
            this.SeedPicker.Size = new System.Drawing.Size(120, 20);
            this.SeedPicker.TabIndex = 13;
            this.SeedPicker.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // SeedLabel
            // 
            this.SeedLabel.AutoSize = true;
            this.SeedLabel.Location = new System.Drawing.Point(598, 41);
            this.SeedLabel.Name = "SeedLabel";
            this.SeedLabel.Size = new System.Drawing.Size(32, 13);
            this.SeedLabel.TabIndex = 14;
            this.SeedLabel.Text = "Seed";
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 585);
            this.Controls.Add(this.SeedLabel);
            this.Controls.Add(this.SeedPicker);
            this.Controls.Add(this.KNNListBox);
            this.Controls.Add(this.InitialPointsLabel);
            this.Controls.Add(this.GenerateLimitedPicker);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.DrawButton);
            this.Controls.Add(this.DebugLabel);
            this.Controls.Add(this.DebugTextBox);
            this.Controls.Add(this.FillingProgressBar);
            this.Controls.Add(this.FillRestButton);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.button1);
            this.Name = "MainPage";
            this.Text = "PointVisualiser";
            this.Load += new System.EventHandler(this.MainPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenerateLimitedPicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SeedPicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Button FillRestButton;
        private System.Windows.Forms.ProgressBar FillingProgressBar;
        private System.Windows.Forms.TextBox DebugTextBox;
        private System.Windows.Forms.Label DebugLabel;
        private System.Windows.Forms.Button DrawButton;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.NumericUpDown GenerateLimitedPicker;
        private System.Windows.Forms.Label InitialPointsLabel;
        private System.Windows.Forms.ComboBox KNNListBox;
        private System.Windows.Forms.NumericUpDown SeedPicker;
        private System.Windows.Forms.Label SeedLabel;
    }
}


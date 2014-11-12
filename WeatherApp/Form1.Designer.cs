namespace WeatherApp
{
    partial class Form1
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
            this.numberOfDays = new System.Windows.Forms.NumericUpDown();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.lblDays = new System.Windows.Forms.Label();
            this.lblZip = new System.Windows.Forms.Label();
            this.btnGetWeather = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDays)).BeginInit();
            this.SuspendLayout();
            // 
            // numberOfDays
            // 
            this.numberOfDays.Location = new System.Drawing.Point(145, 12);
            this.numberOfDays.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numberOfDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numberOfDays.Name = "numberOfDays";
            this.numberOfDays.Size = new System.Drawing.Size(33, 20);
            this.numberOfDays.TabIndex = 1;
            this.numberOfDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(242, 12);
            this.txtZip.MaxLength = 5;
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(46, 20);
            this.txtZip.TabIndex = 2;
            this.txtZip.Text = "22901";
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(6, 12);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(133, 13);
            this.lblDays.TabIndex = 3;
            this.lblDays.Text = "Number of Days to Forecst";
            // 
            // lblZip
            // 
            this.lblZip.AutoSize = true;
            this.lblZip.Location = new System.Drawing.Point(189, 12);
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size(47, 13);
            this.lblZip.TabIndex = 4;
            this.lblZip.Text = "ZipCode";
            // 
            // btnGetWeather
            // 
            this.btnGetWeather.Location = new System.Drawing.Point(296, 10);
            this.btnGetWeather.Name = "btnGetWeather";
            this.btnGetWeather.Size = new System.Drawing.Size(76, 23);
            this.btnGetWeather.TabIndex = 5;
            this.btnGetWeather.Text = "Get Weather";
            this.btnGetWeather.UseVisualStyleBackColor = true;
            this.btnGetWeather.Click += new System.EventHandler(this.btnGetWeather_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(27, 38);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(465, 267);
            this.webBrowser1.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(375, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Get Weather (12 hr)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 317);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.btnGetWeather);
            this.Controls.Add(this.lblZip);
            this.Controls.Add(this.lblDays);
            this.Controls.Add(this.txtZip);
            this.Controls.Add(this.numberOfDays);
            this.Name = "Form1";
            this.Text = "QuickWeather";
            ((System.ComponentModel.ISupportInitialize)(this.numberOfDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numberOfDays;
        private System.Windows.Forms.TextBox txtZip;
        private System.Windows.Forms.Label lblDays;
        private System.Windows.Forms.Label lblZip;
        private System.Windows.Forms.Button btnGetWeather;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button button1;
    }
}


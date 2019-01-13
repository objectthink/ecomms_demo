namespace ECOMMS_APP
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
            this._instrumentsListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _instrumentsListBox
            // 
            this._instrumentsListBox.FormattingEnabled = true;
            this._instrumentsListBox.ItemHeight = 16;
            this._instrumentsListBox.Location = new System.Drawing.Point(12, 12);
            this._instrumentsListBox.Name = "_instrumentsListBox";
            this._instrumentsListBox.Size = new System.Drawing.Size(776, 356);
            this._instrumentsListBox.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._instrumentsListBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _instrumentsListBox;
    }
}


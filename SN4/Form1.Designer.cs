using System.Windows.Forms;

namespace SN4
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
            this.components = new System.ComponentModel.Container();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(this.previewKeyDown);
            this.KeyDown += new KeyEventHandler(this.keyDown);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "SN4";
        }

        #endregion
    }
}


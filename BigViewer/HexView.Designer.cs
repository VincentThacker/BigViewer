using Be.Windows.Forms;

namespace BigViewer
{
    partial class HexView
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
            hexBox = new HexBox();
            SuspendLayout();
            // 
            // hexBox
            // 
            hexBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hexBox.BorderStyle = BorderStyle.FixedSingle;
            hexBox.ColumnInfoVisible = true;
            hexBox.Font = new Font("Consolas", 10F);
            hexBox.LineInfoVisible = true;
            hexBox.Location = new Point(12, 12);
            hexBox.MinimumSize = new Size(862, 0);
            hexBox.Name = "hexBox";
            hexBox.ReadOnly = true;
            hexBox.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            hexBox.Size = new Size(862, 1000);
            hexBox.StringViewVisible = true;
            hexBox.TabIndex = 0;
            hexBox.UseFixedBytesPerLine = true;
            hexBox.VScrollBarVisible = true;
            // 
            // HexView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(886, 1024);
            Controls.Add(hexBox);
            Name = "HexView";
            ResumeLayout(false);
        }

        #endregion

        private HexBox hexBox;
    }
}
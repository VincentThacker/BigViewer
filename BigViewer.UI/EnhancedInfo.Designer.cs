namespace BigViewer.UI
{
    partial class EnhancedInfo
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
            enhancedInfoDataGridView = new DataGridView();
            okButton = new Button();
            virtualId = new DataGridViewTextBoxColumn();
            realId = new DataGridViewTextBoxColumn();
            rleEntry = new DataGridViewTextBoxColumn();
            sectionNumber = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)enhancedInfoDataGridView).BeginInit();
            SuspendLayout();
            // 
            // enhancedInfoDataGridView
            // 
            enhancedInfoDataGridView.AllowUserToAddRows = false;
            enhancedInfoDataGridView.AllowUserToDeleteRows = false;
            enhancedInfoDataGridView.AllowUserToResizeColumns = false;
            enhancedInfoDataGridView.AllowUserToResizeRows = false;
            enhancedInfoDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            enhancedInfoDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            enhancedInfoDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            enhancedInfoDataGridView.ColumnHeadersHeight = 34;
            enhancedInfoDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            enhancedInfoDataGridView.Columns.AddRange(new DataGridViewColumn[] { virtualId, realId, rleEntry, sectionNumber });
            enhancedInfoDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            enhancedInfoDataGridView.Location = new Point(12, 12);
            enhancedInfoDataGridView.MultiSelect = false;
            enhancedInfoDataGridView.Name = "enhancedInfoDataGridView";
            enhancedInfoDataGridView.ReadOnly = true;
            enhancedInfoDataGridView.RowHeadersVisible = false;
            enhancedInfoDataGridView.RowHeadersWidth = 62;
            enhancedInfoDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            enhancedInfoDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            enhancedInfoDataGridView.ShowCellToolTips = false;
            enhancedInfoDataGridView.Size = new Size(434, 840);
            enhancedInfoDataGridView.StandardTab = true;
            enhancedInfoDataGridView.TabIndex = 0;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.Location = new Point(334, 858);
            okButton.Name = "okButton";
            okButton.Size = new Size(112, 34);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // virtualId
            // 
            virtualId.HeaderText = "Virtual";
            virtualId.MinimumWidth = 8;
            virtualId.Name = "virtualId";
            virtualId.ReadOnly = true;
            virtualId.Resizable = DataGridViewTriState.False;
            virtualId.SortMode = DataGridViewColumnSortMode.NotSortable;
            virtualId.Width = 101;
            // 
            // realId
            // 
            realId.HeaderText = "No.";
            realId.MinimumWidth = 8;
            realId.Name = "realId";
            realId.ReadOnly = true;
            realId.Resizable = DataGridViewTriState.False;
            realId.SortMode = DataGridViewColumnSortMode.NotSortable;
            realId.Width = 46;
            // 
            // rleEntry
            // 
            rleEntry.HeaderText = "RLE Block";
            rleEntry.MinimumWidth = 8;
            rleEntry.Name = "rleEntry";
            rleEntry.ReadOnly = true;
            rleEntry.Resizable = DataGridViewTriState.False;
            rleEntry.SortMode = DataGridViewColumnSortMode.NotSortable;
            rleEntry.Width = 93;
            // 
            // sectionNumber
            // 
            sectionNumber.HeaderText = "Section";
            sectionNumber.MinimumWidth = 8;
            sectionNumber.Name = "sectionNumber";
            sectionNumber.ReadOnly = true;
            sectionNumber.Resizable = DataGridViewTriState.False;
            sectionNumber.SortMode = DataGridViewColumnSortMode.NotSortable;
            sectionNumber.Width = 76;
            // 
            // EnhancedInfo
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(458, 904);
            Controls.Add(okButton);
            Controls.Add(enhancedInfoDataGridView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "EnhancedInfo";
            Tag = -4;
            Text = "Additional File Information";
            ((System.ComponentModel.ISupportInitialize)enhancedInfoDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView enhancedInfoDataGridView;
        private Button okButton;
        private DataGridViewTextBoxColumn virtualId;
        private DataGridViewTextBoxColumn realId;
        private DataGridViewTextBoxColumn rleEntry;
        private DataGridViewTextBoxColumn sectionNumber;
    }
}
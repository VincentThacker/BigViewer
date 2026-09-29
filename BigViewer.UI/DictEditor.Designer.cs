namespace BigViewer.UI
{
    partial class DictEditor
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
            dictList = new DataGridView();
            Key = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            cancelButton = new Button();
            saveButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dictList).BeginInit();
            SuspendLayout();
            // 
            // dictList
            // 
            dictList.AllowUserToAddRows = false;
            dictList.AllowUserToDeleteRows = false;
            dictList.AllowUserToResizeColumns = false;
            dictList.AllowUserToResizeRows = false;
            dictList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dictList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dictList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dictList.ColumnHeadersHeight = 34;
            dictList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dictList.Columns.AddRange(new DataGridViewColumn[] { Key, Value });
            dictList.EditMode = DataGridViewEditMode.EditProgrammatically;
            dictList.Location = new Point(12, 12);
            dictList.MultiSelect = false;
            dictList.Name = "dictList";
            dictList.ReadOnly = true;
            dictList.RowHeadersVisible = false;
            dictList.RowHeadersWidth = 62;
            dictList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dictList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dictList.ShowCellToolTips = false;
            dictList.Size = new Size(434, 840);
            dictList.StandardTab = true;
            dictList.TabIndex = 0;
            // 
            // Key
            // 
            Key.HeaderText = "Key";
            Key.MaxInputLength = 8;
            Key.MinimumWidth = 8;
            Key.Name = "Key";
            Key.ReadOnly = true;
            Key.Resizable = DataGridViewTriState.False;
            Key.SortMode = DataGridViewColumnSortMode.NotSortable;
            Key.Width = 46;
            // 
            // Value
            // 
            Value.HeaderText = "Value";
            Value.MinimumWidth = 8;
            Value.Name = "Value";
            Value.ReadOnly = true;
            Value.Resizable = DataGridViewTriState.False;
            Value.SortMode = DataGridViewColumnSortMode.NotSortable;
            Value.Width = 60;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Enabled = false;
            cancelButton.Location = new Point(334, 858);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Enabled = false;
            saveButton.Location = new Point(216, 858);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(112, 34);
            saveButton.TabIndex = 2;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // DictEditor
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(458, 904);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            Controls.Add(dictList);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "DictEditor";
            Text = "DictEditor";
            ((System.ComponentModel.ISupportInitialize)dictList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dictList;
        private DataGridViewTextBoxColumn Key;
        private DataGridViewTextBoxColumn Value;
        private Button cancelButton;
        private Button saveButton;
    }
}
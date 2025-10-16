namespace BigViewer
{
    partial class LittleEditor
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
            editRawButton = new Button();
            resourceList = new DataGridView();
            ItemNumber = new DataGridViewTextBoxColumn();
            Type = new DataGridViewTextBoxColumn();
            Offset = new DataGridViewTextBoxColumn();
            FileSize = new DataGridViewTextBoxColumn();
            RawSize = new DataGridViewTextBoxColumn();
            Format = new DataGridViewTextBoxColumn();
            infoBox = new ListBox();
            searchButton = new Button();
            resultsBox = new ListBox();
            exportSelectedButton = new Button();
            replaceButton = new Button();
            viewRawButton = new Button();
            saveButton = new Button();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)resourceList).BeginInit();
            SuspendLayout();
            // 
            // editRawButton
            // 
            editRawButton.Enabled = false;
            editRawButton.Location = new Point(130, 12);
            editRawButton.Name = "editRawButton";
            editRawButton.Size = new Size(112, 34);
            editRawButton.TabIndex = 1;
            editRawButton.Text = "Edit Raw";
            editRawButton.UseVisualStyleBackColor = true;
            editRawButton.Click += editRawButton_Click;
            // 
            // resourceList
            // 
            resourceList.AllowUserToAddRows = false;
            resourceList.AllowUserToDeleteRows = false;
            resourceList.AllowUserToResizeColumns = false;
            resourceList.AllowUserToResizeRows = false;
            resourceList.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resourceList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            resourceList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            resourceList.ColumnHeadersHeight = 34;
            resourceList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            resourceList.Columns.AddRange(new DataGridViewColumn[] { ItemNumber, Type, Offset, FileSize, RawSize, Format });
            resourceList.EditMode = DataGridViewEditMode.EditProgrammatically;
            resourceList.Location = new Point(12, 212);
            resourceList.MultiSelect = false;
            resourceList.Name = "resourceList";
            resourceList.ReadOnly = true;
            resourceList.RowHeadersVisible = false;
            resourceList.RowHeadersWidth = 62;
            resourceList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resourceList.Size = new Size(702, 600);
            resourceList.TabIndex = 3;
            // 
            // ItemNumber
            // 
            ItemNumber.HeaderText = "No.";
            ItemNumber.MaxInputLength = 5;
            ItemNumber.MinimumWidth = 8;
            ItemNumber.Name = "ItemNumber";
            ItemNumber.ReadOnly = true;
            ItemNumber.Resizable = DataGridViewTriState.False;
            ItemNumber.Width = 76;
            // 
            // Type
            // 
            Type.HeaderText = "Type";
            Type.MinimumWidth = 8;
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Resizable = DataGridViewTriState.False;
            Type.Width = 85;
            // 
            // Offset
            // 
            Offset.HeaderText = "Offset";
            Offset.MinimumWidth = 8;
            Offset.Name = "Offset";
            Offset.ReadOnly = true;
            Offset.Resizable = DataGridViewTriState.False;
            Offset.Width = 97;
            // 
            // FileSize
            // 
            FileSize.HeaderText = "Size";
            FileSize.MinimumWidth = 8;
            FileSize.Name = "FileSize";
            FileSize.ReadOnly = true;
            FileSize.Resizable = DataGridViewTriState.False;
            FileSize.Width = 79;
            // 
            // RawSize
            // 
            RawSize.HeaderText = "Data Size";
            RawSize.MinimumWidth = 8;
            RawSize.Name = "RawSize";
            RawSize.ReadOnly = true;
            RawSize.Resizable = DataGridViewTriState.False;
            RawSize.Width = 121;
            // 
            // Format
            // 
            Format.HeaderText = "Format";
            Format.MinimumWidth = 8;
            Format.Name = "Format";
            Format.ReadOnly = true;
            Format.Resizable = DataGridViewTriState.False;
            Format.Width = 105;
            // 
            // infoBox
            // 
            infoBox.FormattingEnabled = true;
            infoBox.HorizontalScrollbar = true;
            infoBox.Location = new Point(12, 52);
            infoBox.Name = "infoBox";
            infoBox.Size = new Size(348, 154);
            infoBox.TabIndex = 4;
            // 
            // searchButton
            // 
            searchButton.Enabled = false;
            searchButton.Location = new Point(248, 12);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(112, 34);
            searchButton.TabIndex = 6;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchButton_Click;
            // 
            // resultsBox
            // 
            resultsBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            resultsBox.FormattingEnabled = true;
            resultsBox.HorizontalScrollbar = true;
            resultsBox.Location = new Point(366, 52);
            resultsBox.Name = "resultsBox";
            resultsBox.Size = new Size(348, 154);
            resultsBox.TabIndex = 7;
            // 
            // exportSelectedButton
            // 
            exportSelectedButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportSelectedButton.Enabled = false;
            exportSelectedButton.Location = new Point(543, 12);
            exportSelectedButton.Name = "exportSelectedButton";
            exportSelectedButton.Size = new Size(171, 34);
            exportSelectedButton.TabIndex = 8;
            exportSelectedButton.Text = "Export Selected";
            exportSelectedButton.UseVisualStyleBackColor = true;
            exportSelectedButton.Click += exportSelectedButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            replaceButton.Enabled = false;
            replaceButton.Location = new Point(366, 12);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new Size(112, 34);
            replaceButton.TabIndex = 10;
            replaceButton.Text = "Replace";
            replaceButton.UseVisualStyleBackColor = true;
            replaceButton.Click += replaceButton_Click;
            // 
            // viewRawButton
            // 
            viewRawButton.Enabled = false;
            viewRawButton.Location = new Point(12, 12);
            viewRawButton.Name = "viewRawButton";
            viewRawButton.Size = new Size(112, 34);
            viewRawButton.TabIndex = 11;
            viewRawButton.Text = "View Raw";
            viewRawButton.UseVisualStyleBackColor = true;
            viewRawButton.Click += viewRawButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Enabled = false;
            saveButton.Location = new Point(484, 818);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(112, 34);
            saveButton.TabIndex = 12;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Enabled = false;
            cancelButton.Location = new Point(602, 818);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 13;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // LittleEditor
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(726, 864);
            Controls.Add(cancelButton);
            Controls.Add(saveButton);
            Controls.Add(viewRawButton);
            Controls.Add(replaceButton);
            Controls.Add(exportSelectedButton);
            Controls.Add(resultsBox);
            Controls.Add(searchButton);
            Controls.Add(infoBox);
            Controls.Add(resourceList);
            Controls.Add(editRawButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "LittleEditor";
            ((System.ComponentModel.ISupportInitialize)resourceList).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button editRawButton;
        private DataGridView resourceList;
        private ListBox infoBox;
        private Button searchButton;
        private ListBox resultsBox;
        private Button exportSelectedButton;
        private Button replaceButton;
        private Button viewRawButton;
        private Button saveButton;
        private DataGridViewTextBoxColumn ItemNumber;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Offset;
        private DataGridViewTextBoxColumn FileSize;
        private DataGridViewTextBoxColumn RawSize;
        private DataGridViewTextBoxColumn Format;
        private Button cancelButton;
    }
}
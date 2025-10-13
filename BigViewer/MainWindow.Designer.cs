namespace BigViewer
{
    partial class MainWindow
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
            openFileButton = new Button();
            viewButton = new Button();
            pathBox = new TextBox();
            resourceList = new DataGridView();
            ItemNumber = new DataGridViewTextBoxColumn();
            Type = new DataGridViewTextBoxColumn();
            Offset = new DataGridViewTextBoxColumn();
            Size = new DataGridViewTextBoxColumn();
            DataSize = new DataGridViewTextBoxColumn();
            Format = new DataGridViewTextBoxColumn();
            infoBox = new ListBox();
            exportDataButton = new Button();
            searchButton = new Button();
            resultsBox = new ListBox();
            exportSelectedButton = new Button();
            checksumButton = new Button();
            replaceButton = new Button();
            viewRawButton = new Button();
            saveFileButton = new Button();
            ((System.ComponentModel.ISupportInitialize)resourceList).BeginInit();
            SuspendLayout();
            // 
            // openFileButton
            // 
            openFileButton.Location = new Point(12, 12);
            openFileButton.Name = "openFileButton";
            openFileButton.Size = new Size(112, 34);
            openFileButton.TabIndex = 0;
            openFileButton.Text = "Open";
            openFileButton.UseVisualStyleBackColor = true;
            openFileButton.Click += openFileButton_Click;
            // 
            // viewButton
            // 
            viewButton.Enabled = false;
            viewButton.Location = new Point(12, 52);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(112, 34);
            viewButton.TabIndex = 1;
            viewButton.Text = "View";
            viewButton.UseVisualStyleBackColor = true;
            viewButton.Click += viewButton_Click;
            // 
            // pathBox
            // 
            pathBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pathBox.BorderStyle = BorderStyle.FixedSingle;
            pathBox.Location = new Point(130, 12);
            pathBox.Name = "pathBox";
            pathBox.ReadOnly = true;
            pathBox.Size = new Size(584, 34);
            pathBox.TabIndex = 2;
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
            resourceList.Columns.AddRange(new DataGridViewColumn[] { ItemNumber, Type, Offset, Size, DataSize, Format });
            resourceList.EditMode = DataGridViewEditMode.EditProgrammatically;
            resourceList.Location = new Point(12, 292);
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
            // Size
            // 
            Size.HeaderText = "Size";
            Size.MinimumWidth = 8;
            Size.Name = "Size";
            Size.ReadOnly = true;
            Size.Resizable = DataGridViewTriState.False;
            Size.Width = 79;
            // 
            // DataSize
            // 
            DataSize.HeaderText = "Data Size";
            DataSize.MinimumWidth = 8;
            DataSize.Name = "DataSize";
            DataSize.ReadOnly = true;
            DataSize.Resizable = DataGridViewTriState.False;
            DataSize.Width = 121;
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
            infoBox.Location = new Point(12, 132);
            infoBox.Name = "infoBox";
            infoBox.Size = new Size(348, 154);
            infoBox.TabIndex = 4;
            // 
            // exportDataButton
            // 
            exportDataButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportDataButton.Enabled = false;
            exportDataButton.Location = new Point(543, 92);
            exportDataButton.Name = "exportDataButton";
            exportDataButton.Size = new Size(171, 34);
            exportDataButton.TabIndex = 5;
            exportDataButton.Text = "Export All Data";
            exportDataButton.UseVisualStyleBackColor = true;
            exportDataButton.Click += exportDataButton_Click;
            // 
            // searchButton
            // 
            searchButton.Enabled = false;
            searchButton.Location = new Point(366, 52);
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
            resultsBox.Location = new Point(366, 132);
            resultsBox.Name = "resultsBox";
            resultsBox.Size = new Size(348, 154);
            resultsBox.TabIndex = 7;
            // 
            // exportSelectedButton
            // 
            exportSelectedButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportSelectedButton.Enabled = false;
            exportSelectedButton.Location = new Point(366, 92);
            exportSelectedButton.Name = "exportSelectedButton";
            exportSelectedButton.Size = new Size(171, 34);
            exportSelectedButton.TabIndex = 8;
            exportSelectedButton.Text = "Export Selected";
            exportSelectedButton.UseVisualStyleBackColor = true;
            exportSelectedButton.Click += exportSelectedButton_Click;
            // 
            // checksumButton
            // 
            checksumButton.Location = new Point(12, 92);
            checksumButton.Name = "checksumButton";
            checksumButton.Size = new Size(230, 34);
            checksumButton.TabIndex = 9;
            checksumButton.Text = "Calculate Checksum";
            checksumButton.UseVisualStyleBackColor = true;
            checksumButton.Click += checksumButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.Enabled = false;
            replaceButton.Location = new Point(484, 52);
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
            viewRawButton.Location = new Point(130, 52);
            viewRawButton.Name = "viewRawButton";
            viewRawButton.Size = new Size(112, 34);
            viewRawButton.TabIndex = 11;
            viewRawButton.Text = "View Raw";
            viewRawButton.UseVisualStyleBackColor = true;
            viewRawButton.Click += viewRawButton_Click;
            // 
            // saveFileButton
            // 
            saveFileButton.Enabled = false;
            saveFileButton.Location = new Point(602, 52);
            saveFileButton.Name = "saveFileButton";
            saveFileButton.Size = new Size(112, 34);
            saveFileButton.TabIndex = 12;
            saveFileButton.Text = "Save File";
            saveFileButton.UseVisualStyleBackColor = true;
            saveFileButton.Click += saveFileButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(726, 904);
            Controls.Add(saveFileButton);
            Controls.Add(viewRawButton);
            Controls.Add(replaceButton);
            Controls.Add(checksumButton);
            Controls.Add(exportSelectedButton);
            Controls.Add(resultsBox);
            Controls.Add(searchButton);
            Controls.Add(exportDataButton);
            Controls.Add(infoBox);
            Controls.Add(resourceList);
            Controls.Add(pathBox);
            Controls.Add(viewButton);
            Controls.Add(openFileButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            Text = "Big Viewer";
            ((System.ComponentModel.ISupportInitialize)resourceList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button openFileButton;
        private Button viewButton;
        private TextBox pathBox;
        private DataGridView resourceList;
        private ListBox infoBox;
        private Button exportDataButton;
        private Button searchButton;
        private ListBox resultsBox;
        private Button exportSelectedButton;
        private DataGridViewTextBoxColumn ItemNumber;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Offset;
        private DataGridViewTextBoxColumn Size;
        private DataGridViewTextBoxColumn DataSize;
        private DataGridViewTextBoxColumn Format;
        private Button checksumButton;
        private Button replaceButton;
        private Button viewRawButton;
        private Button saveFileButton;
    }
}

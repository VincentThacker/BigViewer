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
            loadButton = new Button();
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
            saveButton = new Button();
            ((System.ComponentModel.ISupportInitialize)resourceList).BeginInit();
            SuspendLayout();
            // 
            // loadButton
            // 
            loadButton.Location = new Point(12, 49);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(112, 34);
            loadButton.TabIndex = 0;
            loadButton.Text = "Load";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // viewButton
            // 
            viewButton.Enabled = false;
            viewButton.Location = new Point(130, 49);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(112, 34);
            viewButton.TabIndex = 1;
            viewButton.Text = "View";
            viewButton.UseVisualStyleBackColor = true;
            viewButton.Click += viewButton_Click;
            // 
            // pathBox
            // 
            pathBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathBox.Location = new Point(12, 12);
            pathBox.Name = "pathBox";
            pathBox.Size = new Size(702, 31);
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
            resourceList.Location = new Point(12, 249);
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
            infoBox.Location = new Point(12, 89);
            infoBox.Name = "infoBox";
            infoBox.Size = new Size(348, 154);
            infoBox.TabIndex = 4;
            // 
            // exportDataButton
            // 
            exportDataButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportDataButton.Enabled = false;
            exportDataButton.Location = new Point(543, 49);
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
            searchButton.Location = new Point(248, 49);
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
            resultsBox.Location = new Point(366, 89);
            resultsBox.Name = "resultsBox";
            resultsBox.Size = new Size(348, 154);
            resultsBox.TabIndex = 7;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.Enabled = false;
            saveButton.Location = new Point(366, 49);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(171, 34);
            saveButton.TabIndex = 8;
            saveButton.Text = "Export Selected";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(726, 861);
            Controls.Add(saveButton);
            Controls.Add(resultsBox);
            Controls.Add(searchButton);
            Controls.Add(exportDataButton);
            Controls.Add(infoBox);
            Controls.Add(resourceList);
            Controls.Add(pathBox);
            Controls.Add(viewButton);
            Controls.Add(loadButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            Text = "Big Viewer";
            ((System.ComponentModel.ISupportInitialize)resourceList).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button loadButton;
        private Button viewButton;
        private TextBox pathBox;
        private DataGridView resourceList;
        private ListBox infoBox;
        private Button exportDataButton;
        private Button searchButton;
        private ListBox resultsBox;
        private Button saveButton;
        private DataGridViewTextBoxColumn ItemNumber;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Offset;
        private DataGridViewTextBoxColumn Size;
        private DataGridViewTextBoxColumn DataSize;
        private DataGridViewTextBoxColumn Format;
    }
}

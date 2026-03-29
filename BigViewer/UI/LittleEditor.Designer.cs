namespace BigViewer.UI
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
            components = new System.ComponentModel.Container();
            editRawButton = new Button();
            resourceList = new DataGridView();
            ItemNumber = new DataGridViewTextBoxColumn();
            Type = new DataGridViewTextBoxColumn();
            Offset = new DataGridViewTextBoxColumn();
            FileSize = new DataGridViewTextBoxColumn();
            RawSize = new DataGridViewTextBoxColumn();
            Format = new DataGridViewTextBoxColumn();
            resourceListContextMenu = new ContextMenuStrip(components);
            resourceListContextMenuItemViewRaw = new ToolStripMenuItem();
            resourceListContextMenuItemEditRaw = new ToolStripMenuItem();
            resourceListContextMenuItemReplace = new ToolStripMenuItem();
            resourceListContextMenuItemExportSelected = new ToolStripMenuItem();
            infoBox = new ListBox();
            searchButton = new Button();
            resultsBox = new ListBox();
            exportSelectedButton = new Button();
            replaceButton = new Button();
            viewRawButton = new Button();
            saveButton = new Button();
            exportAllButton = new Button();
            searchOptionsGroupBox = new GroupBox();
            searchDataOnlyCheckBox = new CheckBox();
            searchTabControl = new TabControl();
            searchTabPageBinary = new TabPage();
            searchTabPageBinaryInput = new TextBox();
            searchTabPageNumber = new TabPage();
            searchTabPageNumber16Bit = new RadioButton();
            searchTabPageNumber32Bit = new RadioButton();
            searchTabPageNumber64Bit = new RadioButton();
            searchTabPageNumberInput = new TextBox();
            searchTabPageString = new TabPage();
            searchTabPageStringUTF16 = new RadioButton();
            searchTabPageStringLatin = new RadioButton();
            searchTabPageStringInput = new TextBox();
            fileOptionsGroupBox = new GroupBox();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)resourceList).BeginInit();
            resourceListContextMenu.SuspendLayout();
            searchOptionsGroupBox.SuspendLayout();
            searchTabControl.SuspendLayout();
            searchTabPageBinary.SuspendLayout();
            searchTabPageNumber.SuspendLayout();
            searchTabPageString.SuspendLayout();
            fileOptionsGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // editRawButton
            // 
            editRawButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            editRawButton.Enabled = false;
            editRawButton.Location = new Point(124, 188);
            editRawButton.Name = "editRawButton";
            editRawButton.Size = new Size(112, 34);
            editRawButton.TabIndex = 5;
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
            resourceList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resourceList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            resourceList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            resourceList.ColumnHeadersHeight = 34;
            resourceList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            resourceList.Columns.AddRange(new DataGridViewColumn[] { ItemNumber, Type, Offset, FileSize, RawSize, Format });
            resourceList.EditMode = DataGridViewEditMode.EditProgrammatically;
            resourceList.Location = new Point(12, 12);
            resourceList.MultiSelect = false;
            resourceList.Name = "resourceList";
            resourceList.ReadOnly = true;
            resourceList.RowHeadersVisible = false;
            resourceList.RowHeadersWidth = 62;
            resourceList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resourceList.ShowCellToolTips = false;
            resourceList.Size = new Size(606, 840);
            resourceList.StandardTab = true;
            resourceList.TabIndex = 2;
            resourceList.CellMouseDown += resourceList_CellMouseDown;
            // 
            // ItemNumber
            // 
            ItemNumber.HeaderText = "No.";
            ItemNumber.MaxInputLength = 5;
            ItemNumber.MinimumWidth = 8;
            ItemNumber.Name = "ItemNumber";
            ItemNumber.ReadOnly = true;
            ItemNumber.Resizable = DataGridViewTriState.False;
            ItemNumber.SortMode = DataGridViewColumnSortMode.NotSortable;
            ItemNumber.Width = 46;
            // 
            // Type
            // 
            Type.HeaderText = "Type";
            Type.MinimumWidth = 8;
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Resizable = DataGridViewTriState.False;
            Type.SortMode = DataGridViewColumnSortMode.NotSortable;
            Type.Width = 55;
            // 
            // Offset
            // 
            Offset.HeaderText = "Offset";
            Offset.MinimumWidth = 8;
            Offset.Name = "Offset";
            Offset.ReadOnly = true;
            Offset.Resizable = DataGridViewTriState.False;
            Offset.SortMode = DataGridViewColumnSortMode.NotSortable;
            Offset.Width = 67;
            // 
            // FileSize
            // 
            FileSize.HeaderText = "Size";
            FileSize.MinimumWidth = 8;
            FileSize.Name = "FileSize";
            FileSize.ReadOnly = true;
            FileSize.Resizable = DataGridViewTriState.False;
            FileSize.SortMode = DataGridViewColumnSortMode.NotSortable;
            FileSize.Width = 49;
            // 
            // RawSize
            // 
            RawSize.HeaderText = "Data Size";
            RawSize.MinimumWidth = 8;
            RawSize.Name = "RawSize";
            RawSize.ReadOnly = true;
            RawSize.Resizable = DataGridViewTriState.False;
            RawSize.SortMode = DataGridViewColumnSortMode.NotSortable;
            RawSize.Width = 91;
            // 
            // Format
            // 
            Format.HeaderText = "Format";
            Format.MinimumWidth = 8;
            Format.Name = "Format";
            Format.ReadOnly = true;
            Format.Resizable = DataGridViewTriState.False;
            Format.SortMode = DataGridViewColumnSortMode.NotSortable;
            Format.Width = 75;
            // 
            // resourceListContextMenu
            // 
            resourceListContextMenu.ImageScalingSize = new Size(24, 24);
            resourceListContextMenu.Items.AddRange(new ToolStripItem[] { resourceListContextMenuItemViewRaw, resourceListContextMenuItemEditRaw, resourceListContextMenuItemReplace, resourceListContextMenuItemExportSelected });
            resourceListContextMenu.Name = "resourceListContextMenu";
            resourceListContextMenu.Size = new Size(160, 132);
            // 
            // resourceListContextMenuItemViewRaw
            // 
            resourceListContextMenuItemViewRaw.Name = "resourceListContextMenuItemViewRaw";
            resourceListContextMenuItemViewRaw.Size = new Size(159, 32);
            resourceListContextMenuItemViewRaw.Text = "View Raw";
            resourceListContextMenuItemViewRaw.Click += viewRawButton_Click;
            // 
            // resourceListContextMenuItemEditRaw
            // 
            resourceListContextMenuItemEditRaw.Name = "resourceListContextMenuItemEditRaw";
            resourceListContextMenuItemEditRaw.Size = new Size(159, 32);
            resourceListContextMenuItemEditRaw.Text = "Edit Raw";
            resourceListContextMenuItemEditRaw.Click += editRawButton_Click;
            // 
            // resourceListContextMenuItemReplace
            // 
            resourceListContextMenuItemReplace.Name = "resourceListContextMenuItemReplace";
            resourceListContextMenuItemReplace.Size = new Size(159, 32);
            resourceListContextMenuItemReplace.Text = "Replace";
            resourceListContextMenuItemReplace.Click += replaceButton_Click;
            // 
            // resourceListContextMenuItemExportSelected
            // 
            resourceListContextMenuItemExportSelected.Name = "resourceListContextMenuItemExportSelected";
            resourceListContextMenuItemExportSelected.Size = new Size(159, 32);
            resourceListContextMenuItemExportSelected.Text = "Export";
            resourceListContextMenuItemExportSelected.Click += exportSelectedButton_Click;
            // 
            // infoBox
            // 
            infoBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            infoBox.BorderStyle = BorderStyle.FixedSingle;
            infoBox.FormattingEnabled = true;
            infoBox.HorizontalScrollbar = true;
            infoBox.Location = new Point(6, 30);
            infoBox.Name = "infoBox";
            infoBox.Size = new Size(348, 152);
            infoBox.TabIndex = 3;
            // 
            // searchButton
            // 
            searchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            searchButton.Enabled = false;
            searchButton.Location = new Point(242, 148);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(112, 34);
            searchButton.TabIndex = 22;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchButton_Click;
            // 
            // resultsBox
            // 
            resultsBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resultsBox.BorderStyle = BorderStyle.FixedSingle;
            resultsBox.FormattingEnabled = true;
            resultsBox.HorizontalScrollbar = true;
            resultsBox.Location = new Point(6, 188);
            resultsBox.Name = "resultsBox";
            resultsBox.Size = new Size(348, 327);
            resultsBox.TabIndex = 23;
            resultsBox.SelectedIndexChanged += resultsBox_SelectedIndexChanged;
            // 
            // exportSelectedButton
            // 
            exportSelectedButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            exportSelectedButton.Enabled = false;
            exportSelectedButton.Location = new Point(6, 228);
            exportSelectedButton.Name = "exportSelectedButton";
            exportSelectedButton.Size = new Size(171, 34);
            exportSelectedButton.TabIndex = 7;
            exportSelectedButton.Text = "Export Selected";
            exportSelectedButton.UseVisualStyleBackColor = true;
            exportSelectedButton.Click += exportSelectedButton_Click;
            // 
            // replaceButton
            // 
            replaceButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            replaceButton.Enabled = false;
            replaceButton.Location = new Point(242, 188);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new Size(112, 34);
            replaceButton.TabIndex = 6;
            replaceButton.Text = "Replace";
            replaceButton.UseVisualStyleBackColor = true;
            replaceButton.Click += replaceButton_Click;
            // 
            // viewRawButton
            // 
            viewRawButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            viewRawButton.Enabled = false;
            viewRawButton.Location = new Point(6, 188);
            viewRawButton.Name = "viewRawButton";
            viewRawButton.Size = new Size(112, 34);
            viewRawButton.TabIndex = 4;
            viewRawButton.Text = "View Raw";
            viewRawButton.UseVisualStyleBackColor = true;
            viewRawButton.Click += viewRawButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Enabled = false;
            saveButton.Location = new Point(754, 818);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(112, 34);
            saveButton.TabIndex = 10;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // exportAllButton
            // 
            exportAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            exportAllButton.Enabled = false;
            exportAllButton.Location = new Point(183, 228);
            exportAllButton.Name = "exportAllButton";
            exportAllButton.Size = new Size(171, 34);
            exportAllButton.TabIndex = 8;
            exportAllButton.Text = "Export All";
            exportAllButton.UseVisualStyleBackColor = true;
            exportAllButton.Click += exportAllButton_Click;
            // 
            // searchOptionsGroupBox
            // 
            searchOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            searchOptionsGroupBox.Controls.Add(searchDataOnlyCheckBox);
            searchOptionsGroupBox.Controls.Add(searchTabControl);
            searchOptionsGroupBox.Controls.Add(searchButton);
            searchOptionsGroupBox.Controls.Add(resultsBox);
            searchOptionsGroupBox.Enabled = false;
            searchOptionsGroupBox.Location = new Point(630, 286);
            searchOptionsGroupBox.Name = "searchOptionsGroupBox";
            searchOptionsGroupBox.Size = new Size(360, 526);
            searchOptionsGroupBox.TabIndex = 11;
            searchOptionsGroupBox.TabStop = false;
            searchOptionsGroupBox.Text = "Search";
            // 
            // searchDataOnlyCheckBox
            // 
            searchDataOnlyCheckBox.AutoSize = true;
            searchDataOnlyCheckBox.Enabled = false;
            searchDataOnlyCheckBox.Location = new Point(6, 152);
            searchDataOnlyCheckBox.Name = "searchDataOnlyCheckBox";
            searchDataOnlyCheckBox.Size = new Size(114, 29);
            searchDataOnlyCheckBox.TabIndex = 21;
            searchDataOnlyCheckBox.Text = "Data only";
            searchDataOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchTabControl
            // 
            searchTabControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchTabControl.Controls.Add(searchTabPageBinary);
            searchTabControl.Controls.Add(searchTabPageNumber);
            searchTabControl.Controls.Add(searchTabPageString);
            searchTabControl.Enabled = false;
            searchTabControl.Location = new Point(6, 30);
            searchTabControl.Name = "searchTabControl";
            searchTabControl.SelectedIndex = 0;
            searchTabControl.Size = new Size(348, 116);
            searchTabControl.TabIndex = 20;
            // 
            // searchTabPageBinary
            // 
            searchTabPageBinary.Controls.Add(searchTabPageBinaryInput);
            searchTabPageBinary.Location = new Point(4, 34);
            searchTabPageBinary.Name = "searchTabPageBinary";
            searchTabPageBinary.Padding = new Padding(3);
            searchTabPageBinary.Size = new Size(340, 78);
            searchTabPageBinary.TabIndex = 0;
            searchTabPageBinary.Text = "Binary";
            searchTabPageBinary.UseVisualStyleBackColor = true;
            // 
            // searchTabPageBinaryInput
            // 
            searchTabPageBinaryInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            searchTabPageBinaryInput.BorderStyle = BorderStyle.FixedSingle;
            searchTabPageBinaryInput.Enabled = false;
            searchTabPageBinaryInput.Location = new Point(6, 6);
            searchTabPageBinaryInput.Multiline = true;
            searchTabPageBinaryInput.Name = "searchTabPageBinaryInput";
            searchTabPageBinaryInput.PlaceholderText = "Enter bytes (separated by \"-\")";
            searchTabPageBinaryInput.Size = new Size(328, 66);
            searchTabPageBinaryInput.TabIndex = 0;
            // 
            // searchTabPageNumber
            // 
            searchTabPageNumber.Controls.Add(searchTabPageNumber16Bit);
            searchTabPageNumber.Controls.Add(searchTabPageNumber32Bit);
            searchTabPageNumber.Controls.Add(searchTabPageNumber64Bit);
            searchTabPageNumber.Controls.Add(searchTabPageNumberInput);
            searchTabPageNumber.Location = new Point(4, 34);
            searchTabPageNumber.Name = "searchTabPageNumber";
            searchTabPageNumber.Padding = new Padding(3);
            searchTabPageNumber.Size = new Size(340, 78);
            searchTabPageNumber.TabIndex = 1;
            searchTabPageNumber.Text = "Number";
            searchTabPageNumber.UseVisualStyleBackColor = true;
            // 
            // searchTabPageNumber16Bit
            // 
            searchTabPageNumber16Bit.AutoSize = true;
            searchTabPageNumber16Bit.Enabled = false;
            searchTabPageNumber16Bit.Location = new Point(6, 43);
            searchTabPageNumber16Bit.Name = "searchTabPageNumber16Bit";
            searchTabPageNumber16Bit.Size = new Size(85, 29);
            searchTabPageNumber16Bit.TabIndex = 1;
            searchTabPageNumber16Bit.TabStop = true;
            searchTabPageNumber16Bit.Text = "16-bit";
            searchTabPageNumber16Bit.UseVisualStyleBackColor = true;
            // 
            // searchTabPageNumber32Bit
            // 
            searchTabPageNumber32Bit.AutoSize = true;
            searchTabPageNumber32Bit.Enabled = false;
            searchTabPageNumber32Bit.Location = new Point(97, 43);
            searchTabPageNumber32Bit.Name = "searchTabPageNumber32Bit";
            searchTabPageNumber32Bit.Size = new Size(85, 29);
            searchTabPageNumber32Bit.TabIndex = 2;
            searchTabPageNumber32Bit.TabStop = true;
            searchTabPageNumber32Bit.Text = "32-bit";
            searchTabPageNumber32Bit.UseVisualStyleBackColor = true;
            // 
            // searchTabPageNumber64Bit
            // 
            searchTabPageNumber64Bit.AutoSize = true;
            searchTabPageNumber64Bit.Enabled = false;
            searchTabPageNumber64Bit.Location = new Point(188, 43);
            searchTabPageNumber64Bit.Name = "searchTabPageNumber64Bit";
            searchTabPageNumber64Bit.Size = new Size(85, 29);
            searchTabPageNumber64Bit.TabIndex = 3;
            searchTabPageNumber64Bit.TabStop = true;
            searchTabPageNumber64Bit.Text = "64-bit";
            searchTabPageNumber64Bit.UseVisualStyleBackColor = true;
            // 
            // searchTabPageNumberInput
            // 
            searchTabPageNumberInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchTabPageNumberInput.BorderStyle = BorderStyle.FixedSingle;
            searchTabPageNumberInput.Enabled = false;
            searchTabPageNumberInput.Location = new Point(6, 6);
            searchTabPageNumberInput.MaxLength = 512;
            searchTabPageNumberInput.Name = "searchTabPageNumberInput";
            searchTabPageNumberInput.PlaceholderText = "Enter number";
            searchTabPageNumberInput.Size = new Size(328, 31);
            searchTabPageNumberInput.TabIndex = 0;
            // 
            // searchTabPageString
            // 
            searchTabPageString.Controls.Add(searchTabPageStringUTF16);
            searchTabPageString.Controls.Add(searchTabPageStringLatin);
            searchTabPageString.Controls.Add(searchTabPageStringInput);
            searchTabPageString.Location = new Point(4, 34);
            searchTabPageString.Name = "searchTabPageString";
            searchTabPageString.Size = new Size(340, 78);
            searchTabPageString.TabIndex = 2;
            searchTabPageString.Text = "String";
            searchTabPageString.UseVisualStyleBackColor = true;
            // 
            // searchTabPageStringUTF16
            // 
            searchTabPageStringUTF16.AutoSize = true;
            searchTabPageStringUTF16.Enabled = false;
            searchTabPageStringUTF16.Location = new Point(142, 43);
            searchTabPageStringUTF16.Name = "searchTabPageStringUTF16";
            searchTabPageStringUTF16.Size = new Size(94, 29);
            searchTabPageStringUTF16.TabIndex = 2;
            searchTabPageStringUTF16.Text = "UTF-16";
            searchTabPageStringUTF16.UseVisualStyleBackColor = true;
            // 
            // searchTabPageStringLatin
            // 
            searchTabPageStringLatin.AutoSize = true;
            searchTabPageStringLatin.Checked = true;
            searchTabPageStringLatin.Enabled = false;
            searchTabPageStringLatin.Location = new Point(6, 43);
            searchTabPageStringLatin.Name = "searchTabPageStringLatin";
            searchTabPageStringLatin.Size = new Size(130, 29);
            searchTabPageStringLatin.TabIndex = 1;
            searchTabPageStringLatin.TabStop = true;
            searchTabPageStringLatin.Text = "ISO-8859-1";
            searchTabPageStringLatin.UseVisualStyleBackColor = true;
            // 
            // searchTabPageStringInput
            // 
            searchTabPageStringInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchTabPageStringInput.BorderStyle = BorderStyle.FixedSingle;
            searchTabPageStringInput.Enabled = false;
            searchTabPageStringInput.Location = new Point(6, 6);
            searchTabPageStringInput.MaxLength = 256;
            searchTabPageStringInput.Name = "searchTabPageStringInput";
            searchTabPageStringInput.PlaceholderText = "Enter string";
            searchTabPageStringInput.Size = new Size(328, 31);
            searchTabPageStringInput.TabIndex = 0;
            // 
            // fileOptionsGroupBox
            // 
            fileOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            fileOptionsGroupBox.Controls.Add(infoBox);
            fileOptionsGroupBox.Controls.Add(viewRawButton);
            fileOptionsGroupBox.Controls.Add(editRawButton);
            fileOptionsGroupBox.Controls.Add(exportAllButton);
            fileOptionsGroupBox.Controls.Add(replaceButton);
            fileOptionsGroupBox.Controls.Add(exportSelectedButton);
            fileOptionsGroupBox.Location = new Point(630, 12);
            fileOptionsGroupBox.Name = "fileOptionsGroupBox";
            fileOptionsGroupBox.Size = new Size(360, 268);
            fileOptionsGroupBox.TabIndex = 3;
            fileOptionsGroupBox.TabStop = false;
            fileOptionsGroupBox.Text = "File";
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(878, 818);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(112, 34);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // LittleEditor
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1002, 864);
            Controls.Add(cancelButton);
            Controls.Add(fileOptionsGroupBox);
            Controls.Add(searchOptionsGroupBox);
            Controls.Add(saveButton);
            Controls.Add(resourceList);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "LittleEditor";
            Text = "Big Viewer";
            ((System.ComponentModel.ISupportInitialize)resourceList).EndInit();
            resourceListContextMenu.ResumeLayout(false);
            searchOptionsGroupBox.ResumeLayout(false);
            searchOptionsGroupBox.PerformLayout();
            searchTabControl.ResumeLayout(false);
            searchTabPageBinary.ResumeLayout(false);
            searchTabPageBinary.PerformLayout();
            searchTabPageNumber.ResumeLayout(false);
            searchTabPageNumber.PerformLayout();
            searchTabPageString.ResumeLayout(false);
            searchTabPageString.PerformLayout();
            fileOptionsGroupBox.ResumeLayout(false);
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
        private Button exportAllButton;
        private ContextMenuStrip resourceListContextMenu;
        private ToolStripMenuItem resourceListContextMenuItemViewRaw;
        private ToolStripMenuItem resourceListContextMenuItemEditRaw;
        private ToolStripMenuItem resourceListContextMenuItemReplace;
        private ToolStripMenuItem resourceListContextMenuItemExportSelected;
        private GroupBox searchOptionsGroupBox;
        private GroupBox fileOptionsGroupBox;
        private TabControl searchTabControl;
        private TabPage searchTabPageBinary;
        private TabPage searchTabPageNumber;
        private TextBox searchTabPageBinaryInput;
        private TextBox searchTabPageNumberInput;
        private RadioButton searchTabPageNumber16Bit;
        private RadioButton searchTabPageNumber32Bit;
        private RadioButton searchTabPageNumber64Bit;
        private TabPage searchTabPageString;
        private TextBox searchTabPageStringInput;
        private DataGridViewTextBoxColumn ItemNumber;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Offset;
        private DataGridViewTextBoxColumn FileSize;
        private DataGridViewTextBoxColumn RawSize;
        private DataGridViewTextBoxColumn Format;
        private CheckBox searchDataOnlyCheckBox;
        private RadioButton searchTabPageStringLatin;
        private RadioButton searchTabPageStringUTF16;
        private Button cancelButton;
    }
}
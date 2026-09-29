using System.Text;

using BigViewer.Core;

namespace BigViewer.UI
{
    public partial class MainWindow : Form
    {
        internal ResourceFile? currentFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void viewRawButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;

                    bool alreadyOpen = false;
                    foreach (Form childForm in this.OwnedForms)
                    {
                        if (childForm.Tag != null && (int)childForm.Tag == selectedIndex)
                        {
                            alreadyOpen = true;
                            childForm.Focus();
                            break;
                        }
                    }
                    if (!alreadyOpen)
                    {
                        Utils.DisplayRaw(currentFile.GetRawData(selectedIndex).ToArray(), currentFile.GetTypes(selectedIndex), "[View] " + selectedIndex.ToString() + " in " + pathBox.Text, selectedIndex, this);
                    }
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void editRawButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    bool alreadyOpen = false;
                    foreach (Form childForm in this.OwnedForms)
                    {
                        if (childForm.Tag != null && (int)childForm.Tag == selectedIndex)
                        {
                            alreadyOpen = true;
                            childForm.Focus();
                            break;
                        }
                    }
                    if (!alreadyOpen)
                    {
                        Utils.DisplayEditRaw(currentFile.GetRawData(selectedIndex), "[Edit] " + selectedIndex.ToString() + " in " + pathBox.Text, currentFile, selectedIndex, DisplayInfoUI, this);
                    }
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    string selectedPath = Utils.OpenFilePath(Utils.GetFileTypeFilter(currentFile.GetTypes(resourceList.SelectedRows[0].Index)));
                    if (selectedPath.Length > 0)
                    {
                        try
                        {
                            currentFile.ReplaceResourceRaw(resourceList.SelectedRows[0].Index, File.ReadAllBytes(selectedPath));
                            DisplayInfoUI();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageBinary)
            {
                SearchAndDisplayResult(Utils.ConvertByteString(searchTabPageBinaryInput.Text));
            }
            else if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageNumber)
            {
                if (searchTabPageNumber16Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(Half.Parse(searchTabPageNumberInput.Text)));
                    }
                    else if (searchTabPageNumberInput.Text.Contains('-'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(short.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(ushort.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else if (searchTabPageNumber32Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(float.Parse(searchTabPageNumberInput.Text)));
                    }
                    else if (searchTabPageNumberInput.Text.Contains('-'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(int.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(uint.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else if (searchTabPageNumber64Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(double.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(long.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else
                {
                    MessageBox.Show("No length selected!", "Error");
                }
            }
            else if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageString)
            {
                if (searchTabPageStringLatin.Checked == true)
                {
                    try
                    {
                        SearchAndDisplayResult(Encoding.Latin1.GetBytes(searchTabPageStringInput.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                }
                else if (searchTabPageStringUTF16.Checked == true)
                {
                    try
                    {
                        SearchAndDisplayResult(Encoding.Unicode.GetBytes(searchTabPageStringInput.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                }
            }
        }

        private void resultsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resourceList.CurrentCell = resourceList.Rows[int.Parse(Utils.resultsBoxIdRegex.Match(resultsBox.GetItemText(resultsBox.SelectedItem) ?? "0:").Value)].Cells[0];
        }

        private void resourceList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Handle the right click
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.RowIndex < resourceList.RowCount)
                {
                    resourceList.Rows[e.RowIndex].Selected = true;
                    resourceList.CurrentCell = resourceList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    resourceListContextMenu.Show(Cursor.Position);
                }
            }
        }

        private void searchTabPageNumberInput_TextChanged(object? sender, EventArgs e)
        {
            ChangeSearchTabPageNumberButtons(Utils.ParseNumber(searchTabPageNumberInput.Text));
        }

        private void ChangeSearchTabPageNumberButtons(int mode)
        {
            switch (mode)
            {
                case 1:
                    searchTabPageNumber16Bit.Enabled = true;
                    searchTabPageNumber32Bit.Enabled = true;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                case 2:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = true;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                case 3:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber32Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = false;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                default:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber32Bit.Checked = false;
                    searchTabPageNumber64Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = false;
                    searchTabPageNumber64Bit.Enabled = false;
                    return;
            }
        }

        private void SearchAndDisplayResult(byte[] pattern)
        {
            if (currentFile != null && pattern.Length > 0)
            {
                resultsBox.Items.Clear();
                for (int i = 0; i < currentFile.ResourceCount; i++)
                {
                    if (!searchDataOnlyCheckBox.Checked || currentFile.GetTypes(i) == Constants.resourceTypeData)
                    {
                        int[] resu = Common.FindSequence(currentFile.GetRawData(i), pattern);
                        int resCount = resu.Length;
                        if (resCount > 0)
                        {
                            string[] searchResults = new string[resCount];
                            for (int j = 0; j < resCount; j++)
                            {
                                searchResults[j] = "0x" + resu[j].ToString("X");
                            }
                            resultsBox.Items.Add(i + ": " + string.Join(", ", searchResults));
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No file is open or input is invalid!", "Error");
            }
        }

        private void DisplayInfoUI()
        {
            if (currentFile != null)
            {
                int prevSelect = (resourceList.SelectedRows.Count == 1) ? resourceList.SelectedRows[0].Index : -1;
                infoBox.Items.Clear();
                resultsBox.Items.Clear();
                resourceList.Rows.Clear();
                // Display file info
                infoBox.Items.Add("Resource count: " + currentFile.ResourceCount.ToString());
                infoBox.Items.Add("Header size: " + "0x" + currentFile.HeaderSize.ToString("X"));
                infoBox.Items.Add("TOC start: " + "0x" + currentFile.TableStart.ToString("X"));
                infoBox.Items.Add("Content start: " + "0x" + currentFile.ContentStart.ToString("X"));
                infoBox.Items.Add("Additional header count: " + currentFile.RleCount.ToString());
                for (int i = 0; i < currentFile.RleCount; i++)
                {
                    infoBox.Items.Add(currentFile.GetRleEntryStartingVirtualId(i).ToString() + " " + currentFile.GetRleEntryCount(i).ToString() + " " + currentFile.GetRleEntryStartingId(i).ToString());
                }
                // Populate DataGridView using resource list
                for (int i = 0; i < currentFile.ResourceCount; i++)
                {
                    resourceList.Rows.Add(i.ToString(), currentFile.GetTypeName(i), "0x" + currentFile.GetOffset(i).ToString("X"), "0x" + currentFile.GetSize(i).ToString("X"), "0x" + currentFile.GetRawSize(i).ToString("X"), currentFile.GetFormatName(i));
                }
                resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                // Return to previously selected row
                if (prevSelect > 0)
                {
                    resourceList.Rows[prevSelect].Selected = true;
                    resourceList.CurrentCell = resourceList.Rows[prevSelect].Cells[0];
                }
            }
        }

        private void Reset()
        {
            currentFile = null;
            pathBox.Clear();
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            GC.Collect();
            editRawButton.Enabled = false;
            viewRawButton.Enabled = false;
            exportSelectedToolStripMenuItem.Enabled = false;
            exportAllToolStripMenuItem.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            replaceButton.Enabled = false;

            searchTabPageNumberInput.TextChanged -= searchTabPageNumberInput_TextChanged;
            foreach (Control control in searchTabPageBinary.Controls)
            {
                control.Enabled = false;
            }
            searchTabPageNumberInput.Enabled = false;
            ChangeSearchTabPageNumberButtons(0);
            foreach (Control control in searchTabPageString.Controls)
            {
                control.Enabled = false;
            }
            searchTabControl.Enabled = false;

            searchDataOnlyCheckBox.Enabled = false;
            searchButton.Enabled = false;
            searchOptionsGroupBox.Enabled = false;
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i] != this)
                {
                    Application.OpenForms[i].Close();
                }
            }
            Reset();
            string selectedPath = Utils.OpenFilePath("BIG files (*.big)|*.big|All files (*.*)|*.*");
            if (selectedPath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(selectedPath, FileMode.Open, FileAccess.Read))
                    {
                        currentFile = new ResourceFile(fs);
                    }
                    // currentFile = new ResourceFile(File.ReadAllBytes(selectedPath));
                    pathBox.Text = selectedPath;

                    DisplayInfoUI();

                    viewRawButton.Enabled = true;
                    editRawButton.Enabled = true;
                    replaceButton.Enabled = true;
                    exportSelectedToolStripMenuItem.Enabled = true;
                    exportAllToolStripMenuItem.Enabled = true;
                    saveFileToolStripMenuItem.Enabled = true;

                    searchOptionsGroupBox.Enabled = true;
                    searchTabControl.Enabled = true;
                    foreach (Control control in searchTabPageBinary.Controls)
                    {
                        control.Enabled = true;
                    }
                    searchTabPageNumberInput.Enabled = true;
                    ChangeSearchTabPageNumberButtons(0);
                    foreach (Control control in searchTabPageString.Controls)
                    {
                        control.Enabled = true;
                    }
                    searchTabPageNumberInput.TextChanged += searchTabPageNumberInput_TextChanged;

                    searchDataOnlyCheckBox.Enabled = true;
                    searchButton.Enabled = true;

                    int[] result = currentFile.ScanForEligibleDict(true);
                    if (result.Length == 1)
                    {
                        MessageBox.Show("Automatically selected dictionary resource found at index " + result[0].ToString(), "Info");
                        currentFile.TryEnableEnhanced();
                        if (currentFile.IsEnhanced)
                        {
                            MessageBox.Show("The current file is self-consistent. Additional information can be viewed.", "Info");
                        }
                        else
                        {
                            MessageBox.Show("The current file is not self-consistent. Additional information cannot be viewed.", "Info");
                        }
                    }
                    else if (result.Length == 0)
                    {
                        MessageBox.Show("No eligible dictionary resources found; operating in manual mode", "Info");
                    }
                    else
                    {
                        object? choice = Utils.ComboBoxDialog(result.Cast<object>(), "Select dictionary resource", this);
                        if (choice != null)
                        {
                            currentFile.DictResId = (int)choice;
                            currentFile.TryEnableEnhanced();
                            if (currentFile.IsEnhanced)
                            {
                                MessageBox.Show("The current file is self-consistent. Additional information can be viewed.", "Info");
                            }
                            else
                            {
                                MessageBox.Show("The current file is not self-consistent. Additional information cannot be viewed.", "Info");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
            }
        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    // Resource res = currentFile.Resources[resourceList.SelectedRows[0].Index];
                    Utils.SaveDataToFile(currentFile.GetRawData(selectedIndex), Utils.GetFileTypeFilter(currentFile.GetTypes(selectedIndex)), pathBox.Text, "_resource" + selectedIndex.ToString());
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                string folderPath = Utils.OpenFolderPath();
                string fileName = Path.GetFileNameWithoutExtension(pathBox.Text);
                if (folderPath.Length > 0)
                {
                    for (int i = 0; i < currentFile.ResourceCount; i++)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(folderPath, fileName + "_resource" + i.ToString() + Utils.GetTypeExt(currentFile.GetTypes(i))), FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(currentFile.GetRawData(i));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Path!", "Error");
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.InitialDirectory = Path.GetDirectoryName(pathBox.Text);
                    saveDialog.Filter = "BIG files (*.big)|*.big|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 1;
                    saveDialog.RestoreDirectory = true;

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                        {
                            currentFile.ConstructFileAndWrite(fs);
                            // fs.Write(currentFile.ConstructFile());
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No file open!", "Error");
            }
        }

        private void additionalInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile != null && currentFile.IsEnhanced)
            {
                bool alreadyOpen = false;
                foreach (Form childForm in this.OwnedForms)
                {
                    if (childForm.Tag != null && (int)childForm.Tag == -4)
                    {
                        alreadyOpen = true;
                        childForm.Focus();
                        break;
                    }
                }
                if (!alreadyOpen)
                {
                    new EnhancedInfo(currentFile.GetEnhancedTable()).Show(this);
                }
            }
            else
            {
                MessageBox.Show("No file open, or current file is not self-consistent!", "Error");
            }
        }
    }
}
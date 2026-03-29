using System.Text;

using BigViewer.Common;
using BigViewer.Resources;

namespace BigViewer.UI
{
    public partial class MainWindow : Form
    {
        internal ResourceFile? currentFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFileButton_Click(object sender, EventArgs e)
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
                    currentFile = new ResourceFile(selectedPath, File.ReadAllBytes(selectedPath));
                    pathBox.Text = selectedPath;

                    DisplayInfoUI();

                    viewRawButton.Enabled = true;
                    editRawButton.Enabled = true;
                    replaceButton.Enabled = true;
                    exportSelectedButton.Enabled = true;
                    exportAllButton.Enabled = true;
                    saveFileButton.Enabled = true;

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
            }
        }

        private void viewRawButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    if (selectedIndex == currentFile.resources[selectedIndex].id)
                    {
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
                            Resource res = currentFile.resources[resourceList.SelectedRows[0].Index];
                            Utils.DisplayRaw(res.rawData, res.type, "[View] " + res.id.ToString() + " in " + pathBox.Text, res.id, this);
                        }
                    }
                    else
                    {
                        MessageBox.Show("List mismatch! Please relaunch.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void editRawButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    if (selectedIndex == currentFile.resources[selectedIndex].id)
                    {
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
                            Resource res = currentFile.resources[resourceList.SelectedRows[0].Index];
                            Utils.DisplayEditRaw(res.rawData, res.type, "[Edit] " + res.id.ToString() + " in " + pathBox.Text, currentFile, res.id, DisplayInfoUI, this);
                        }
                    }
                    else
                    {
                        MessageBox.Show("List mismatch! Please relaunch.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    string selectedPath = Utils.OpenFilePath(Utils.GetFileTypeFilter(currentFile.resources[resourceList.SelectedRows[0].Index].type));
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
        }

        private void exportSelectedButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    Resource res = currentFile.resources[resourceList.SelectedRows[0].Index];
                    Utils.SaveDataToFile(res.rawData, Utils.GetFileTypeFilter(res.type), pathBox.Text, "_resource" + res.id.ToString());
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                string folderPath = Utils.OpenFolderPath();
                string fileName = Path.GetFileNameWithoutExtension(pathBox.Text);
                if (folderPath.Length > 0)
                {
                    foreach (Resource res in currentFile.resources)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(folderPath, fileName + "_resource" + res.id.ToString() + Utils.GetTypeExt(res.type)), FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(res.rawData, 0, res.rawData.Length);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Path!", "Error");
                }
            }
        }

        private void saveFileButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                Utils.SaveDataToFile(currentFile.ConstructFile(), "BIG files (*.big)|*.big|All files (*.*)|*.*", pathBox.Text, null);
            }
        }

        private void checksumButton_Click(object sender, EventArgs e)
        {
            string selectedPath = Utils.OpenFilePath("All files (*.*)|*.*");
            if (selectedPath.Length > 0)
            {
                try
                {
                    long check = Utils.GetChecksum(File.ReadAllBytes(selectedPath));
                    MessageBox.Show("0x" + check.ToString("X") + "\n" + check.ToString(), "Result");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
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
                resourceList.Rows[e.RowIndex].Selected = true;
                resourceList.CurrentCell = resourceList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                resourceListContextMenu.Show(Cursor.Position);
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
                foreach (Resource res in searchDataOnlyCheckBox.Checked ? currentFile.resources.Where((x) => { return x.type.SequenceEqual(Utils.resourceTypeData); }) : currentFile.resources)
                {
                    int[] resu = Utils.FindSequence(res.rawData, pattern);
                    int resCount = resu.Length;
                    if (resCount > 0)
                    {
                        string[] searchResults = new string[resCount];
                        for (int i = 0; i < resCount; i++)
                        {
                            searchResults[i] = "0x" + resu[i].ToString("X");
                        }
                        resultsBox.Items.Add(res.id.ToString() + ": " + string.Join(", ", searchResults));
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid input!", "Error");
            }
        }

        private void DisplayInfoUI()
        {
            if (currentFile != null)
            {
                int prevSelect = -1;
                if (resourceList.SelectedRows.Count == 1)
                {
                    prevSelect = resourceList.SelectedRows[0].Index;
                }
                infoBox.Items.Clear();
                resultsBox.Items.Clear();
                resourceList.Rows.Clear();
                // Display file info
                infoBox.Items.Add("Resource count: " + currentFile.resourceCount.ToString());
                infoBox.Items.Add("Header size: " + "0x" + currentFile.headerSize.ToString("X"));
                infoBox.Items.Add("Additional header count: " + currentFile.addHeaderCount.ToString());
                infoBox.Items.Add("TOC start: " + "0x" + currentFile.tableStart.ToString("X"));
                infoBox.Items.Add("Content start: " + "0x" + currentFile.tableEnd.ToString("X"));
                // Populate DataGridView using resource list
                foreach (Resource res in currentFile.resources)
                {
                    resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName);
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
            exportSelectedButton.Enabled = false;
            exportAllButton.Enabled = false;
            saveFileButton.Enabled = false;
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
    }
}
using System.Text;

using BigViewer.Common;
using BigViewer.Resources;

namespace BigViewer.UI
{
    internal partial class LittleEditor : Form
    {
        internal LittleResourceFile? currentLittleFile;
        private ResourceFile? parentResourceFile;
        private LittleResourceFile? parentLittleResourceFile;
        private int idInParent = -1;
        private Action? action;

        // Editing resource from parent ResourceFile
        public LittleEditor(byte[] data, string title, ResourceFile _parentResourceFile, int _id, Action _action)
        {
            InitializeComponent();

            if (_parentResourceFile != null)
            {
                if (_id >= 0 && _id < _parentResourceFile.resourceCount)
                {
                    parentResourceFile = _parentResourceFile;
                    idInParent = _id;
                }
                else
                {
                    throw new ArgumentException("Invalid resource ID received!");
                }
            }
            else
            {
                throw new ArgumentException("Parent resource cannot be null!");
            }
            parentLittleResourceFile = null;
            idInParent = _id;
            action = _action;
            this.Tag = _id;
            this.Text = title;

            currentLittleFile = new LittleResourceFile(data);

            DisplayInfoUI();

            viewRawButton.Enabled = true;
            editRawButton.Enabled = true;
            replaceButton.Enabled = true;
            exportSelectedButton.Enabled = true;
            exportAllButton.Enabled = true;
            saveButton.Enabled = true;

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

        // Editing resource from parent LittleResourceFile
        public LittleEditor(byte[] data, string title, LittleResourceFile _parentLittleResourceFile, int _id, Action _action)
        {
            InitializeComponent();

            if (_parentLittleResourceFile != null)
            {
                if (_id >= 0 && _id < _parentLittleResourceFile.resourceCount)
                {
                    parentLittleResourceFile = _parentLittleResourceFile;
                    idInParent = _id;
                }
                else
                {
                    throw new ArgumentException("Invalid resource ID received!");
                }
            }
            else
            {
                throw new ArgumentException("Parent resource cannot be null!");
            }
            parentResourceFile = null;
            idInParent = _id;
            action = _action;
            this.Tag = _id;
            this.Text = title;

            currentLittleFile = new LittleResourceFile(data);

            DisplayInfoUI();

            viewRawButton.Enabled = true;
            editRawButton.Enabled = true;
            replaceButton.Enabled = true;
            exportSelectedButton.Enabled = true;
            exportAllButton.Enabled = true;
            saveButton.Enabled = true;

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

        private void viewRawButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    if (selectedIndex == currentLittleFile.resources[selectedIndex].id)
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
                            Resource res = currentLittleFile.resources[resourceList.SelectedRows[0].Index];
                            Utils.DisplayRaw(res.rawData, res.type, "[View] " + res.id.ToString() + " in " + idInParent.ToString(), res.id, this);
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
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    int selectedIndex = resourceList.SelectedRows[0].Index;
                    if (selectedIndex == currentLittleFile.resources[selectedIndex].id)
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
                            Resource res = currentLittleFile.resources[resourceList.SelectedRows[0].Index];
                            Utils.DisplayEditRaw(res.rawData, res.type, "[Edit] " + res.id.ToString() + " in " + idInParent.ToString(), currentLittleFile, res.id, DisplayInfoUI, this);
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
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    string selectedPath = Utils.OpenFilePath(Utils.GetFileTypeFilter(currentLittleFile.resources[resourceList.SelectedRows[0].Index].type));
                    if (selectedPath.Length > 0)
                    {
                        try
                        {
                            currentLittleFile.ReplaceResourceRaw(resourceList.SelectedRows[0].Index, File.ReadAllBytes(selectedPath));
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
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    Resource res = currentLittleFile.resources[resourceList.SelectedRows[0].Index];
                    Utils.SaveDataToFile(res.rawData, Utils.GetFileTypeFilter(res.type), null, "_littleresource" + res.id.ToString());
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null)
            {
                string folderPath = Utils.OpenFolderPath();
                // string fileName = Path.GetFileNameWithoutExtension(pathBox.Text);
                if (folderPath.Length > 0)
                {
                    foreach (Resource res in currentLittleFile.resources)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(folderPath, "res_" + idInParent.ToString() + "_littleresource" + res.id.ToString() + Utils.GetTypeExt(res.type)), FileMode.Create, FileAccess.Write))
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null && idInParent != -1 && action != null)
            {
                if (parentResourceFile != null)
                {
                    parentResourceFile.ReplaceResourceRaw(idInParent, currentLittleFile.ConstructFile());
                    action();
                }
                else if (parentLittleResourceFile != null)
                {
                    parentLittleResourceFile.ReplaceResourceRaw(idInParent, currentLittleFile.ConstructFile());
                    action();
                }
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
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
            if (currentLittleFile != null && pattern.Length > 0)
            {
                resultsBox.Items.Clear();
                foreach (Resource res in searchDataOnlyCheckBox.Checked ? currentLittleFile.resources.Where((x) => { return x.type.SequenceEqual(Utils.resourceTypeData); }) : currentLittleFile.resources)
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
            if (currentLittleFile != null)
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
                infoBox.Items.Add("Core: " + currentLittleFile.isCore.ToString());
                infoBox.Items.Add("Total size: " + "0x" + currentLittleFile.totalSize.ToString("X"));
                infoBox.Items.Add("Resource count: " + currentLittleFile.resourceCount.ToString());
                infoBox.Items.Add("Type TOC start: " + "0x" + currentLittleFile.typeTableStart.ToString("X"));
                infoBox.Items.Add("Content start: " + "0x" + currentLittleFile.typeTableEnd.ToString("X"));
                // Populate DataGridView using resource list
                if (!currentLittleFile.isCore)
                {
                    foreach (Resource res in currentLittleFile.resources)
                    {
                        resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName);
                    }
                }
                else
                {
                    DataGridViewTextBoxColumn otherData = new DataGridViewTextBoxColumn();
                    otherData.HeaderText = "Other Data";
                    otherData.MinimumWidth = 8;
                    otherData.Name = "OtherData";
                    // otherData.ReadOnly = true;
                    otherData.Resizable = DataGridViewTriState.False;
                    otherData.SortMode = DataGridViewColumnSortMode.NotSortable;
                    resourceList.Columns.Add(otherData);
                    foreach (Resource res in currentLittleFile.resources)
                    {
                        resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName, BitConverter.ToString(res.otherData));
                    }
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
    }
}
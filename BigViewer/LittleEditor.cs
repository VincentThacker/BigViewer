using Microsoft.VisualBasic;

namespace BigViewer
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

            editRawButton.Enabled = true;
            viewRawButton.Enabled = true;
            exportSelectedButton.Enabled = true;
            searchButton.Enabled = true;
            replaceButton.Enabled = true;
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
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

            editRawButton.Enabled = true;
            viewRawButton.Enabled = true;
            exportSelectedButton.Enabled = true;
            searchButton.Enabled = true;
            replaceButton.Enabled = true;
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
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
                            if ((int)childForm.Tag == selectedIndex)
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
                            if ((int)childForm.Tag == selectedIndex)
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

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null)
            {
                string input = Interaction.InputBox("Enter sequence of bytes to search (separated by -)", "Search");
                byte[] pattern = Utils.ConvertStringToBytes(input);
                if (pattern.Length > 0)
                {
                    resultsBox.Items.Clear();
                    foreach (Resource res in currentLittleFile.resources)
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
                            resultsBox.Items.Add(res.id.ToString() + ": " + String.Join(", ", searchResults));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid input!", "Error");
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

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    string selectedPath = Utils.OpenFilePath(Utils.GetFileTypeFilter(currentLittleFile.resources[resourceList.SelectedRows[0].Index].type));
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
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
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

        private void DisplayInfoUI()
        {
            if (currentLittleFile != null)
            {
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
                    otherData.ReadOnly = true;
                    otherData.Resizable = DataGridViewTriState.False;
                    resourceList.Columns.Add(otherData);
                    foreach (Resource res in currentLittleFile.resources)
                    {
                        resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName, BitConverter.ToString(res.otherData));
                    }
                }
                resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            }
        }
    }
}
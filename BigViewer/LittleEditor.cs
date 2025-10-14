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
            try
            {
                Reset();

                parentResourceFile = _parentResourceFile;
                parentLittleResourceFile = null;
                idInParent = _id;
                action = _action;
                this.Text = title;

                currentLittleFile = new LittleResourceFile(data);

                DisplayInfoUI();

                editRawButton.Enabled = true;
                viewRawButton.Enabled = true;
                exportSelectedButton.Enabled = true;
                saveButton.Enabled = true;
                searchButton.Enabled = true;
                replaceButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Reset();
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }


        // Editing resource from parent ResourceFile
        public LittleEditor(byte[] data, string title, LittleResourceFile _parentLittleResourceFile, int _id, Action _action)
        {
            InitializeComponent();
            try
            {
                Reset();

                parentResourceFile = null;
                parentLittleResourceFile = _parentLittleResourceFile;

                idInParent = _id;
                action = _action;
                this.Text = title;

                currentLittleFile = new LittleResourceFile(data);

                DisplayInfoUI();

                editRawButton.Enabled = true;
                viewRawButton.Enabled = true;
                exportSelectedButton.Enabled = true;
                saveButton.Enabled = true;
                searchButton.Enabled = true;
                replaceButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Reset();
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }

        private void viewRawButton_Click(object sender, EventArgs e)
        {
            if (currentLittleFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    Resource res = currentLittleFile.resources[resourceList.SelectedRows[0].Index];
                    Utils.DisplayRaw(res.rawData, res.type, res.id.ToString());
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
                    Resource res = currentLittleFile.resources[resourceList.SelectedRows[0].Index];
                    Utils.DisplayEditRaw(res.rawData, res.type, res.id.ToString(), currentLittleFile, res.id, DisplayInfoUI);
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (parentResourceFile != null && action != null)
            {
                parentResourceFile.ReplaceResourceRaw(idInParent, currentLittleFile.ConstructFile());
                action();
            }
            else if (parentLittleResourceFile != null && action != null)
            {
                parentLittleResourceFile.ReplaceResourceRaw(idInParent, currentLittleFile.ConstructFile());
                action();
            }
            Close();
        }

        public void DisplayInfoUI()
        {
            if (currentLittleFile != null)
            {
                infoBox.Items.Clear();
                resultsBox.Items.Clear();
                resourceList.Rows.Clear();
                // Display file info
                infoBox.Items.Add("Total size: " + "0x" + currentLittleFile.totalSize.ToString("X"));
                infoBox.Items.Add("Resource count: " + currentLittleFile.resourceCount.ToString());

                // infoBox.Items.Add("Content start: " + "0x" + currentFile.tableEnd.ToString("X"));
                // infoBox.Items.Add("Clean size: " + currentFile.contentSize.ToString());

                // Populate DataGridView using resource list
                foreach (Resource res in currentLittleFile.resources)
                {
                    resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName);
                }
                resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            }
        }

        void Reset()
        {
            currentLittleFile = null;
            idInParent = -1;
            action = null;
            this.Text = "";
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            GC.Collect();
            editRawButton.Enabled = false;
            viewRawButton.Enabled = false;
            exportSelectedButton.Enabled = false;
            saveButton.Enabled = false;
            searchButton.Enabled = false;
            replaceButton.Enabled = false;
        }


    }
}

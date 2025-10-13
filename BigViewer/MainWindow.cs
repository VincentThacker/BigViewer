using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BigViewer
{
    public partial class MainWindow : Form
    {
        ResourceFile? currentFile;

        public MainWindow()
        {
            InitializeComponent();
            pathBox.AutoSize = false;
            pathBox.Size = new Size(pathBox.Size.Width, openFileButton.Size.Height);
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            string selectedPath = Utils.OpenFilePath("BIG files (*.big)|*.big|All files (*.*)|*.*");
            try
            {
                Reset();
                currentFile = new ResourceFile(selectedPath, File.ReadAllBytes(selectedPath));
                pathBox.Text = selectedPath;

                DisplayInfoUI();

                viewButton.Enabled = true;
                viewRawButton.Enabled = true;
                exportSelectedButton.Enabled = true;
                exportDataButton.Enabled = true;
                saveFileButton.Enabled = true;
                searchButton.Enabled = true;
                replaceButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Reset();
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    currentFile.resources[resourceList.SelectedRows[0].Index].Display();
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void viewRawButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    currentFile.resources[resourceList.SelectedRows[0].Index].DisplayRaw();
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                string input = Interaction.InputBox("Enter sequence of bytes to search (separated by -)", "Search");
                if ((new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$")).IsMatch(input))
                {
                    resultsBox.Items.Clear();
                    byte[] pattern = Utils.ConvertStringToBytes(input);
                    foreach (Resource res in currentFile.resources)
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
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    Resource res = currentFile.resources[resourceList.SelectedRows[0].Index];
                    string filter;
                    switch (res.type)
                    {
                        case [0x78, 0x86, 0x17, 0xB7]:
                            filter = "PNG files (*.png)|*.png";
                            break;
                        case [0x54, 0x77, 0x8A, 0xFD]:
                            filter = "WAV files (*.wav)|*.wav";
                            break;
                        case [0x52, 0x49, 0x46, 0x46]:
                            filter = "OGG files (*.ogg)|*.ogg";
                            break;
                        default:
                            filter = "Data (*.bin)|*.bin";
                            break;
                    }
                    Utils.SaveDataToFile(res.rawData, filter, pathBox.Text, "_resource" + res.id.ToString());
                }
                else
                {
                    MessageBox.Show("Number of selected items is incorrect!", "Error");
                }
            }
        }

        private void exportDataButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
                List<Resource> resourcesDataOnly = currentFile.resources.Where((x) => x.type.SequenceEqual(dataType)).ToList();
                List<byte> dat = new List<byte>();
                foreach (Resource res in resourcesDataOnly)
                {
                    dat.AddRange(res.rawData);
                }
                Utils.SaveDataToFile(dat, "Data (*.bin)|*.bin", pathBox.Text, "_alldata");
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
            try
            {
                MessageBox.Show(Utils.GetChecksum(File.ReadAllBytes(selectedPath)).ToString(), "Result");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                if (resourceList.SelectedRows.Count == 1)
                {
                    string selectedPath = Utils.OpenFilePath(Utils.GetFileTypeFilter(currentFile.resources[resourceList.SelectedRows[0].Index].type));
                    try
                    {
                        currentFile.ReplaceResource(resourceList.SelectedRows[0].Index, File.ReadAllBytes(selectedPath));
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

        public void DisplayInfoUI()
        {
            if (currentFile != null)
            {
                infoBox.Items.Clear();
                resultsBox.Items.Clear();
                resourceList.Rows.Clear();
                // Display file info
                infoBox.Items.Add("Header size: " + "0x" + currentFile.headerSize.ToString("X"));
                infoBox.Items.Add("TOC start: " + "0x" + currentFile.tableStart.ToString("X"));
                infoBox.Items.Add("Resource count: " + currentFile.resourceCount.ToString());
                infoBox.Items.Add("Content start: " + "0x" + currentFile.tableEnd.ToString("X"));
                infoBox.Items.Add("Clean size: " + currentFile.contentSize.ToString());
                infoBox.Items.Add("TOC end: " + "0x" + currentFile.firstResourceOffset.ToString("X"));

                // Populate DataGridView using resource list
                foreach (Resource res in currentFile.resources)
                {
                    resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName);
                }
                resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            }
        }

        void Reset()
        {
            pathBox.Clear();
            currentFile = null;
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            GC.Collect();
            viewButton.Enabled = false;
            viewRawButton.Enabled = false;
            exportSelectedButton.Enabled = false;
            exportDataButton.Enabled = false;
            saveFileButton.Enabled = false;
            searchButton.Enabled = false;
            replaceButton.Enabled = false;
        }
    }
}
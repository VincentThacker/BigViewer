using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace BigViewer
{
    public partial class MainWindow : Form
    {
        ResourceFile currentFile;
        string validPath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            // File found
            if (File.Exists(pathBox.Text))
            {
                try
                {
                    Reset();
                    currentFile = new ResourceFile(pathBox.Text, File.ReadAllBytes(pathBox.Text));
                    validPath = pathBox.Text;

                    // Display file info
                    infoBox.Items.Add("Header size: " + "0x" + currentFile.headerSize.ToString("X"));
                    infoBox.Items.Add("TOC start: " + "0x" + currentFile.tableStart.ToString("X"));
                    infoBox.Items.Add("Resource count: " + currentFile.resourceCount.ToString());
                    infoBox.Items.Add("Content start: " + "0x" + currentFile.tableEnd.ToString("X"));
                    infoBox.Items.Add("Clean size: " + currentFile.contentSize.ToString());
                    infoBox.Items.Add("TOC end: " + "0x" + currentFile.firstResourceOffset.ToString("X"));

                    // Populate DataGridView using masterList
                    foreach (Resource res in currentFile.resources)
                    {
                        resourceList.Rows.Add(res.id.ToString(), res.typeName, "0x" + res.offset.ToString("X"), "0x" + res.size.ToString("X"), "0x" + res.rawSize.ToString("X"), res.formatName);
                    }
                    resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    viewButton.Enabled = true;
                    saveButton.Enabled = true;
                    searchButton.Enabled = true;
                    exportDataButton.Enabled = true;
                }
                catch (Exception ex)
                {
                    Reset();
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }

            }
            // File not found
            else
            {
                MessageBox.Show("File not found", "Error");
            }
        }

        private void viewButton_Click(object sender, EventArgs e)
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

        private void saveButton_Click(object sender, EventArgs e)
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
                SaveDataToFile(res.rawData, filter, "_resource" + res.id.ToString());
            }
            else
            {
                MessageBox.Show("Number of selected items is incorrect!", "Error");
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
            // List<Dictionary<string, object>> masterListDataOnly = masterList.Where((x) => ((byte[])x["type"]).SequenceEqual(dataType)).ToList();
            string input = Interaction.InputBox("Enter sequence of bytes to search (separated by -)", "Search");
            if ((new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$")).IsMatch(input))
            {
                resultsBox.Items.Clear();
                byte[] pattern = GetBytes(input);
                foreach (Resource res in currentFile.resources)
                {
                    int[] resu = FindSequence(res.rawData, pattern);
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
        }

        private void exportDataButton_Click(object sender, EventArgs e)
        {
            byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
            List<Resource> resourcesDataOnly = currentFile.resources.Where((x) => x.type.SequenceEqual(dataType)).ToList();
            List<byte> dat = new List<byte>();
            foreach (Resource res in resourcesDataOnly)
            {
                dat.AddRange(res.rawData);
            }
            SaveDataToFile(dat, "Data (*.bin)|*.bin", "_alldata");
        }

        void Reset()
        {
            validPath = "";
            currentFile = null;
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            GC.Collect();
            viewButton.Enabled = false;
            saveButton.Enabled = false;
            searchButton.Enabled = false;
            exportDataButton.Enabled = false;
        }

        byte[] GetBytes(string input)
        {
            return input.Split('-').Select((s) => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray();
        }

        int[] FindSequence(byte[] data, byte[] pattern)
        {
            List<int> matchesList = [];
            for (int i = 0; i + pattern.Length <= data.Length; i++)
            {
                bool allSame = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        allSame = false;
                        break;
                    }
                }
                if (allSame)
                {
                    matchesList.Add(i);
                }
            }
            return matchesList.ToArray();
        }

        void SaveDataToFile(byte[] dataToSave, string filter, string fileNameAppend)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Path.GetDirectoryName(validPath);
            saveDialog.Filter = filter;
            saveDialog.FilterIndex = 1;
            saveDialog.FileName = Path.GetFileNameWithoutExtension(validPath) + fileNameAppend;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(dataToSave.ToArray(), 0, dataToSave.Length);
                }
            }
        }

        void SaveDataToFile(List<byte> dataToSave, string filter, string fileNameAppend)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Path.GetDirectoryName(validPath);
            saveDialog.Filter = filter;
            saveDialog.FilterIndex = 1;
            saveDialog.FileName = Path.GetFileNameWithoutExtension(validPath) + fileNameAppend;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(dataToSave.ToArray(), 0, dataToSave.Count);
                }
            }
        }
    }
}

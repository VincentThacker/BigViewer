using Microsoft.VisualBasic;
using NAudio.Vorbis;
using NAudio.Wave;
using System.Diagnostics;
using System.IO.Compression;
using System.Media;
using System.Text.RegularExpressions;

namespace BigViewer
{
    public partial class MainWindow : Form
    {
        byte[] master;
        List<Dictionary<string, object>> masterList = [];

        uint contentSize = 0;
        uint tableEnd = 0;
        uint resourceCount = 0;
        uint tableStart = 0;
        uint firstResourceOffset = 0;
        string validPath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            GC.Collect();
            // File found
            if (File.Exists(pathBox.Text))
            {
                master = File.ReadAllBytes(pathBox.Text);
                // Correct FGIB header
                if (master[0x0..0x4].SequenceEqual(new byte[] { 0x46, 0x47, 0x49, 0x42 }))
                {
                    Reset();

                    validPath = pathBox.Text;

                    // Read file header and display in infoBox1
                    infoBox.Items.Add("Header size: " + "0x" + BitConverter.ToUInt32(master[0x8..0xC]).ToString("X"));

                    tableStart = BitConverter.ToUInt32(master[0x10..0x14]);
                    resourceCount = BitConverter.ToUInt32(master[0x14..0x18]);
                    tableEnd = BitConverter.ToUInt32(master[0x18..0x1C]);
                    contentSize = BitConverter.ToUInt32(master[0x1C..0x20]);
                    firstResourceOffset = BitConverter.ToUInt32(master[checked((int)(tableStart + 4))..checked((int)(tableStart + 8))]);

                    infoBox.Items.Add("TOC start: " + "0x" + tableStart.ToString("X"));
                    infoBox.Items.Add("Resource count: " + resourceCount.ToString());
                    infoBox.Items.Add("Content start: " + "0x" + tableEnd.ToString("X"));
                    infoBox.Items.Add("Clean size: " + contentSize.ToString());
                    infoBox.Items.Add("TOC end: " + "0x" + firstResourceOffset.ToString("X"));

                    // Check for inconsistencies and show error if present
                    string errs = "";
                    if (resourceCount * 8 != (tableEnd - tableStart - 8))
                    {
                        errs += "TOC length and resource count mismatch!\n";
                    }
                    if (firstResourceOffset != tableEnd)
                    {
                        errs += "TOC end and content start mismatch!\n";
                    }
                    if ((contentSize + tableEnd) != master.Length)
                    {
                        errs += "Content Size + TOC and file size mismatch!\n";
                    }

                    if (errs.Length > 0)
                    {
                        MessageBox.Show(errs, "Error");
                    }
                    errs = null;

                    // Check if TOC is ordered
                    List<uint> testList = [];
                    List<uint> sortList = [];
                    for (int i = 1; i < resourceCount; i++)
                    {
                        var num = tableStart + (i * 8);
                        testList.Add(BitConverter.ToUInt32(master[checked((int)(num + 4))..checked((int)(num + 8))]));
                        sortList.Add(BitConverter.ToUInt32(master[checked((int)(num + 4))..checked((int)(num + 8))]));
                        sortList.Sort();
                    }
                    if (!sortList.SequenceEqual(testList))
                    {
                        testList = null;
                        sortList = null;
                        MessageBox.Show("TOC is not ordered! Abandoning.", "Error");
                        Reset();
                        master = null;
                        return;
                    }
                    else
                    {
                        testList = null;
                        sortList = null;
                    }

                    // Read TOC, check for compression and save processed data
                    masterList.Clear();
                    for (int i = 0; i < resourceCount; i++)
                    {
                        var num = tableStart + (i * 8);
                        uint addr = BitConverter.ToUInt32(master[checked((int)(num + 4))..checked((int)(num + 8))]); // offset
                        byte[] type = master[checked((int)(num))..checked((int)(num + 4))]; // 4-byte type
                        uint size = BitConverter.ToUInt32(master[checked((int)(num + 12))..checked((int)(num + 16))]) - addr; // size
                        uint format = 0;
                        byte[] processedData;
                        (format, processedData) = ProcessRawResourceData(master[checked((int)addr)..checked((int)(addr + size))]);
                        // no., type, offset, size, first 4 bytes, format, processed data
                        masterList.Add(new Dictionary<string, object>
                        {
                            {"id",  i},
                            {"type", type},
                            {"offset", addr},
                            {"size", size},
                            // {"starter", master[checked((int)addr)..checked((int)(addr + 4))]}
                            {"datasize", processedData.Length},
                            {"format", format},
                            {"data", processedData}
                        });
                    }

                    // Populate DataGridView using masterList
                    for (int i = 0; i < resourceCount; i++)
                    {
                        Dictionary<string, object> resourceItem = masterList[i];
                        string resourceType;
                        switch ((byte[])resourceItem["type"])
                        {
                            case [0x23, 0x22, 0xE0, 0xF4]:
                                resourceType = "data";
                                break;
                            case [0x78, 0x86, 0x17, 0xB7]:
                                resourceType = "png";
                                break;
                            case [0x54, 0x77, 0x8A, 0xFD]:
                                resourceType = "wav";
                                break;
                            case [0x52, 0x49, 0x46, 0x46]:
                                resourceType = "ogg";
                                break;
                            case [0x05, 0xC5, 0xE4, 0x69]:
                                resourceType = "[start]";
                                break;
                            case [0xDC, 0xAA, 0x86, 0xF6]:
                                resourceType = "[end]";
                                break;
                            default:
                                resourceType = BitConverter.ToString((byte[])resourceItem["type"]);
                                break;
                        }
                        string formatType;
                        switch ((uint)resourceItem["format"])
                        {
                            case 1:
                                formatType = "none";
                                break;
                            case 2:
                                formatType = "zlib";
                                break;
                            default:
                                formatType = "unknown";
                                break;
                        }
                        resourceList.Rows.Add(((int)resourceItem["id"]).ToString(), resourceType, "0x" + ((uint)resourceItem["offset"]).ToString("X"), "0x" + ((uint)resourceItem["size"]).ToString("X"), "0x" + ((int)resourceItem["datasize"]).ToString("X"), formatType);
                    }
                    resourceList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    resourceList.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    viewButton.Enabled = true;
                    saveButton.Enabled = true;
                    searchButton.Enabled = true;
                    exportDataButton.Enabled = true;
                }
                // Incorrect FGIB header
                else
                {
                    master = null;
                    MessageBox.Show("Incorrect BIG header", "Error");
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
                Dictionary<string, object> resourceItem = masterList[resourceList.SelectedRows[0].Index];
                switch ((byte[])resourceItem["type"])
                {
                    /*
                    case [0x23, 0x22, 0xE0, 0xF4]:
                        try
                        {
                            HexView datView = new HexView((byte[])resourceItem["data"]);
                            datView.Text = resourceItem["id"].ToString() + " in " + validPath;
                            datView.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    */
                    case [0x78, 0x86, 0x17, 0xB7]:
                        try
                        {
                            // Check for PNG header
                            if (((byte[])resourceItem["data"])[0..0x8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                            {
                                Form imageView = new Form();
                                imageView.MinimumSize = System.Drawing.Size.Empty;
                                imageView.AutoSize = true;
                                imageView.MaximizeBox = false;
                                imageView.FormBorderStyle = FormBorderStyle.FixedSingle;
                                imageView.Text = resourceItem["id"].ToString() + " in " + validPath;
                                PictureBox imageBox = new PictureBox();
                                imageBox.Image = Image.FromStream(new MemoryStream((byte[])resourceItem["data"]));
                                imageBox.SizeMode = PictureBoxSizeMode.AutoSize;
                                imageView.Controls.Add(imageBox);
                                imageView.Show();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect file header", "Error");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    case [0x54, 0x77, 0x8A, 0xFD]:
                        try
                        {
                            // Check for WAV header
                            if (((byte[])resourceItem["data"])[0..0x4].SequenceEqual(new byte[] { 0x52, 0x49, 0x46, 0x46 }) && ((byte[])resourceItem["data"])[0x8..0xC].SequenceEqual(new byte[] { 0x57, 0x41, 0x56, 0x45 }))
                            {
                                new SoundPlayer(new MemoryStream((byte[])resourceItem["data"])).Play();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect file header", "Error");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    case [0x52, 0x49, 0x46, 0x46]:
                        try
                        {
                            // Check for OGG header
                            if (((byte[])resourceItem["data"])[0..0x4].SequenceEqual(new byte[] { 0x4F, 0x67, 0x67, 0x53 }))
                            {
                                WaveOut waveOut = new WaveOut();
                                waveOut.Init(new VorbisWaveReader(new MemoryStream((byte[])resourceItem["data"])));
                                waveOut.Play();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect file header", "Error");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    /*
                    case [0xDC, 0xAA, 0x86, 0xF6]:
                        try
                        {
                            HexView datView = new HexView((byte[])resourceItem["data"]);
                            datView.Text = resourceItem["id"].ToString() + " in " + validPath;
                            datView.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    */
                    default:
                        try
                        {
                            HexView datView = new HexView((byte[])resourceItem["data"]);
                            datView.Text = resourceItem["id"].ToString() + " in " + validPath;
                            datView.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                }
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
                Dictionary<string, object> resourceItem = masterList[resourceList.SelectedRows[0].Index];
                string filter;
                switch ((byte[])resourceItem["type"])
                {
                    /*
                    case [0x23, 0x22, 0xE0, 0xF4]:
                        filter = "Data (*.bin)|*.bin";
                        break;
                    */
                    case [0x78, 0x86, 0x17, 0xB7]:
                        filter = "PNG files (*.png)|*.png";
                        break;
                    case [0x54, 0x77, 0x8A, 0xFD]:
                        filter = "WAV files (*.wav)|*.wav";
                        break;
                    case [0x52, 0x49, 0x46, 0x46]:
                        filter = "OGG files (*.ogg)|*.ogg";
                        break;
                    /*
                    case [0xDC, 0xAA, 0x86, 0xF6]:
                        filter = "Data (*.bin)|*.bin";
                        break;
                    */
                    default:
                        filter = "Data (*.bin)|*.bin";
                        break;
                }

                byte[] dat = (byte[])resourceItem["data"];
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = filter;
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(validPath) + "_resource" + resourceItem["id"].ToString();
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(dat, 0, dat.Length);
                    }
                }
            }
            else
            {
                MessageBox.Show("Number of selected items is incorrect!", "Error");
            }
            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
            List<Dictionary<string, object>> masterListDataOnly = masterList.Where((x) => ((byte[])x["type"]).SequenceEqual(dataType)).ToList();
            string input = Interaction.InputBox("Enter sequence of bytes to search (separated by -)", "Search");
            if ((new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$")).IsMatch(input))
            {
                resultsBox.Items.Clear();
                byte[] pattern = GetBytes(input);
                foreach (Dictionary<string, object> resourceItem in masterListDataOnly)
                {
                    int[] resu = FindSequence((byte[])resourceItem["data"], pattern);
                    int resCount = resu.Length;
                    if (resCount > 0)
                    {
                        string[] r = new string[resCount];
                        for (int i = 0; i < resCount; i++)
                        {
                            r[i] = "0x" + resu[i].ToString("X");
                        }
                        resultsBox.Items.Add(resourceItem["id"].ToString() + ": " + String.Join(", ", r));
                    }
                }
            }
        }

        private void exportDataButton_Click(object sender, EventArgs e)
        {
            byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
            List<Dictionary<string, object>> masterListDataOnly = masterList.Where((x) => ((byte[])x["type"]).SequenceEqual(dataType)).ToList();
            List<byte> dat = new List<byte>();
            foreach (Dictionary<string, object> resourceItem in masterListDataOnly)
            {
                dat.AddRange((byte[])resourceItem["data"]);
            }

            // Save to file
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Data (*.bin)|*.bin";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(validPath) + "_alldata";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(dat.ToArray(), 0, dat.Count);
                }
            }
        }

        void Reset()
        {
            // master = null;
            masterList = [];
            contentSize = 0;
            tableEnd = 0;
            resourceCount = 0;
            tableStart = 0;
            firstResourceOffset = 0;
            validPath = "";
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            viewButton.Enabled = false;
            saveButton.Enabled = false;
            searchButton.Enabled = false;
            exportDataButton.Enabled = false;
        }

        MemoryStream DecompZlibData(byte[] data)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                new ZLibStream(new MemoryStream(data), CompressionMode.Decompress).CopyTo(result);
                result.Position = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
            return result;
        }

        (uint, byte[]) ProcessRawResourceData(byte[] data)
        {
            // Check compression type
            if  (data.Length > 0x4 && data[0x0..0x4].SequenceEqual(new byte[] { 0x04, 0x00, 0x00, 0x00 }))
            {
                // none
                return (1, data[0x4..]);
            }
            else if (data.Length > 0xC && data[0x0..0x4].SequenceEqual(new byte[] { 0x04, 0x00, 0x80, 0x00 }) && data[0xC..0xE].SequenceEqual(new byte[] { 0x78, 0xDA }) && BitConverter.ToUInt32(data[0x8..0xC]) == data.Length - 0xC)
            {
                // zlib
                return (2, DecompZlibData(data[0xC..]).ToArray());
            }
            else
            {
                // Default
                return (0, data);
            }
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
    }
}

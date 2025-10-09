using Microsoft.VisualBasic;
using NAudio.Vorbis;
using NAudio.Wave;
using System.IO.Compression;
using System.Media;
using System.Text.RegularExpressions;

namespace BigViewer
{
    public partial class MainWindow : Form
    {
        byte[] master;
        List<List<Object>> masterList = [];

        uint contentSize = 0;
        uint contentStartPosition = 0;
        uint resourceCount = 0;
        uint tableStart = 0;
        uint tableEnd = 0;
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
                    contentStartPosition = BitConverter.ToUInt32(master[0x18..0x1C]);
                    contentSize = BitConverter.ToUInt32(master[0x1C..0x20]);
                    tableEnd = BitConverter.ToUInt32(master[checked((int)(tableStart + 4))..checked((int)(tableStart + 8))]);

                    infoBox.Items.Add("TOC start: " + "0x" + tableStart.ToString("X"));
                    infoBox.Items.Add("Resource count: " + resourceCount.ToString());
                    infoBox.Items.Add("Content start: " + "0x" + contentStartPosition.ToString("X"));
                    infoBox.Items.Add("Clean size: " + contentSize.ToString());
                    infoBox.Items.Add("TOC end: " + "0x" + tableEnd.ToString("X"));

                    // Check for inconsistencies and show error if present
                    string errs = "";
                    if (resourceCount * 8 != (tableEnd - tableStart - 8))
                    {
                        errs += "TOC length and resource count mismatch!\n";
                    }
                    if (tableEnd != contentStartPosition)
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
                    for (int i = 1; i < resourceCount; i++)
                    {
                        var num = tableStart + (i * 8);
                        uint addr = BitConverter.ToUInt32(master[checked((int)(num + 4))..checked((int)(num + 8))]); // offset
                        byte[] type = master[checked((int)(num))..checked((int)(num + 4))]; // 4-byte type
                        uint size = BitConverter.ToUInt32(master[checked((int)(num + 12))..checked((int)(num + 16))]) - addr; // size
                        uint format = 0;
                        byte[] processedData;
                        // Check compression type
                        if (size > 0xC && master[checked((int)(addr + 0xC))..checked((int)(addr + 0xE))].SequenceEqual(new byte[] { 0x78, 0xDA }) && BitConverter.ToUInt32(master[checked((int)(addr + 0x8))..checked((int)(addr + 0xC))]) == size - 0xC)
                        {
                            // zlib
                            format = 1;
                            processedData = DecompZlibData(master[checked((int)(addr + 0xC))..checked((int)(addr + size))]).ToArray();
                        }
                        else
                        {
                            // Default
                            format = 0;
                            processedData = master[checked((int)(addr + 0x4))..checked((int)(addr + size))];
                        }
                        // no., type, offset, size, first 4 bytes, format, processed data
                        masterList.Add(new List<Object> { i, master[checked((int)(num))..checked((int)(num + 4))], addr, size, master[checked((int)addr)..checked((int)(addr + 4))], format, processedData });
                    }

                    // Populate DataGridView using masterList
                    for (int i = 0; i < resourceCount - 1; i++)
                    {
                        List<Object> resourceItem = masterList[i];
                        string resourceType;
                        switch ((byte[])resourceItem[1])
                        {
                            case [0x23, 0x22, 0xE0, 0xF4]:
                                resourceType = "DATA";
                                break;
                            case [0x78, 0x86, 0x17, 0xB7]:
                                resourceType = "PNG";
                                break;
                            case [0x54, 0x77, 0x8A, 0xFD]:
                                resourceType = "WAV";
                                break;
                            case [0x52, 0x49, 0x46, 0x46]:
                                resourceType = "OGG";
                                break;
                            case [0xDC, 0xAA, 0x86, 0xF6]:
                                resourceType = "[END]";
                                break;
                            default:
                                resourceType = BitConverter.ToString((byte[])resourceItem[1]);
                                break;
                        }
                        string formatType;
                        switch ((uint)resourceItem[5])
                        {
                            case 0:
                                formatType = "None";
                                break;
                            case 1:
                                formatType = "zlib";
                                break;
                            default:
                                formatType = "Unknown";
                                break;
                        }
                        resourceList.Rows.Add(((int)resourceItem[0]).ToString(), resourceType, "0x" + ((uint)resourceItem[2]).ToString("X"), "0x" + ((uint)resourceItem[3]).ToString("X"), BitConverter.ToString((byte[])resourceItem[4]), formatType);
                    }
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
                List<Object> resourceItem = masterList[resourceList.SelectedRows[0].Index];
                switch ((byte[])resourceItem[1]) // type
                {
                    case [0x23, 0x22, 0xE0, 0xF4]:
                        try
                        {
                            Form stringView = new Form();
                            stringView.MinimumSize = System.Drawing.Size.Empty;
                            stringView.AutoSize = true;
                            stringView.MaximizeBox = false;
                            stringView.FormBorderStyle = FormBorderStyle.FixedSingle;
                            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                            tableLayoutPanel.AutoSize = true;
                            TextBox dataLabel = new TextBox();
                            dataLabel.ReadOnly = true;
                            dataLabel.Multiline = true;
                            dataLabel.Size = new Size(1000, 500);
                            dataLabel.Text = BitConverter.ToString((byte[])resourceItem[6]);
                            // TextBox charLabel = new TextBox();
                            // charLabel.ReadOnly = true;
                            // charLabel.Size = new Size(1000, 500);
                            // charLabel.Text = System.Text.Encoding.UTF8.GetString(res);
                            tableLayoutPanel.Controls.Add(dataLabel);
                            // tableLayoutPanel.Controls.Add(charLabel);
                            stringView.Controls.Add(tableLayoutPanel);
                            stringView.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    case [0x78, 0x86, 0x17, 0xB7]:
                        try
                        {
                            // Check for PNG header
                            if (((byte[])resourceItem[6])[0..0x8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                            {
                                Form imageView = new Form();
                                imageView.MinimumSize = System.Drawing.Size.Empty;
                                imageView.AutoSize = true;
                                imageView.MaximizeBox = false;
                                imageView.FormBorderStyle = FormBorderStyle.FixedSingle;
                                PictureBox imageBox = new PictureBox();
                                imageBox.Image = Image.FromStream(new MemoryStream((byte[])resourceItem[6]));
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
                            if (((byte[])resourceItem[6])[0..0x4].SequenceEqual(new byte[] { 0x52, 0x49, 0x46, 0x46 }) && ((byte[])resourceItem[6])[0x8..0xC].SequenceEqual(new byte[] { 0x57, 0x41, 0x56, 0x45 }))
                            {
                                new SoundPlayer(new MemoryStream((byte[])resourceItem[6])).Play();
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
                            if (((byte[])resourceItem[6])[0..0x4].SequenceEqual(new byte[] { 0x4F, 0x67, 0x67, 0x53 }))
                            {
                                WaveOut waveOut = new WaveOut();
                                waveOut.Init(new VorbisWaveReader(new MemoryStream((byte[])resourceItem[6])));
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
                    case [0xDC, 0xAA, 0x86, 0xF6]:
                        try
                        {
                            Form stringView = new Form();
                            stringView.MinimumSize = System.Drawing.Size.Empty;
                            stringView.AutoSize = true;
                            stringView.MaximizeBox = false;
                            stringView.FormBorderStyle = FormBorderStyle.FixedSingle;
                            TextBox charLabel = new TextBox();
                            charLabel.ReadOnly = true;
                            charLabel.Size = new Size(1000, 500);
                            charLabel.Text = System.Text.Encoding.UTF8.GetString((byte[])resourceItem[6]);
                            stringView.Controls.Add(charLabel);
                            stringView.Show();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                        }
                        break;
                    default:
                        try
                        {
                            // byte[] res = dat.ToArray();
                            Form stringView = new Form();
                            stringView.MinimumSize = System.Drawing.Size.Empty;
                            stringView.AutoSize = true;
                            stringView.MaximizeBox = false;
                            stringView.FormBorderStyle = FormBorderStyle.FixedSingle;
                            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                            tableLayoutPanel.AutoSize = true;
                            TextBox dataLabel = new TextBox();
                            dataLabel.ReadOnly = true;
                            dataLabel.Multiline = true;
                            dataLabel.Size = new Size(1000, 500);
                            dataLabel.Text = BitConverter.ToString((byte[])resourceItem[6]);
                            // TextBox charLabel = new TextBox();
                            // charLabel.ReadOnly = true;
                            // charLabel.Size = new Size(1000, 500);
                            // charLabel.Text = System.Text.Encoding.UTF8.GetString(res);
                            tableLayoutPanel.Controls.Add(dataLabel);
                            // tableLayoutPanel.Controls.Add(charLabel);
                            stringView.Controls.Add(tableLayoutPanel);
                            stringView.Show();
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
                List<Object> resourceItem = masterList[resourceList.SelectedRows[0].Index];
                string filter;
                switch ((byte[])resourceItem[1])
                {
                    case [0x23, 0x22, 0xE0, 0xF4]:
                        filter = "Data (*.bin)|*.bin";
                        break;
                    case [0x78, 0x86, 0x17, 0xB7]:
                        filter = "PNG files (*.png)|*.png";
                        break;
                    case [0x54, 0x77, 0x8A, 0xFD]:
                        filter = "WAV files (*.wav)|*.wav";
                        break;
                    case [0x52, 0x49, 0x46, 0x46]:
                        filter = "OGG files (*.ogg)|*.ogg";
                        break;
                    case [0xDC, 0xAA, 0x86, 0xF6]:
                        filter = "Data (*.bin)|*.bin";
                        break;
                    default:
                        filter = "All files (*.*)|*.*";
                        break;
                }

                byte[] dat = (byte[])resourceItem[6];
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = filter;
                saveFileDialog1.FilterIndex = 1;
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
            List<List<Object>> masterListDataOnly = masterList.Where(x => ((byte[])x[1]).SequenceEqual(dataType)).ToList();
            string input = Interaction.InputBox("Enter sequence of bytes to search (separated by -)", "Search");
            if ((new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$")).IsMatch(input))
            {
                resultsBox.Items.Clear();
                byte[] pattern = GetBytes(input);
                foreach (List<Object> resourceItem in masterListDataOnly)
                {
                    int[] resu = FindSequence((byte[])resourceItem[6], pattern);
                    int resCount = resu.Length;
                    if (resCount > 0)
                    {
                        string[] r = new string[resCount];
                        for (int i = 0; i < resCount; i++)
                        {
                            r[i] = "0x" + resu[i].ToString("X");
                        }
                        resultsBox.Items.Add(resourceItem[0].ToString() + ": " + String.Join(", ", r));
                    }
                }
            }
        }

        private void exportDataButton_Click(object sender, EventArgs e)
        {
            byte[] dataType = new byte[] { 0x23, 0x22, 0xE0, 0xF4 };
            List<List<Object>> masterListDataOnly = masterList.Where(x => ((byte[])x[1]).SequenceEqual(dataType)).ToList();
            List<byte> data = new List<byte>();
            foreach (List<Object> resourceItem in masterListDataOnly)
            {
                data.AddRange((byte[])resourceItem[6]);
            }

            // Write to file
            try
            {
                using (FileStream fs = new FileStream(validPath + "data", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data.ToArray(), 0, data.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
            dataType = null;
        }

        void Reset()
        {
            // master = null;
            masterList = [];
            contentSize = 0;
            contentStartPosition = 0;
            resourceCount = 0;
            tableStart = 0;
            tableEnd = 0;
            validPath = "";
            infoBox.Items.Clear();
            resultsBox.Items.Clear();
            resourceList.Rows.Clear();
            viewButton.Enabled = false;
            saveButton.Enabled = false;
            searchButton.Enabled = false;
            exportDataButton.Enabled = false;
        }

        static MemoryStream DecompZlibData(byte[] data)
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

        public static byte[] GetBytes(string input)
        {
            return input.Split('-').Select((s) => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray();
        }

        static int[] FindSequence(byte[] data, byte[] pattern)
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

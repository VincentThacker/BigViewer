using System.IO.Compression;
using System.Media;
using System.Text.RegularExpressions;

namespace BigViewer
{
    internal static class Utils
    {
        public static string OpenFilePath(string filter)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = filter;
                openDialog.FilterIndex = 1;
                openDialog.RestoreDirectory = true;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    return openDialog.FileName;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SaveDataToFile(byte[] dataToSave, string filter, string? filePath, string? fileNameAppend)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                if (filePath != null)
                {
                    saveDialog.InitialDirectory = Path.GetDirectoryName(filePath);
                    if (fileNameAppend != null)
                    {
                        saveDialog.FileName = Path.GetFileNameWithoutExtension(filePath) + fileNameAppend;
                    }
                }
                saveDialog.Filter = filter;
                saveDialog.FilterIndex = 1;
                saveDialog.RestoreDirectory = true;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(dataToSave.ToArray(), 0, dataToSave.Length);
                    }
                }
            }
        }

        public static void SaveDataToFile(List<byte> dataToSave, string filter, string? filePath, string? fileNameAppend)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                if (filePath != null)
                {
                    saveDialog.InitialDirectory = Path.GetDirectoryName(filePath);
                    if (fileNameAppend != null)
                    {
                        saveDialog.FileName = Path.GetFileNameWithoutExtension(filePath) + fileNameAppend;
                    }
                }
                saveDialog.Filter = filter;
                saveDialog.FilterIndex = 1;
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

        public static byte[] ConvertStringToBytes(string input)
        {
            if ((new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$")).IsMatch(input))
            {
                return input.Split('-').Select((x) => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
            }
            else
            {
                return [];
            }
        }

        public static int[] FindSequence(byte[] data, byte[] pattern)
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

        public static long GetChecksum(byte[] data)
        {
            using (MemoryStream fileContent = new MemoryStream(data))
            {
                byte[] arrayOfByte = new byte[1024];
                int i = 0;
                long result = 0L;
                while (i < data.Length)
                {
                    if ((i & 0x400) != 0)
                    {
                        fileContent.Seek(130048L, SeekOrigin.Current);
                        i += 130048;
                        continue;
                    }
                    int j = fileContent.Read(arrayOfByte);
                    for (int b = 0; b < j; b++)
                    {
                        i += 1;
                        result += (arrayOfByte[b] << (i & 0xF));
                    }
                }
                return result;
            }
        }

        public static byte[] DecompZlibData(byte[] data)
        {
            using (MemoryStream result = new MemoryStream())
            {
                try
                {
                    using (ZLibStream zs = new ZLibStream(new MemoryStream(data), CompressionMode.Decompress))
                    {
                        zs.CopyTo(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
                return result.ToArray();
            }
        }

        public static byte[] CompZlibData(byte[] data)
        {
            using (MemoryStream result = new MemoryStream())
            {
                try
                {
                    using (ZLibStream zs = new ZLibStream(result, CompressionLevel.SmallestSize))
                    {
                        (new MemoryStream(data)).CopyTo(zs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
                return result.ToArray();
            }
        }

        public static string GetTypeName(byte[] type)
        {
            switch (type)
            {
                case [0x23, 0x22, 0xE0, 0xF4]:
                    return "data";
                case [0xDC, 0xAA, 0x86, 0xF6]:
                    return "string";
                case [0x78, 0x86, 0x17, 0xB7]:
                    return "png";
                case [0x54, 0x77, 0x8A, 0xFD]:
                    return "wav";
                /*
                case [0x52, 0x49, 0x46, 0x46]:
                    return "ogg";
                */
                case [0x05, 0xC5, 0xE4, 0x69]:
                    return "little";
                default:
                    return BitConverter.ToString(type);
            }
        }

        public static string GetFileTypeFilter(byte[] type)
        {
            switch (type)
            {
                case [0x78, 0x86, 0x17, 0xB7]:
                    return "PNG files (*.png)|*.png";
                case [0x54, 0x77, 0x8A, 0xFD]:
                    return "WAV files (*.wav)|*.wav";
                /*
                case [0x52, 0x49, 0x46, 0x46]:
                    return "OGG files (*.ogg)|*.ogg";
                */
                default:
                    return "Data (*.bin)|*.bin";
            }
        }

        public static string GetFormatName(uint format)
        {
            switch (format)
            {
                case 1:
                    return "none";
                case 2:
                    return "zlib";
                default:
                    return "unknown";
            }
        }

        public static (uint, byte[]) DecodeResource(byte[] data)
        {
            // Check compression type
            if (data.Length > 0x4 && data[0x0..0x4].SequenceEqual(new byte[] { 0x04, 0x00, 0x00, 0x00 }))
            {
                // none
                return (1, data[0x4..]);
            }
            else if (data.Length > 0xC && data[0x0..0x4].SequenceEqual(new byte[] { 0x04, 0x00, 0x80, 0x00 }) && data[0xC..0xE].SequenceEqual(new byte[] { 0x78, 0xDA }) && BitConverter.ToUInt32(data[0x8..0xC]) == data.Length - 0xC)
            {
                // zlib
                return (2, DecompZlibData(data[0xC..]));
            }
            else
            {
                // Default
                return (0, data);
            }
        }

        public static byte[] EncodeResource(byte[] rawData, uint format)
        {
            switch (format)
            {
                case 1:
                    // none
                    return (new byte[] { 0x04, 0x00, 0x00, 0x00 }).Concat(rawData).ToArray();
                case 2:
                    // zlib
                    List<byte> result = [];
                    result.AddRange(new byte[] { 0x04, 0x00, 0x80, 0x00 });
                    result.AddRange(BitConverter.GetBytes(rawData.Length));
                    byte[] data = CompZlibData(rawData);
                    result.AddRange(BitConverter.GetBytes(data.Length));
                    result.AddRange(data);
                    return result.ToArray();
                default:
                    return rawData;
            }
        }

        public static bool CheckPNGHeader(byte[] data)
        {
            if (data.Length > 0x8 && data[0x0..0x8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }) && data[^0x8..^0x0].SequenceEqual(new byte[] { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 }))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckWAVHeader(byte[] data)
        {
            if (data.Length > 0xC && data[0x0..0x4].SequenceEqual(new byte[] { 0x52, 0x49, 0x46, 0x46 }) && data[0x8..0xC].SequenceEqual(new byte[] { 0x57, 0x41, 0x56, 0x45 }))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DisplayEditRaw(byte[] rawData, byte[] type, string title, ResourceFile parentResourceFile, int resId, Action act, Form parentForm)
        {
            // parentResourceFile and resId is for updating parent resource file.
            // Action is for updating DataGridView of parent Form.
            switch (type)
            {
                case [0x05, 0xC5, 0xE4, 0x69]:
                    try
                    {
                        LittleEditor littleEditor = new LittleEditor(rawData, title, parentResourceFile, resId, act);
                        littleEditor.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
                default:
                    try
                    {
                        HexEditor datView = new HexEditor(rawData, title, parentResourceFile, resId, act);
                        datView.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
            }
        }

        public static void DisplayEditRaw(byte[] rawData, byte[] type, string title, LittleResourceFile parentLittleResourceFile, int resId, Action act, Form parentForm)
        {
            // parentResourceFile and resId is for updating parent little resource file.
            // Action is for updating DataGridView of parent Form.
            switch (type)
            {
                case [0x05, 0xC5, 0xE4, 0x69]:
                    try
                    {
                        LittleEditor littleEditor = new LittleEditor(rawData, title, parentLittleResourceFile, resId, act);
                        littleEditor.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
                default:
                    try
                    {
                        HexEditor datView = new HexEditor(rawData, title, parentLittleResourceFile, resId, act);
                        datView.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
            }
        }

        public static void DisplayRaw(byte[] rawData, byte[] type, string title, int resId, Form parentForm)
        {
            switch (type)
            {
                case [0x78, 0x86, 0x17, 0xB7]:
                    try
                    {
                        // Check for PNG header
                        if (CheckPNGHeader(rawData))
                        {
                            Form imageView = new Form();
                            imageView.Tag = resId;
                            imageView.MinimumSize = Size.Empty;
                            imageView.AutoSize = true;
                            imageView.MaximizeBox = false;
                            imageView.FormBorderStyle = FormBorderStyle.FixedSingle;
                            imageView.Text = title;
                            PictureBox imageBox = new PictureBox();
                            imageBox.Image = Image.FromStream(new MemoryStream(rawData));
                            imageBox.SizeMode = PictureBoxSizeMode.AutoSize;
                            imageView.Controls.Add(imageBox);
                            imageView.Show(parentForm);
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
                        if (CheckWAVHeader(rawData))
                        {
                            new SoundPlayer(new MemoryStream(rawData)).Play();
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
                case [0x52, 0x49, 0x46, 0x46]:
                    try
                    {
                        // Check for OGG header
                        if (rawData[0..0x4].SequenceEqual(new byte[] { 0x4F, 0x67, 0x67, 0x53 }))
                        {
                            WaveOut waveOut = new WaveOut();
                            waveOut.Init(new VorbisWaveReader(new MemoryStream(rawData)));
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
                */
                default:
                    try
                    {
                        HexEditor datView = new HexEditor(rawData, title, resId);
                        datView.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
            }
        }

        /*
        public static void Display(byte[] data, string title)
        {
            try
            {
                HexEditor datView = new HexEditor(data, title);
                datView.Text = title;
                datView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }
        */
    }
}
using System.IO.Compression;

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
            return input.Split('-').Select((x) => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
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

        public static MemoryStream DecompZlibData(byte[] data)
        {
            using (MemoryStream result = new MemoryStream())
            {
                try
                {
                    using (ZLibStream zs = new ZLibStream(new MemoryStream(data), CompressionMode.Decompress))
                    {
                        zs.CopyTo(result);
                        result.Position = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
                return result;
            }
        }

        public static MemoryStream CompZlibData(byte[] data)
        {
            using (MemoryStream result = new MemoryStream())
            {
                try
                {
                    using (ZLibStream zs = new ZLibStream(result, CompressionLevel.SmallestSize))
                    {
                        (new MemoryStream(data)).CopyTo(zs);
                    }
                    // result.Position = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                }
                return result;
            }
        }

        public static string GetTypeName(byte[] type)
        {
            switch (type)
            {
                case [0x23, 0x22, 0xE0, 0xF4]:
                    return "data";
                case [0x78, 0x86, 0x17, 0xB7]:
                    return "png";
                case [0x54, 0x77, 0x8A, 0xFD]:
                    return "wav";
                case [0x52, 0x49, 0x46, 0x46]:
                    return "ogg";
                case [0x05, 0xC5, 0xE4, 0x69]:
                    return "little";
                case [0xDC, 0xAA, 0x86, 0xF6]:
                    return "string";
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
                case [0x52, 0x49, 0x46, 0x46]:
                    return "OGG files (*.ogg)|*.ogg";
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
                return (2, DecompZlibData(data[0xC..]).ToArray());
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
                    byte[] data = CompZlibData(rawData).ToArray();
                    result.AddRange(BitConverter.GetBytes(data.Length));
                    result.AddRange(data);
                    return result.ToArray();
                default:
                    return rawData;
            }
        }
    }
}

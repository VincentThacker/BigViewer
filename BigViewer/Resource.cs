using NAudio.Vorbis;
using NAudio.Wave;
using System.IO.Compression;
using System.Media;

namespace BigViewer
{
    internal class Resource
    {
        public int id;
        public byte[] type;
        public string typeName;
        public uint offset;
        public uint format;
        public string formatName;
        public uint size;
        public uint rawSize;
        public byte[] data;
        public byte[] rawData;

        public static Dictionary<uint, string> formatNames = new Dictionary<uint, string> {
            {1, "none"},
            {2, "zlib"},
            {0, "unknown"}
        };

        public Resource(int _id, byte[] _type, uint _offset, byte[] _data)
        {
            id = _id;
            type = _type;
            typeName = GetTypeName(_type);
            offset = _offset;
            data = _data;
            size = checked((uint)_data.Length);
            (format, rawData) = DecodeResource(data);
            rawSize = checked((uint)rawData.Length);
            if (!formatNames.TryGetValue(format, out formatName))
            {
                formatName = "invalid";
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
                    return "[start]";
                case [0xDC, 0xAA, 0x86, 0xF6]:
                    return "[end]";
                default:
                    return BitConverter.ToString(type);
            }
        }

        public void Display()
        {
            switch (type)
            {
                case [0x78, 0x86, 0x17, 0xB7]:
                    try
                    {
                        // Check for PNG header
                        if (rawData[0..0x8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                        {
                            Form imageView = new Form();
                            imageView.MinimumSize = System.Drawing.Size.Empty;
                            imageView.AutoSize = true;
                            imageView.MaximizeBox = false;
                            imageView.FormBorderStyle = FormBorderStyle.FixedSingle;
                            imageView.Text = id.ToString();
                            PictureBox imageBox = new PictureBox();
                            imageBox.Image = Image.FromStream(new MemoryStream(rawData));
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
                        if (rawData[0..0x4].SequenceEqual(new byte[] { 0x52, 0x49, 0x46, 0x46 }) && rawData[0x8..0xC].SequenceEqual(new byte[] { 0x57, 0x41, 0x56, 0x45 }))
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
                default:
                    try
                    {
                        HexView datView = new HexView(rawData);
                        datView.Text = id.ToString();
                        datView.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
            }
        }

        public static MemoryStream DecompZlibData(byte[] data)
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

        public static MemoryStream CompZlibData(byte[] data)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                new ZLibStream(new MemoryStream(data), CompressionLevel.SmallestSize).CopyTo(result);
                result.Position = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
            return result;
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

        public void UpdateData(byte[] newRawData, byte[]? newType)
        {
            rawData = newRawData;
            rawSize = checked((uint)newRawData.Length);
            data = EncodeResource(newRawData, format);
            size = checked((uint)data.Length);
            if (newType != null)
            {
                type = newType;
                typeName = GetTypeName(newType);
            }
        }
    }
}

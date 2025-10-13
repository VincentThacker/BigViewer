using NAudio.Vorbis;
using NAudio.Wave;
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

        public Resource(int _id, byte[] _type, uint _offset, byte[] _data)
        {
            id = _id;
            type = _type;
            typeName = Utils.GetTypeName(_type);
            offset = _offset;
            data = _data;
            size = checked((uint)_data.Length);
            (format, rawData) = Utils.DecodeResource(data);
            formatName = Utils.GetFormatName(format);
            rawSize = checked((uint)rawData.Length);
        }

        public void DisplayRaw()
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
                            imageView.MinimumSize = Size.Empty;
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

        public void Display()
        {
            try
            {
                HexView datView = new HexView(data);
                datView.Text = id.ToString();
                datView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }
    }
}

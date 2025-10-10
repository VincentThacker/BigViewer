using Be.Windows.Forms;

namespace BigViewer
{
    public partial class HexView : Form
    {
        private DynamicByteProvider byteProvider;

        public HexView(byte[] displayData)
        {
            InitializeComponent();
            this.Width += SystemInformation.VerticalScrollBarWidth;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            for (int i = 0; i < displayData.Length; i++)
            {
                byteProvider.WriteByte(i, displayData[i]);
            }
        }
    }
}

using Be.Windows.Forms;

namespace BigViewer
{
    internal partial class HexEditor : Form
    {
        private ResourceFile? parentResourceFile;
        private LittleResourceFile? parentLittleResourceFile;
        private int idInParent = -1;
        private Action? action;
        private DynamicByteProvider byteProvider;

        // Editing resource raw form in ResourceFile
        public HexEditor(byte[] displayData, string title, ResourceFile _parentResourceFile, int _id, Action _action)
        {
            InitializeComponent();
            parentResourceFile = _parentResourceFile;
            parentLittleResourceFile = null;
            idInParent = _id;
            action = _action;
            this.Text = title;
            this.Width += SystemInformation.VerticalScrollBarWidth;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            for (int i = 0; i < displayData.Length; i++)
            {
                byteProvider.WriteByte(i, displayData[i]);
            }
        }

        // Editing resource raw form in LittleResourceFile
        public HexEditor(byte[] displayData, string title, LittleResourceFile _parentLittleResourceFile, int _id, Action _action)
        {
            InitializeComponent();
            parentResourceFile = null;
            parentLittleResourceFile = _parentLittleResourceFile;
            idInParent = _id;
            action = _action;
            this.Text = title;
            this.Width += SystemInformation.VerticalScrollBarWidth;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            for (int i = 0; i < displayData.Length; i++)
            {
                byteProvider.WriteByte(i, displayData[i]);
            }
        }

        // View only
        public HexEditor(byte[] displayData, string title)
        {
            InitializeComponent();
            parentResourceFile = null;
            parentLittleResourceFile = null;
            action = null;
            this.saveButton.Enabled = false;
            this.cancelButton.Enabled = false;
            this.Text = title;
            this.Width += SystemInformation.VerticalScrollBarWidth;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            for (int i = 0; i < displayData.Length; i++)
            {
                byteProvider.WriteByte(i, displayData[i]);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (parentResourceFile != null && action != null)
            {
                parentResourceFile.ReplaceResourceRaw(idInParent, GetCurrentBytes());
                action();
            }
            else if (parentLittleResourceFile != null && action != null)
            {
                parentLittleResourceFile.ReplaceResourceRaw(idInParent, GetCurrentBytes());
                action();
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public byte[] GetCurrentBytes()
        {
            return byteProvider.Bytes.ToArray();
        }
    }
}

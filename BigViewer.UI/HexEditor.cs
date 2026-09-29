using BigViewer.Core;

using Be.Windows.Forms;

namespace BigViewer.UI
{
    internal partial class HexEditor : Form
    {
        private ResourceFile? parentResourceFile;
        private int idInParent = -1;
        private Action? action;
        private DynamicByteProvider byteProvider;
        private int[] searchResults = [];

        // Editing resource raw form in ResourceFile
        public HexEditor(ReadOnlySpan<byte> displayData, string title, ResourceFile _parentResourceFile, int _id, Action _action)
        {
            InitializeComponent();
            if (_parentResourceFile != null)
            {
                if (_id >= 0 && _id < _parentResourceFile.ResourceCount)
                {
                    parentResourceFile = _parentResourceFile;
                    idInParent = _id;
                }
                else
                {
                    throw new ArgumentException("Invalid resource ID received!");
                }
            }
            else
            {
                throw new ArgumentException("Parent resource cannot be null!");
            }
            idInParent = _id;
            action = _action;
            this.Tag = _id;
            this.Text = title;
            this.Width += SystemInformation.VerticalScrollBarWidth;
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            hexBox.ReadOnly = false;
        }

        // View only
        public HexEditor(ReadOnlySpan<byte> displayData, string title, int _id)
        {
            InitializeComponent();
            parentResourceFile = null;
            idInParent = _id;
            action = null;
            this.Tag = _id;
            this.Text = title;
            this.Width += SystemInformation.VerticalScrollBarWidth;
            saveButton.Enabled = false;
            cancelButton.Enabled = false;
            byteProvider = new DynamicByteProvider(displayData);
            hexBox.ByteProvider = byteProvider;
            hexBox.ReadOnly = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (parentResourceFile != null && action != null)
            {
                parentResourceFile.ReplaceResourceRaw(idInParent, byteProvider.Bytes.ToArray());
                action();
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Enter sequence of bytes to search (separated by -)
            byte[] pattern = Utils.ConvertByteString(searchBox.Text);
            if (pattern.Length > 0)
            {
                searchResults = Common.FindSequence(byteProvider.Bytes, pattern);
                resultsBox.Items.Clear();
                hexBox.HighlightedRegions.RemoveAll((x) => { return x.Color == Color.LightGreen; });
                foreach (int i in searchResults)
                {
                    resultsBox.Items.Add("0x" + i.ToString("X"));
                    hexBox.HighlightedRegions.Add(new HexBox.HighlightedRegion(i, pattern.Length, Color.LightGreen));
                }
                hexBox.Refresh();
            }
            else
            {
                MessageBox.Show("Invalid input!", "Error");
            }
        }

        private void resultsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultsBox.SelectedIndex != -1)
            {
                hexBox.ScrollByteIntoView(searchResults[resultsBox.SelectedIndex]);
            }
        }
    }
}
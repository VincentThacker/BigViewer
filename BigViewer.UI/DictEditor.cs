using BigViewer.Core;

namespace BigViewer.UI
{
    public partial class DictEditor : Form
    {
        private ResourceFile? parentResourceFile;
        private int idInParent = -1;
        private Action? action;

        // Edit mode
        public DictEditor(string title, ResourceFile _parentResourceFile, int _id, Action _action)
        {
            throw new NotImplementedException();
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
            saveButton.Enabled = true;
            cancelButton.Enabled = true;
        }

        // View only
        public DictEditor(string title, int _id)
        {
            throw new NotImplementedException();
            InitializeComponent();
            parentResourceFile = null;
            idInParent = _id;
            action = null;
            this.Tag = _id;
            this.Text = title;
            saveButton.Enabled = false;
            cancelButton.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

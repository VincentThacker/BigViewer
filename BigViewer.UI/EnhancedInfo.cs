using BigViewer.Core;

namespace BigViewer.UI
{
    public partial class EnhancedInfo : Form
    {
        private ResourceFile? parentResourceFile;
        private int idInParent = -1;

        // View only
        public EnhancedInfo(string[,] data)
        {
            if (data.GetLength(1) != 4)
            {
                throw new InvalidDataException("Incorrect dimensions!");
            }
            InitializeComponent();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                enhancedInfoDataGridView.Rows.Add(data[i, 0], data[i, 1], data[i, 2], data[i, 3]);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

using System.Text;

using BigViewer.Core;

namespace BigViewer.UI
{
    internal partial class AggEditor : Form
    {
        //internal LittleResourceFile? currentLittleFile;
        private ResourceFile? parentResourceFile;
        private int idInParent = -1;
        private Action? action;

        // Editing resource from parent ResourceFile
        public AggEditor(byte[] data, string title, ResourceFile _parentResourceFile, int _id, Action _action)
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

            // DisplayInfoUI();

            viewRawButton.Enabled = true;
            editRawButton.Enabled = true;
            replaceButton.Enabled = true;
            exportSelectedButton.Enabled = true;
            exportAllButton.Enabled = true;
            saveButton.Enabled = true;

            searchOptionsGroupBox.Enabled = true;
            searchTabControl.Enabled = true;
            foreach (Control control in searchTabPageBinary.Controls)
            {
                control.Enabled = true;
            }
            searchTabPageNumberInput.Enabled = true;
            ChangeSearchTabPageNumberButtons(0);
            foreach (Control control in searchTabPageString.Controls)
            {
                control.Enabled = true;
            }
            searchTabPageNumberInput.TextChanged += searchTabPageNumberInput_TextChanged;

            searchDataOnlyCheckBox.Enabled = true;
            searchButton.Enabled = true;
        }

        private void viewRawButton_Click(object sender, EventArgs e)
        {

        }

        private void editRawButton_Click(object sender, EventArgs e)
        {

        }

        private void replaceButton_Click(object sender, EventArgs e)
        {

        }

        private void exportSelectedButton_Click(object sender, EventArgs e)
        {

        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageBinary)
            {
                SearchAndDisplayResult(Utils.ConvertByteString(searchTabPageBinaryInput.Text));
            }
            else if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageNumber)
            {
                if (searchTabPageNumber16Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(Half.Parse(searchTabPageNumberInput.Text)));
                    }
                    else if (searchTabPageNumberInput.Text.Contains('-'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(short.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(ushort.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else if (searchTabPageNumber32Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(float.Parse(searchTabPageNumberInput.Text)));
                    }
                    else if (searchTabPageNumberInput.Text.Contains('-'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(int.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(uint.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else if (searchTabPageNumber64Bit.Checked == true)
                {
                    if (searchTabPageNumberInput.Text.Contains('.'))
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(double.Parse(searchTabPageNumberInput.Text)));
                    }
                    else
                    {
                        SearchAndDisplayResult(BitConverter.GetBytes(long.Parse(searchTabPageNumberInput.Text)));
                    }
                }
                else
                {
                    MessageBox.Show("No length selected!", "Error");
                }
            }
            else if (searchTabControl.TabPages[searchTabControl.SelectedIndex] == searchTabPageString)
            {
                if (searchTabPageStringLatin.Checked == true)
                {
                    try
                    {
                        SearchAndDisplayResult(Encoding.Latin1.GetBytes(searchTabPageStringInput.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                }
                else if (searchTabPageStringUTF16.Checked == true)
                {
                    try
                    {
                        SearchAndDisplayResult(Encoding.Unicode.GetBytes(searchTabPageStringInput.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                }
            }
        }

        private void resultsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resourceList.CurrentCell = resourceList.Rows[int.Parse(Utils.resultsBoxIdRegex.Match(resultsBox.GetItemText(resultsBox.SelectedItem) ?? "0:").Value)].Cells[0];
        }

        private void resourceList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Handle the right click
            if (e.Button == MouseButtons.Right)
            {
                resourceList.Rows[e.RowIndex].Selected = true;
                resourceList.CurrentCell = resourceList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                resourceListContextMenu.Show(Cursor.Position);
            }
        }

        private void searchTabPageNumberInput_TextChanged(object? sender, EventArgs e)
        {
            ChangeSearchTabPageNumberButtons(Utils.ParseNumber(searchTabPageNumberInput.Text));
        }

        private void ChangeSearchTabPageNumberButtons(int mode)
        {
            switch (mode)
            {
                case 1:
                    searchTabPageNumber16Bit.Enabled = true;
                    searchTabPageNumber32Bit.Enabled = true;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                case 2:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = true;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                case 3:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber32Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = false;
                    searchTabPageNumber64Bit.Enabled = true;
                    return;
                default:
                    searchTabPageNumber16Bit.Checked = false;
                    searchTabPageNumber32Bit.Checked = false;
                    searchTabPageNumber64Bit.Checked = false;
                    searchTabPageNumber16Bit.Enabled = false;
                    searchTabPageNumber32Bit.Enabled = false;
                    searchTabPageNumber64Bit.Enabled = false;
                    return;
            }
        }

        private void SearchAndDisplayResult(byte[] pattern)
        {

        }

    }
}
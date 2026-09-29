using System.Media;
using System.Text.RegularExpressions;

using BigViewer.Core;

namespace BigViewer.UI
{
    internal static class Utils
    {
        public static readonly byte[] pngStarting = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        public static readonly byte[] pngEnding = [0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82];
        public static readonly byte[] wavStarting1 = [0x52, 0x49, 0x46, 0x46];
        public static readonly byte[] wavStarting2 = [0x57, 0x41, 0x56, 0x45];

        public static readonly Regex intRegex = new Regex(@"^-?[1-9][0-9]{0,19}$");
        public static readonly Regex decRegex = new Regex(@"^-?(0|[1-9][0-9]*)\.[0-9]+$");
        public static readonly Regex bytesRegex = new Regex(@"^([0-9a-fA-F]{2}-)*([0-9a-fA-F]{2})$");
        public static readonly Regex resultsBoxIdRegex = new Regex(@"^[0-9]+(?=:)");

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

        public static string OpenFolderPath()
        {
            using (FolderBrowserDialog openDialog = new FolderBrowserDialog())
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    return openDialog.SelectedPath;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SaveDataToFile(ReadOnlySpan<byte> dataToSave, string filter, string? filePath, string? fileNameAppend)
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
                        fs.Write(dataToSave);
                    }
                }
            }
        }

        public static string GetFileTypeFilter(uint type)
        {
            return type switch
            {
                Constants.resourceTypePng => "PNG files (*.png)|*.png",
                Constants.resourceTypeWav => "WAV files (*.wav)|*.wav",
                _ => "Data (*.bin)|*.bin",
            };
        }

        public static string GetTypeExt(uint type)
        {
            return type switch
            {
                Constants.resourceTypePng => ".png",
                Constants.resourceTypeWav => ".wav",
                _ => ".bin",
            };
        }

        public static int ParseNumber(string input)
        {
            if (intRegex.IsMatch(input))
            {
                if (long.TryParse(input, out long result))
                {
                    if (short.MinValue <= result && result <= ushort.MaxValue)
                    {
                        return 1;
                    }
                    else if (int.MinValue <= result && result <= uint.MaxValue)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else if (decRegex.IsMatch(input))
            {
                if (!Half.IsInfinity(Half.Parse(input)))
                {
                    return 1;
                }
                else if (!float.IsInfinity(float.Parse(input)))
                {
                    return 2;
                }
                else if (!double.IsInfinity(double.Parse(input)))
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public static byte[] ConvertByteString(string input)
        {
            if (bytesRegex.IsMatch(input))
            {
                return input.Split('-').Select((x) => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
            }
            else
            {
                return [];
            }
        }

        public static void DisplayEditRaw(ReadOnlySpan<byte> rawData, string title, ResourceFile parentResourceFile, int resId, Action act, Form parentForm)
        {
            // parentResourceFile and resId is for updating parent resource file.
            // Action is for updating DataGridView of parent Form.
            try
            {
                new HexEditor(rawData, title, parentResourceFile, resId, act).Show(parentForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }
        }

        public static void DisplayRaw(ReadOnlySpan<byte> rawData, uint type, string title, int resId, Form parentForm)
        {
            switch (type)
            {
                case Constants.resourceTypePng:
                    try
                    {
                        Form imageView = new Form
                        {
                            Tag = resId,
                            MinimumSize = Size.Empty,
                            AutoSize = true,
                            MaximizeBox = false,
                            FormBorderStyle = FormBorderStyle.FixedSingle,
                            Text = title
                        };
                        PictureBox imageBox = new PictureBox
                        {
                            SizeMode = PictureBoxSizeMode.AutoSize
                        };
                        using (MemoryStream ms = new MemoryStream(rawData.Length))
                        {
                            ms.Write(rawData);
                            ms.Position = 0;
                            imageBox.Image = Image.FromStream(ms);
                        }
                        imageView.Controls.Add(imageBox);
                        imageView.Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;

                case Constants.resourceTypeWav:
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(rawData.Length))
                        {
                            ms.Write(rawData);
                            ms.Position = 0;
                            new SoundPlayer(ms).Play();
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
                        new HexEditor(rawData, title, resId).Show(parentForm);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                    }
                    break;
            }
        }

        public static object? ComboBoxDialog(IEnumerable<object> items, string title, Form parentForm)
        {
            if (items.Any())
            {
                Form dialog = new Form
                {
                    MinimumSize = Size.Empty,
                    AutoSize = true,
                    ControlBox = false,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    Text = title,
                    StartPosition = FormStartPosition.CenterParent
                };

                ComboBox comboBox = new ComboBox
                {
                    Left = 12,
                    Top = 12,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                foreach (object item in items)
                {
                    comboBox.Items.Add(item);
                }
                comboBox.SelectedIndex = 0;

                Button okButton = new Button
                {
                    Text = "OK",
                    Left = 12,
                    Top = 52,
                    Width = 112,
                    Height = 34,
                    DialogResult = DialogResult.OK
                };

                dialog.Controls.Add(comboBox);
                dialog.Controls.Add(okButton);
                dialog.ShowDialog(parentForm);

                return comboBox.SelectedItem;
            }
            else
            {
                return null;
            }
        }
    }
}
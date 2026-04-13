using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VarBinarySaver
{
    public partial class Form1 : Form
    {
        private string _selectedCsvPath;
        private string _outputDir;

        public Form1()
        {
            InitializeComponent();
            btnSelectCsv.Click += BtnSelectCsv_Click;
            SaveButton.Click += SaveButton_Click;
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
        }

        public static string ConvertToWindowsFileName(string urlText, char[] invalidCharsToAllow = null)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            if (invalidCharsToAllow == null)
            {
                invalidCharsToAllow = Array.Empty<char>();
            }

            string invalidCharsRemoved = new string(urlText
              .Where(x => !invalidChars.Contains(x) || invalidCharsToAllow.Contains(x))
              .ToArray());

            return invalidCharsRemoved;
        }

        private void BtnSelectCsv_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                ofd.Title = "Select CSV File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SetCsvFile(ofd.FileName);
                }
            }
        }

        private void SetCsvFile(string filePath)
        {
            _selectedCsvPath = filePath;
            lblCsvPath.Text = _selectedCsvPath;
            Log($"Selected CSV: {_selectedCsvPath}");
            // Set output directory: <csv folder>\\<csv base name> binaries
            var csvDir = Path.GetDirectoryName(_selectedCsvPath);
            var csvBase = Path.GetFileNameWithoutExtension(_selectedCsvPath);
            _outputDir = Path.Combine(csvDir, csvBase + " binaries");
            Directory.CreateDirectory(_outputDir);
            Log($"Output directory: {_outputDir}");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            TextBox.Clear();
            if (string.IsNullOrWhiteSpace(_selectedCsvPath) || !File.Exists(_selectedCsvPath))
            {
                Log("No CSV file selected or file does not exist.");
                return;
            }
            if (string.IsNullOrWhiteSpace(_outputDir))
            {
                Log("Output directory is not set.");
                return;
            }
            string csvInput = File.ReadAllText(_selectedCsvPath);
            CsvBinarySaver.SaveBinariesFromCsv(
                csvInput,
                _outputDir,
                name => ConvertToWindowsFileName(name),
                Log
            );
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                SetCsvFile(files[0]);
            }
        }

        private void Log(string message)
        {
            if (TextBox.InvokeRequired)
            {
                TextBox.Invoke(new Action(() => Log(message)));
            }
            else
            {
                TextBox.AppendText(message + Environment.NewLine);
            }
        }
    }
}
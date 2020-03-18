using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VarBinarySaver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach(var s in ReadLines(TextBox.Text))
            {
                SaveLine(s);
            }
        }

        private void SaveLine(string line)
        {
            var split = line.Split(new[] { "0x" }, StringSplitOptions.None);

            if (split.Length == 2)
            {
                var name = split[0].Trim();
                name = ConvertToWindowsFileName(name);

                var bytes = HexStringToBytes(split[1]);
                var path = Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), name);
                File.WriteAllBytes(path, bytes);
            }
        }

        internal IEnumerable<string> ReadLines(string s)
        {
            string line;
            using (var sr = new StringReader(s))
                while ((line = sr.ReadLine()) != null)
                    yield return line;
        }

        private byte[] HexStringToBytes(string hex)
        {
            List<byte> byteList = new List<byte>();

            for (int i = 0; i < hex.Length / 2; i++)
            {
                string hexNumber = hex.Substring(i * 2, 2);
                byteList.Add((byte)Convert.ToInt32(hexNumber, 16));
            }

            byte[] original = byteList.ToArray();
            return original;
        }


        public static string ConvertToWindowsFileName(string urlText, char[] invalidCharsToAllow = null)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            if (invalidCharsToAllow == null)
            {
                invalidCharsToAllow = Array.Empty<char>();
            }

            // get chars that are either not invalid, or are invalid chars to allow
            string invalidCharsRemoved = new string(urlText
              .Where(x => !invalidChars.Contains(x) || invalidCharsToAllow.Contains(x))
              .ToArray());

            return invalidCharsRemoved;
        }

    }
}

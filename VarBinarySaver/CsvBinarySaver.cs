using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VarBinarySaver
{
    public static class CsvBinarySaver
    {
        /// <summary>
        /// Processes CSV input, saving any columns starting with 0x as binary files.
        /// The first column is used as the base filename (sanitized). The first binary saves as the filename, subsequent binaries as filename+1, filename+2, etc.
        /// </summary>
        /// <param name="csvInput">CSV text input</param>
        /// <param name="outputDirectory">Directory to save files</param>
        /// <param name="convertToWindowsFileName">Delegate to sanitize filenames</param>
        /// <param name="logger">Optional logger for status/error messages</param>
        public static void SaveBinariesFromCsv(string csvInput, string outputDirectory, Func<string, string> convertToWindowsFileName, Action<string> logger = null)
        {
            if (string.IsNullOrWhiteSpace(csvInput))
            {
                logger?.Invoke("CSV input is empty.");
                return;
            }

            using (var reader = new StringReader(csvInput))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var columns = line.Split(',').Select(col => col.Trim()).ToArray();
                    if (columns.Length == 0)
                        continue;

                    var baseName = convertToWindowsFileName(columns[0]);
                    if (string.IsNullOrWhiteSpace(baseName))
                    {
                        logger?.Invoke($"Line {lineNumber}: Skipped due to empty or invalid filename.");
                        continue;
                    }

                    int binIndex = 1;
                    for (int i = 1; i < columns.Length; i++)
                    {
                        var col = columns[i];
                        if (col.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var bytes = HexStringToBytes(col.Substring(2));
                                string fileName = binIndex == 1 ? baseName : $"{baseName}_{binIndex}";
                                string filePath = Path.Combine(outputDirectory, fileName);
                                File.WriteAllBytes(filePath, bytes);
                                logger?.Invoke($"Line {lineNumber}: Saved {bytes.Length} bytes to '{fileName}'.");
                                binIndex++;
                            }
                            catch (Exception ex)
                            {
                                logger?.Invoke($"Line {lineNumber}: Error saving binary column {i + 1}: {ex.Message}");
                            }
                        }
                    }
                    if (binIndex == 1)
                    {
                        logger?.Invoke($"Line {lineNumber}: No binary columns found.");
                    }
                }
            }
        }

        private static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Array.Empty<byte>();
            if (hex.Length % 2 != 0)
                throw new FormatException("Hex string must have even length.");
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return bytes;
        }
    }
}
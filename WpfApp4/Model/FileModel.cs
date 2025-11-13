using System;

namespace WpfApp4.Model
{
    public class FileModel
    {
        public string Extension { get; set; } = "";
        public long FileCount { get; set; }
        public long TotalBytes { get; set; }

        public string TotalSize => FormatBytes(TotalBytes);

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size:0.##} {units[unit]}";
        }
    }
}

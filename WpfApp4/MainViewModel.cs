using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WpfApp4.Model;
using WpfApp4.Service;

namespace WpfApp4.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string? _selectedFolder;
        public string? SelectedFolder
        {
            get => _selectedFolder;
            set { _selectedFolder = value; OnPropertyChanged(nameof(SelectedFolder)); }
        }

        private bool _isIndeterminate;
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set { _isIndeterminate = value; OnPropertyChanged(nameof(IsIndeterminate)); }
        }

        private double _progressMax = 1;
        public double ProgressMax
        {
            get => _progressMax;
            set { _progressMax = value; OnPropertyChanged(nameof(ProgressMax)); }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set { _progressValue = value; OnPropertyChanged(nameof(ProgressValue)); }
        }

        private string _progressText = "";
        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; OnPropertyChanged(nameof(ProgressText)); }
        }

        public ObservableCollection<FileModel> ExtensionStats { get; } = new();

        // 원본 전체 목록 보관(TOP N 적용/해제용)
        private FileModel[] _all = Array.Empty<FileModel>();

        public void SetFromDictionary(System.Collections.Generic.Dictionary<string, ExtStat> dict)
        {
            var list = dict.Select(kv => new FileModel
            {
                Extension = kv.Key,
                FileCount = kv.Value.Count,
                TotalBytes = kv.Value.TotalBytes
            })
            .OrderByDescending(m => m.TotalBytes)
            .ToArray();

            _all = list;

            ExtensionStats.Clear();
            foreach (var m in list)
                ExtensionStats.Add(m);
        }

        public void ApplyTopN(int n)
        {
            if (_all.Length == 0) return;

            var top = _all
                .OrderByDescending(m => m.TotalBytes)
                .Take(n)
                .ToArray();

            ExtensionStats.Clear();
            foreach (var m in top)
                ExtensionStats.Add(m);
        }

        public static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size:0.##} {units[unit]}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

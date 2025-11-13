using Ookii.Dialogs.Wpf;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp4.Service;
using WpfApp4.ViewModels;
using WpfApp4;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        private readonly FileService _fileService = new FileService();
        private readonly MainViewModel _vm = new MainViewModel();
        private CancellationTokenSource? _cts;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog
            {
                Description = "스캔할 폴더를 선택하세요",
                UseDescriptionForTitle = true
            };
            if (dlg.ShowDialog(this) == true)
            {
                _vm.SelectedFolder = dlg.SelectedPath;
                Title = $"Folder Scanner - {_vm.SelectedFolder}";
            }
        }

        private async void btnScan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_vm.SelectedFolder) || !System.IO.Directory.Exists(_vm.SelectedFolder))
            {
                MessageBox.Show("먼저 '폴더 선택'을 해주세요.");
                return;
            }

            // UI 잠금
            btnScan.IsEnabled = false;
            btnBrowse.IsEnabled = false;
            btnTOPN.IsEnabled = false;
            btnCancel.IsEnabled = true;

            _cts = new CancellationTokenSource();

            try
            {
                // 1) 총 파일수 계산하는 동안 무한로딩
                _vm.IsIndeterminate = true;
                _vm.ProgressText = "파일 개수 계산 중...";
                long totalFiles = await _fileService.CountFilesAsync(_vm.SelectedFolder!, _cts.Token);

                // 2) 정량 진행률로 전환
                _vm.IsIndeterminate = false;
                _vm.ProgressMax = Math.Max(1, totalFiles);
                _vm.ProgressValue = 0;
                _vm.ProgressText = $"스캔 중... 0 / {_vm.ProgressMax:N0}";

                var progress = new Progress<long>(processed =>
                {
                    _vm.ProgressValue = Math.Min(processed, _vm.ProgressMax);
                    _vm.ProgressText = $"스캔 중... {_vm.ProgressValue:N0} / {_vm.ProgressMax:N0}";
                });

                // 3) 스캔 실행
                var dict = await _fileService.ScanByExtensionAsync(_vm.SelectedFolder!, _cts.Token, progress);

                // 4) ViewModel 리스트로 투입
                _vm.SetFromDictionary(dict);

                // 5) 완료 상태
                long totalBytes = dict.Values.Sum(v => v.TotalBytes);
                _vm.ProgressValue = _vm.ProgressMax;
                _vm.ProgressText = $"완료: 파일 {_vm.ProgressMax:N0}개, 용량 {MainViewModel.FormatBytes(totalBytes)}";
            }
            catch (OperationCanceledException)
            {
                _vm.ProgressText = "취소됨";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"스캔 중 오류: {ex.Message}");
                _vm.ProgressText = "오류";
            }
            finally
            {
                btnScan.IsEnabled = true;
                btnBrowse.IsEnabled = true;
                btnTOPN.IsEnabled = true;
                btnCancel.IsEnabled = false;

                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private void btnTOPN_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.ExtensionStats.Any())
            {
                MessageBox.Show("먼저 스캔을 완료해주세요.");
                return;
            }
            if (!int.TryParse(TOPN.Text, out int n) || n <= 0)
            {
                MessageBox.Show("TOP N에는 1 이상의 정수를 입력하세요.");
                return;
            }
            _vm.ApplyTopN(n);
        }
    }
}

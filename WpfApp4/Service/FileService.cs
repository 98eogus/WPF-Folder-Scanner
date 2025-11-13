using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp4.Service
{
    // 확장자별 집계 결과
    public class ExtStat
    {
        public long TotalBytes { get; set; }
        public long Count { get; set; }
    }

    public class FileService
    {
        public async Task<long> CountFilesAsync(string root, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                return 0;

            return await Task.Run(() =>
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    ReturnSpecialDirectories = false,
                    AttributesToSkip = FileAttributes.ReparsePoint
                };

                long total = 0;
                foreach (var _ in Directory.EnumerateFiles(root, "*", options))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    total++;
                }
                return total;
            }, cancellationToken);
        }

        public async Task<Dictionary<string, ExtStat>> ScanByExtensionAsync(
            string root,
            CancellationToken cancellationToken,
            IProgress<long>? filesScanned = null)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                throw new DirectoryNotFoundException("유효한 폴더가 아닙니다.");

            var dict = new Dictionary<string, ExtStat>(StringComparer.OrdinalIgnoreCase);

            return await Task.Run(() =>
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    ReturnSpecialDirectories = false,
                    AttributesToSkip = FileAttributes.ReparsePoint
                };

                long processed = 0;
                var lastReport = System.Diagnostics.Stopwatch.StartNew();

                foreach (var path in Directory.EnumerateFiles(root, "*", options))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        string ext = Path.GetExtension(path);
                        if (string.IsNullOrEmpty(ext)) ext = "<no extension>";

                        long len;
                        try { len = new FileInfo(path).Length; }
                        catch { continue; }

                        if (dict.TryGetValue(ext, out var s))
                        {
                            s.TotalBytes += len;
                            s.Count += 1;
                        }
                        else
                        {
                            dict[ext] = new ExtStat { TotalBytes = len, Count = 1 };
                        }
                    }
                    catch
                    {
                        // 개별 파일 문제는 무시
                    }
                    finally
                    {
                        processed++;

                        // 너무 자주 갱신하면 UI가 느려질 수 있어 약간 스로틀링
                        if (filesScanned != null &&
                            (processed % 10 == 0 && lastReport.ElapsedMilliseconds >= 50))
                        {
                            filesScanned.Report(processed);
                            lastReport.Restart();
                        }
                    }
                }

                filesScanned?.Report(processed);
                return dict;
            }, cancellationToken);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NewsAnalyzer
{
    public class NewsSourceManager
    {
        private static readonly Random random = new Random();
        private static readonly List<string> newsSources = new List<string>
        {
            "VNExpress", "Tuổi Trẻ", "Thanh Niên", "BBC", "CNN", "Reuters", "Associated Press"
        };

        // Mô phỏng tải dữ liệu từ một nguồn tin
        public static async Task<string> GetNewsAsync(string source, CancellationToken token = default)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔄 Đang tải từ {source}...");
            
            try
            {
                // Giả lập thời gian tải khác nhau cho mỗi nguồn
                int delay = random.Next(2000, 5000);
                
                // Kiểm tra cancellation token trong quá trình delay
                for (int i = 0; i < delay; i += 100)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(100, token);
                }

                // Giả lập lỗi từ nguồn CNN
                if (source == "CNN")
                {
                    throw new HttpRequestException($"Không thể kết nối đến {source} - Lỗi mạng");
                }

                // Tạo nội dung giả lập
                string content = GenerateFakeNewsContent(source);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Hoàn thành tải từ {source} ({content.Length} ký tự)");
                
                return content;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Đã hủy tải từ {source}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Lỗi khi tải từ {source}: {ex.Message}");
                throw;
            }
        }

        private static string GenerateFakeNewsContent(string source)
        {
            var contents = new Dictionary<string, string[]>
            {
                ["VNExpress"] = new[] { "Tin tức kinh tế Việt Nam", "Thể thao trong nước", "Giải trí showbiz" },
                ["Tuổi Trẻ"] = new[] { "Tin tức giáo dục", "Đời sống xã hội", "Khoa học công nghệ" },
                ["Thanh Niên"] = new[] { "Tin tức chính trị", "Văn hóa nghệ thuật", "Du lịch" },
                ["BBC"] = new[] { "World news", "Business updates", "Technology trends" },
                ["Reuters"] = new[] { "Financial markets", "Global politics", "Breaking news" },
                ["Associated Press"] = new[] { "International affairs", "Sports coverage", "Weather updates" }
            };

            var sourceContent = contents.ContainsKey(source) ? contents[source] : new[] { "General news content" };
            var selectedContent = sourceContent[random.Next(sourceContent.Length)];
            
            // Tạo nội dung có độ dài ngẫu nhiên
            return $"{selectedContent} từ {source}. " + new string('*', random.Next(100, 500));
        }

        // Phương thức tải tất cả tin tức bằng async/await
        public static async Task<Dictionary<string, string>> FetchAllNewsAsync(CancellationToken token = default)
        {
            Console.WriteLine("\n🚀 Bắt đầu tải tin tức từ tất cả nguồn bằng async/await...\n");
            
            var results = new Dictionary<string, string>();
            var tasks = new List<Task>();

            foreach (var source in newsSources)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var content = await GetNewsAsync(source, token);
                        lock (results)
                        {
                            results[source] = content;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Bỏ qua khi bị hủy
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Nguồn {source} thất bại: {ex.Message}");
                    }
                }, token));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Một số task có thể thất bại, nhưng ta vẫn trả về kết quả từ các nguồn thành công
            }

            return results;
        }

        // Phương thức tải bằng Parallel.ForEach
        public static Dictionary<string, string> FetchAllNewsParallel(CancellationToken token = default)
        {
            Console.WriteLine("\n⚡ Bắt đầu tải tin tức bằng Parallel.ForEach...\n");
            
            var results = new Dictionary<string, string>();
            var lockObject = new object();

            try
            {
                Parallel.ForEach(newsSources, new ParallelOptions 
                { 
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, source =>
                {
                    try
                    {
                        // Vì Parallel.ForEach không hỗ trợ async, ta dùng GetAwaiter().GetResult()
                        var content = GetNewsAsync(source, token).GetAwaiter().GetResult();
                        
                        lock (lockObject)
                        {
                            results[source] = content;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Bỏ qua khi bị hủy
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Nguồn {source} thất bại: {ex.Message}");
                    }
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("🛑 Quá trình tải song song đã bị hủy");
            }

            return results;
        }

        // So sánh hiệu suất Thread vs Task
        public static async Task CompareThreadVsTask()
        {
            Console.WriteLine("\n📊 So sánh hiệu suất Thread vs Task...\n");

            // Test với Thread
            Console.WriteLine("🧵 Sử dụng Thread:");
            var stopwatch = Stopwatch.StartNew();
            
            var threads = new List<Thread>();
            for (int i = 0; i < 3; i++)
            {
                int threadId = i;
                var thread = new Thread(() =>
                {
                    Console.WriteLine($"Thread {threadId} bắt đầu");
                    Thread.Sleep(2000); // Giả lập công việc
                    Console.WriteLine($"Thread {threadId} hoàn thành");
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            stopwatch.Stop();
            Console.WriteLine($"⏱️ Thời gian Thread: {stopwatch.ElapsedMilliseconds}ms\n");

            // Test với Task
            Console.WriteLine("📋 Sử dụng Task:");
            stopwatch.Restart();
            
            var tasks = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () =>
                {
                    Console.WriteLine($"Task {taskId} bắt đầu");
                    await Task.Delay(2000); // Giả lập công việc bất đồng bộ
                    Console.WriteLine($"Task {taskId} hoàn thành");
                }));
            }

            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            Console.WriteLine($"⏱️ Thời gian Task: {stopwatch.ElapsedMilliseconds}ms\n");
        }

        // Hiển thị kết quả tổng hợp
        public static void DisplayResults(Dictionary<string, string> results)
        {
            Console.WriteLine("\n📈 Kết quả tổng hợp:");
            Console.WriteLine(new string('=', 50));
            
            if (results.Count == 0)
            {
                Console.WriteLine("❌ Không có dữ liệu nào được tải thành công.");
                return;
            }

            int totalCharacters = 0;
            foreach (var result in results)
            {
                Console.WriteLine($"✅ {result.Key}: {result.Value.Length} ký tự");
                totalCharacters += result.Value.Length;
            }
            
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"📊 Tổng số nguồn thành công: {results.Count}/{newsSources.Count}");
            Console.WriteLine($"📝 Tổng số ký tự: {totalCharacters:N0}");
            Console.WriteLine($"📊 Trung bình ký tự/nguồn: {(results.Count > 0 ? totalCharacters / results.Count : 0):N0}");
        }
    }

    class Program
    {
        private static CancellationTokenSource cancellationTokenSource;

        static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 HỆ THỐNG PHÂN TÍCH TIN TỨC ĐA NGUỒN");
            Console.WriteLine(new string('=', 60));
            
            while (true)
            {
                ShowMenu();
                
                var choice = Console.ReadKey(true).KeyChar;
                Console.WriteLine();

                switch (choice)
                {
                    case '1':
                        await StartAsyncFetch();
                        break;
                    case '2':
                        await StartParallelFetch();
                        break;
                    case '3':
                        await NewsSourceManager.CompareThreadVsTask();
                        break;
                    case '4':
                        CancelCurrentOperation();
                        break;
                    case '5':
                        Console.WriteLine("👋 Cảm ơn bạn đã sử dụng hệ thống!");
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }

                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\n📋 MENU CHỨC NĂNG:");
            Console.WriteLine("1. 🔄 Tải tin tức bằng Async/Await");
            Console.WriteLine("2. ⚡ Tải tin tức bằng Parallel.ForEach");
            Console.WriteLine("3. 📊 So sánh Thread vs Task");
            Console.WriteLine("4. 🛑 Hủy thao tác hiện tại");
            Console.WriteLine("5. 🚪 Thoát");
            Console.WriteLine("\n💡 Mẹo: Trong quá trình tải, bạn có thể nhấn 'q' để hủy");
            Console.Write("\nChọn chức năng (1-5): ");
        }

        private static async Task StartAsyncFetch()
        {
            cancellationTokenSource = new CancellationTokenSource();
            
            // Tạo task để lắng nghe phím 'q'
            var keyTask = Task.Run(() => ListenForCancelKey(cancellationTokenSource));
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var results = await NewsSourceManager.FetchAllNewsAsync(cancellationTokenSource.Token);
                stopwatch.Stop();
                
                Console.WriteLine($"\n⏱️ Tổng thời gian thực hiện: {stopwatch.ElapsedMilliseconds}ms");
                NewsSourceManager.DisplayResults(results);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n🛑 Quá trình tải đã bị hủy bởi người dùng");
            }
            finally
            {
                cancellationTokenSource?.Dispose();
            }
        }

        private static async Task StartParallelFetch()
        {
            cancellationTokenSource = new CancellationTokenSource();
            
            var keyTask = Task.Run(() => ListenForCancelKey(cancellationTokenSource));
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var results = await Task.Run(() => NewsSourceManager.FetchAllNewsParallel(cancellationTokenSource.Token));
                stopwatch.Stop();
                
                Console.WriteLine($"\n⏱️ Tổng thời gian thực hiện: {stopwatch.ElapsedMilliseconds}ms");
                NewsSourceManager.DisplayResults(results);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n🛑 Quá trình tải đã bị hủy bởi người dùng");
            }
            finally
            {
                cancellationTokenSource?.Dispose();
            }
        }

        private static void CancelCurrentOperation()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                Console.WriteLine("🛑 Đã gửi tín hiệu hủy thao tác!");
            }
            else
            {
                Console.WriteLine("ℹ️ Không có thao tác nào đang chạy để hủy.");
            }
        }

        private static void ListenForCancelKey(CancellationTokenSource cts)
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    {
                        Console.WriteLine("\n🔴 Người dùng nhấn 'q' - Đang hủy thao tác...");
                        cts.Cancel();
                        break;
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
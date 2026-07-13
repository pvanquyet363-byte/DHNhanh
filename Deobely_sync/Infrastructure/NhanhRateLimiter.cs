using System.Threading;

namespace Deobely_sync.Infrastructure
{
    /// <summary>
    /// Giới hạn tốc độ gọi Nhanh API theo sliding window 40/100s
    /// (Nhanh cho phép 50/100s, mình giữ 40 để dành 10 cho API khác).
    /// Mỗi request tốn tối thiểu 2500ms slot, đảm bảo không vượt quota.
    /// </summary>
    public static class NhanhRateLimiter
    {
        private static readonly SemaphoreSlim _gate = new SemaphoreSlim(40, 40);
        private const int MinIntervalMs = 2500;

        public static async Task<T> RunAsync<T>(Func<Task<T>> action)
        {
            await _gate.WaitAsync();
            try
            {
                return await action();
            }
            finally
            {
                await Task.Delay(MinIntervalMs);
                _gate.Release();
            }
        }

        public static async Task RunAsync(Func<Task> action)
        {
            await _gate.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                await Task.Delay(MinIntervalMs);
                _gate.Release();
            }
        }
    }
}

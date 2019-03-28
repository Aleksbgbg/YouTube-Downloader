namespace YouTube.Downloader.Extensions
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal static class ProcessExtensions
    {
        internal static Task<int> RunAndWaitForExitAsync(this Process process)
        {
            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();

            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) =>
            {
                taskCompletionSource.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return taskCompletionSource.Task;
        }
    }
}
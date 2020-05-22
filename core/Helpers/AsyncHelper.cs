using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     The class is used to run async methods in places where
    ///     async methods do not work (for example getters).
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory =
            new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
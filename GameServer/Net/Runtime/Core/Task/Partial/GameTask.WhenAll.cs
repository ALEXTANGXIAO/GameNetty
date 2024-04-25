using System.Collections.Generic;

namespace GameNetty
{
    /// <summary>
    /// 提供用于异步任务操作的静态方法。
    /// </summary>
    public partial class GameTask
    {
        /// <summary>
        /// 等待所有任务完成的异步方法。
        /// </summary>
        /// <param name="tasks">要等待的任务列表。</param>
        public static async GameTask WhenAll(List<GameTask> tasks)
        {
            if (tasks.Count <= 0)
            {
                return;
            }
            
            var count = tasks.Count;
            var sTaskCompletionSource = Create();
     
            foreach (var task in tasks)
            {
                RunSTask(sTaskCompletionSource, task).Coroutine();
            }

            await sTaskCompletionSource;

            async GameVoid RunSTask(GameTask tcs, GameTask task)
            {
                await task;
                count--;
                
                if (count <= 0)
                {
                    tcs.SetResult();
                }
            }
        }

        /// <summary>
        /// 等待任意一个任务完成的异步方法。
        /// </summary>
        /// <param name="tasks">要等待的任务数组。</param>
        public static async GameTask Any(params GameTask[] tasks)
        {
            if (tasks == null || tasks.Length <= 0)
            {
                return;
            }

            var tcs = GameTask.Create();

            int count = 1;
            
            foreach (GameTask task in tasks)
            {
                RunSTask(task).Coroutine();
            }
            
            await tcs;

            async GameVoid RunSTask(GameTask task)
            {
                await task;

                count--;

                if (count == 0)
                {
                    tcs.SetResult();
                }
            }
        }
    }

    /// <summary>
    /// 提供用于异步任务操作的静态方法，支持泛型参数。
    /// </summary>
    public partial class GameTask<T>
    {
        /// <summary>
        /// 等待所有任务完成的异步方法。
        /// </summary>
        /// <param name="tasks">要等待的任务列表。</param>
        public static async GameTask WhenAll(List<GameTask<T>> tasks)
        {
            if (tasks.Count <= 0)
            {
                return;
            }
            
            var count = tasks.Count;
            var sTaskCompletionSource = GameTask.Create();

            foreach (var task in tasks)
            {
                RunSTask(sTaskCompletionSource, task).Coroutine();
            }

            await sTaskCompletionSource;

            async GameVoid RunSTask(GameTask tcs, GameTask<T> task)
            {
                await task;
                count--;
                if (count == 0)
                {
                    tcs.SetResult();
                }
            }
        }

        /// <summary>
        /// 等待所有任务完成的异步方法。
        /// </summary>
        /// <param name="tasks">要等待的任务数组。</param>
        public static async GameTask WhenAll(params GameTask<T>[] tasks)
        {
            if (tasks == null || tasks.Length <= 0)
            {
                return;
            }
            
            var count = tasks.Length;
            var tcs = GameTask.Create();

            foreach (var task in tasks)
            {
                RunSTask(task).Coroutine();
            }

            await tcs;

            async GameVoid RunSTask(GameTask<T> task)
            {
                await task;
                count--;
                if (count == 0)
                {
                    tcs.SetResult();
                }
            }
        }

        /// <summary>
        /// 等待任意一个任务完成的异步方法。
        /// </summary>
        /// <param name="tasks">要等待的任务数组。</param>
        public static async GameTask WaitAny(params GameTask<T>[] tasks)
        {
            if (tasks == null || tasks.Length <= 0)
            {
                return;
            }

            var tcs = GameTask.Create();

            int count = 1;
            
            foreach (GameTask<T> task in tasks)
            {
                RunSTask(task).Coroutine();
            }
            
            await tcs;

            async GameVoid RunSTask(GameTask<T> task)
            {
                await task;

                count--;

                if (count == 0)
                {
                    tcs.SetResult();
                }
            }
        }
    }
}
namespace Bars.NuGet.Querying.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class TaskPatch
    {
        public static Task<IEnumerable<TResult>> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            return WhenAllCore< IEnumerable<TResult>> (tasks.Cast<Task>(), (completedTasks, tcs) =>
                tcs.TrySetResult(completedTasks.Select(t => ((Task<TResult>)t).Result).ToArray()));
        }

        private static Task<TResult> WhenAllCore<TResult>(
           IEnumerable<Task> tasks,
           Action<Task[], TaskCompletionSource<TResult>> setResultAction)
        {
            // Create a TCS to represent the completion of all of the tasks.  This TCS's Task is
            // completed by a ContinueWhenAll continuation
            var tcs = new TaskCompletionSource<TResult>();
            var taskArr = tasks as Task[] ?? tasks.ToArray();
            if (taskArr.Length == 0)
            {
                setResultAction(taskArr, tcs);
            }
            else
            {
                Task.Factory.ContinueWhenAll(taskArr, completedTasks =>
                {
                    // Get exceptions for any faulted or canceled tasks
                    List<Exception> exceptions = null;
                    bool canceled = false;
                    foreach (var task in completedTasks)
                    {
                        if (task.IsFaulted) AddPotentiallyUnwrappedExceptions(ref exceptions, task.Exception);
                        else if (task.IsCanceled) canceled = true;
                    }

                    // Set up the resulting task.
                    if (exceptions != null && exceptions.Count > 0) tcs.TrySetException(exceptions);
                    else if (canceled) tcs.TrySetCanceled();
                    else setResultAction(completedTasks, tcs);
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            // Return the resulting task
            return tcs.Task;
        }

        private static void AddPotentiallyUnwrappedExceptions(ref List<Exception> targetList, Exception exception)
        {
            var ae = exception as AggregateException;
            
            // Initialize the list if necessary
            if (targetList == null) targetList = new List<Exception>();

            // If the exception is an aggregate and it contains only one exception, add just its inner exception
            if (ae != null)
            {
                targetList.Add(ae.InnerExceptions.Count == 1 ? exception.InnerException : exception);
            }
            // Otherwise, add the exception
            else targetList.Add(exception);
        }
    }
}

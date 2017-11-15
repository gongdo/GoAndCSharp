using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Goroutines
{
    public class Concurrency
    {
        // Uses ConcurrentBag to simulate Go's log output.
        private ConcurrentBag<string> logs = new ConcurrentBag<string>();
        public IEnumerable<string> Logs => logs.ToArray();
        
        public void Say(string message, TimeSpan duration)
        {
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(duration);  // do some time/IO comsuming work 'synchronously'
                logs.Add(message);
            }
        }

        // Bonus: in C# world,
        // Task with async/await pattern is highly recommended over the 'blocking' operations.
        // Unlike Go's goroutine, Task can have state, scheduler and return value.
        public async Task SayAsync(string message, TimeSpan duration)
        {
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(duration);  // do some time/IO comsuming work 'asychronously'
                logs.Add(message);
            }
        }

        // Bonus2: Task with return value.
        public async Task<TimeSpan> BenchmarkSayAsync(string message, TimeSpan duration)
        {
            var now = DateTime.Now;
            // async/await takes care of whole synchronization code.
            await SayAsync(message, duration);
            // and you can returns value as non-Task methods do.
            return DateTime.Now - now;
        }
    }
}

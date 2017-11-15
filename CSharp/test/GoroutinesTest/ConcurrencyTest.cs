using System;
using Xunit;
using Goroutines;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace GoroutinesTest
{
    public class ConcurrencyTest
    {
        [Fact]
        public void TestSay()
        {
            var sut = new Concurrency();
            var duration = TimeSpan.FromMilliseconds(5);
            Task.Run(() => sut.Say("Hello", duration));
            sut.Say("World", duration);
            // wait for task same as Go's.
            Thread.Sleep(duration);

            var output = sut.Logs.ToArray();
            Assert.Equal(10, output.Length);
            Assert.Equal(5, output.Count(message => message.EndsWith("Hello")));
        }

        [Fact]
        public void TestSay_Task_runs_concurrently()
        {
            var sut = new Concurrency();
            Task.Run(() => sut.Say("Hello", TimeSpan.FromMilliseconds(10)));
            sut.Say("World", TimeSpan.FromMilliseconds(1));

            var output = sut.Logs.ToArray();
            Assert.Equal(5, output.Length);
            Assert.Equal(5, output.Count(message => message.EndsWith("World")));
        }

        [Fact]
        public void Test_wait_Task()
        {
            var sut = new Concurrency();
            // Unlike Go's goroutine, Task has its state.
            var task = Task.Run(() => sut.Say("Hi", TimeSpan.FromMilliseconds(5)));

            // wait task completes with timeout.
            task.Wait(TimeSpan.FromSeconds(1));

            var output = sut.Logs.ToArray();
            Assert.True(output.All(message => message == "Hi"));
        }

        [Fact]
        public async Task Test_await_Task()
        {
            var sut = new Concurrency();
            // async/await pattern is recommended over 'blocking' methods.
            // but it forces to reimplement all blocking methods as they return Task or Task<T> type.
            await sut.SayAsync("Hi", TimeSpan.FromMilliseconds(5));
            // you can also  mix Task.Run and blocking method though, it is not recommended.
            // await Task.Run(() => sut.Say("Hi", TimeSpan.FromMilliseconds(5)));

            var output = sut.Logs.ToArray();
            Assert.True(output.All(message => message == "Hi"));
        }

        [Fact]
        public async Task Test_await_Task_result()
        {
            var sut = new Concurrency();
            // Also unlike Go's goroutine, Task can 'returns' result after the task completed.
            var result = await sut.BenchmarkSayAsync("Hi", TimeSpan.FromMilliseconds(5));

            // note: there's always overhead to run async operations.
            Assert.True(result > TimeSpan.FromMilliseconds(25));
        }
    }
}

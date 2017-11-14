using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Channels
{
    // There's no exact match for Go's channels in C#.
    // In this particular example, BlockingCollection<T> might be the most comparable functionality.
    // see also: https://docs.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview
    public class Pipelines
    {
        public BlockingCollection<int> Generate(params int[] numbers) 
        {
            var output = new BlockingCollection<int>(numbers.Length);
            Task.Run(() =>
            {
                foreach(var n in numbers)
                {
                    output.Add(n);
                }
                output.CompleteAdding();
            });
            return output;
        }

        public BlockingCollection<int> Squre(BlockingCollection<int> input)
        {
            var output = new BlockingCollection<int>();
            Task.Run(() =>
            {
                foreach(var n in input.GetConsumingEnumerable())
                {
                    output.Add(n * n);
                }
                output.CompleteAdding();
            });
            return output;
        }

        public BlockingCollection<int> FanIn(params BlockingCollection<int>[] inputs)
        {
            var output = new BlockingCollection<int>();
            var tasks = inputs.Select(input => Task.Run(() =>
            {
                foreach(var n in input.GetConsumingEnumerable())
                {
                    output.Add(n);
                }
            }));
            // Unlike Go, C#'s Task has self state and awaiting mechanism.
            Task.WhenAll(tasks).ContinueWith(task =>
            {
                output.CompleteAdding();
            });
            return output;
        }
    }
}

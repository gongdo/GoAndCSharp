using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Channels
{
    // .NET provides standard cancellation machanism called CancellationToken.
    // In Go, you need to provide cancellation mechanism by your self.
    // In .NET, you just pass a CancellationToken to the GetConsumingEnumerable().

    // Note that .NET prefer throwing an exception.
    // If the CancellationToken is cancelled,
    // the next GetConsumingEnumerable() iteration will throw an OperationCancelledException.

    // Cancellation with CancellationToken is not a synchronized operation.
    // But it's ok, the runtime will dispose the cancelled BlockingCollection<T>.
    public class ExplicitCancellation
    {
        // Usually a CancellationToken parameter is the last parameter of the method.
        // However if the last parameter is a params,
        // a CancellationToken parameter will be at front of the params.
        public BlockingCollection<int> Generate(
            CancellationToken cancellationToken,
            params int[] numbers) 
        {
            var output = new BlockingCollection<int>();
            Task.Run(() =>
            {
                foreach(var n in numbers)
                {
                    // check cancelled or not.
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    output.Add(n);
                }
            }, cancellationToken)
            .ContinueWith(task =>
            {
                output.CompleteAdding();
            });
            return output;
        }
        
        public BlockingCollection<int> Squre(
            BlockingCollection<int> input,
            CancellationToken cancellationToken)
        {
            var output = new BlockingCollection<int>();
            Task.Run(() =>
            {
                foreach(var n in input.GetConsumingEnumerable(cancellationToken))
                {
                    output.Add(n * n);
                }
            }, cancellationToken)
            .ContinueWith(task =>
            {
                output.CompleteAdding();
            });
            return output;
        }

        public BlockingCollection<int> FanIn(
            CancellationToken cancellationToken,
            params BlockingCollection<int>[] inputs)
        {
            var output = new BlockingCollection<int>();
            var tasks = inputs.Select(input => 
                Task.Run(() =>
                {
                    foreach(var n in input.GetConsumingEnumerable(cancellationToken))
                    {
                        output.Add(n);
                    }
                }, cancellationToken)
            );
            Task.WhenAll(tasks).ContinueWith(task =>
            {
                output.CompleteAdding();
            });
            return output;
        }
    }
}

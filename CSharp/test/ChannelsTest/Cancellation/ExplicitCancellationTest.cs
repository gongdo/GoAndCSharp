using Xunit;
using Channels;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ChannelsTest
{
    public class ExplicitTest
    {
        [Fact]
        public void TestCancellation()
        {
            var sut = new ExplicitCancellation();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var numbers = new int[] { 1, 2, 3, 4 };
            var source = sut.Generate(cancellationToken, numbers);
            var sequence1 = sut.Squre(source, cancellationToken);
            var sequence2 = sut.Squre(source, cancellationToken);
            var merged = sut.FanIn(cancellationToken, sequence1, sequence2);

            // pass a CancellationToken to the enumerator.
            var sequence = merged.GetConsumingEnumerable(cancellationToken);
            // consumes first one.
            var value = sequence.FirstOrDefault();
            Assert.True(value > 0);

            // Cancellation is not a synchronized operation.
            cts.Cancel();

            // An OperationCancelledException will be thrown when get next item from the collection.
            Assert.Throws<OperationCanceledException>(() => sequence.FirstOrDefault());
        }

        [Fact]
        public void TestCancellation_during_enumerating()
        {
            var sut = new ExplicitCancellation();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var numbers = new int[] { 1, 2, 3, 4 };
            var source = sut.Generate(cancellationToken, numbers);
            var sequence1 = sut.Squre(source, cancellationToken);
            var sequence2 = sut.Squre(source, cancellationToken);
            var merged = sut.FanIn(cancellationToken, sequence1, sequence2);

            var sequence = merged.GetConsumingEnumerable(cancellationToken);
            Assert.Throws<OperationCanceledException>(() =>
            {
                foreach(var n in sequence)
                {
                    // cancel at the first iteration.
                    cts.Cancel();
                    // It will be thrown at the next iteration.
                }
            });
        }
    }
}

using System;
using Xunit;
using Channels;
using System.Collections.Concurrent;
using System.Linq;

namespace ChannelsTest
{
    public class PipelinesTest
    {
        [Fact]
        public void Generate()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3 };
            var sequence = sut.Generate(numbers).GetConsumingEnumerable();
            var i = 0;
            foreach(var n in sequence)
            {
                Assert.Equal(numbers[i], n);
                i++;
            }
            Assert.Equal(3, i);
        }

        [Fact]
        public void Generate_completes_collection_after_loop()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3 };
            var source = sut.Generate(numbers);
            foreach(var n in source.GetConsumingEnumerable())
            {
            }
            Assert.True(source.IsCompleted);
        }

        [Fact]
        public void Squre()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3 };
            var expected = new int[] { 1, 4, 9 };
            var source = sut.Generate(numbers);
            var sequence = sut.Squre(source).GetConsumingEnumerable();
            var i = 0;
            foreach(var n in sequence)
            {
                Assert.Equal(expected[i], n);
                i++;
            }
            Assert.Equal(3, i);
        }
        
        [Fact]
        public void Squre_completes_collection_after_loop()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3 };
            var source = sut.Squre(sut.Generate(numbers));
            foreach(var n in source.GetConsumingEnumerable())
            {
            }
            Assert.True(source.IsCompleted);
        }

        [Fact]
        public void FanIn()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3, 4 };
            var expected = new int[] { 1, 4, 9, 16 };
            var source = sut.Generate(numbers);
            var sequence1 = sut.Squre(source);
            var sequence2 = sut.Squre(source);
            var count = 0;
            var merged = sut.FanIn(sequence1, sequence2).GetConsumingEnumerable();
            foreach(var n in merged)
            {
                Assert.Contains(n, expected);
                count++;
            }
            Assert.Equal(4, count);
        }
        
        [Fact]
        public void FanIn_completes_collection_after_loop()
        {
            var sut = new Pipelines();
            var numbers = new int[] { 1, 2, 3, 4 };
            var source = sut.Generate(numbers);
            var sequence1 = sut.Squre(source);
            var sequence2 = sut.Squre(source);
            var mergedSource = sut.FanIn(sequence1, sequence2);
            foreach(var n in mergedSource.GetConsumingEnumerable())
            {
            }
            Assert.True(mergedSource.IsCompleted);
        }
    }
}

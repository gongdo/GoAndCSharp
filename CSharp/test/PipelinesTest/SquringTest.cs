using System;
using Xunit;
using Pipelines;
using System.Reactive;
using System.Reactive.Linq;

namespace PipelinesTest
{
    public class SquringTest
    {
        [Fact]
        public void TestGenerate()
        {
            var sut = new Squaring();
            var numbers = new int[] { 1, 2, 3 };
            var sequence = sut.Generate(numbers);
            var i = 0;
            sequence.Do(item =>
            {
                Assert.Equal(numbers[i], item);
                i++;
            }).Subscribe();
            Assert.Equal(3, i);
        }

        [Fact]
        public void TestSqure()
        {
            var sut = new Squaring();
            var numbers = new int[] { 1, 2, 3 };
            var expected = new int[] { 1, 4, 9 };
            var sequence = sut.Squre(numbers.ToObservable());
            var i = 0;
            sequence.Do(item =>
            {
                Assert.Equal(expected[i], item);
                i++;
            }).Subscribe();
            Assert.Equal(3, i);
        }
    }
}

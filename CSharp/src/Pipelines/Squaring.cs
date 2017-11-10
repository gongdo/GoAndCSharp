using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Pipelines
{
    public class Squaring
    {
        public IObservable<int> Generate(params int[] numbers) => numbers.ToObservable();

        public IObservable<int> Squre(IObservable<int> input)
        {
            return input.Select(n => n * n);
        }
    }
}

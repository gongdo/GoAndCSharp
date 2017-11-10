using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Pipelines
{
    // There's no exact match for Go's chan in C#.
    // In this particular example, Observable might be the most comparable functionality.
    public class Squaring
    {
        // actually, c# doesn't need a function like this
        // since System.Reactive already support Array<T>.ToObservable() extension method.
        public IObservable<int> Generate(params int[] numbers) 
            => numbers.ToObservable();

        // you can say this also as pipeline, map, transformation.
        // in C# world, 'Select' is more proper term.
        public IObservable<int> Squre(IObservable<int> input)
        {
            return input.Select(n => n * n);
        }
    }
}

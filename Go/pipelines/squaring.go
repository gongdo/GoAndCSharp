// Package pipelines explains what is a pipeline.
package pipelines

// Gen converts a list of integer values to a channel that emits the values.
func Gen(nums ...int) <-chan int {
	out := make(chan int)
	go func() {
		for _, n := range nums {
			out <- n
		}
		close(out)
	}()
	return out
}

// Sq receives integer values from a channel and returns a channel that emits the square of each value.
func Sq(in <-chan int) <-chan int {
	out := make(chan int)
	go func() {
		for n := range in {
			out <- n * n
		}
		close(out)
	}()
	return out
}

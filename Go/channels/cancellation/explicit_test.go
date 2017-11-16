package cancellation

import (
	"testing"

	"../../channels"
)

func TestDone(t *testing.T) {
	in := channels.Gen(2, 3)

	// Set up a done channel that's shared by the whole pipeline,
	// and close that channel when this pipeline exits, as a signal
	// for all the goroutines we started to exit.
	done := make(chan struct{})

	// Distribute the sq work across two goroutines that both read from in.
	c1 := Sq(done, in)
	c2 := Sq(done, in)

	out := FanIn(done, c1, c2)
	// Consume the first value from output.
	<-out // 4 or 9

	// close done that causes to close all unconsumed channel.
	close(done)
	val, ok := <-out
	if val != 0 || ok {
		t.Error("Must not be ok when receive on close channel.")
	}
}

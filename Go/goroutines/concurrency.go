package goroutines

import (
	"log"
	"time"
)

// Say logs given string for 5 times with waiting each.
// see also: https://tour.golang.org/concurrency/1
func Say(s string, duration time.Duration) {
	for i := 0; i < 5; i++ {
		time.Sleep(duration) // do some time/IO comsuming work 'synchronously'
		log.Println(s)
	}
}

// Unlike C#'s Task, goroutine is extremely simple.
// There's no state, no user-defined scheduler, neither result.
// In real world, you will need Channels or Mutex to communicate propery.

// Although I compared goroutine with Task, they are different.
// It's not a replaceable one for the other, even if you can.
// Instead, you need to use right code for the language you choose.

package goroutines

import (
	"strings"
	"testing"
	"time"

	"../tools"
)

func TestSay(t *testing.T) {
	output := tools.CaptureLogLines(func() {
		duration := 5 * time.Millisecond
		go Say("Hello", duration)
		Say("World", duration)
		// since 'go Say' goroutine is running concurrently,
		// you need to wait to make sure the goroutine were ended.
		// Unlike C#'s Task, you never know when the goroutine will be ended.
		// You must use synchronization mechanism like Channels or sync functions.
		// This naive approch bellow is test purpose only.
		// You must not use time.Sleep() in order to wait any goroutines.
		time.Sleep(duration)
	})

	if lines := len(output); lines != 10 {
		t.Errorf("It must be 10, but %d\n%v", lines, output)
	}

	filter := func(v string) bool {
		return strings.HasSuffix(v, "Hello")
	}
	if occurs := len(tools.Filter(output, filter)); occurs != 5 {
		t.Errorf("Hello must occurs 5 times, but %d", occurs)
	}
}

func TestSay_go_routine_runs_concurrently(t *testing.T) {
	output := tools.CaptureLogLines(func() {
		// Warning: naive approch again.
		go Say("Hello", 10*time.Millisecond)
		Say("World", 1*time.Millisecond)
	})

	// 'go Say' will not be finished before the 'Say' ending.
	if lines := len(output); lines != 5 {
		t.Errorf("It must be 5, but %d\n%v", lines, output)
	}

	filter := func(v string) bool {
		return strings.HasSuffix(v, "World")
	}
	if occurs := len(tools.Filter(output, filter)); occurs != 5 {
		t.Errorf("World must occurs 5 times, but %d", occurs)
	}
}

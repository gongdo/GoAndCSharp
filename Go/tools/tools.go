// Package tools provides utilities to help testing and reducing repetitive works.
package tools

import (
	"bytes"
	"log"
	"os"
	"strings"
)

// CaptureLog captures StdOut while running the given func,
// then returns it as plain string.
func CaptureLog(f func()) string {
	var buf bytes.Buffer
	log.SetOutput(&buf)
	f()
	log.SetOutput(os.Stdout)
	return buf.String()
}

// CaptureLogLines captures StdOut while running the given func,
// then returns it as linebreak separated string array.
// Becarful! it will not only split lines with '\n' but also remove last empty element.
func CaptureLogLines(f func()) []string {
	split := strings.Split(CaptureLog(f), "\n")
	if len := len(split) - 1; split[len] == "" {
		return split[:len]
	}
	return split
}

// Filter filters given array with predicate func.
func Filter(array []string, predicate func(string) bool) []string {
	output := make([]string, 0)
	for _, v := range array {
		if predicate(v) {
			output = append(output, v)
		}
	}
	return output
}

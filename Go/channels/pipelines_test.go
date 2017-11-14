package channels

import (
	"fmt"
	"sync"
	"testing"
)

func TestGen(t *testing.T) {
	nums := []int{1, 2, 3}
	c := Gen(nums...)
	i := 0
	for num := range c {
		if nums[i] != num {
			t.Errorf("Expected %d but %d", nums[i], num)
		}
		i++
	}
	if i != 3 {
		t.Error("i must be 3.")
	}
}

func TestGen_closed_after_range_loop(t *testing.T) {
	nums := []int{1, 2, 3}
	c := Gen(nums...)
	for range c {
	}
	if _, ok := <-c; ok {
		t.Errorf("Channel must be closed.")
	}
}

func TestSq(t *testing.T) {
	nums := []int{1, 2, 3}
	expected := []int{1, 4, 9}
	c := make(chan int)
	go func() {
		for _, num := range nums {
			c <- num
		}
		close(c)
	}()
	d := Sq(c)
	i := 0
	for num := range d {
		if expected[i] != num {
			t.Errorf("Expected %d but %d", expected[i], num)
		}
		i++
	}
	if i != 3 {
		t.Error("i must be 3.")
	}
}

func TestSq_closed_after_range_loop(t *testing.T) {
	nums := []int{1, 2, 3}
	c := make(chan int)
	go func() {
		for _, num := range nums {
			c <- num
		}
		close(c)
	}()
	d := Sq(c)
	for range d {
	}
	if _, ok := <-d; ok {
		t.Errorf("Channel must be closed.")
	}
}

func TestFanIn(t *testing.T) {
	in := Gen(2, 3, 4, 5)

	// Distribute the sq works across two goroutines that both read from in.
	c1 := Sq(in)
	c2 := Sq(in)

	// Consume the merged output from c1 and c2.
	count := 0
	for n := range FanIn(c1, c2) {
		if n != 4 && n != 9 && n != 16 && n != 25 {
			t.Errorf("Unexpected number %d detected.", n)
		}
		count++
	}
	if count != 4 {
		t.Errorf("Unexpected count %d.", count)
	}
}

func TestFanIn_distribution_level(t *testing.T) {
	numbers := make([]int, 100)
	for i := 0; i < 100; i++ {
		numbers[i] = i + 1
	}
	in := Gen(numbers...)
	counts := make([]int, 2)
	var mtx sync.Mutex

	check := func(c <-chan int, index int) <-chan int {
		out := make(chan int)
		go func() {
			for n := range c {
				out <- n
				mtx.Lock()
				counts[index] = counts[index] + 1
				mtx.Unlock()
			}
			close(out)
		}()
		return out
	}
	c1 := check(in, 0)
	c2 := check(in, 1)

	for range FanIn(c1, c2) {
	}

	// distribution of the works can not be predictable
	fmt.Printf("distribution: %d:%d\n", counts[0], counts[1])

	if sum := counts[0] + counts[1]; sum != 100 {
		t.Errorf("Unexpected counts %d.", sum)
	}
}

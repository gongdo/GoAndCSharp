package pipelines

import "testing"

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

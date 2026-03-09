package main

import (
    "bufio"
    "fmt"
    "os"
    "sort"
    "strings"
)

// CountWays calculates the number of ways to create the design using patterns.
func CountWays(design string, patterns []string, memo map[string]int64) int64 {
    if design == "" {
        return 1
    }

    if val, exists := memo[design]; exists {
        return val
    }

    var count int64
    for _, pattern := range patterns {
        if strings.HasPrefix(design, pattern) {
            remaining := design[len(pattern):]
            count += CountWays(remaining, patterns, memo)
        }
    }

    memo[design] = count
    return count
}

func main() {
    // Open the file
    file, err := os.Open("e:/source/advent2024/12-19/towels.txt")
    if err != nil {
        fmt.Println("Error opening file:", err)
        return
    }
    defer file.Close()

    // Read the file
    scanner := bufio.NewScanner(file)
    var lines []string
    for scanner.Scan() {
        lines = append(lines, scanner.Text())
    }

    if err := scanner.Err(); err != nil {
        fmt.Println("Error reading file:", err)
        return
    }

    // Extract patterns and designs
    patterns := strings.Split(lines[0], ",")
    for i := range patterns {
        patterns[i] = strings.TrimSpace(patterns[i])
    }
    sort.SliceStable(patterns, func(i, j int) bool {
        return len(patterns[i]) < len(patterns[j])
    })

    designs := lines[2:]

    // Calculate the number of ways for each design
    memo := make(map[string]int64)
    var totalWays int64

    for _, design := range designs {
        ways := CountWays(design, patterns, memo)
        totalWays += ways
    }

    fmt.Printf("Total Ways: %d\n", totalWays)
}
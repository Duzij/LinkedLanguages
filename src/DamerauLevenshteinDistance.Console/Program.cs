// See https://aka.ms/new-console-template for more information

using DamerauLevenshteinDistance.Console;

using System;

Console.WriteLine($"Word one {args[0]}, word two {args[1]}");
Console.WriteLine($"Distance is {DamerauLevenshteinCalculator.Calc(args[0], args[1])}.");



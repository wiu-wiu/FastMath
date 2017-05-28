# FastMath
FastMath is a C# library with very fast but not very accurate (at least 1e-3 precision) System.Math methods implementations.
It uses memoization technique to calculate methods as fast as possible.

## [Nuget package](https://www.nuget.org/packages/FastMath)

# Benchmarks
In general System.Math is highly optimized, here are some benchmarks done at with my laptop and BenchmarkDotNet:
![System.Math benchmark](https://github.com/wiu-wiu/FastMath/blob/master/BenchmarkResults/SystemMathPerformance.png)

FastMath methods perform much faster in all runtimes except Mono.
![FastMath benchmark](https://github.com/wiu-wiu/FastMath/blob/master/BenchmarkResults/FastMathPerformance.png)
other chart - represents all other methods from System.Math except atan2, sin and cos.
linear interpolated method - shows performance of methods with linear interpolation. Such methods provide better accuracy, but perfoms a bit slower.

This table show performance gain by each method at every runtime. Here (x11) stands for 11 times performance gain of FastMath method comparing to the original System.Math method in this runtime.

 | Method    | Clr x86      | Clr x64      | Net Core x64 | Mono x64       |
 |---------- |------------- |------------- |------------- |--------------- |
 | sin/cos   | 2.7 ns (x11) | 1.7 ns (x20) | 1.7 ns (x7)  | 18.1 ns (x2)   |
 | tan       | 3.1 ns (x13) | 1.9 ns (x21) | 1.9 ns (x10) | 18.5 ns (x1)   | 
 | asin/acos | 3.1 ns (x11) | 1.9 ns (x24) | 1.9 ns (x29) | 18.5 ns (x0.3) |
 | atan      | 3.1 ns (x15) | 1.9 ns (x8)  | 1.9 ns (x8)  | 18.5 ns (x1)   |
 | sinh/cosh | 3.1 ns (x17) | 1.9 ns (x9)  | 1.9 ns (x8)  | 18.5 ns (x1)   |
 | tanh      | 3.1 ns (x18) | 1.9 ns (x14) | 1.9 ns (x11) |	18.5 ns (x1)   |
 | sqrt      | 3.1 ns (x1)  | 1.9 ns (x2)  | 1.9 ns (x2)  |	18.5 ns (x0.2) |
 | pow       | 3.1 ns (x15) | 1.9 ns (x22) | 1.9 ns (x22) |	18.5 ns (x3)   |
 | log       | 3.1 ns (x15) | 1.9 ns (x14) | 1.9 ns (x14) | 18.5 ns (x2)   |
 | atan2     | 18.9 ns (x3) | 7.0 ns (x5)  | 7.0 ns (x5)  | 34.8 ns (x1)   |

# Usage
The usage of FastMath library methods is divided into two parts: creation of a method and usage of it.
To create a method you should provide its "working interval".
For example working intervals of sin and cos methods are always from 0 to 2 * PI.
You can't use library's methods without working interval, or when its too big.
So if your task is to calculate log with any positive float argument, this library IS NOT what you are looking for.
Almost all methods provide 3 ways two create it:
- use a constructor that takes min and max arguments and number of values that would be memoized.
```cs
// create sin method with 1000 memoized values.
var sin = new MemoizedSin(1000);
```
- use a ConstructByStep method, that takes min and max arguments and step with which values would be memoized.
```cs
// create pow method that works in range from 0 to 100, calculates x^3, and store values with step 0.01.
var pow = MemoizedPow.ConstructByStep(minArgument: 0, maxArgument: 100, power: 3, step: 0.01f); 
```
- use a ConstructByMaxError method, that takes min and max arguments and max error, that shouldn't be reached.
```cs
// create sqrt method that works in range from 1 to 1000 and with errors less than 0.1.
var sqrt = MemoizedSqrt.ConstructByMaxError(minArgument: 1, maxArgument: 1000, maxError: 0.1f);
```

The last one is a recommended option. 
Max error argument should be:
- from 1 to 1e-5 for sin and cos methods
- from 1 to 1e-3 for all other methods. 
If you try to create method with too low max error (e.g. 1e-6) constructor will either throw an exception or create a 500 MB of memoized data, so be careful with it :)

## Examples

Get working range by printing min and max arguments.
```cs
var sin = MemoizedSin.ConstructByMaxError(0.001f);
Console.WriteLine(sin.MinArgument + ", " + sin.MaxArgument);
```
Calculate sin of PI. Notice: all results and arguments are floats.
```cs
sin.Calculate((float) Math.PI * 100);
```
Error! Can't calculate sin of argument more than sin.MaxArgument. Method will throw an exception.
```cs
sin.Calculate((float) Math.PI * 100);
```
Special version of calculate method that could take any argument, but perfoms a bit slower (usually in 2-3 times slower than the original methods).
```cs
sin.CalculateUnbound((float) Math.PI * 100);
```
# Additional information
The library with methods which values ranges are around zero (e.g. sin, cos, asin, e^x where < 0, etc.).
As long as values are not very high or low you don't need to store a lot of method's values to provide good accuracy.
But if you try to create memoized method for e^x where x from 0 to 10, you will find out that a huge amount of values is needed to provide even low accuracy.

This problem is partly solved with linear interpolated versions of methods. This approach requires less values to provide similar accuracy, although they performacne is a bit slower. However it also has a limitations - if you try to create linear interpolated method for e^x where x from 0 to 100, linear interpolated methods wouldn't help you.

You could create your own memoized methods by using MemoizedMethod and LinearInterpolatedMethod classes.

FastMath.Core namespace contains methods that could help you to check created method accuracy:
```cs
var sin = new MemoizedSin(1000);
sin.MaxError(); // calculates max error of created method
sin.MeanError(); // calculates mean error of created method
```

# FastMath
FastMath is a C# library with very fast but not very accurate (at least 1e-3 precision) realizations of System.Math methods.
It uses memoization technique to calculate methods as fast as possible.

# Benchmarks
Generally System.Math is highly optimized, here are some benchmarks done at my laptop with BenchmarkDotNet:
![System.Math benchmark](https://github.com/wiu-wiu/FastMath/blob/master/BenchmarkResults/SystemMathPerformance.png)

FastMath methods are calculated with much higher speed at all runtimes except Mono.
![FastMath benchmark](https://github.com/wiu-wiu/FastMath/blob/master/BenchmarkResults/FastMathPerformance.png)
other chart - means all other methods from System.Math except atan2, sin and cos.
linear interpolated method - means version of methods with enabled linear interpolation. Such methods provide better accuracy, but calculated a bit slower.

This table show performance gain by each method at every runtime. For example (x11) means that original method from System.Math runs at this runtime in 11 times slower than the FastMath method.

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
To create method you should provide "working interval" of the method.
For example working intervals of sin and cos methods are always from 0 to 2 * PI.
You could only calculate method in provided interval.
You can't use library's methods without working interval, or when it is very big.
So if your task is to calculate log with any positive float argument, this library IS NOT what you are looking for.
Almost all methods provide 3 ways two create it:
- use a constructor that takes min and max arguments and number of values that would be memoized.
- use a ConstructByStep method, that takes min and max arguments and step with which values would be memoized.
- use a ConstructByMaxError method, that takes min and max arguments and max error, that shouldn't be reached.

The last one is a recommended option. Max error argument should be from 1 to 1e-5 for sin and cos methods, and from 1 to 1e-3 for all other methods. If you try to create method with very low max error (for example 1e-6) exception will be thrown or method might be created but with 500 MB of memoized data, so be careful with it :)

## Examples

Method creation by constructor
```cs
var sin = new MemoizedSin(1000); // create sin method with 1000 memoized values.
```

Method creation by ConstructByStep
```cs
var pow = MemoizedPow.ConstructByStep(minArgument: 0, maxArgument: 100, power: 3, step: 0.01f); 
// create pow method that works in range from 0 to 100, calculates x^3, and store values with step 0.01.
```

Method creation by ConstructByMaxError
```cs
var sqrt = MemoizedSqrt.ConstructByMaxError(minArgument: 1, maxArgument: 1000, maxError: 0.1f);
// create sqrt method that works in range from 1 to 1000 and with errors less than 0.1.
```

Usage of created method
```cs
var sin = MemoizedSin.ConstructByMaxError(0.001f);
Console.WriteLine(sin.MinArgument + ", " + sin.MaxArgument); // shows min and max arguments.
sin.Calculate((float) Math.PI); // calculates sin of PI. All results arnd arguments are floats.
sin.Calculate((float) Math.PI * 100); // ERROR! can't calculate sin of argument more than sin.MaxArgument.
sin.CalculateUnbound((float) Math.PI * 100); // special version of Calculate method that could take any argument, but calculated a bit slower (usually in 2-3 times than the original methods).
```
# Additional information
The best usage of library is possible with methods that has values near zero range. For example: sin, cos, asin, e^x where < 0 and so on.
As long as values are not very high or low you don't need to store a lot of method's values to provide good accuracy.
But if you try to create memoized method for e^x where x from 0 to 10, you will find that a huge amount of values is needed to provide even low accuracy.

This problem is partly solved with linear interpolated versions of methods. Such methods need less values to get similar accuracy, but they are calculated a bit slower. But they also has a limitations - if you try to create linear interpolated method for e^x where x from 0 to 100, linear interpolated methods wouldn't help you.

FastMath.Core namespace contains methods that could help you to check created method accuracy:
```cs
var sin = new MemoizedSin(1000);
sin.MaxError(); // calculates max error of created method
sin.MeanError(); // calculates mean error of created method
```

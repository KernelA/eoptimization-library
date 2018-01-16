# EOptimization library 

## Summary

The library for solving constrained optimization problems. Implemented three algorithms: Big bung - big crunch, Fireworks, Grenade explosion. Implementation for .Net Core. Supported version .NET Standard is 1.0.

It does not require third-party dependencies.

**Problem formulation**

![Minimization f(x)](/Docs/Images/eq.png)

**Example**

![Example 1](/Docs/Images/example1.png)

## Building

1. Use Visual Studio 2017.
2. CLI:
    * Only once, run in command prompt `dotnet restore`.
    * Run `dotnet build -c Release`.
    * If necessary, run tests `dotnet test ./EOptimizationTests/EOptimizationTest.csproj`.

## Description of methods

You can read about methods in the next articles:

1. [Big bung - big crunch method.](http://www.sciencedirect.com/science/article/pii/S0965997805000827)
2. [Fireworks method.](http://link.springer.com/chapter/10.1007/978-3-642-13495-1_44)
3. [GEM method.](http://www.sciencedirect.com/science/article/pii/S0096300309000058)

## How to use

See [wiki.](https://github.com/KernelA/EOptimization-library/wiki)

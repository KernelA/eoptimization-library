# EOptimization library 

## Summary

The library for solving constrained optimization problems. Implemented foure algorithms: Big bung - big crunch, Fireworks, Grenade explosion and multiobjective fireworks. Implementation for .Net Core. Supported version .NET Standard is 2.0. 

**Problem formulation**

![Minimization f(x)](/Docs/Images/eq.png)

**Example**

![Example 1](/Docs/Images/example1.png)

## Requirements

1. [.Net Core SDK 3.0 or higher (main library is targeting on .Net Standard 2.0)](https://dotnet.microsoft.com/download).
2. [nds (solution contains local nuget package)](https://github.com/KernelA/nds)

## Building

### Visual Studio 2019

Simple open solution and run.

### [CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x):
    
* Run `dotnet build -c Release .`.
* If necessary, run tests `dotnet test .\EOptimizationTests\EOptTests.csproj`.

## Description of methods

You can read about methods in the next articles:

1. [Big bung - big crunch method.](http://www.sciencedirect.com/science/article/pii/S0965997805000827)
2. [Fireworks method.](http://link.springer.com/chapter/10.1007/978-3-642-13495-1_44)
3. [GEM method.](http://www.sciencedirect.com/science/article/pii/S0096300309000058)
4. [Multiobjective fireworks method](https://avia.mstuca.ru/jour/article/view/1520/1154)

## How to use

See `OOExample` and `MOExample` project.
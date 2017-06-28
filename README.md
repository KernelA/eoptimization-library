# EOptimization library .Net Core

## Summary
The library for solving constrained optimization problems. Implemented three algorithms: Big bung - big crunch, Fireworks, Grenade explosion. Implementation for .Net Core.

**Problem formulation**

![Minimization f(x)](/Docs/Images/eq.png)

**Example**

![Example 1](/Docs/Images/example1.png)

## Branches

Exist two branches:

1. `master` library for .Net framework 4.5.
2. `dotnet_core` library for .Net Core.

## Building

1. Use Visual Studio 2017.
2. CLI interface:
    * Only once, run in command prompt `dotnet restore`.
    * Run `dotnet build -c Release`.
    * If necessary, run tests `dotnet test ./EOptimizationTests/EOptimizationTest.csproj`.

## Description of methods

You can read about methods in next articles.

**References to atricle**

1. [Big bung - big crunch method.](http://www.sciencedirect.com/science/article/pii/S0965997805000827)
2. [Fireworks method.](http://link.springer.com/chapter/10.1007/978-3-642-13495-1_44)
3. [GEM method.](http://www.sciencedirect.com/science/article/pii/S0096300309000058)

## How use?

See [wiki.](https://github.com/KernelA/EOptimization-library/wiki)

## License

The library distributed under [MIT license](https://mit-license.org/).

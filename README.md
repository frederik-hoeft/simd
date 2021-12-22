# simd-unhexlify
A fast and simple c# hex-decode function using AVX2 and SSSE3 X86 Intel intrinsics.

Using Benchmark.NET on Intel Xeon E3 1230 v3 @ 3.30GHz reveals a 400% performance increase compared to the .NET 6 implementation of `Convert.FromHexString()`.

## Benchmark.NET results for 3580 byte test string:

the longer the data the faster the SIMD version should be.

```
// * Detailed results *
HexDecoder.Unhexlify: DefaultJob
Runtime = .NET 6.0.1 (6.0.121.56705), X64 RyuJIT; GC = Concurrent Workstation
Mean = 1.891 us, StdErr = 0.011 us (0.59%), N = 91, StdDev = 0.106 us
Min = 1.719 us, Q1 = 1.800 us, Median = 1.866 us, Q3 = 1.960 us, Max = 2.171 us
IQR = 0.159 us, LowerFence = 1.561 us, UpperFence = 2.199 us
ConfidenceInterval = [1.853 us; 1.928 us] (CI 99.9%), Margin = 0.038 us (2.00% of Mean)
Skewness = 0.59, Kurtosis = 2.53, MValue = 2.15
-------------------- Histogram --------------------
[1.688 us ; 1.727 us) | @
[1.727 us ; 1.789 us) | @@@@@@@@@@@@@
[1.789 us ; 1.850 us) | @@@@@@@@@@@@@@@@@@@@@@@@@@@
[1.850 us ; 1.917 us) | @@@@@@@@@@@@@@@@@@@
[1.917 us ; 1.988 us) | @@@@@@@@@@@
[1.988 us ; 2.050 us) | @@@@@@@@@@@@@
[2.050 us ; 2.123 us) | @@@@@
[2.123 us ; 2.202 us) | @@
---------------------------------------------------

HexDecoder.ConvertFromHex: DefaultJob
Runtime = .NET 6.0.1 (6.0.121.56705), X64 RyuJIT; GC = Concurrent Workstation
Mean = 7.902 us, StdErr = 0.032 us (0.41%), N = 15, StdDev = 0.126 us
Min = 7.778 us, Q1 = 7.821 us, Median = 7.845 us, Q3 = 7.958 us, Max = 8.183 us
IQR = 0.137 us, LowerFence = 7.615 us, UpperFence = 8.163 us
ConfidenceInterval = [7.767 us; 8.036 us] (CI 99.9%), Margin = 0.134 us (1.70% of Mean)
Skewness = 1.06, Kurtosis = 2.7, MValue = 2
-------------------- Histogram --------------------
[7.745 us ; 8.069 us) | @@@@@@@@@@@@@
[8.069 us ; 8.250 us) | @@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 7 SP1 (6.1.7601.0)
Intel Xeon CPU E3-1230 v3 3.30GHz, 1 CPU, 8 logical and 4 physical cores
Frequency=3215234 Hz, Resolution=311.0194 ns, Timer=TSC
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


|         Method |     Mean |     Error |    StdDev |
|--------------- |---------:|----------:|----------:|
|      Unhexlify | 1.891 us | 0.0378 us | 0.1059 us |
| ConvertFromHex | 7.902 us | 0.1343 us | 0.1256 us |
```

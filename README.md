# simd-unhexlify
A fast and simple c# hex-decode function using AVX2 and SSSE3 X86 Intel intrinsics.

Using Benchmark.NET on Intel Xeon E3 1230 v3 @ 3.30GHz reveals a 400% performance increase compared to the .NET 6 implementation of `Convert.FromHexString()`.

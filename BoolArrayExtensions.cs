using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Testing;

public static class BoolArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static unsafe int CountTrueBytes(this bool[] myBools)
    {
        // we need to get a pointer to the bool array to do our magic
        fixed (bool* ptr = myBools)
        {
            // reinterpret all booleans as bytes
            byte* bytePtr = (byte*)ptr;

            // calculate the number of 32 bit integers that would fit into the array
            int dwordLength = myBools.Length >> 2;

            // for SIMD, allocate a result vector
            Vector128<int> result = Vector128<int>.Zero;

            // loop variable
            int i = 0;

            // it could be that SSSE3 isn't supported...
            if (Ssse3.IsSupported)
            {
                // remember: we're assuming little endian!
                // we need this mask to convert the byte vectors to valid int vectors
                Vector128<int> cleanupMask = Vector128.Create(0x000000FF);

                // iterate over the array processing 16 bytes at once
                // TODO: you could even go to 32 byte chunks if AVX-2 is supported...
                for (; i < dwordLength - Vector128<int>.Count; i += Vector128<int>.Count)
                {
                    // load 16 bools / bytes from memory
                    Vector128<byte> v = Sse2.LoadVector128((byte*)((int*)bytePtr + i));

                    // now count the number of "true" bytes in every 32 bit integers
                    // 1. shift
                    Vector128<int> v0 = v.As<byte, int>();
                    Vector128<int> v1 = Sse2.ShiftRightLogical128BitLane(v, 1).As<byte, int>();
                    Vector128<int> v2 = Sse2.ShiftRightLogical128BitLane(v, 2).As<byte, int>();
                    Vector128<int> v3 = Sse2.ShiftRightLogical128BitLane(v, 3).As<byte, int>();

                    // 2. cleanup invalid bytes
                    v0 = Sse2.And(v0, cleanupMask);
                    v1 = Sse2.And(v1, cleanupMask);
                    v2 = Sse2.And(v2, cleanupMask);
                    v3 = Sse2.And(v3, cleanupMask);

                    // 3. add them together. We now have a vector of ints holding the number
                    // of "true" booleans / 0x01 bytes in their 32 bit memory region
                    Vector128<int> roundResult = Sse2.Add(Sse2.Add(Sse2.Add(v0, v1), v2), v3);

                    // 4 now add everything to the result
                    result = Sse2.Add(result, roundResult);
                }

                // reduce the result vector to a scalar by horizontally adding log_2(n) times
                // where n is the number of words in out vector
                result = Ssse3.HorizontalAdd(result, result);
                result = Ssse3.HorizontalAdd(result, result);
            }
            int totalNumberOfTrueBools = result.ToScalar();

            // now add all remaining booleans together 
            // (if the input array wasn't a multiple of 16 bytes or SSSE3 wasn't supported)
            i <<= 2;
            for (; i < myBools.Length; totalNumberOfTrueBools += bytePtr[i], i++);
            return totalNumberOfTrueBools;
        }
    }
}
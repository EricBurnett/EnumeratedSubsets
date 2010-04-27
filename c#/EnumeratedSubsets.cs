using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

public class EnumerateSubsets {
    // We choose to memorize calls to Choose(...) as we run them, since the same
    // values will likely be needed in quick succession.
    private static Dictionary<KeyValuePair<Int32, Int32>,BigInteger > chooseResults_ =
            new Dictionary<KeyValuePair<Int32, Int32>, BigInteger>();
    
    // For n > 5000 or so, you should call PrecacheChoose(n, k) for the maximum
    // n and k you expect to need. This method may stack overflow otherwise.
    private static BigInteger Choose(int n, int k) {
        // nCk == nCn-k, so work with the smaller of the two.
        if (k > n-k) {
            k = n-k;
        }
        
        KeyValuePair<Int32, Int32> e =
                new KeyValuePair<Int32, Int32>(n, k);
        
        if (!chooseResults_.ContainsKey(e)) {
            BigInteger result;
            if (k == 0) {
                result = BigInteger.ONE;
            } else {
                result = Choose(n-1, k)+(Choose(n-1, k-1));
            }
            chooseResults_.Add(e, result);
        }
        
        return chooseResults_[e];
    }
    
    // Force cache some values, to prevent stack overflows in Choose(...).
    // Necessary for n > 5000 or so. Use the maximum n and k you will need
    // subsets for.
    private static void PrecacheChoose(int maxN, int maxK) {
        for (int k = 1; k <= maxK; ++k) {
            for (int n = k; n <= maxN; ++n) {
                Choose(n, k);
            }
        }
    }
    
    // Generate the subset indexed by i as a BitSet of length n with exactly k
    // bits set.
    public static BitArray GenerateSubset(int n, int k, BigInteger i) {
        BitArray b = new BitArray(n);
        return GenerateSubset(n, k, i, b, 0);
    }

    public static BitArray GenerateSubset(
            int n, int k, BigInteger i, BitArray b, int offset) {
        BigInteger upperBound = Choose(n, k);
        if (i>=upperBound) {
            // There are only nCk possible subsets to return.
            return null;
        }
        
        int zeros = 0; // Count of zeros on the front before the first 1.
        BigInteger low = BigInteger.ZERO;
        BigInteger high = Choose(n-1, k-1);
        while (i>=high){
            zeros += 1;
            low = high;
            high = high+Choose(n-zeros-1, k-1);
        }
        
        if (zeros + k > n) {
            // Something is wrong! Not enough bits exist.
            throw new Exception("Too many zeros!");
        }
        
        b[offset+zeros] = true;

        if (k == 1) return b;
        else return GenerateSubset(
                n-zeros-1, k-1, i-low, b, offset+zeros+1);
    }

    // Demonstrate the use of this class.
    static void Main(string[] args) {
        // For sets of length 1 through 7, show all the subsets indexed for each
        // subset length (lengths capped at 4, to limit the output a bit).
        for (int l = 1; l < 8; ++l) {
            for (int k = 1; k <= l && k < 5; ++k) {
                BigInteger i = BigInteger.ZERO;
                while (true) {
                    BitArray b = GenerateSubset(l, k, i);
                    if (b == null) break;
                    Console.Out.WriteLine("{0} {1} {2}: {3}",
                                      l, k, i, BitArrayToString(b));
                    i += 1;
                }
            }
        }

        Console.Out.WriteLine();

        {
            // To support big calls (n > ~5000 for a default stack), force precache
            // the choice values with the maximum n and k you will need.
            DateTime start = DateTime.Now;
            PrecacheChoose(10000, 12);
            Console.Out.WriteLine("Precache up to (10000, 12) took: " + (DateTime.Now - start));

            // And now show it working on something bigger.
            BigInteger i = new BigInteger("16000000000000000000", 10);
            start = DateTime.Now;
            BitArray b = GenerateSubset(10000, 12, i);
            Console.Out.WriteLine("Generate took: " + (DateTime.Now - start));
            Console.Out.WriteLine("100000 12 {0}: {1}", i, BitArrayToString(b));
        }
    }

    // Returns a string in the form "{0, 2, 4}" containing the indices of "true"
    // bits in the given BitArray.
    static string BitArrayToString(BitArray b) {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        bool started = false;
        for (int i = 0; i < b.Length; ++i) {
            if (b.Get(i)) {
                if (started) {
                    sb.Append(", ");
                }
                else {
                    started = true;
                }

                sb.Append(i);
            }
        }
        sb.Append("}");
        return sb.ToString();
    }
};
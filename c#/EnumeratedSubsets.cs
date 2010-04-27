using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

public class EnumerateSubsets {
    // We choose to memorize calls to Choose(...) as we run them, since the same
    // values will likely be needed in quick succession.
    private Dictionary<KeyValuePair<Int32, Int32>,BigInteger > chooseResults_ =
            new Dictionary<KeyValuePair<Int32, Int32>, BigInteger>();
    
    // For n > 5000 or so, you should call PrecacheChoose(n, k) for the maximum
    // n and k you expect to need. This method may stack overflow otherwise.
    private BigInteger Choose(int n, int k) {
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
    private void PrecacheChoose(int maxN, int maxK) {
        for (int k = 1; k <= maxK; ++k) {
            for (int n = k; n <= maxN; ++n) {
                Choose(n, k);
            }
        }
    }
    
    // Generate the subset indexed by i as a BitSet of length n with exactly k
    // bits set.
    public BitArray GenerateSubset(int n, int k, BigInteger i) {
        BitArray b = new BitArray(n);
        return GenerateSubset(n, k, i, b, 0);
    }

    public BitArray GenerateSubset(
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
            low = high;
            high = high+Choose(n-zeros-1, k-1);
        }
        
        if (zeros + k > n) {
            // Something is wrong! Not enough bits exist.
            throw new Exception("Too many zeros!");
        }
        
        // b.set(offset+zeros);
        b[offset+zeros]= true;

        if (k == 1) return b;
        else return GenerateSubset(
                n-zeros-1, k-1, i-low, b, offset+zeros+1);
    }
   
};
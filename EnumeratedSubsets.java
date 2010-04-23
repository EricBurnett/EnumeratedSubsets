import java.math.BigInteger;
import java.util.BitSet;
import java.util.HashMap;
import java.util.Map;

public class EnumeratedSubsets {
    // We choose to memoize calls to Choose(...) as we run them, since the same
    // values will likely be needed in quick succession.
    private static Map<Pair<Integer, Integer>, BigInteger> chooseResults_ = 
            new HashMap<Pair<Integer, Integer>, BigInteger>();
    
    // For n > 5000 or so, you should call PrecacheChoose(n, k) for the maximum
    // n and k you expect to need. This method may stack overflow otherwise.
    private static BigInteger Choose(int n, int k) {
        // nCk == nCn-k, so work with the smaller of the two.
        if (k > n-k) {
            k = n-k;
        }
        
        Pair<Integer, Integer> e = 
                new Pair<Integer, Integer>(new Integer(n), new Integer(k));
        
        if (!chooseResults_.containsKey(e)) {
            BigInteger result;
            if (k == 0) {
                result = BigInteger.ONE;
            } else {
                result = Choose(n-1, k).add(Choose(n-1, k-1));
            }
            chooseResults_.put(e, result);
        }
        
        return chooseResults_.get(e);
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
    public static BitSet GenerateSubset(int n, int k, BigInteger i) {
        BitSet b = new BitSet(n);
        return GenerateSubset(n, k, i, b, 0);
    }
    public static BitSet GenerateSubset(
            int n, int k, BigInteger i, BitSet b, int offset) {
        BigInteger upperBound = Choose(n, k);
        if (i.compareTo(upperBound) >= 0) {
            // There are only nCk possible subsets to return.
            return null;
        }
        
        int zeros = 0;  // Count of zeros on the front before the first 1.
        BigInteger low = BigInteger.ZERO;
        BigInteger high = Choose(n-1, k-1);
        while (i.compareTo(high) >= 0) {
            zeros += 1;
            low = high;
            high = high.add(Choose(n-zeros-1, k-1));
        }
        
        if (zeros + k > n) {
            // Something is wrong! Not enough bits exist.
            throw new RuntimeException("Too many zeros!");
        }
        
        b.set(offset+zeros);
        if (k == 1) return b;
        else return GenerateSubset(
                n-zeros-1, k-1, i.subtract(low), b, offset+zeros+1);
    }
    
    // Demonstrate the use of this class.
	public static void main( String[] args ) {
        // For sets of length 1 through 9, show all the subsets indexed for each
        // subset length (lengths capped at 4, to limit the output a bit).
        for (int l = 1; l < 10; ++l) {
            for (int k = 1; k <= l && k < 5; ++k) {
                BigInteger i = BigInteger.ZERO;
                while (true) {
                    BitSet b = GenerateSubset(l, k, i);
                    if (b == null) break;
                    System.out.format("%d %d %s: %s%n", 
                                      l, k, i.toString(), b.toString());
                    i = i.add(BigInteger.ONE);
                }
            }
        }
        
        // Show it works for subsets close to full as well.
        BigInteger i = BigInteger.ZERO;
        while (true) {
            BitSet b = GenerateSubset(25, 24, i);
            if (b == null) break;
            System.out.format("%d %d %s: %s%n", 
                              25, 24, i.toString(), b.toString());
            i = i.add(BigInteger.ONE);
        }
        
        // To support big calls (n > ~5000 for a default stack), force precache
        // the choice values with the maximum n and k you will need.
        PrecacheChoose(10000, 12);
        
        // And now show it working on something...bigger.
        i = new BigInteger("160000");
        BitSet b = GenerateSubset(100, 3, i);
        System.out.format("100 3 %s: %s%n", i.toString(), b.toString());
        
        
        // Bigger!
        i = new BigInteger("160000000");
        b = GenerateSubset(1000, 12, i);
        System.out.format("1000 12 %s: %s%n", i.toString(), b.toString());
        
        
        // Bigger!!!!
        i = new BigInteger("160000000000000000000000000000");
        b = GenerateSubset(10000, 12, i);
        System.out.format("10000 12 %s: %s%n", i.toString(), b.toString());
    }
}
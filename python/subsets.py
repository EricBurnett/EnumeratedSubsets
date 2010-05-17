# Written May 2010 by Josiah Carlson based on Eric Burnett's description here:
# http://www.thelowlyprogrammer.com/2010/04/indexing-and-enumerating-subsets-of.html
# and java code here:
# http://github.com/EricBurnett/EnumeratedSubsets
#
# This source is public domain.

class EnumeratedSubsets(object):
    def __init__(self):
        self.cache = {}
    def choose(self, n, k):
        # handle the simple cases
        k = min(k, n-k)
        if k == 0:
            return 1
        elif k < 0:
            raise Exception("negative k?")
        elif k == 1:
            return n
        # handle caching and subcalls
        c = (n,k)
        if c not in self.cache:
            self.cache[c] = self.choose(n-1, k) + self.choose(n-1, k-1)
        return self.cache[c]

    def precacheChoose(self, n, k=None):
        # adjust your bounds
        if k is None:
            k = (n+1)//2 - 1
        for k_i in xrange(1, k+1):
            for n_i in xrange(k_i, n+1):
                self.choose(n_i, k_i)

    def generateSubset(self, n, k, i):
        # non-recursive generateSubset(n, k, i), which will choose the i'th
        # subset of size k from n items (numbered 0..n-1)
        b = None
        offset = 0
        while 1:
            if b is None:
                b = set()

            upperBound = self.choose(n,k)
            if i >= upperBound:
                return None

            zeros = 0
            low = 0
            high = self.choose(n-1, k-1)
            while i >= high:
                zeros += 1
                low = high
                high += self.choose(n-zeros-1, k-1)

            if (zeros + k) > n:
                raise Exception("Too many zeros!")

            b.add(offset + zeros)
            if k == 1:
                return sorted(b)
            else:
                n -= zeros + 1
                k -= 1
                i -= low
                offset += zeros+1

    def test(self):
        for l in xrange(1, 10):
            for k in xrange(1, min(l, 5)):
                i = 0
                while 1:
                    b = self.generateSubset(l, k, i)
                    if b is None:
                        break
                    print "%i %i %s: %s"%(l, k, i, b)
                    i += 1

        i = 0
        while 1:
            b = self.generateSubset(25, 24, i)
            if b is None:
                break
            print "%i %i %s: %s"%(25, 24, i, b)
            i += 1

        self.precacheChoose(10000, 12)
        i = 160000
        b = self.generateSubset(100, 3, i)
        print "%i %i %s: %s"%(100, 3, i, b)

        i = 160000000000000000000000000000
        b = self.generateSubset(10000, 12, i)
        print "%i %i %s: %s"%(10000, 12, i, b)

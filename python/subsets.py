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
		if (k>n):
			return 0
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
					print("%i %i %s: %s"%(l, k, i, b))
					i += 1

		i = 0
		while 1:
			b = self.generateSubset(25, 24, i)
			if b is None:
				break
			print("%i %i %s: %s"%(25, 24, i, b))
			i += 1

		self.precacheChoose(10000, 12)
		i = 160000
		b = self.generateSubset(100, 3, i)
		print("%i %i %s: %s"%(100, 3, i, b))

		i = 160000000000000000000000000000
		b = self.generateSubset(10000, 12, i)
		print("%i %i %s: %s"%(10000, 12, i, b))

	def invert(self,n,l):
		k=len(l)
		assert(k<=n) #assert subset of {1,...,n}
		assert(k==len(set(l)))#assert unique elements
		assert(max(l)<n)
		l.sort()
		if k==1:
			return l[0]
		elif k==0:
			return 0
		else:
			return self.choose(n,k)-self.choose(n-l[0],k)+self.invert(n-l[0]-1,list(map(lambda x: x-l[0]-1,l[1:])))
	def algebraicInvert(self,n,l):
		k=len(l)
		l.sort()
		return  self.choose(n,k) - self.choose(n-l[0],k) + sum([self.choose(n-l[i-1]-1,k-i) - self.choose(n-l[i],k-i) for i in range(1,k)])


if __name__=='__main__':
	a = EnumeratedSubsets()
	max_n = 15
	for n in range(1,max_n):
		for k in range(1,n+1):
			for i in range(0,a.choose(n,k)):
				l= a.generateSubset(n,k,i)
				assert(a.invert(n,l)==i)
				assert(a.invert(n,l)==a.algebraicInvert(n,l))

				print("generateSubset(" + str(n) + "," + str(k) + "," + str(i) + ") = " + str(l))
				print("invert(" + str(n) + "," + str(l) + ") = " + str(a.invert(n,l)))
				print("algebraicInvert(" + str(n) + "," + str(l) + ") = " + str(a.algebraicInvert(n,l)))
				print()

	else:
		print("Passed all tests")

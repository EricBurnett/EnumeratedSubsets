
$s = new subsets;
print_r( $s->all( 5, 2 ) );

class subsets
{
	var $cache = array();

	// generate all subsets of size $k from a set size $n
	function all( $n, $k )
	{
		$this->precache_choose( $n, $k );
	
		$i = 0;
		$r = array();
		while( ( $result = $this->generate_subset( $n, $k, $i++ ) ) )
			$r[] = $result;
	
		return $r;
	}

	// http://www.thelowlyprogrammer.com/2010/04/indexing-and-enumerating-subsets-of.html
	// O( n lg n )
	
	// not entirely sure how it works, straight port from python
	function choose( $n, $k )
	{	
		$k = min( $k, $n - $k );
		
		if( $k == 0 ) return 1;
		if( $k < 0  ) die( "Negative K?");
		if( $k == 1 ) return $n;
		
		$c = $n.",".$k;
		
		if( !isset( $this->cache[$c] ) )
			$this->cache[$c] = $this->choose( $n - 1, $k ) + $this->choose( $n - 1, $k - 1 );
	
		return $this->cache[$c];
	}
	
	function precache_choose( $n, $k = false )
	{	
		if( $k === false )
			$k = $n + 1;
		
		for( $ki = 1; $ki <= $n+1; $ki++ )
			for( $ni = $ki; $ni <= $n+1; $ni++ )
				$this->choose( $ni, $ki );
	}
	
	function generate_subset( $n, $k, $i )
	{
	
		$b = false;
		$offset = 0;
		
		while( true )
		{
			if( $b === false )
				$b = array();
				
			$upper = $this->choose( $n, $k );
			if( $i >= $upper )
				return false;
				
			$zeros = 0;
			$low = 0;
	
			$high = $this->choose( $n - 1, $k - 1 );
		
			while( $i >= $high )
			{
				$zeros++;
				$low = $high;
				$high += $this->choose( $n - $zeros - 1, $k -1 );
			}
			
			if( ($zeros + $k) > $n )
				die( "Too many zeroes :(" );
				
			$b[] = $offset + $zeros;
			if( $k == 1 )
			{
				sort( $b );
				return $b;
			}
			
			$n -= $zeros + 1;
			$k--;
			$i -= $low;
			$offset += $zeros + 1;
		}
	}
}


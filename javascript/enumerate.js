Graph.Subsets = Class.create();
Graph.Subsets.prototype = {
        initialize: function()
        {
        	this.cache = {}
        },
	list: function( n, k )
	{
		i = 0
		r = []
				
		this.precache_choose( n, k )
		
		while( (result = this.generate_subset( n, k, i++ ) ) )
			if( result.length == k && result[0] > 0 )
				r.push(result)
		
		return r	
	},
	choose: function( n, k )
	{
		k = Math.min( k, n - k );
		
		if( k < 0 ) return alert( "Negative k? Oh dear..." );
		
		if( k == 0 ) return 1;
		if( k == 1 ) return n;
		
		c = n+","+k
		
		if( typeof( this.cache[ c ] ) == "undefined" )
			this.cache[c] = this.choose( n - 1, k ) + this.choose( n - 1, k - 1 )
	
		return this.cache[c]
	},
	precache_choose: function( n, k )
	{
		if( k == null || k == false || k == undefined )
			k = n + 1
		
		for( var ki = 1; ki <= n+1; ki++ )
			for( var ni = ki; ni <= n+1; ni++ )
				this.choose( ni, ki );
	},
	generate_subset: function( n, k, i )
	{
		b = false;
		offset = 0;
		
		while( true )
		{
			if( b == false )
				b = []
			
			upper = this.choose( n, k )
			if( i >= upper )
				return false;
				
			zeros = 0;
			low = 0;
			high = this.choose( n - 1, k - 1 )
			
			while( i >= high )
			{
				zeros++
				low = high
				high += this.choose( n - zeros - 1, k -1 )
			}
			
			if( zeros + k > n )
				return alert( "Too many zeros :(" )
				
			b.push( offset + zeros )
			if( k == 1 )
				return b.sort();

			n -= zeros + 1
			i -= low
			offset += zeros + 1
			k--
		}
	},
	disjoint: function( a, b )
	{
		for( var i = 0; i < a.length; i++ )
			for( var j = 0; j < b.length; j++ )			
				if( a[i] == b[j] )
					return false
		return true;
	}
}

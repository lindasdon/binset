package com.mycompany.myapp;

public class Binset
{
	private final boolean isSet = true;
	public Binset one, two;
	
	public Binset() { one = null; two = null; }
	public Binset(int i){
		if(i <= 0)
		{ one = null; two = null; }
		else if(i == 0)
		{ one = new Binset();
		  two = new Binset();
		}
		else
		{ one = new Binset((i + 1)/2);
		  two = new Binset(i/2);			
		}
	}
	public Binset(Binset b)
	{
		if(b.isEmpty())
		{ one = null; two = null; }
		else if(b.isOne())
		{ one = new Binset();
		  two = new Binset(); }
		else
		{ one = new Binset(b.one);
		  two = new Binset(b.two); }
	}
	public Binset(Binset a, Binset b){
		if(b.isEmpty())
		{
			if(a.isEmpty())
			{one = null; two = null; }
			else if(a.isOne())
			{ one = new Binset();
			  two = new Binset(); }
			else
			{ one = new Binset(a.one);
			  two = new Binset(a.two); }
		}
		else{
			one = new Binset(a);
			two = new Binset(b);
		}
	}
	public boolean isEmpty()
	{ return (one == null)
	  && (two == null); }
	public boolean isOne(){
		return !isEmpty() &&
		one.isEmpty() && two.isEmpty();
	}
	public int Count(){
		if(isEmpty()) return 0;
		if(isOne()) return 1;
		return one.Count() +
		two.Count();
	}
	public boolean setEquals(Binset b){
		if(this == b) return true;
		if(isEmpty()) return b.isEmpty();
		if(b.isEmpty()) return false;
		return one.setEquals(b.one) &&
		two.setEquals(b.two);
	}
	public Binset andWith(Binset b){
		if(isEmpty() || b.isEmpty())
			return new Binset();
		if(isOne())
		{
			if(b.isOne())
				return new Binset(1);
			return andWith(b.one);
		}
		if(b.isOne())
			return one.andWith(b);
		return new Binset(
			one.andWith(b.one),
			two.andWith(b.two)
		);
	}
	public boolean Contains(Binset b)
	{
		return andWith(b).setEquals(b);
	}
	public Binset excWith(Binset b){
		if(isEmpty() || b.isEmpty())
			return new Binset(this);
		if(b.Contains(this))
			return new Binset();
		if(isOne())
			return new Binset(1);
		if(b.isOne())
			return new Binset(
				one.excWith(b),
				two
			);
		return new Binset(
			one.excWith(b.one),
			two.excWith(b.two)
		);
	}
	public Binset xorWith(Binset b)
	{
		if(isEmpty())
			return new Binset(b);
		if(b.isEmpty())
			return new Binset(this);
		if(isOne())
		{
			if(b.isOne())
				return new Binset();
			return new Binset(xorWith(b.one), b.two);
		}
		if(b.isOne())
			return new Binset(one.xorWith(b), two);
		return new Binset(
			one.xorWith(b.one),
			two.xorWith(b.two)
		);
	}
	public Binset orWith(Binset b){
		if(isEmpty())
			return new Binset(b);
		if(b.isEmpty())
			return new Binset(this);
		if(isOne())
		{
			if(b.isOne())
				return new Binset(1);
			return new Binset(
				orWith(b.one),
				b.two
			);
		}
		if(b.isOne())
	 		return new Binset(
				one.orWith(b),
				two
			);
		return new Binset(
			one.orWith(b.one),
			two.orWith(b.two)
		);
	}
	public Binset plus(Binset b){
		if(isEmpty())
			return new Binset(b);
		if(b.isEmpty())
			return new Binset(this);
		if(isOne())
		{
			if(b.isOne())
				return new Binset(2);
			return new Binset(
				plus(b.one),
				b.two
			);
		}
		if(b.isOne())
			return new Binset(
				one.plus(b),
				two
			);
		return new Binset(
			one.plus(b.one),
			two.plus(b.two)
		);
	}
	public Binset times(Binset b){
		if(isEmpty() || b.isEmpty())
			return new Binset();
		if(isOne())
			return new Binset(b);
		if(b.isOne())
			return new Binset(this);
		return new Binset(
			one.times(b),
			two.times(b)
		);
	}
}

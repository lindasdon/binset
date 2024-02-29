using System;
using System.Collections;
using System.Collections.Generic;

namespace binset
{
	public class binset : ICloneable, IComparable<binset>, ISet<binset>
	{
		readonly bool nil = true;
		public binset one, two;
		public binset()
		{ one = null; two = null; }
		public void Clear()
		{ one = null; two = null; }
		public bool IsReadOnly
		{ get{ return false; } }
		public binset(int i)
		{
			if(i <= 0)
			{ one = null; two = null; }
			else if(i == 1)
			{ one = new binset();
			  two = new binset(); }
			else
			{ one = new binset((i + 1)/2);
			  two = new binset(i/2); }
		}
		public binset(binset b)
		{
			if(b.isEmpty)
			{ one = null; two = null; }
			else if(b.isOne)
			{ one = new binset();
			  two = new binset(); }
			else
			{ one = new binset(b.one);
			  two = new binset(b.two); }
		}
		public virtual object Clone()
		{
			return new binset(this);
		}
		public binset(binset a, binset b)
		{
			if(b.isEmpty)
			{
				if(a.isEmpty)
				{ one = null; two = null; }
				else if(a.isOne)
				{ one = new binset();
				  two = new binset(); }
				else
				{ one = new binset(a.one);
				  two = new binset(a.two); }
			}
			else{
				one = new binset(a);
				two = new binset(b);
			}
		}
		public virtual bool isEmpty{
			get{ return !(one is binset)
			&& !(two is binset); }
		}
		public virtual bool isOne{
			get{ return !isEmpty &&
			one.isEmpty && two.isEmpty; }
		}
		public virtual int Count{
			get{ if(isEmpty) return 0;
				if(isOne) return 1;
				return one.Count + two.Count;
			}
		}
		public override bool Equals(object o)
		{
			return (o is binset) && 
			o.GetType().Equals(GetType()) &&
			this == (binset)o;
		}
		public override int GetHashCode()
		{ return Hash32(5); }
		int Hash32(int power)
		{
			if(isEmpty) return 0;
			if(isOne || (power == 0))
				return 1;
			return (one.Hash32(power - 1) << power)
				+ two.Hash32(power - 1); 
		}
		public virtual bool SetEquals(binset b)
		{
			return Equals(b);
		}
		public binset UnionOf(IEnumerable<binset> b)
		{
			binset ret = new binset();
			foreach(binset bs in b)
				ret |= bs;
			return ret;
		}
		public bool SetEquals(IEnumerable<binset> b)
		{ return SetEquals(UnionOf(b)); }
		public int CompareTo(binset b)
		{
			if(isEmpty)
			{
				if(b.isEmpty) return 0;
				return -1;
			}
			if(b.isEmpty) return 1;
			if(isOne)
			{
				if(b.isOne) return 0;
				return -1;
			}
			if(b.isOne) return 1;
			int cmp = one.CompareTo(b.one);
			if(cmp != 0) return cmp;
			return two.CompareTo(b.two);
		}
		public static bool operator <(
		binset a, binset b)
		{ return a.CompareTo(b) < 0; }
		public static bool operator >(
		binset a, binset b)
		{ return a.CompareTo(b) > 0; }
		public static bool operator ==(
		binset a, binset b)
		{ return a.CompareTo(b) == 0; }
		public static bool operator !=(
		binset a, binset b)
		{ return a.CompareTo(b) != 0; }
		public static binset operator &(
		binset a, binset b)
		{
			if(a.isEmpty || b.isEmpty)
				return new binset();
			if(a.isOne)
			{
				if(b.isOne)
					return new binset(1);
				return a & b.one;
			}
			if(b.isOne)
				return a.one & b;
			return new binset(a.one & b.one,
			a.two & b.two);
		}
		public bool Contains(binset b)
		{ return (this & b).SetEquals(b); }
		public void IntersectWith(IEnumerable<binset> b)
		{
			binset ret = this & UnionOf(b);
			one = ret.one;
			two = ret.two;
		}
		public bool IsSupersetOf(IEnumerable<binset> b)
		{ return Contains(UnionOf(b)); }
		public bool IsSubsetOf(IEnumerable<binset> b)
		{ return UnionOf(b).Contains(this); }
		public bool IsProperSupersetOf(IEnumerable<binset> b)
		{ binset tmp = UnionOf(b);
		  return Contains(tmp) && !SetEquals(tmp); }
		public bool IsProperSubsetOf(IEnumerable<binset> b)
		{ binset tmp = UnionOf(b);
		  return tmp.Contains(this) && !SetEquals(tmp); }
		public bool Overlaps(IEnumerable<binset> b)
		{
			foreach(binset bs in b)
				if(Contains(bs)) return true;
			return false;
		}
		public static binset operator -(
		binset a, binset b)
		{
			if(a.isEmpty || b.isEmpty)
				return new binset(a);
			if(b.Contains(a))
				return new binset();
			if(a.isOne)
				return new binset(1);
			if(b.isOne)
				return new binset(
				a.one - b, a.two
				);
			return new binset(
				a.one - b.one,
				a.two - b.two
			);
		}
		public void ExceptWith(IEnumerable<binset> b)
		{
			binset ret = this - UnionOf(b);
			one = ret.one;
			two = ret.two;
		}
		public bool Remove(binset b)
		{
			binset ret = this - b;
			if(SetEquals(ret)) return false;
			one = ret.one;
			two = ret.two;
			return true;
		}
		public static binset operator ^(
		binset a, binset b)
		{
			if(a.isEmpty)
				return new binset(b);
			if(b.isEmpty)
				return new binset(a);
			if(a.isOne)
			{
				if(b.isOne)
					return new binset();
				return new binset(
					a ^ b.one,
					b.two
				);
			}
			if(b.isOne)
				return new binset(
					a.one ^ b,
					a.two
				);
			return new binset(
				a.one ^ b.one,
				a.two ^ b.two
			);
		}
		public void SymmetricExceptWith(IEnumerable<binset> b)
		{ binset ret = this ^ UnionOf(b);
		  one = ret.one;
		  two = ret.two;
		}
		public static binset operator |(
		binset a, binset b)
		{
			if(a.isEmpty)
				return new binset(b);
			if(b.isEmpty)
				return new binset(a);
			if(a.isOne)
			{
				if(b.isOne)
					return new binset(1);
				return new binset(
					a | b.one,
					b.two
				);
			}
			if(b.isOne)
				return new binset(
					a.one | b,
					a.two
				);
			return new binset(
				a.one | b.one,
				a.two | b.two
			);
		}
		public void UnionWith(IEnumerable<binset> b)
		{
			binset ret = this | UnionOf(b);
			one = ret.one;
			two = ret.two;
		}
		public bool Add(binset b)
		{
			binset ret = this | b;
			if(SetEquals(ret)) return false;
			one = ret.one;
			two = ret.two;
			return true;
		}
		void ICollection<binset>.Add(binset b)
		{ Add(b); }
		public static binset operator +(
		binset a, binset b)
		{
			if(a.isEmpty)
				return new binset(b);
			if(b.isEmpty)
				return new binset(a);
			if(a.isOne)
			{
				if(b.isOne)
					return new binset(2);
				return new binset(
					a + b.one,
					b.two
				);
			}
			if(b.isOne)
				return new binset(
					a.one + b,
					a.two
				);
			return new binset(
				a.one + b.one,
				a.two + b.two
			);
		}
		public static binset operator *(
		binset a, binset b)
		{
			if(a.isEmpty || b.isEmpty)
				return new binset();
			if(a.isOne)
				return new binset(b);
			if(b.isOne)
				return new binset(a);
			return new binset(
				a.one * b,
				a.two * b
			);
		}
		void ICollection<binset>.CopyTo(binset[] arr, int start)
		{
			int ix = start;
			foreach(binset bs in this)
				arr[ix++] = bs;	
		}
		public IEnumerator<binset> GetEnumerator()
		{ return new BSEnum(this); }
		IEnumerator IEnumerable.GetEnumerator()
		{ return GetEnumerator(); }
	}
	public class BSEnum : IEnumerator<binset>
	{
		binset bs, current;
		IEnumerator<binset> oneEnum, twoEnum;
		bool started,finished,oneisempty,
			twostarted;

		public BSEnum(binset input)
		{
			bs = input;
			current = null;
			oneEnum = null; twoEnum = null;
			started = finished = oneisempty
			= twostarted = false;
		}
		public void Reset()
		{
			current = null;
			oneEnum = null; twoEnum = null;
			started = finished = oneisempty
			= twostarted = false;
		}
		void IDisposable.Dispose() { }
		public binset Current
		{ get{ return current; } }
		object IEnumerator.Current
		{ get{ return Current; } }
		public bool MoveNext()
		{
			if(finished)
			{ current = null;
			  return false; }
			if(!started)
			{
				started = true;
				if(bs.isEmpty)
				{ current = null;
				  finished = true;
				  return false; }
				if(bs.isOne)
				{ current = bs;
				  finished = true;
				  return true; }
				if(bs.one.isEmpty)
					oneisempty = true;
				else
					oneEnum = new BSEnum(bs.one);
				twoEnum = new BSEnum(bs.two);
			}
			if(!twostarted)
			{
				if(!oneisempty)
				{
					if(oneEnum.MoveNext())
					{ current = oneEnum.Current;
					  return true; }
					twostarted = true;
				}
				else if(twoEnum.MoveNext())
				{
					current = new binset(new binset(),
						twoEnum.Current);
					return true;
				}
				else
				{ current = null;
				  finished = true;
				  return false; }
			}
			if(oneisempty)
			{
				if(twoEnum.MoveNext())
				{ current = new binset(
				    new binset(),
				    twoEnum.Current
				  );
				  return true;
				}
				else
				{ current = null;
				  finished = true;
				  return false; }
			}
			if(oneEnum.MoveNext())
			{
				if(!(twoEnum.Current is binset))
					twoEnum.MoveNext();
				current = new binset(
					oneEnum.Current,
					twoEnum.Current
				);
				return true;
			}
			if(!twoEnum.MoveNext())
			{ current = null;
			  finished = true;
			  return false; }
			oneEnum.Reset();
			oneEnum.MoveNext();
			current = new binset(
				oneEnum.Current,
				twoEnum.Current
			);
			return true;
		}
	}
	public class binarray<T> : binset, ICloneable, IEnumerable<T>
	where T : binset, new()
	{
		public int size;
		public binarray() : base() { size = 2; }
		public override object Clone()
		{
			binarray<T> ret = new binarray<T>(size);
			if(isEmpty)
			{ ret.one = null; ret.two = null; return ret; }
			if(one is binarray<T>)
				ret.one = (binarray<T>)(((binarray<T>)one).Clone());
			else if(one is T)
				ret.one = (T)(((T)one).Clone());
			else ret.one = new binset();
			if(two is binarray<T>)
				ret.two = (binarray<T>)(((binarray<T>)two).Clone());
			else if(two is T)
				ret.two = (T)(((T)two).Clone());
			else ret.two = new binset();
			return ret;
		}
		public binarray(int sz)
		: base() { size = (sz < 2) ? 2 : sz; }
		public T this[int ix]{
			get{
				if((ix < 0) || (ix >= size))
					throw new IndexOutOfRangeException();
				if(size == 2)
				{
					if(ix == 0)
						return (one is T) ? (T)one : new T();
					return (two is T) ? (T)two : new T();
				}
				if((size == 3) && (ix == 2))
					return (two is T) ? (T)two : new T();
				if(ix < (size + 1)/2)
				{
					if(one is binarray<T>)
						return ((binarray<T>)one)[ix];
					return new T();
				}
				if(two is binarray<T>)
					return ((binarray<T>)two)[ix - (size + 1)/2];
				return new T();
			}
			set{
				if((ix < 0) || (ix >= size))
					throw new IndexOutOfRangeException();
				if(size == 2)
				{
					if(ix == 0)
					{
						one = value;
						if(!(two is binset))
							two = new binset();
					}						
					else
					{
						two = value;
						if(!(one is binset))
							one = new binset();
					}
					return;
				}
				if((size == 3) && (ix == 2))
				{
					two = value;
					if(!(one is binset))
						one = new binset();					
					return;
				}
				if(ix < (size + 1)/2)
				{
					if(!(one is binarray<T>))
						one = new binarray<T>((size + 1)/2);
					((binarray<T>)one)[ix] = value;
					if(!(two is binset))
						two = new binset();
				}
				else
				{
					if(!(two is binarray<T>))
						two = new binarray<T>(size/2);
					((binarray<T>)two)[ix - (size + 1)/2] = value;
					if(!(one is binset))
						one = new binset();
				}
			}
		}
		public override int Count{
			get{
				if(isEmpty || isOne)
					return 0;
				int ct = 0;
				if(one is binarray<T>)
					ct += ((binarray<T>)one).Count;
				else if (!one.isEmpty && (one is T))
					ct++;
				if(two is binarray<T>)
					ct += ((binarray<T>)two).Count;
				else if (!two.isEmpty && (two is T))
					ct++;
				return ct;
			}
		}
		public new IEnumerator<T> GetEnumerator()
		{ return new BAEnum<T>(this); }
		IEnumerator IEnumerable.GetEnumerator()
		{ return GetEnumerator(); }
	}
	public class BAEnum<T> : IEnumerator<T>
	where T : binset, new()
	{
		binarray<T> ba;
		T current;
		IEnumerator<T> tmpEnum;
		bool started,finished,
		onefinished,twostarted;

		public BAEnum(binarray<T> input)
		{
			ba = input;
			current = null;
			tmpEnum = null;
			started = finished = onefinished
			= twostarted = false;
		}
		public void Reset()
		{
			current = null;
			tmpEnum = null;
			started = finished = onefinished
			= twostarted = false;			
		}
		void IDisposable.Dispose() { }
		public T Current
		{ get{ return current; } }
		object IEnumerator.Current
		{ get{ return Current; } }
		public bool MoveNext()
		{
			if(finished)
			{ current = null;
			  return false; }
			if(!started)
			{
				started = true;
				if(ba.isEmpty)
				{ current = null;
				  finished = true;
				  return false; }
				if(ba.one is binarray<T>)
					tmpEnum = new BAEnum<T>(
					(binarray<T>)(ba.one));
				else if(ba.one is T)
				{ current = (T)(ba.one); 
				  onefinished = true;
				  return true; }
				else
					onefinished = true;
			}
			if(onefinished && !twostarted)
			{
				twostarted = true;
				if(ba.two is binarray<T>)
					tmpEnum = new BAEnum<T>((binarray<T>)(ba.two));
				else if(ba.two is T)
				{ current = (T)(ba.two);
				  finished = true;
				  return true; }
				else
				{ current = null;
				  finished = true;
				  return false; }
			}
			if(tmpEnum.MoveNext())
			{ current = tmpEnum.Current;
			  return true; }
			if(!twostarted)
			{
				twostarted = true;
				if(ba.two is binarray<T>)
				{ tmpEnum = new BAEnum<T>((binarray<T>)(ba.two));
				  if(tmpEnum.MoveNext())
				  { current = tmpEnum.Current;
				    return true; }
				}
				else if(ba.two is T)
				{ current = (T)(ba.two);
				  finished = true;
				  return true; }
			}
			current = null;
			finished = true;
			return false;
		}
	}
}					

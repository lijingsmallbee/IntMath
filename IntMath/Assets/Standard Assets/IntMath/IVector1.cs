using System;

[Serializable]
public struct IVector1
{
	public int i;

	public float scalar
	{
		get
		{
			return (float)this.i * 0.001f;
		}
	}

	public IVector1(int i)
	{
		this.i = i;
	}

	public IVector1(float f)
	{
		this.i = (int)Math.Round((double)(f * 1000f));
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		IVector1 vInt = (IVector1)o;
		return this.i == vInt.i;
	}

	public override int GetHashCode()
	{
		return this.i.GetHashCode();
	}

	public static IVector1 Min(IVector1 a, IVector1 b)
	{
		return new IVector1(Math.Min(a.i, b.i));
	}

	public static IVector1 Max(IVector1 a, IVector1 b)
	{
		return new IVector1(Math.Max(a.i, b.i));
	}

	public override string ToString()
	{
		return this.scalar.ToString();
	}

	public static explicit operator IVector1(float f)
	{
		//here must use double,float will not cover the biggest int
		return new IVector1((int)Math.Round((double)(f * 1000f)));
	}

	public static implicit operator IVector1(int i)
	{
		return new IVector1(i);
	}

	public static explicit operator float(IVector1 ob)
	{
		return (float)ob.i * 0.001f;
	}

	public static explicit operator long(IVector1 ob)
	{
		return (long)ob.i;
	}

	public static IVector1 operator +(IVector1 a, IVector1 b)
	{
		return new IVector1(a.i + b.i);
	}

	public static IVector1 operator -(IVector1 a, IVector1 b)
	{
		return new IVector1(a.i - b.i);
	}

	public static bool operator ==(IVector1 a, IVector1 b)
	{
		return a.i == b.i;
	}

	public static bool operator !=(IVector1 a, IVector1 b)
	{
		return a.i != b.i;
	}
}

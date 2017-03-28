using System;
using UnityEngine;

public class IntMath
{
	public static IntFactor atan2(int y, int x)
	{
		int num;
		int num2;
		if (x < 0)
		{
			if (y < 0)
			{
                //第三象限
				x = -x;
				y = -y;
				num = 1;
			}
			else
			{
                //第二象限
				x = -x;
				num = -1;
			}
            //-PI 乘以10000
			num2 = -31416;
		}
		else
		{
			if (y < 0)
			{
                //第四象限
				y = -y;
				num = -1;
			}
			else
			{
                //第一象限
				num = 1;
			}
			num2 = 0;
		}
		int dIM = IntAtan2Table.DIM;   //2^7 = 128
		long num3 = (long)(dIM - 1);  //127
        //下边这段的意思是，把xy归一化后映射到0-127闭区间上，然后去查表
        //y做行，x做列去查询设置好的二维表
		long b = (long)((x >= y) ? x : y);
		int num4 = (int)IntMath.Divide((long)x * num3, b);
		int num5 = (int)IntMath.Divide((long)y * num3, b);
		int num6 = IntAtan2Table.table[num5 * dIM + num4];
		return new IntFactor
		{
            //num2的意思是，不同象限通过第一象限的值进行反推的计算公式不一样
            //如果想要得到以x轴为0度，向上为0~180，向下是 0~-180度的结果
            //这个跟几何书上1-4象限是 0-360度的表示方法不太一样
            //第一象限假设结果是angle，代表从x轴转向该向量的角度是angle，num
            //第二象限，绝对值相同的tan值对应的角度是 PI-angle，num2 = -PI，num = -1
            //第三象限，绝对值相同的tan值对应的角度是 -PI+angle，num2 = -PI，num = 1
            //第四象限，绝对值相同的tan值对应的角度是 -angle， num2 = 0，num = -1
			numerator = (long)((num6 + num2) * num),
			denominator = 10000L
		};
	}

	public static IntFactor acos(long nom, long den)
	{
        //计算acos就比较简单了，因为cos的取值就是-1~1，不存在无穷的问题
        //如果把cos比作x/length,当length等于1的时候，x就是cos值，所以只需要把-1~1平均分成IntAcosTable.COUNT份
        //然后做一个-1~1与0~IntAcosTable.COUNT的映射即可，该函数的功能就是做这个映射
        //由于cos函数与角度不是线性的，即两者的变化率不一样，所以多少有点值分配不均匀的问题，不过分成1024份，影响不大
        int num = (int)IntMath.Divide(nom * (long)IntAcosTable.HALF_COUNT, den) + IntAcosTable.HALF_COUNT;
		num = Mathf.Clamp(num, 0, IntAcosTable.COUNT);
		return new IntFactor
		{
			numerator = (long)IntAcosTable.table[num],
			denominator = 10000L
		};
	}
     

    public static IntFactor sin(long nom, long den)
	{
        //索引值求的原理见对应函数内注释 
		int index = IntSinCosTable.getIndex(nom, den);
		return new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static IntFactor cos(long nom, long den)
	{
        //索引值求的原理见对应函数内注释 
        int index = IntSinCosTable.getIndex(nom, den);
		return new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static void sincos(out IntFactor s, out IntFactor c, long nom, long den)
	{
		int index = IntSinCosTable.getIndex(nom, den);
		s = new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
		c = new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static void sincos(out IntFactor s, out IntFactor c, IntFactor angle)
	{
		int index = IntSinCosTable.getIndex(angle.numerator, angle.denominator);
		s = new IntFactor((long)IntSinCosTable.sin_table[index], (long)IntSinCosTable.FACTOR);
		c = new IntFactor((long)IntSinCosTable.cos_table[index], (long)IntSinCosTable.FACTOR);
	}

	public static long Divide(long a, long b)
	{
        //这个表示看不懂
		long num = (long)((ulong)((a ^ b) & -9223372036854775808L) >> 63);
		long num2 = num * -2L + 1L;
		return (a + b / 2L * num2) / b;
	}

	public static int Divide(int a, int b)
	{
        //得到符号
		int num = (int)((uint)((a ^ b) & -2147483648) >> 31);
		int num2 = num * -2 + 1;
		return (a + b / 2 * num2) / b;
	}

	public static IVector3 Divide(IVector3 a, long m, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x * m, b);
		a.y = (int)IntMath.Divide((long)a.y * m, b);
		a.z = (int)IntMath.Divide((long)a.z * m, b);
		return a;
	}

	public static IVector2 Divide(IVector2 a, long m, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x * m, b);
		a.y = (int)IntMath.Divide((long)a.y * m, b);
		return a;
	}

	public static IVector3 Divide(IVector3 a, int b)
	{
		a.x = IntMath.Divide(a.x, b);
		a.y = IntMath.Divide(a.y, b);
		a.z = IntMath.Divide(a.z, b);
		return a;
	}

	public static IVector3 Divide(IVector3 a, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x, b);
		a.y = (int)IntMath.Divide((long)a.y, b);
		a.z = (int)IntMath.Divide((long)a.z, b);
		return a;
	}

	public static IVector2 Divide(IVector2 a, long b)
	{
		a.x = (int)IntMath.Divide((long)a.x, b);
		a.y = (int)IntMath.Divide((long)a.y, b);
		return a;
	}

	public static uint Sqrt32(uint a)
	{
        //经典的逐位确认法
		uint num = 0u;
		uint num2 = 0u;
		for (int i = 0; i < 16; i++)
		{
			num2 <<= 1;
			num <<= 2;
			num += a >> 30;
			a <<= 2;
			if (num2 < num)
			{
				num2 += 1u;
				num -= num2;
				num2 += 1u;
			}
		}
		return num2 >> 1 & 65535u;
	}

	public static ulong Sqrt64(ulong a)
	{
        //经典的逐位确认法
        ulong num = 0uL;
		ulong num2 = 0uL;
		for (int i = 0; i < 32; i++)
		{
			num2 <<= 1;
			num <<= 2;
			num += a >> 62;
			a <<= 2;
			if (num2 < num)
			{
				num2 += 1uL;
				num -= num2;
				num2 += 1uL;
			}
		}
		return num2 >> 1 & unchecked((ulong)-1);
	}

	public static long SqrtLong(long a)
	{
		if (a <= 0L)
		{
			return 0L;
		}
		if (a <= unchecked((long)(unchecked((ulong)-1))))
		{
			return (long)((ulong)IntMath.Sqrt32((uint)a));
		}
		return (long)IntMath.Sqrt64((ulong)a);
	}

	public static int Sqrt(long a)
	{
		if (a <= 0L)
		{
			return 0;
		}
		if (a <= unchecked((long)(unchecked((ulong)-1))))
		{
			return (int)IntMath.Sqrt32((uint)a);
		}
		return (int)IntMath.Sqrt64((ulong)a);
	}

	public static long Clamp(long a, long min, long max)
	{
		if (a < min)
		{
			return min;
		}
		if (a > max)
		{
			return max;
		}
		return a;
	}

	public static long Max(long a, long b)
	{
		return (a <= b) ? b : a;
	}

	public static IVector3 Transform(ref IVector3 point, ref IVector3 axis_x, ref IVector3 axis_y, ref IVector3 axis_z, ref IVector3 trans)
	{
        //就是一个3*3矩阵变换一个方向的计算公式 
		return new IVector3(IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x, IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y, IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z);
	}

	public static IVector3 Transform(IVector3 point, ref IVector3 axis_x, ref IVector3 axis_y, ref IVector3 axis_z, ref IVector3 trans)
	{
		return new IVector3(IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x, IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y, IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z);
	}

	public static IVector3 Transform(ref IVector3 point, ref IVector3 axis_x, ref IVector3 axis_y, ref IVector3 axis_z, ref IVector3 trans, ref IVector3 scale)
	{
		long num = (long)point.x * (long)scale.x;
		long num2 = (long)point.y * (long)scale.x;
		long num3 = (long)point.z * (long)scale.x;
		return new IVector3((int)IntMath.Divide((long)axis_x.x * num + (long)axis_y.x * num2 + (long)axis_z.x * num3, 1000000L) + trans.x, (int)IntMath.Divide((long)axis_x.y * num + (long)axis_y.y * num2 + (long)axis_z.y * num3, 1000000L) + trans.y, (int)IntMath.Divide((long)axis_x.z * num + (long)axis_y.z * num2 + (long)axis_z.z * num3, 1000000L) + trans.z);
	}

	public static IVector3 Transform(ref IVector3 point, ref IVector3 forward, ref IVector3 trans)
	{
		IVector3 up = IVector3.up;
		IVector3 vInt = IVector3.Cross(IVector3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
	}

	public static IVector3 Transform(IVector3 point, IVector3 forward, IVector3 trans)
	{
		IVector3 up = IVector3.up;
		IVector3 vInt = IVector3.Cross(IVector3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans);
	}

	public static IVector3 Transform(IVector3 point, IVector3 forward, IVector3 trans, IVector3 scale)
	{
		IVector3 up = IVector3.up;
		IVector3 vInt = IVector3.Cross(IVector3.up, forward);
		return IntMath.Transform(ref point, ref vInt, ref up, ref forward, ref trans, ref scale);
	}

	public static int Lerp(int src, int dest, int nom, int den)
	{
		return IntMath.Divide(src * den + (dest - src) * nom, den);
	}

	public static long Lerp(long src, long dest, long nom, long den)
	{
		return IntMath.Divide(src * den + (dest - src) * nom, den);
	}

	public static bool IsPowerOfTwo(int x)
	{
		return (x & x - 1) == 0;
	}

	public static int CeilPowerOfTwo(int x)
	{
		x--;
		x |= x >> 1;
		x |= x >> 2;
		x |= x >> 4;
		x |= x >> 8;
		x |= x >> 16;
		x++;
		return x;
	}

	public static void SegvecToLinegen(ref IVector2 segSrc, ref IVector2 segVec, out long a, out long b, out long c)
	{
		a = (long)segVec.y;
		b = (long)(-(long)segVec.x);
		c = (long)segVec.x * (long)segSrc.y - (long)segSrc.x * (long)segVec.y;
	}

	private static bool IsPointOnSegment(ref IVector2 segSrc, ref IVector2 segVec, long x, long y)
	{
		long num = x - (long)segSrc.x;
		long num2 = y - (long)segSrc.y;
		return (long)segVec.x * num + (long)segVec.y * num2 >= 0L && num * num + num2 * num2 <= segVec.sqrMagnitudeLong;
	}

	public static bool IntersectSegment(ref IVector2 seg1Src, ref IVector2 seg1Vec, ref IVector2 seg2Src, ref IVector2 seg2Vec, out IVector2 interPoint)
	{
		long num;
		long num2;
		long num3;
		IntMath.SegvecToLinegen(ref seg1Src, ref seg1Vec, out num, out num2, out num3);
		long num4;
		long num5;
		long num6;
		IntMath.SegvecToLinegen(ref seg2Src, ref seg2Vec, out num4, out num5, out num6);
		long num7 = num * num5 - num4 * num2;
		if (num7 != 0L)
		{
			long num8 = IntMath.Divide(num2 * num6 - num5 * num3, num7);
			long num9 = IntMath.Divide(num4 * num3 - num * num6, num7);
			bool result = IntMath.IsPointOnSegment(ref seg1Src, ref seg1Vec, num8, num9) && IntMath.IsPointOnSegment(ref seg2Src, ref seg2Vec, num8, num9);
			interPoint.x = (int)num8;
			interPoint.y = (int)num9;
			return result;
		}
		interPoint = IVector2.zero;
		return false;
	}
    //射线检测法，一条经过点p平行于x轴的射线与每条线段交点个数，偶数为在外侧，奇数在内测
	public static bool PointInPolygon(ref IVector2 pnt, IVector2[] plg)
	{
		if (plg == null || plg.Length < 3)
		{
			return false;
		}
		bool flag = false;
		int i = 0;
		int num = plg.Length - 1;
		while (i < plg.Length)
		{
			IVector2 vInt = plg[i];
			IVector2 vIVector2 = plg[num];
			if ((vInt.y <= pnt.y && pnt.y < vIVector2.y) || (vIVector2.y <= pnt.y && pnt.y < vInt.y))
			{
				int num2 = vIVector2.y - vInt.y;
				long num3 = (long)(pnt.y - vInt.y) * (long)(vIVector2.x - vInt.x) - (long)(pnt.x - vInt.x) * (long)num2;
				if (num2 > 0)
				{
					if (num3 > 0L)
					{
						flag = !flag;
					}
				}
				else if (num3 < 0L)
				{
					flag = !flag;
				}
			}
			num = i++;
		}
		return flag;
	}

	public static bool SegIntersectPlg(ref IVector2 segSrc, ref IVector2 segVec, IVector2[] plg, out IVector2 nearPoint, out IVector2 projectVec)
	{
		nearPoint = IVector2.zero;
		projectVec = IVector2.zero;
		if (plg == null || plg.Length < 2)
		{
			return false;
		}
		bool result = false;
		long num = -1L;
		int num2 = -1;
		for (int i = 0; i < plg.Length; i++)
		{
			IVector2 vInt = plg[(i + 1) % plg.Length] - plg[i];
			IVector2 vIVector2;
			if (IntMath.IntersectSegment(ref segSrc, ref segVec, ref plg[i], ref vInt, out vIVector2))
			{
				long sqrMagnitudeLong = (vIVector2 - segSrc).sqrMagnitudeLong;
				if (num < 0L || sqrMagnitudeLong < num)
				{
					nearPoint = vIVector2;
					num = sqrMagnitudeLong;
					num2 = i;
					result = true;
				}
			}
		}
		if (num2 >= 0)
		{
			IVector2 lhs = plg[(num2 + 1) % plg.Length] - plg[num2];
			IVector2 vIVector3 = segSrc + segVec - nearPoint;
			long num3 = (long)vIVector3.x * (long)lhs.x + (long)vIVector3.y * (long)lhs.y;
			if (num3 < 0L)
			{
				num3 = -num3;
				lhs = -lhs;
			}
			long sqrMagnitudeLong2 = lhs.sqrMagnitudeLong;
			projectVec.x = (int)IntMath.Divide((long)lhs.x * num3, sqrMagnitudeLong2);
			projectVec.y = (int)IntMath.Divide((long)lhs.y * num3, sqrMagnitudeLong2);
		}
		return result;
	}
}

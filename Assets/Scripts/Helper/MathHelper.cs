using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////
	
public enum Direction4
{
	MinX,
	MaxX,
	MinZ,
	MaxZ
}

///////////////////////////////////////////////////////////////////////////
	
public enum Direction8
{
	MinX,
	MaxX,
	MinZ,
	MaxZ,

	MinXMinZ,
	MinXMaxZ,
	MaxXMinZ,
	MaxXMaxZ,
}

///////////////////////////////////////////////////////////////////////////

public static class MathHelper 
{
	///////////////////////////////////////////////////////////////////////////

	public static Vector2 DirectionToOffset(Direction4 direction)
	{
		switch (direction)
		{
			case Direction4.MinX:	return new Vector2(-1, 0);
			case Direction4.MaxX:	return new Vector2( 1, 0);
			case Direction4.MinZ:	return new Vector2( 0,-1);
			case Direction4.MaxZ:	return new Vector2( 0, 1);
		}

		return new Vector2(0,0);
	}

	///////////////////////////////////////////////////////////////////////////

	public static Vector2 DirectionToOffset(Direction8 direction)
	{
		switch (direction)
		{
			case Direction8.MinX:		return new Vector2(-1, 0);
			case Direction8.MaxX:		return new Vector2( 1, 0);
			case Direction8.MinZ:		return new Vector2( 0,-1);
			case Direction8.MaxZ:		return new Vector2( 0, 1);

			case Direction8.MinXMinZ:	return new Vector2(-1, -1);
			case Direction8.MinXMaxZ:	return new Vector2(-1,  1);
			case Direction8.MaxXMinZ:	return new Vector2( 1, -1);
			case Direction8.MaxXMaxZ:	return new Vector2( 1,  1);
		}

		return new Vector2(0,0);
	}

	///////////////////////////////////////////////////////////////////////////

	public static Vector2Int DirectionToOffsetInt(Direction4 direction)
	{
		switch (direction)
		{
			case Direction4.MinX:	return new Vector2Int(-1, 0);
			case Direction4.MaxX:	return new Vector2Int( 1, 0);
			case Direction4.MinZ:	return new Vector2Int( 0,-1);
			case Direction4.MaxZ:	return new Vector2Int( 0, 1);
		}

		return new Vector2Int(0,0);
	}

	///////////////////////////////////////////////////////////////////////////

	public static Vector2Int DirectionToOffsetInt(Direction8 direction)
	{
		switch (direction)
		{
			case Direction8.MinX:		return new Vector2Int(-1, 0);
			case Direction8.MaxX:		return new Vector2Int( 1, 0);
			case Direction8.MinZ:		return new Vector2Int( 0,-1);
			case Direction8.MaxZ:		return new Vector2Int( 0, 1);

			case Direction8.MinXMinZ:	return new Vector2Int(-1, -1);
			case Direction8.MinXMaxZ:	return new Vector2Int(-1,  1);
			case Direction8.MaxXMinZ:	return new Vector2Int( 1, -1);
			case Direction8.MaxXMaxZ:	return new Vector2Int( 1,  1);
		}

		return new Vector2Int(0,0);
	}

	///////////////////////////////////////////////////////////////////////////

	public static float StepTowardsAngle(float oldValueDegrees, float targetValueDegrees, float stepSizeDegrees)
	{
		return Mathf.MoveTowardsAngle(oldValueDegrees, targetValueDegrees, stepSizeDegrees);
	}

	///////////////////////////////////////////////////////////////////////////

	public static float GetClampedRotationDeltaLinear(float rotOld, float rotTarget, float stepSize)
	{
		float deltaRotationTotal = Mathf.DeltaAngle(rotOld, rotTarget);

		if (Mathf.Abs(deltaRotationTotal) < stepSize)
		{
			return deltaRotationTotal;
		}
		else
		{
			return deltaRotationTotal > 0 ? stepSize : -stepSize;
		}
	}

	///////////////////////////////////////////////////////////////////////////

	// see http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
	private static float ApplyDeltaTimeToLerpFactor(float lerpFactor, float dt)
	{
		Debug.Assert(lerpFactor >= 0.0f && lerpFactor <= 1.0f);

		double adjustedDt = ((double)dt) * 60.0f; //< map a dt of 1/60 to a value an exponent of 1. Generally the factor is arbitrarily, but the smaller the value, the harsher the difference between 0.999 and 1.0
		double simpleLerpFactor = 1 - System.Math.Pow(1.0 - (double)lerpFactor, adjustedDt);
		return (float) simpleLerpFactor;
	}

	///////////////////////////////////////////////////////////////////////////

	public static float LerpWithDeltaTime(float source, float target, float lerpFactor, float dt)
	{
		return Mathf.Lerp(source, target, ApplyDeltaTimeToLerpFactor(lerpFactor, dt));
	}

	///////////////////////////////////////////////////////////////////////////

	public static Vector2 LerpWithDeltaTime(Vector2 source, Vector2 target, float lerpFactor, float dt)
	{
		return Vector2.Lerp(source, target, ApplyDeltaTimeToLerpFactor(lerpFactor, dt));
	}

	///////////////////////////////////////////////////////////////////////////

	public static float GetClampedRotationDeltaLerp(float rotOld, float targetRot, float lerpFactor)
	{
		float newAngle		= Mathf.LerpAngle(rotOld, targetRot, lerpFactor);
		float deltaAngle	= Mathf.DeltaAngle(rotOld, newAngle);
				
		return deltaAngle;
	}

	///////////////////////////////////////////////////////////////////////////
	
	public static float GetDistanceSqToRect(Vector2 referencePos, Rect rect)
	{
		Vector2 rectCenter = rect.center;

		float distCenterX	= Mathf.Abs(referencePos.x - rectCenter.x);
		float distRectX		= Mathf.Max(distCenterX - rect.width * 0.5f, 0.0f);
			
		float distCenterY	= Mathf.Abs(referencePos.y - rectCenter.y);
		float distRectY		= Mathf.Max(distCenterY - rect.height * 0.5f, 0.0f);

		return (distRectX * distRectX) + (distRectY * distRectY);
	}

		
	///////////////////////////////////////////////////////////////////////////
	
	public static float GetMaxOneDimensionalDistanceToRect(Vector2 referencePos, Rect rect)
	{
		Vector2 rectCenter = rect.center;

		float distCenterX	= Mathf.Abs(referencePos.x - rectCenter.x);
		float distRectX		= Mathf.Max(distCenterX - rect.width * 0.5f, 0.0f);
			
		float distCenterY	= Mathf.Abs(referencePos.y - rectCenter.y);
		float distRectY		= Mathf.Max(distCenterY - rect.height * 0.5f, 0.0f);

		return Mathf.Max(distRectX, distRectY);
	}

	///////////////////////////////////////////////////////////////////////////

	public static Vector2 GetRandomPointWithinUnitCircle(bool useSyncRandom)
	{
		float t = 2.0f * Mathf.PI * Random.Range(0.0f, 1.0f);
		float u = Random.Range(0.0f, 1.0f) + Random.Range(0.0f, 1.0f);
		float r = u > 1.0f ? 2.0f - u : u;
		return new Vector2(r*Mathf.Cos(t), r*Mathf.Sin(t));
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static float LerpAngleRad(float angle1, float angle2, float lerpAmount)
	{
		return Mathf.LerpAngle(angle1 * Mathf.Rad2Deg, angle2 * Mathf.Rad2Deg, lerpAmount) * Mathf.Deg2Rad;
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static int Mod_SupportNegative(int x, int m)
	{
		int r = x % m;
		return (r < 0) ? r + m : r;
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static float Mod_SupportNegative(float x, float m)
	{
		float r = x % m;
		return (r < 0) ? r + m : r;
	}

	///////////////////////////////////////////////////////////////////////////

	public static float NormalizeAngle(float angleDegrees)
	{
		return Mod_SupportNegative(angleDegrees, 360);
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static float GetFixedRandom(int seed)
	{
		int lolValue = (int)0b01010101010101010101010101010101;
		seed ^= lolValue;
		seed *= 1103515245;
		seed *= seed;
		seed ^= lolValue;

		int rndInt = seed & 0xfff;

		double rndDouble = ((double)rndInt) / (double)(0xfff);
		return Mathf.Clamp((float)rndDouble, 0.0f, 1.0f);
	}

	///////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// maps [0,1] to [0,1] by appying S/inverse S-Curve
	/// </summary>
	/// <param name="x">in [0,1]</param>
	/// <param name="k">(-1,0] gives s-curve. [0,1) gives inverse s-curve</param>
	public static float GetSCurveValue(float x, float k)
	{
		// see https://dhemery.github.io/DHE-Modules/technical/sigmoid/#function (normalized tunable sigmoid function)

		// |      .'			  |       '				|      .'''
		// |    .'				  |  ....' 				|     '
		// |  .'				  | '					|    '
		// |.'_________			  |'________			|...'________
		//
		// k == 0				  k == 0.5				k == -0.5

		// original form ([-1,1] -> [-1,1])		is		(x - k * x) / ((k - 2*k*Mathf.Abs(x)) + 1)

		// modified version ([0,1] -> [0,1])
		float xNorm = (x * 2.0f - 1);
		float y		= (xNorm - k * xNorm) / ((k - 2*k*Mathf.Abs(xNorm)) + 1);
		float yNorm = (y * 0.5f) + 0.5f;

		return yNorm;
	}

	///////////////////////////////////////////////////////////////////////////

	public static int GetFixedRandomInt(int seed, int minInclusive, int maxInclusive)
	{
		float rndNorm = GetFixedRandom(seed);

		float rndF = (float)minInclusive + (float)(maxInclusive - minInclusive) * rndNorm;
		int rndI = Mathf.RoundToInt(rndF);

		return Mathf.Clamp(rndI, minInclusive, maxInclusive);
	}

	///////////////////////////////////////////////////////////////////////////

	// equidistant seeds can yield equidistant rands (e.g. 0,4,8,16 could yield (100, 60, 20, 120))
	public static float GetFixedRand_Simple(int perObjectSeed)
	{
		// Note: shitty range, because noone should use this anyways
		return perObjectSeed * 0.263f % 123.123f;
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static Vector2 DirectionFromAngle(float angleDegrees)
	{
		float angleRadians = angleDegrees * Mathf.Deg2Rad;
		return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
	}

	///////////////////////////////////////////////////////////////////////////
		
	public static Color GetRandomColor_NotTooDark(int seed)
	{
		switch (seed)
		{
			case 0:		return Color.red;
			case 1:		return Color.green;
			case 2:		return Color.blue;
			case 3:		return Color.white;
			case 4:		return Color.cyan;
			case 5:		return Color.magenta;
			case 6:		return Color.yellow;
		}

		float r = GetFixedRandom(seed);
		float g = GetFixedRandom(seed + 132);
		float b = GetFixedRandom(seed + 172);

		if (r >= 0.5 || g >= 0.5f || b >= 0.5f)
		{
			return new Color(r,g,b);
		}

		float maxVal = Mathf.Max(r, g, b);

		if (maxVal < 0.01f)
		{
			return new Color(0.5f, 0.5f, 0.5f);
		}

		// try to boost to 0.8
		float boostFactor = (0.8f / maxVal);

		r *= boostFactor;
		g *= boostFactor;
		b *= boostFactor;

		return new Color(r,g,b);
	}

	///////////////////////////////////////////////////////////////////////////

	public static void Swap<T>(ref T a, ref T b)
	{
		T tmp = a;
		a = b;
		b = tmp;
	}

    ///////////////////////////////////////////////////////////////////////////

    //public static bool IsTrueEveryOnceInAWhile(float interval, int seed)
    //{
    //	if (Time.deltaTime == 0)
    //	{
    //		return false;
    //	}

    //	float timePerTick				= Time.deltaTime;
    //	float triggerEveryNTicks		= 1.0f / timePerTick;
    //	triggerEveryNTicks *= interval;

    //	int triggerEveryNTicksI = Mathf.Max((int) triggerEveryNTicks, 1);

    //	int curTick	= (int) Singletons.networkManager.GetUpdateCounter();

    //	curTick += seed;

    //	if (curTick % triggerEveryNTicksI == 0)
    //	{
    //		return true;
    //	}

    //	return false;
    //}

    ///////////////////////////////////////////////////////////////////////////
    ///
    public static float Remap(float value, float rangeAMin, float rangeAMax, float rangeBMin, float rangeBMax)
    {
        //1.
        //first we figure out at what percentage the value is, relative to range A
        //for example: Range A= [1 - 7], value = 3
        //gotta subtract the 1 from the 7 to get the range to be [0 - 6] (the step is rangeAMax - rangeAMin)
        //gotta also subtract the 1 from the 3 to get it relative to the range again ( the step is value - rangeAMin)
        //now we divide to get the percentage 

        float inverseLerpResult = (value - rangeAMin) / (rangeAMax - rangeAMin);

        //2.
        //now we're doing the lerp
        //we gotta map this percentage to the new range 
        //example: [3,9]
        //we also gotta subtract the rangeMin from rangeMax to get the range to start at 0 ( rangeBMax - rangeBMin) [9-3] = [0,6]
        //now we multiply the rangeBMax with the inverseLerpresult (percentage) and add the minBRange to it ((value * rangeBMax) + rangeBMin)

        float lerpResult = ((rangeBMax - rangeBMin) * inverseLerpResult) + rangeBMin;

        return lerpResult;
    }
}

///////////////////////////////////////////////////////////////////////////



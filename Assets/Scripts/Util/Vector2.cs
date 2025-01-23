using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 RotateBy(this Vector2 vector, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }
    public static  bool IsInTriangle(this Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float as_x = point.x - a.x;
        float as_y = point.y - a.y;
        bool s_ab = (b.x - a.x) * as_y - (b.y - a.y) * as_x > 0;
        if ((c.x - a.x) * as_y - (c.y - a.y) * as_x > 0 == s_ab) return false;
        if ((c.x - b.x) * (point.y - b.y) - (c.y - b.y) * (point.x - b.x) > 0 != s_ab) return false;
        return true;
    }
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }
    public static Vector2 DirectionTo(this Vector2 vector, Vector2 other)
    {
        return (other - vector).normalized;
    }
    /// <summary>
    /// In Degrees
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float ToRotation(this Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
}

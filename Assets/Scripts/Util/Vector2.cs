using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 RotateBy(this Vector2 vector, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }
}

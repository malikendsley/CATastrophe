using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    private static readonly System.Random Rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    // For finite lines:
    public static Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 line_start, Vector3 line_end)
    {
        Vector3 line_direction = line_end - line_start;
        float line_length = line_direction.magnitude;
        line_direction.Normalize();
        float project_length = Mathf.Clamp(Vector3.Dot(point - line_start, line_direction), 0f, line_length);
        return line_start + line_direction * project_length;
    }

    public static Vector3 GetClosestPointOnInfiniteLine(Vector3 point, Vector3 line_start, Vector3 line_end)
    {
        return line_start + Vector3.Project(point - line_start, line_end - line_start);
    }
}
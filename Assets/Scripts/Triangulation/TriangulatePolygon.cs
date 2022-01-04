using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lisenced under MIT <br/>
/// https://github.com/twobitcoder101/Polygon-Triangulation/blob/main/TriangulatePolygon.cs
/// </summary>
public static class TriangulatePolygon
{
    public static float FindPolygonArea(Vector2[] vertices)
    {
        float totalArea = 0f;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Length];

            float dy = (a.y + b.y) / 2f;
            float dx = b.x - a.x;

            float area = dy * dx;
            totalArea += area;
        }

        return Mathf.Abs(totalArea);
    }

    /// <summary>
    /// Computes the triangulation of a polygon defined by a 2d vertex array, and generates a triangle array to display it.
    /// </summary>
    /// <param name="vertices">Vertex location input for the polygon.</param>
    /// <param name="triangles">Sets this array pointer to vertices pointers. Undefined if this method returns false.</param>
    /// <param name="errorMessage">Error message outing. This pointer will be set to an error description if this method returns false</param>
    /// <returns>True if the triangulation was sucessful.</returns>
    public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
    {
        triangles = null;
        errorMessage = string.Empty;

        if (vertices is null)
        {
            errorMessage = "The vertex list is null.";
            return false;
        }

        if (vertices.Length < 3)
        {
            errorMessage = "The vertex list must have at least 3 vertices.";
            return false;
        }

        if (vertices.Length > 1024)
        {
            errorMessage = "The max vertex list length is 1024";
            return false;
        }

        List<int> indexList = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
            indexList.Add(i);

        int totalTriangleCount = vertices.Length - 2;
        int totalTriangleIndexCount = totalTriangleCount * 3;

        triangles = new int[totalTriangleIndexCount];
        int triangleIndexCount = 0;

        int alarm = 1000;

        while (indexList.Count > 3 && alarm >= 0)
        {
            alarm--;
            for (int i = 0; i < indexList.Count; i++)
            {
                int a = indexList[i];
                int b = GetLooping(indexList, i - 1);
                int c = GetLooping(indexList, i + 1);

                Vector2 va = vertices[a];
                Vector2 vb = vertices[b];
                Vector2 vc = vertices[c];

                Vector2 va_to_vb = vb - va;
                Vector2 va_to_vc = vc - va;

                // Is ear test vertex convex?
                if (Cross2(va_to_vb, va_to_vc) < 0f)
                {
                    continue;
                }

                bool isEar = true;

                // Does test ear contain any polygon vertices?
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (j == a || j == b || j == c)
                    {
                        continue;
                    }

                    Vector2 p = vertices[j];

                    if (PointInTriangle(p, vb, va, vc))
                    {
                        isEar = false;
                        break;
                    }
                }

                if (isEar)
                {
                    triangles[triangleIndexCount++] = b;
                    triangles[triangleIndexCount++] = a;
                    triangles[triangleIndexCount++] = c;

                    indexList.RemoveAt(i);
                    break;
                }
            }
        }

        triangles[triangleIndexCount++] = indexList[0];
        triangles[triangleIndexCount++] = indexList[1];
        triangles[triangleIndexCount++] = indexList[2];

        return alarm > 0;
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = Sign(pt, v1, v2);
        d2 = Sign(pt, v2, v3);
        d3 = Sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    private static T GetLooping<T>(List<T> storage, int index)
    {
        if (index < 0)
            return storage[index % storage.Count + storage.Count];
        return storage[index % storage.Count];
    }

    private static float Cross2(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}

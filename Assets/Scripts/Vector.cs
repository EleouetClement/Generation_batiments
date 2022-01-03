using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public static Vector zero = new Vector(0.0f, 0.0f, 0.0f);

    public Vector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = 0.0f;
    }
    public Vector crossProduct(Vector vb)
    {
        return new Vector(x * vb.x, y * vb.y, z * vb.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }

    public static Vector zero = new Vector(0.0f, 0.0f, 0.0f);

    public Vector(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector(double x, double y)
    {
        this.x = x;
        this.y = y;
        this.z = 0.0f;
    }
    public Vector crossProduct(Vector vb)
    {
        return new Vector(x * vb.x, y * vb.y, z * vb.z);
    }


    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }

    public Vector Clone()
    {
        return new Vector(x, y, z);
    }

}

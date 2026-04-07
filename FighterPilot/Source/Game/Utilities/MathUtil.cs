using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

public class MathUtil
{
    //Scale 6

    public static Vector3 Scale(Vector3 a, Vector3 b)
    {

        float resultABX = a.X * b.X;
        float resultABY = a.Y * b.Y;
        float resultABZ = a.Z * b.Z;

        if (Mathf.Abs(resultABX) < 0 || Mathf.Abs(resultABY) < 0 || Mathf.Abs(resultABY) < 0) return Vector3.Zero;
        return new Vector3(resultABX, resultABY, resultABZ);
    }
    public static Vector3 Scale6(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, Vector3 f)
    {

        Vector3 resultAB = Scale(a, b);
        Vector3 resultBC = Scale(c, d);
        Vector3 resultDE = Scale(d, e);
        Vector3 resultEF = Scale(e, f);

        float x = resultAB.X * resultBC.X * resultDE.X * resultEF.X;
        float y = resultAB.Y * resultBC.Y * resultDE.Y * resultEF.Y;
        float z = resultAB.Z * resultBC.Z * resultDE.Z * resultEF.Z;
        if (Mathf.Abs(x) < 0 || Mathf.Abs(y) < 0 || Mathf.Abs(z) < 0) return Vector3.Zero;
        return new Vector3(x, y, z);
    }
    public static float NewtonsToPounds()
    {
        return 4.448f;
    }
    public static float PoundsToNewtons()
    {
        return 4.44822f;
    }
    //Proportional Navigation For AI
}
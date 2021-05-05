using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaCalculator : MonoBehaviour
{
    public static float calculateArea(Vector3[] vertices)
    {
        float a = 0.0f;
        float p = 0.0f;
        float x = vertices[0].x;
        float y = vertices[0].y;
        int i = 0;

        while(i < vertices.Length)
        {
            a += vertices[i].x * y - vertices[i].y * x;
			p += Math.Abs((vertices[i].x) - x + (vertices[i].y - y));
			x = vertices[i].x;
			y = vertices[i].y;
			i++;
        }
        return Math.Abs(a/2.0f);
    }

    public float calculateAreaFromCoords(Corner[] vertices)
    {
        float a = 0.0f;
        float p = 0.0f;
        float x = vertices[0].coord.x;
        float y = vertices[0].coord.y;
        int i = 0;

        while(i < vertices.Length)
        {
            Debug.Log("Entered loop");
            a += vertices[i].coord.x * y - vertices[i].coord.y * x;
			p += Math.Abs((vertices[i].coord.x) - x + (vertices[i].coord.y - y));
			x = vertices[i].coord.x;
			y = vertices[i].coord.y;
			i++;
        }
        return Math.Abs(a/2.0f);
    }
}

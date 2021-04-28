using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaCalculator : MonoBehaviour
{
    Vector3[] testing = {new Vector3(0,0,0), new Vector3(1.23435f,0,0), new Vector3(2.435f,4.2345f,0)};
    void Start()
    {
        float testingArea = calculateArea(testing);
        Debug.Log(String.Format("Area of testing polygon: {0}", testingArea));
    }

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
}

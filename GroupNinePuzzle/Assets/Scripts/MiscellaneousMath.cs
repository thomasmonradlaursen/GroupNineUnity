using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JSONPuzzleTypes;

public class MiscellaneousMath
{
    public float CalculateAreaFromCoords(Corner[] vertices)
    {
        float a = 0.0f;
        float p = 0.0f;
        float x = vertices[0].coord.x;
        float y = vertices[0].coord.y;
        int i = 0;

        while(i < vertices.Length)
        {
            a += vertices[i].coord.x * y - vertices[i].coord.y * x;
			p += Math.Abs((vertices[i].coord.x) - x + (vertices[i].coord.y - y));
			x = vertices[i].coord.x;
			y = vertices[i].coord.y;
			i++;
        }
        return Math.Abs(a/2.0f);
    }

    public float[] CalculateSideLengthsAndAngles(Corner[] corners)
    {
        int n = corners.Length;
        float[] sides = new float[n];
        int i = 0;
        while(i < n){
            float x = corners[i].coord.x - corners[i+1].coord.x;
            float y = corners[i].coord.y - corners[i+1].coord.y;
            sides[i] = Mathf.Sqrt(x*x + y*y);
        }
        float x1 = corners[n].coord.x - corners[0].coord.x;
        float y1 = corners[n].coord.y - corners[0].coord.y;
        sides[n] = Mathf.Sqrt(x1*x1 + y1*y1);
        return sides;
    }
   
   public float[] CalculateAngles(Corner[] corners)
   {
        int n = corners.Length;
        float[] angles = new float[n];
        int i = 0;
        Vector3 a = new Vector3(corners[n-1].coord.x, corners[n-1].coord.y, 0);
        Vector3 b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
        Vector3 c = new Vector3(corners[i+1].coord.x, corners[i+1].coord.y, 0);
        a = b-a;
        b = b-c;
        angles[i] = Vector3.Angle(a, b);
        i++;
        while(i < n){
            if(i < n-1){
                a = new Vector3(corners[i-1].coord.x, corners[i-1].coord.y, 0);
                b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
                c = new Vector3(corners[i+1].coord.x, corners[i+1].coord.y, 0);
                a = b-a;
                b = b-c;
                angles[i] = Vector3.Angle(a, b);
            }else {
                a = new Vector3(corners[i-1].coord.x, corners[i-1].coord.y, 0);
                b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
                c = new Vector3(corners[0].coord.x, corners[0].coord.y, 0);
                a = b-a;
                b = b-c;
                angles[i] = Vector3.Angle(a, b);
            }
            i++;
        }
        return angles;
   }
}

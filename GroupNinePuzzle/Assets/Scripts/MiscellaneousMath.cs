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
        public float CalculateAreaFromMesh(Mesh mesh)
    {
        float a = 0.0f;
        float p = 0.0f;
        float x = mesh.vertices[0].x;
        float y = mesh.vertices[0].y;
        int i = 0;

        while(i < mesh.vertices.Length)
        {
            a += mesh.vertices[i].x * y - mesh.vertices[i].y * x;
			p += Math.Abs((mesh.vertices[i].x) - x + (mesh.vertices[i].y - y));
			x = mesh.vertices[i].x;
			y = mesh.vertices[i].y;
			i++;
        }
        return Math.Abs(a/2.0f);
    }
    public Vector3 CalculateCenterOfMass(Mesh mesh)
    {
        float xCoordinateForCenter = 0.0f;
        float yCoordinateForCenter = 0.0f;
        foreach(Vector3 vertex in mesh.vertices)
        {
            xCoordinateForCenter += vertex.x;
            yCoordinateForCenter += vertex.y;
        }
        xCoordinateForCenter /= mesh.vertices.Length;
        yCoordinateForCenter /= mesh.vertices.Length;
        return new Vector3(xCoordinateForCenter, yCoordinateForCenter, 0.0f);
    }

    public float[] CalculateSideLengths(Corner[] corners)
    {
        int n = corners.Length;
        float[] sides = new float[n];
        int i = 0;
        while(i < n-1){
            float x = corners[i].coord.x - corners[i+1].coord.x;
            float y = corners[i].coord.y - corners[i+1].coord.y;
            sides[i] = Mathf.Sqrt(x*x + y*y);
            i++;
        }
        float x1 = corners[n-1].coord.x - corners[0].coord.x;
        float y1 = corners[n-1].coord.y - corners[0].coord.y;
        sides[n-1] = Mathf.Sqrt(x1*x1 + y1*y1);
        return sides;
    }
   public float[] CalculateSideLengthsFromMesh(Mesh mesh)
    {
        int n = mesh.vertices.Length;
        float[] sides = new float[n];
        int i = 0;
        while(i < n-1){
            float x = mesh.vertices[i].x - mesh.vertices[i+1].x;
            float y = mesh.vertices[i].y - mesh.vertices[i+1].y;
            sides[i] = Mathf.Sqrt(x*x + y*y);
            i++;
        }
        float x1 = mesh.vertices[n-1].x - mesh.vertices[0].x;
        float y1 = mesh.vertices[n-1].y - mesh.vertices[0].y;
        sides[n-1] = Mathf.Sqrt(x1*x1 + y1*y1);
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
      public float[] CalculateAnglesFromMesh(Mesh mesh)
   {
        int n = mesh.vertices.Length;
        float[] angles = new float[n];
        int i = 0;
        Vector3 a = new Vector3(mesh.vertices[n-1].x, mesh.vertices[n-1].y, 0);
        Vector3 b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
        Vector3 c = new Vector3(mesh.vertices[i+1].x, mesh.vertices[i+1].y, 0);
        a = b-a;
        b = b-c;
        angles[i] = Vector3.Angle(a, b);
        i++;
        while(i < n){
            if(i < n-1){
                a = new Vector3(mesh.vertices[i-1].x, mesh.vertices[i-1].y, 0);
                b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
                c = new Vector3(mesh.vertices[i+1].x, mesh.vertices[i+1].y, 0);
                a = b-a;
                b = b-c;
                angles[i] = Vector3.Angle(a, b);
            }else {
                a = new Vector3(mesh.vertices[i-1].x, mesh.vertices[i-1].y, 0);
                b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
                c = new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, 0);
                a = b-a;
                b = b-c;
                angles[i] = Vector3.Angle(a, b);
            }
            i++;
        }
        return angles;
   }
}

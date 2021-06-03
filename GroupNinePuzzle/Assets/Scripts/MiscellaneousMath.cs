using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JSONPuzzleTypes;

public class MiscellaneousMath
{   
    //https://gamedev.stackexchange.com/questions/165643/how-to-calculate-the-surface-area-of-a-mesh
    public float CalculateAreaFromMesh(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        float area = 0.0f;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            area += Vector3.Cross(a, b).magnitude;
        }
        return (float)(area/2);
    }

    public float[] CalculateSideLengthsAndAngles(Corner[] corners)
    {
        int n = corners.Length;
        float[] sides = new float[n];
        int i = 0;
        while (i < n)
        {
            float x = corners[i].coord.x - corners[i + 1].coord.x;
            float y = corners[i].coord.y - corners[i + 1].coord.y;
            sides[i] = Mathf.Sqrt(x * x + y * y);
        }
        float x1 = corners[n].coord.x - corners[0].coord.x;
        float y1 = corners[n].coord.y - corners[0].coord.y;
        sides[n] = Mathf.Sqrt(x1 * x1 + y1 * y1);
        return sides;
    }

    public float[] CalculateAngles(Corner[] corners)
    {
        int n = corners.Length;
        float[] angles = new float[n];
        int i = 0;
        Vector3 a = new Vector3(corners[n - 1].coord.x, corners[n - 1].coord.y, 0);
        Vector3 b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
        Vector3 c = new Vector3(corners[i + 1].coord.x, corners[i + 1].coord.y, 0);
        a = b - a;
        b = b - c;
        angles[i] = Vector3.Angle(a, b);
        i++;
        while (i < n)
        {
            if (i < n - 1)
            {
                a = new Vector3(corners[i - 1].coord.x, corners[i - 1].coord.y, 0);
                b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
                c = new Vector3(corners[i + 1].coord.x, corners[i + 1].coord.y, 0);
                a = b - a;
                b = b - c;
                angles[i] = Vector3.Angle(a, b);
            }
            else
            {
                a = new Vector3(corners[i - 1].coord.x, corners[i - 1].coord.y, 0);
                b = new Vector3(corners[i].coord.x, corners[i].coord.y, 0);
                c = new Vector3(corners[0].coord.x, corners[0].coord.y, 0);
                a = b - a;
                b = b - c;
                angles[i] = Vector3.Angle(a, b);
            }
            i++;
        }
        return angles;
    }

    public Vector3 CalculateCentroid(Vector3[] vertices, float area)
    {
        float xCentroid = 0.0f;
        float yCentroid = 0.0f;
        for (int index = 0; index < vertices.Length - 1; index++)
        {
            xCentroid += SumXCoordintesForCentroid(index, vertices);
            yCentroid += SumYCoordintesForCentroid(index, vertices);
        }
        xCentroid += FinalXCoordintesForCentroid(vertices.Length - 1, vertices);
        yCentroid += FinalYCoordintesForCentroid(vertices.Length - 1, vertices);
        xCentroid = xCentroid / (6 * area);
        yCentroid = yCentroid / (6 * area);
        return new Vector3(xCentroid, yCentroid, 0.0f);
    }
    float SumXCoordintesForCentroid(int index, Vector3[] vertices)
    {
        return ((vertices[index].x + vertices[index + 1].x) * (vertices[index].x * vertices[index + 1].y - vertices[index + 1].x * vertices[index].y));
    }
    float SumYCoordintesForCentroid(int index, Vector3[] vertices)
    {
        return ((vertices[index].y + vertices[index + 1].y) * (vertices[index].x * vertices[index + 1].y - vertices[index + 1].x * vertices[index].y));
    }
    float FinalXCoordintesForCentroid(int index, Vector3[] vertices)
    {
        return ((vertices[index].x + vertices[0].x) * (vertices[index].x * vertices[0].y - vertices[0].x * vertices[index].y));
    }
    float FinalYCoordintesForCentroid(int index, Vector3[] vertices)
    {
        return ((vertices[index].y + vertices[0].y) * (vertices[index].x * vertices[0].y - vertices[0].x * vertices[index].y));
    }
}
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

            area += Vector3.Cross(a,b).magnitude;
        }
        return (float)(area/2);
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
    public float[] CalculateAnglesFromMesh(Mesh mesh)
    {
        //moving clockwise around the piece
        int n = mesh.vertices.Length;
        float[] angles = new float[n];
        int i = 0;
        Vector3 a = new Vector3(mesh.vertices[n-1].x, mesh.vertices[n-1].y, 0);
        Vector3 b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
        Vector3 c = new Vector3(mesh.vertices[i+1].x, mesh.vertices[i+1].y, 0);
        Vector3 temp1 = b-a;
        Vector3 temp2 = b-c;

        float angle = Vector3.SignedAngle(temp1, temp2, Vector3.up);
        Vertex vertex = new Vertex(b);
        Vertex prev = new Vertex(a); Vertex next = new Vertex(c);
        vertex.prevVertex = prev; vertex.nextVertex = next; 
        PolygonTriangulation.CheckIfReflexOrConvex(vertex);
        if(vertex.isReflex == true){
            angle = 360 - angle;
        }
        angles[i] = angle;
        i++;
        while(i < n){
            if(i < n-1){
                a = new Vector3(mesh.vertices[i-1].x, mesh.vertices[i-1].y, 0);
                b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
                c = new Vector3(mesh.vertices[i+1].x, mesh.vertices[i+1].y, 0);
                temp1 = b-a;
                temp2 = b-c;
                angle = Vector3.SignedAngle(temp1, temp2, Vector3.up);
                vertex = new Vertex(b);
                prev = new Vertex(a); next = new Vertex(c);
                vertex.prevVertex = prev; vertex.nextVertex = next; 
                PolygonTriangulation.CheckIfReflexOrConvex(vertex);
                if(vertex.isReflex == true){
                    angle = 360 - angle;
                }
                angles[i] = angle;
            }else {
                a = new Vector3(mesh.vertices[i-1].x, mesh.vertices[i-1].y, 0);
                b = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 0);
                c = new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, 0);
                temp1 = b-a;
                temp2 = b-c;
                angle = Vector3.SignedAngle(temp1, temp2, Vector3.up);
                vertex = new Vertex(b);
                prev = new Vertex(a); next = new Vertex(c);
                vertex.prevVertex = prev; vertex.nextVertex = next;
                PolygonTriangulation.CheckIfReflexOrConvex(vertex);
                if(vertex.isReflex == true){
                    angle = 360 - angle;
                }
                angles[i] = angle;
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

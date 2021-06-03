using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JSONPuzzleTypes;

public class MiscellaneousMath
{
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

}

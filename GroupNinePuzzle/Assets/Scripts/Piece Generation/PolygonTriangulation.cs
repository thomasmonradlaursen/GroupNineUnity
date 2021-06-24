using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

// Author: Gustav Nilsson Pedersen
// Note: A lot of the code in this class is based on: https://www.habrador.com/tutorials/math/10-triangulation/

public class Vertex
{
    public Vector3 vertex { get; set; }
    public Vertex previousVertex { get; set; }
    public Vertex nextVertex { get; set; }
    public bool isConvex { get; set; } // Angle smaller than 180 degrees
    public bool isReflex { get; set; } // Angle larger than 180 degrees

    public Vertex(Vector3 vertexIn)
    {
        this.vertex = vertexIn;
    }

    public Vector2 GetXY()
    {
        return new Vector2(
            this.vertex.x,
            this.vertex.y
            );
    }

}

public class Triangle
{
    public Vertex vertex1 { get; set; }
    public Vertex vertex2 { get; set; }
    public Vertex vertex3 { get; set; }

    public Triangle(Vector3 vertex1in, Vector3 vertex2in, Vector3 vertex3in)
    {
        this.vertex1 = new Vertex(vertex1in);
        this.vertex2 = new Vertex(vertex2in);
        this.vertex3 = new Vertex(vertex3in);
    }
}

public class PolygonTriangulation
{
    // ===== BELOW CODE IS BASED ON https://www.habrador.com/tutorials/math/10-triangulation/  =====

    //This assumes that we have a polygon and now we want to triangulate it
    //The points on the polygon should be ordered counter-clockwise
    //This alorithm is called ear clipping and it's O(n*n) Another common algorithm is dividing it into trapezoids and it's O(n log n)
    //One can maybe do it in O(n) time but no such version is known
    //Assumes we have at least 3 points
    public static List<Triangle> TriangulateConcavePolygon(List<Vector3> points)
    {
        //The list with triangles the method returns
        List<Triangle> triangles = new List<Triangle>();

        //If we just have three points, then we dont have to do all calculations
        if (points.Count == 3)
        {
            triangles.Add(new Triangle(points[0], points[1], points[2]));

            return triangles;
        }

        //Step 1. Store the vertices in a list and we also need to know the next and previous vertex
        List<Vertex> vertices = new List<Vertex>();

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(new Vertex(points[i]));
        }

        //Find the next and previous vertex
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextPos = GetWrappingIndex(i + 1, vertices.Count);
            int prevPos = GetWrappingIndex(i - 1, vertices.Count);

            vertices[i].previousVertex = vertices[prevPos];
            vertices[i].nextVertex = vertices[nextPos];
        }

        //Step 2. Find the reflex (concave) and convex vertices, and ear vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            CheckIfReflexOrConvex(vertices[i]);
        }

        //Have to find the ears after we have found if the vertex is reflex or convex
        List<Vertex> earVertices = new List<Vertex>();

        for (int i = 0; i < vertices.Count; i++)
        {
            IsVertexEar(vertices[i], vertices, earVertices);
        }

        //Step 3. Triangulate!
        while (true)
        {
            if (earVertices.Count == 0) break;

            //This means we have just one triangle left
            if (vertices.Count == 3)
            {
                if (IsTriangleOrientedClockwise(vertices[0].GetXY(), vertices[0].previousVertex.GetXY(), vertices[0].nextVertex.GetXY()))
                {
                    triangles.Add(new Triangle(vertices[0].vertex, vertices[0].previousVertex.vertex, vertices[0].nextVertex.vertex));
                }
                else
                {
                    triangles.Add(new Triangle(vertices[0].previousVertex.vertex, vertices[0].vertex, vertices[0].nextVertex.vertex));
                }

                break;
            }

            //Make a triangle of the first ear
            //(we know the vertices in the ear are oriented clockwise)
            Vertex earVertex = earVertices[0];

            Vertex earVertexPrev = earVertex.previousVertex;
            Vertex earVertexNext = earVertex.nextVertex;

            Triangle newTriangle = new Triangle(earVertex.vertex, earVertexPrev.vertex, earVertexNext.vertex);

            triangles.Add(newTriangle);

            //Remove the vertex from the lists
            earVertices.Remove(earVertex);

            vertices.Remove(earVertex);

            //Update the previous vertex and next vertex
            earVertexPrev.nextVertex = earVertexNext;
            earVertexNext.previousVertex = earVertexPrev;

            //...see if we have found a new ear by investigating the two vertices that were part of the ear
            CheckIfReflexOrConvex(earVertexPrev);
            CheckIfReflexOrConvex(earVertexNext);

            earVertices.Remove(earVertexPrev);
            earVertices.Remove(earVertexNext);

            IsVertexEar(earVertexPrev, vertices, earVertices);
            IsVertexEar(earVertexNext, vertices, earVertices);
        }

        return triangles;
    }



    //Check if a vertex if reflex or convex, and add to appropriate list
    public static void CheckIfReflexOrConvex(Vertex v)
    {
        v.isReflex = false;
        v.isConvex = false;

        //This is a reflex vertex if its triangle is oriented clockwise
        Vector2 a = v.previousVertex.GetXY();
        Vector2 b = v.GetXY();
        Vector2 c = v.nextVertex.GetXY();

        if (IsTriangleOrientedClockwise(a, b, c))
        {
            v.isReflex = true;
        }
        else
        {
            v.isConvex = true;
        }
    }

    //Takes the three vertices in a triangle as input and determines if the order a-b-c is clockwise
    //https://math.stackexchange.com/questions/1324179/how-to-tell-if-3-connected-points-are-connected-clockwise-or-counter-clockwise
    //https://en.wikipedia.org/wiki/Curve_orientation
    private static bool IsTriangleOrientedClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        bool isClockWise = true;

        float determinant = a.x * b.y + c.x * a.y + b.x * c.y - a.x * c.y - c.x * b.y - b.x * a.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }


    //Check if a vertex is an ear
    private static void IsVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> earVertices)
    {
        //A reflex vertex can't be an ear!
        if (v.isReflex)
        {
            return;
        }

        //This triangle to check point in triangle
        Vector2 a = v.previousVertex.GetXY();
        Vector2 b = v.GetXY();
        Vector2 c = v.nextVertex.GetXY();

        bool hasPointInside = false;

        for (int i = 0; i < vertices.Count; i++)
        {
            //We only need to check if a reflex vertex is inside of the triangle
            if (vertices[i].isReflex)
            {
                //Debug.Log("inner vertex reflex, index: " + i);

                Vector2 p = vertices[i].GetXY();

                //This means inside and not on the hull
                if (IsPointInTriangle(a, b, c, p))
                {
                    hasPointInside = true;
                    break;
                }
            }
        }

        if (!hasPointInside)
        {
            earVertices.Add(v);
        }
    }

    //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
    //p is the testpoint, and the other points are corners in the triangle
    public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p, float precision = 0f)
    {
        bool isWithinTriangle = false;

        //Based on Barycentric coordinates
        float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

        float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
        float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
        float c = 1 - a - b;

        //The point is within the triangle if 0 < a < 1 and 0 < b < 1 and 0 < c < 1
        if (a > 0f + precision && a < 1f - precision && b > 0f + precision && b < 1f - precision && c > 0f + precision && c < 1f - precision)
        {
            isWithinTriangle = true;
        }

        return isWithinTriangle;
    }
    public static int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }
}
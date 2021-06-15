using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTriangle;

public class Triangulation : MonoBehaviour
{
    public List<DelaunayTriangle> BowyerWatsonTriangulate(List<Vector3> points)
    {
        List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();
        (DelaunayTriangle, DelaunayTriangle) supertriangles = CreateSupertriangles(GetComponent<DivisionModel>().corners);
        triangles.Add(supertriangles.Item1);
        triangles.Add(supertriangles.Item2);
        foreach (Vector3 point in points)
        {
            List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
            foreach (DelaunayTriangle triangle in triangles)
            {
                if (Vector3.Distance(point, triangle.circumcenter) <= triangle.circumradius)
                {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> polygon = new List<Edge>();
            bool isEdgeUnique = true;
            foreach (DelaunayTriangle outerTriangle in badTriangles)
            {
                foreach (Edge outerEdge in outerTriangle.edges)
                {
                    isEdgeUnique = true;
                    foreach (DelaunayTriangle innerTriangle in badTriangles)
                    {
                        foreach (Edge innerEdge in innerTriangle.edges)
                        {
                            if (innerTriangle.id != outerTriangle.id)
                            {
                                if (innerEdge.innerHalf.Equals(outerEdge.innerHalf))
                                {
                                    isEdgeUnique = false;
                                }
                                else if (innerEdge.outerHalf.Equals(outerEdge.innerHalf))
                                {
                                    isEdgeUnique = false;
                                }
                                else if (innerEdge.outerHalf.Equals(outerEdge.outerHalf))
                                {
                                    isEdgeUnique = false;
                                }
                            }
                        }
                    }
                    if (isEdgeUnique)
                    {
                        polygon.Add(outerEdge);
                    }
                }
            }
            foreach (DelaunayTriangle triangle in badTriangles)
            {
                for (int index = 0; index < triangles.Count; index++)
                {
                    if (triangle.Equals(triangles[index]))
                    {
                        triangles.RemoveAt(index);
                        break;
                    }
                }
            }
            badTriangles.Clear();
            foreach (Edge edge in polygon)
            {
                DelaunayTriangle newTriangle = new DelaunayTriangle();
                newTriangle = CreateTriangle(point, edge.innerHalf.Item1, edge.innerHalf.Item2);
                triangles.Add(newTriangle);
            }
        }
        if (GetComponent<DivisionModel>().numberOfPieces % 2 != 0)
        {
            DividePiece(triangles);
        }
        return triangles;
    }
    void DividePiece(List<DelaunayTriangle> triangles)
    {
            int index = Random.Range(0, triangles.Count);
            float midpointX = (triangles[index].vertices[0].x + triangles[index].vertices[1].x) / 2;
            float midPointY = (triangles[index].vertices[0].y + triangles[index].vertices[1].y) / 2;
            Vector3 midPoint = new Vector3(midpointX, midPointY, 0.0f);
            triangles.Add(CreateTriangle(triangles[index].vertices[0],triangles[index].vertices[2], midPoint));
            triangles.Add(CreateTriangle(triangles[index].vertices[1], midPoint, triangles[index].vertices[2]));
            triangles.RemoveAt(index);
    }
    (DelaunayTriangle, DelaunayTriangle) CreateSupertriangles(List<Vector3> corners)
    {
        DelaunayTriangle topTriangle = CreateTriangle(corners[0], corners[2], corners[3]);
        DelaunayTriangle bottomTriangle = CreateTriangle(corners[0], corners[1], corners[2]);
        return (topTriangle, bottomTriangle);
    }
    DelaunayTriangle CreateTriangle(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree)
    {
        Circumscribed circumscriber = new Circumscribed();
        DelaunayTriangle triangle = new DelaunayTriangle();
        triangle.id = GetComponent<DivisionModel>().triangleId;
        triangle.vertices = new Vector3[3] { pointTwo, pointOne, pointThree };
        triangle.edges = new Edge[3] { new Edge(), new Edge(), new Edge() };
        triangle.edges[0].innerHalf = (pointOne, pointTwo);
        triangle.edges[0].outerHalf = (pointTwo, pointOne);
        triangle.edges[1].innerHalf = (pointTwo, pointThree);
        triangle.edges[1].outerHalf = (pointThree, pointTwo);
        triangle.edges[2].innerHalf = (pointThree, pointOne);
        triangle.edges[2].outerHalf = (pointOne, pointThree);
        (Vector3, float) centerAndRadius = circumscriber.GetCircumcenterAndCircumradius(pointOne, pointThree, pointTwo);
        triangle.circumcenter = centerAndRadius.Item1;
        triangle.circumradius = centerAndRadius.Item2;
        GetComponent<DivisionModel>().triangleId += 1;
        return triangle;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTriangle;

public class DelaunayTriangulation : MonoBehaviour
{
    Circumscribed circumscriber = new Circumscribed();
    public List<Vector3> points = new List<Vector3>();
    public List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();
    public DelaunayTriangle topSupertriangle;
    public DelaunayTriangle bottomSupertriangle;
    public int triangleId = 1;
    public List<DelaunayTriangle> RunDelaunayTriangulation()
    {
        points = GetComponent<RandomPieceGenerator>().GetPoints();
        (DelaunayTriangle, DelaunayTriangle) supertriangles = SetupSupertriangles(GetComponent<RandomPieceGenerator>().boardSize);
        topSupertriangle = supertriangles.Item1;
        bottomSupertriangle = supertriangles.Item2;
        triangles.Add(topSupertriangle);
        triangles.Add(bottomSupertriangle);
        BowyerWatsonTriangulate(points, triangles);
        return triangles;
    }
    void BowyerWatsonTriangulate(List<Vector3> points, List<DelaunayTriangle> triangles)
    {
        int numberOfLoops = 1;
        Debug.Log("Number of points: " + points.Count);
        foreach (Vector3 point in points)
        {
            Debug.Log("Number of triangles at round " + numberOfLoops + ": " + triangles.Count);
            List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
            foreach (DelaunayTriangle triangle in triangles)
            {
                if (Vector3.Distance(point, triangle.circumcenter) <= triangle.circumradius)
                {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> polygon = new List<Edge>();
            Debug.Log("Remove bad triangles " + numberOfLoops);
            Debug.LogFormat("Number of triangles: {0}", triangles.Count);
            bool isEdgeUnique = true;
            foreach (DelaunayTriangle outerTriangle in badTriangles)
            {
                Debug.Log("Outer triangle");
                foreach (Edge outerEdge in outerTriangle.edges)
                {
                    isEdgeUnique = true;
                    Debug.Log("Outer edge");
                    foreach (DelaunayTriangle innerTriangle in badTriangles)
                    {
                        Debug.Log("Inner triangle");
                        foreach (Edge innerEdge in innerTriangle.edges)
                        {
                            Debug.Log("Inner edge");
                            {
                                if (innerTriangle.id != outerTriangle.id)
                                {
                                    if (innerEdge.innerHalf.Equals(outerEdge.innerHalf))
                                    {
                                        Debug.LogFormat("Inner inner-edge: {0}, Outer inner-edge: {1}", innerEdge.innerHalf, outerEdge.innerHalf);
                                        isEdgeUnique = false;
                                    }
                                    else if (innerEdge.outerHalf.Equals(outerEdge.innerHalf))
                                    {
                                        Debug.LogFormat("Inner outer-edge: {0}, Outer inner-edge: {1}", innerEdge.outerHalf, outerEdge.innerHalf);
                                        isEdgeUnique = false;
                                    }
                                    else if (innerEdge.outerHalf.Equals(outerEdge.outerHalf))
                                    {
                                        Debug.LogFormat("Inner outer-edge: {0}, Outer outer-edge: {1}", innerEdge.outerHalf, outerEdge.outerHalf);
                                        isEdgeUnique = false;
                                    }
                                }
                            }
                        }
                    }
                    if (isEdgeUnique)
                    {
                        Debug.Log("Found unique edge: " + outerEdge.innerHalf);
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
            foreach (Edge edge in polygon)
            {
                Debug.Log("Triangle added at round" + numberOfLoops);
                DelaunayTriangle newTriangle = new DelaunayTriangle();
                newTriangle = CreateTriangle(point, edge.innerHalf.Item1, edge.innerHalf.Item2);
                triangles.Add(newTriangle);
            }
            numberOfLoops++;
        }
    }
    (DelaunayTriangle, DelaunayTriangle) SetupSupertriangles(Vector2 boardSize)
    {
        Vector3 bottomLeftCorner = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 bottomRightCorner = new Vector3(boardSize.x, 0.0f, 0.0f);
        Vector3 topLeftCorner = new Vector3(0.0f, boardSize.y, 0.0f);
        Vector3 topRightCorner = new Vector3(boardSize.x, boardSize.y, 0.0f);
        DelaunayTriangle topTriangle = CreateTriangle(bottomLeftCorner, topRightCorner, topLeftCorner);
        DelaunayTriangle bottomTriangle = CreateTriangle(bottomLeftCorner, bottomRightCorner, topRightCorner);
        Debug.Log(bottomTriangle.vertices[1]);
        return (topTriangle, bottomTriangle);
    }
    DelaunayTriangle CreateTriangle(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree)
    {
        DelaunayTriangle triangle = new DelaunayTriangle();
        triangle.id = triangleId;
        triangle.vertices = new Vector3[3] {pointTwo, pointOne, pointThree };
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
        triangleId++;
        return triangle;
    }
}

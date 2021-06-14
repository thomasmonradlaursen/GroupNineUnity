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
    void Start()
    {
        points = GetComponent<RandomPieceGenerator>().GetPoints();
        (DelaunayTriangle, DelaunayTriangle) supertriangles = SetupSupertriangles(GetComponent<RandomPieceGenerator>().boardSize);
        topSupertriangle = supertriangles.Item1;
        bottomSupertriangle = supertriangles.Item2;
        triangles.Add(topSupertriangle);
        triangles.Add(bottomSupertriangle);
        BowyerWatsonTriangulate(points, triangles);
    }
    void BowyerWatsonTriangulate(List<Vector3> points, List<DelaunayTriangle> triangles)
    {
        int numberOfLoops = 1;
        Debug.Log("Number of points: " + points.Count);
        foreach (Vector3 point in points)
        {
            Debug.Log("Number of triangles at round " + numberOfLoops + ": " + triangles.Count);
            List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
            foreach (DelaunayTriangle triangle in badTriangles)
            {
                if (Vector3.Distance(point, triangle.circumcenter) <= triangle.circumradius)
                {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> polygon = new List<Edge>();
            Debug.Log("Remove bad triangles " + numberOfLoops);
            foreach (DelaunayTriangle outerTriangle in badTriangles)
            {
                foreach (Edge outerEdge in outerTriangle.edges)
                {
                    bool isEdgeUnique = true;
                    foreach (DelaunayTriangle innerTriangle in badTriangles)
                    {
                        foreach (Edge innerEdge in innerTriangle.edges)
                        {
                            {
                                if(innerEdge.Equals(outerEdge) || innerTriangle.Equals(outerTriangle))
                                {
                                    Debug.Log("Found common edge" +  numberOfLoops);
                                    isEdgeUnique = false;
                                }
                            }
                        }
                    }
                    if(isEdgeUnique)
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
            foreach (Edge edge in polygon)
            {
                Debug.Log("Triangle added at round" + numberOfLoops);
                DelaunayTriangle newTriangle = new DelaunayTriangle();
                newTriangle = CreateTriangle(point, edge.coordinates.Item1, edge.coordinates.Item2);
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
        triangle.vertices = new Vector3[3] { pointOne, pointTwo, pointThree };
        triangle.edges = new Edge[3] { new Edge(), new Edge(), new Edge() };
        triangle.edges[0].coordinates = (pointOne, pointTwo);
        triangle.edges[1].coordinates = (pointTwo, pointThree);
        triangle.edges[2].coordinates = (pointThree, pointOne);
        triangle.edges[0].length = Vector3.Distance(pointOne, pointTwo);
        triangle.edges[1].length = Vector3.Distance(pointTwo, pointThree);
        triangle.edges[2].length = Vector3.Distance(pointThree, pointOne);
        (Vector3, float) centerAndRadius = circumscriber.GetCircumcenterAndCircumradius(pointOne, pointTwo, pointThree);
        triangle.circumcenter = centerAndRadius.Item1;
        triangle.circumradius = centerAndRadius.Item2;
        return triangle;
    }
}

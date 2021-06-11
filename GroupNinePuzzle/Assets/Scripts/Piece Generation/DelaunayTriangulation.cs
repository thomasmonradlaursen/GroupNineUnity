using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTriangle;

public class DelaunayTriangulation : MonoBehaviour
{
    Circumscribed circumscriber = new Circumscribed();
    List<Vector3> points = new List<Vector3>();
    public List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();
    public DelaunayTriangle supertriangle;
    void Start()
    {
        points = GetComponent<RandomPieceGenerator>().GetPoints();
        supertriangle = SetupSupertriangle(GetComponent<RandomPieceGenerator>().boardSize);
        //BowyerWatsonTriangulate(points, triangles);
    }
    void BowyerWatsonTriangulate(List<Vector3> points, List<DelaunayTriangle> triangles)
    {
        foreach (Vector3 point in points)
        {
            List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
            foreach(DelaunayTriangle triangle in badTriangles)
            {
                if (Vector3.Distance(point, triangle.circumcenter) <= triangle.circumradius)
                {
                    badTriangles.Add(triangle);
                }
            }
            DelaunayTriangle polygon = new DelaunayTriangle();
            for (int index = 0; index < badTriangles.Count; index++)
            {
                foreach (Vector3 vertex in badTriangles[index].vertices)
                {
                    //if(vertex.Equals())
                }
            }
            foreach (DelaunayTriangle triangle in badTriangles)
            {
                for(int index = 0; index < triangles.Count; index++)
                {
                    if(triangle.Equals(triangles[index]))
                    {
                        triangles.RemoveAt(index);
                        break;
                    }
                }
            }
            foreach (Edge edge in polygon.edges)
            {

            }
        }

    }
    DelaunayTriangle SetupSupertriangle(Vector2 boardSize)
    {
        float centerOfAlongX = boardSize.x / 2.0f;
        float centerOfAlongY = boardSize.y / 2.0f;
        Vector3 pointOne = new Vector3(-centerOfAlongX, -1.0f, 0.0f);
        Vector3 pointTwo = new Vector3(boardSize.x + centerOfAlongX, -1.0f, 0.0f);
        Vector3 pointThree = new Vector3(centerOfAlongX, 2 * boardSize.y + centerOfAlongY, 0.0f);
        return CreateTriangle(pointOne, pointTwo, pointThree);
    }
    DelaunayTriangle CreateTriangle(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree)
    {
        DelaunayTriangle triangle = new DelaunayTriangle();
        triangle.vertices = new Vector3[3]{pointOne, pointTwo, pointThree};
        triangle.edges = new Edge[3]{new Edge(), new Edge(), new Edge()};
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

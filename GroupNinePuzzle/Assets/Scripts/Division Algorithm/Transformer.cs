using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DivisionTriangles;

public static class Transformer
{
    public static List<DivisionTriangle> CenterTriangles(List<DivisionTriangle> triangles)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            Vector3 centroid = new Vector3();
            for(int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                centroid.x += triangles[triangleIndex].vertices[vertexIndex].x;
                centroid.y += triangles[triangleIndex].vertices[vertexIndex].y;
            }
            centroid /= triangles[triangleIndex].vertices.Length;
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                triangles[triangleIndex].vertices[vertexIndex].x -= centroid.x;
                triangles[triangleIndex].vertices[vertexIndex].y -= centroid.y;
            }
        }
        return triangles;
    }
    public static List<DivisionTriangle> RandomizeInitialPlacement(List<DivisionTriangle> triangles, Vector2 boardSize)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            float displacementX = Random.Range(-boardSize.x / 2, boardSize.x / 2);
            float displacementY = Random.Range(-boardSize.y / 2, boardSize.y / 2);
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                triangles[triangleIndex].vertices[vertexIndex].x += displacementX;
                triangles[triangleIndex].vertices[vertexIndex].y += displacementY;
            }
        }
        return triangles;
    }
    public static List<DivisionTriangle> RandomizeInitialOrientation(List<DivisionTriangle> triangles)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            float rotationTheta = Random.Range(0.0f, 2*Mathf.PI);
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                float rotatedX = triangles[triangleIndex].vertices[vertexIndex].x * Mathf.Cos(rotationTheta) - triangles[triangleIndex].vertices[vertexIndex].y * Mathf.Sin(rotationTheta);
                float rotatedY = triangles[triangleIndex].vertices[vertexIndex].x * Mathf.Sin(rotationTheta) + triangles[triangleIndex].vertices[vertexIndex].y * Mathf.Cos(rotationTheta);
                triangles[triangleIndex].vertices[vertexIndex].x = rotatedX;
                triangles[triangleIndex].vertices[vertexIndex].y = rotatedY;
            }
        }
        return triangles;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTriangle;

public static class PieceAligner
{
    public static List<DelaunayTriangle> CenterTriangles(List<DelaunayTriangle> triangles)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            float selectedX = triangles[triangleIndex].vertices[0].x;
            float selectedY = triangles[triangleIndex].vertices[0].y;
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                triangles[triangleIndex].vertices[vertexIndex].x -= selectedX;
                triangles[triangleIndex].vertices[vertexIndex].y -= selectedY;
            }
        }
        return triangles;
    }
    public static List<DelaunayTriangle> RandomizeInitialPlacement(List<DelaunayTriangle> triangles, Vector2 boardSize)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            float displacementX = Random.Range(-boardSize.x / 5, boardSize.x / 5);
            float displacementY = Random.Range(-boardSize.y / 5, boardSize.y / 5);
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                triangles[triangleIndex].vertices[vertexIndex].x += displacementX;
                triangles[triangleIndex].vertices[vertexIndex].y += displacementY;
            }
        }
        return triangles;
    }
    public static List<DelaunayTriangle> RandomizeInitialOrientation(List<DelaunayTriangle> triangles)
    {
        for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
        {
            float rotationTheta = Random.Range(0.0f, 360.0f);
            for (int vertexIndex = 0; vertexIndex < triangles[triangleIndex].vertices.Length; vertexIndex++)
            {
                triangles[triangleIndex].vertices[vertexIndex].x = triangles[triangleIndex].vertices[vertexIndex].x * Mathf.Cos(rotationTheta) - triangles[triangleIndex].vertices[vertexIndex].y * Mathf.Sin(rotationTheta);
                triangles[triangleIndex].vertices[vertexIndex].y = triangles[triangleIndex].vertices[vertexIndex].x * Mathf.Sin(rotationTheta) + triangles[triangleIndex].vertices[vertexIndex].y * Mathf.Cos(rotationTheta);
            }
        }
        return triangles;
    }
}

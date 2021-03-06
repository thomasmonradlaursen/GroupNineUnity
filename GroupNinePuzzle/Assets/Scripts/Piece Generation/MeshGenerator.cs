using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;
using UnityEditor;
using DivisionTriangles;

//Author: Gustav Nilsson Pedersen

public class MeshGenerator : MonoBehaviour

{
    public void GenerateMeshes(JSONPuzzle puzzle)
    {
        foreach (var piece in puzzle.pieces)
        {
            var mesh = new Mesh();
            var vertices = new Vector3[piece.corners.Length];
            var idx = 0;
            foreach (var corner in piece.corners)
            {
                vertices[idx].x = corner.coord.x;
                vertices[idx].y = corner.coord.y;
                idx++;
            }
            mesh.vertices = vertices;
            mesh.uv = GetComponent<MeshModel>().newUV;
            var verticesList = new List<Vector3>();
            foreach (var vertex in vertices)
            {
                verticesList.Add(vertex);
            }
            var triangles = PolygonTriangulation.TriangulateConcavePolygon(verticesList);
            var trianglesAsIntArray = new int[triangles.Count * 3];
            idx = 0;
            foreach (var triangle in triangles)
            {
                trianglesAsIntArray[idx] = Array.IndexOf(vertices, triangle.vertex1.GetXY());
                trianglesAsIntArray[idx + 1] = Array.IndexOf(vertices, triangle.vertex2.GetXY());
                trianglesAsIntArray[idx + 2] = Array.IndexOf(vertices, triangle.vertex3.GetXY());
                idx += 3;
            }
            mesh.triangles = trianglesAsIntArray;
            GetComponent<MeshModel>().meshes.Add(mesh);
        }
    }
    public void MeshesFromRandom()
    {
        foreach (DivisionTriangle triangle in GetComponentInParent<DivisionModel>().triangles)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = triangle.vertices;
            mesh.triangles = new int[3] { 0, 2, 1 };
            GetComponent<MeshModel>().meshes.Add(mesh);
        }
    }
}

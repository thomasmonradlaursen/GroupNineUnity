using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class MeshFromJsonGenerator : MonoBehaviour

{
    JSONPuzzle Puzzle;
    List<Mesh> meshArray = new List<Mesh>();
    int[] triangles;
    MiscellaneousMath mM = new MiscellaneousMath();
    public string selected = "empty";
    public GameObject selectedObject = null;
    public string previousSelected = "empty";
    public GameObject previousSelectedObject = null;
    void Start()
    {
        GenerateMeshes();
        CreatePieces();
    }

    void CreatePieces()
    {
        
        var idx = 0;
        foreach (var mesh in meshArray)
        {
            var newGameObject = new GameObject("Piece " + Puzzle.pieces[idx].piece);
            newGameObject.AddComponent<MeshFilter>();
            newGameObject.GetComponent<MeshFilter>().mesh = mesh;
            newGameObject.AddComponent<MeshRenderer>();
            newGameObject.AddComponent<MeshCollider>();
            newGameObject.AddComponent<Translation>();
            newGameObject.AddComponent<Rotation>();
            newGameObject.AddComponent<PieceInfo>();
            newGameObject.transform.parent = this.transform;

            var renderer = newGameObject.GetComponent<MeshRenderer>();
            var test = renderer.materials;
            test[0].color = Color.blue;

            idx++;
        }
    }

    void GenerateMeshes()
    {
        var jsonDeserializer = GetComponent<JSONDeserializer>();

        jsonDeserializer.Start();

        Puzzle = jsonDeserializer.Puzzle;

        foreach (var piece in Puzzle.pieces)
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

            var verticesList = new List<Vector3>();
            foreach (var vertex in vertices)
            {
                verticesList.Add(vertex);
                //Debug.Log($"vertex: a ({vertex.x}, {vertex.y})");
            }

            var triangles = PolygonTriangulation.TriangulateConcavePolygon(verticesList);

            var trianglesAsIntArray = new int[triangles.Count * 3];
            idx = 0;
            foreach (var triangle in triangles)
            {
                /*
                Debug.Log("x value of vertex1 in triangle: " + triangle.vertex1.GetXY().x);
                Debug.Log("y value of vertex1 in triangle: " + triangle.vertex1.GetXY().y);
                Debug.Log("x value of vertex2 in triangle: " + triangle.vertex2.GetXY().x);
                Debug.Log("y value of vertex2 in triangle: " + triangle.vertex2.GetXY().y);
                Debug.Log("x value of vertex3 in triangle: " + triangle.vertex3.GetXY().x);
                Debug.Log("y value of vertex3 in triangle: " + triangle.vertex3.GetXY().y);
                */
                trianglesAsIntArray[idx] = Array.IndexOf(vertices, triangle.vertex1.GetXY());
                trianglesAsIntArray[idx + 1] = Array.IndexOf(vertices, triangle.vertex2.GetXY());
                trianglesAsIntArray[idx + 2] = Array.IndexOf(vertices, triangle.vertex3.GetXY());
                idx += 3;
            }

            //Debug.Log($"List of vertices:");
            foreach (var vertex in vertices)
            {
                //Debug.Log($"{vertex.x}, {vertex.y}");
            }

            foreach (var index in trianglesAsIntArray)
            {
                //Debug.Log("index of vertex: " + index);
                //Debug.Log($"vertex at index: {vertices[index].x}, {vertices[index].y}");
            }

            mesh.triangles = trianglesAsIntArray;

            meshArray.Add(mesh);
        }
    }
}

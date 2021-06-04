﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour

{
    MiscellaneousMath mM = new MiscellaneousMath();
    List<Mesh> meshArray = new List<Mesh>();
    Vector3[] vertices;
    int[] triangles;
    public string selected = "empty";
    void Start()
    {
        GenerateMeshes();
        CreatePieceObjects();       
    }
    void CreatePieceObjects()
    {
        int idx = 0;
        foreach(var mesh in meshArray){
            var newGameObject = new GameObject("Piece " + idx);
            newGameObject.AddComponent<MeshFilter>();
            newGameObject.GetComponent<MeshFilter>().mesh = mesh;
            newGameObject.AddComponent<MeshRenderer>();
            newGameObject.AddComponent<MeshCollider>();
            newGameObject.AddComponent<Translation>();
            newGameObject.AddComponent<Rotation>();
            newGameObject.AddComponent<PieceInfo>();
            newGameObject.AddComponent<SnapIntoPlace>();
            newGameObject.AddComponent<SphereCollider>();
            newGameObject.GetComponent<SphereCollider>().radius = 1.1f;
            newGameObject.GetComponent<SphereCollider>().center = newGameObject.GetComponent<PieceInfo>().centerOfMass;
            newGameObject.transform.parent = this.transform;
            idx++;
        } 
    }

    void GenerateMeshes() 
    {
        var verticesGenerator = GetComponent<VerticesGenerator>();

        verticesGenerator.Start();

        vertices = verticesGenerator.vertices;
        // Debug.Log("length: " + vertices.Length + Environment.NewLine);

        var noOfVertical = VerticesGenerator.numberOfVerticalVertices;
        var noOfHorizontal = VerticesGenerator.numberOfHorizontalVertices;

        for(int idx = 0; idx < (noOfVertical-1)*(noOfHorizontal); idx++){ // should only be -1 instead of -2 when we have coordinates of vertices on edges in array as well

            var mesh = new Mesh();
            mesh.vertices = new Vector3[]{
                vertices[idx],
                vertices[idx+noOfHorizontal], // one higher vertically
                vertices[idx+1], // one higher horizontally
                vertices[idx+noOfHorizontal+1], // one higher vertically and horizontally
            };
            // Debug.Log(idx + "")
            mesh.triangles = new int[]{
                0,1,2, //clockwise
                1,3,2  //clockwise
            };
            meshArray.Add(mesh);

            if((idx+2)%(noOfHorizontal) == 0 && idx != 0){ 
                idx++; // skips rightmost vertice in row
            }
        }
    }
}
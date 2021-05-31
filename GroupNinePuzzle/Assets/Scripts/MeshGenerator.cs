using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour

{

    List<Mesh> meshArray = new List<Mesh>();
    Vector3[] vertices;
    int[] triangles;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HERE ALKSDÆJFASJDFÆLASJDFÆLASKJDFALÆSDFJKAÆLSKDFJASDJFLÆASDKJFÆLKASDJFÆASJDKFÆASJDFLÆASJDFÆLKASJDFÆLKAJSDFLÆKJASDKLÆFASDF");
        // mesh = new Mesh();
        
        GenerateMeshes();

        var idx = 0;
        foreach(var mesh in meshArray){
            // GetComponent<MeshFilter>().mesh = mesh;
            var newGameObject = new GameObject("mesh" + idx);
            newGameObject.AddComponent<MeshFilter>();
            newGameObject.GetComponent<MeshFilter>().mesh = mesh;
            // meshFilter.mesh = mesh;
            newGameObject.AddComponent<MeshRenderer>();
            newGameObject.AddComponent<MeshCollider>();
            // Instantiate(newGameObject);
            // newGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            newGameObject.AddComponent<DragNDrop>();
            newGameObject.AddComponent<RotationOfMesh>();

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
            // Debug.Log("X :" + idx%noOfHorizontal);
            // Debug.Log("Y: " + Math.Floor((decimal) idx/noOfHorizontal));
            // Debug.Log("X :" + vertices[idx].x);
            // Debug.Log("Y: " + vertices[idx].y);
            
            // Debug.Log("c1: " + idx);
            // Debug.Log("c1: " + idx);
            // Debug.Log("c2: " + (idx+noOfHorizontal));
            // Debug.Log("c3: " + (idx+1));
            // Debug.Log("c4: " + (idx+noOfHorizontal+1));

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
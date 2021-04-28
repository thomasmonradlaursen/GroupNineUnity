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

            idx++;
        }        
    }


    void GenerateMeshes() 
    {
        var verticesGenerator = GetComponent<VerticesGenerator>();

        vertices = verticesGenerator.vertices;

        var noOfVertical = VerticesGenerator.numberOfVerticalVertices;
        var noOfHorizontal = VerticesGenerator.numberOfHorizontalVertices;

        for(int idx = 0; idx < (noOfVertical*(noOfHorizontal-1)); idx++){ // should only be -1 instead of -2 when we have coordinates of vertices on edges in array as well
            // Debug.Log("X :" + idx%noOfHorizontal);
            // Debug.Log("Y: " + Math.Floor((decimal) idx/noOfHorizontal));
            // Debug.Log("X :" + vertices[idx].x);
            // Debug.Log("Y: " + vertices[idx].y);

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

            if(idx%(noOfHorizontal-2) == 0 && idx != 0){ // Should be -2 when we have coordinates of vertices on edges in array as well
                idx++; // skips rightmost vertice in row
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOfMesh : MonoBehaviour
{
    
    private Transform target;
    private Mesh theMesh;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;


    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && target!=null)
        {
            Debug.Log("Rotation pressed");
            MatrixRotation();
        }

        if(Input.GetKeyDown(KeyCode.A) && target!=null)
        {
            Debug.Log("A has been pressed");
            ScaleMesh();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B has been pressed");
        }
    }

    void MatrixRotation()
    {
        Debug.Log("Entered Rotation");
        
        float rotationTheta = 1.0f;

        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].y * Mathf.Sin(rotationTheta) - originalVertices[index].x * Mathf.Cos(rotationTheta);
        }

        LogVertices(rotatedVertices, "Rotated: ");
        LogVertices(originalVertices, "Original: ");

        theMesh.SetVertices(rotatedVertices);
    }

    void ScaleMesh()
    {
        Debug.Log("Entered scaling");
        
        float scalingFactor = 1.2f;

        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index] = scalingFactor * originalVertices[index];
        }

        LogVertices(rotatedVertices, "Scaled: ");
        LogVertices(originalVertices, "Original: ");

        theMesh.SetVertices(rotatedVertices);
        target.GetComponent<MeshCollider>().sharedMesh = theMesh;
    }

    void OnMouseDown()
    {
        Debug.Log("ROTATION: Mouse pressed down");

        if(!target)
        {
            target = this.transform;
        }
        
        theMesh = target.GetComponent<MeshFilter>().mesh;
     
        originalVertices = new Vector3[theMesh.vertices.Length];
        originalVertices = theMesh.vertices;
        rotatedVertices = new Vector3[ originalVertices.Length];

        LogVertices(theMesh.vertices, "Current: ");
    }

    void LogVertices(Vector3[] vertices, string label) 
    {
        foreach(Vector3 vertex in vertices)
        {
            Debug.Log(label + vertex);
        }
    }
}

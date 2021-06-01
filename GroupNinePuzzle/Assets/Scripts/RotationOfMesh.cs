using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOfMesh : MonoBehaviour
{
    
    private GameObject target;
    private Mesh theMesh;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;


    void Update() 
    {
        if (Input.GetKey(KeyCode.UpArrow) && target!=null)
        {
            Debug.Log("Rotation pressed");
            RotateMesh();
        }

        if(Input.GetKeyDown(KeyCode.A) && target!=null)
        {
            Debug.Log("A has been pressed");
            ScaleMesh();
        }

        if(Input.GetKeyDown(KeyCode.B) && target!=null)
        {
            Debug.Log("B has been pressed");
            InverseVertices();
        }
    }

    void RotateMesh()
    {
        Debug.Log("Entered Rotation");
        
        float rotationTheta = (10*Mathf.PI)/180;
        Debug.Log("Rotation theta: " + rotationTheta);

        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationTheta) + originalVertices[index].y * Mathf.Cos(rotationTheta);
        }

        LogVertices(rotatedVertices, "Rotated: ");

        theMesh.SetVertices(rotatedVertices);
        //target.GetComponent<MeshCollider>().sharedMesh = theMesh;
    }

    void InverseVertices()
    {
        Debug.Log("Entered scaling");

        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index] = -originalVertices[index];
        }

        LogVertices(rotatedVertices, "Inverted: ");

        theMesh.SetVertices(rotatedVertices);
        target.GetComponent<MeshCollider>().sharedMesh = theMesh;
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

        theMesh.SetVertices(rotatedVertices);
        target.GetComponent<MeshCollider>().sharedMesh = theMesh;
    }

    void OnMouseUp()
    {
        SelectMesh();
    }

    void SelectMesh()
    {
        Debug.Log("ROTATION: Selected mesh");

        target = this.gameObject;
        
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

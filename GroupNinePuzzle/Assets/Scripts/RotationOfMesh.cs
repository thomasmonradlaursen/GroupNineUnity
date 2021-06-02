using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOfMesh : MonoBehaviour
{
    public Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;

    void Update() 
    {
        if(Input.GetKey(KeyCode.R) && mesh != null)
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                Debug.Log("Rotation clockwise");
                RotateMesh((1*Mathf.PI)/180);
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                Debug.Log("Rotation counter clockwise");
                RotateMesh(-(1*Mathf.PI)/180);
            }
        }
        if(Input.GetKeyUp(KeyCode.R)) Clear();   
    }

    void RotateMesh(float rotationIntervalAndDirection)
    {

        Vector3 centerOfMass = CalculateCenterOfMass();
        CentralizeVertices(centerOfMass);

        Debug.Log("Entered Rotation");
        
        float rotationTheta = rotationIntervalAndDirection;
        Debug.Log("Rotation theta: " + rotationTheta);

        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationTheta) + originalVertices[index].y * Mathf.Cos(rotationTheta);
        }

        RestorePositionOfVertices(centerOfMass);

        LogVertices(rotatedVertices, "Rotated: ");

        mesh.SetVertices(rotatedVertices);
        originalVertices = mesh.vertices;

        GetComponentInParent<MeshCollider>().sharedMesh = mesh;
    }
    
    void OnMouseUp() {
       
        mesh = GetComponentInParent<MeshFilter>().mesh;
     
        originalVertices = new Vector3[mesh.vertices.Length];
        originalVertices = mesh.vertices;
        rotatedVertices = new Vector3[ originalVertices.Length];

        LogVertices(mesh.vertices, "Current: ");
    }

    void Clear()
    {
        mesh = null;
        originalVertices = null;
        rotatedVertices = null;
    }

    void LogVertices(Vector3[] vertices, string label) 
    {
        foreach(Vector3 vertex in vertices)
        {
            Debug.Log(label + vertex);
        }
    }
    
    Vector3 CalculateCenterOfMass()
    {
        float xCoordinateForCenter = 0.0f;
        float yCoordinateForCenter = 0.0f;
        foreach(Vector3 vertex in originalVertices)
        {
            xCoordinateForCenter += vertex.x;
            yCoordinateForCenter += vertex.y;
        }
        xCoordinateForCenter /= originalVertices.Length;
        yCoordinateForCenter /= originalVertices.Length;
        return new Vector3(xCoordinateForCenter, yCoordinateForCenter, 0.0f);
    }

    void CentralizeVertices(Vector3 centerOfMass)
    {
        Debug.Log("Center of mass: " + centerOfMass);
        for(int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x -= centerOfMass.x;
            originalVertices[index].y -= centerOfMass.y;
        }
    }

    void RestorePositionOfVertices(Vector3 centerOfMass)
    {
        for(int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x += centerOfMass.x;
            rotatedVertices[index].y += centerOfMass.y;
        }
    } 
}

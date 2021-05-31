using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOfMesh : MonoBehaviour
{
    private Mesh theMesh;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;
    public float rotatedAngleY = 0.0f;
    private  Vector3 mouseOffset;
    private float mouseZcoord = -10;

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Rotation pressed");
            RotateMesh();
        }
    }
    
    void RotateMesh() 
    {
        Quaternion qAngle = Quaternion.AngleAxis(rotatedAngleY, Vector3.up );
        for (int vertex = 0; vertex < originalVertices.Length; vertex ++)
        {
            rotatedVertices[vertex] = qAngle * originalVertices[vertex];
        }
        theMesh.vertices = rotatedVertices;
    }

    void OnMouseDown(){
        mouseZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - MouseWorldPosition();
        transform.position = MouseWorldPosition() + mouseOffset;

        theMesh = gameObject.GetComponent<MeshFilter>().mesh as Mesh;
        
        originalVertices = new Vector3[theMesh.vertices.Length];
        originalVertices = theMesh.vertices;
        
        rotatedVertices = new Vector3[originalVertices.Length];
    }

    private Vector3 MouseWorldPosition(){
        Vector3 mousePoint = Input.mousePosition;
        
        mousePoint.z = mouseZcoord;
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePoint);

        return mouseWorldPosition; 
    }
}

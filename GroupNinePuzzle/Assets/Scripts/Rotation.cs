﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();
    void FixedUpdate()
    {
        if(this.name.Equals(this.GetComponentInParent<MeshFromJsonGenerator>().selected))
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                RotateMesh((1*Mathf.PI)/180);
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                RotateMesh(-(1*Mathf.PI)/180);
            }
            if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                Debug.Log("# ROTATION #");
                Debug.Log("Vertices of " + this.name + " after rotation:");
                //LogVertices(mesh.vertices);
            }
        }
    }
    void RotateMesh(float rotationIntervalAndDirection)
    {
        float area = 0.6f;
        Vector3 centroid = miscellaneousMath.CalculateCentroid(originalVertices, area);
        Debug.Log("Area: " + area);
        Debug.Log("Centroid: " + centroid);
        CentralizeVertices(centroid);
        float rotationTheta = rotationIntervalAndDirection;
        for(int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationTheta) + originalVertices[index].y * Mathf.Cos(rotationTheta);
        }
        RestorePositionOfVertices(centroid);
        mesh.SetVertices(rotatedVertices);
        originalVertices = mesh.vertices;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void OnMouseDown()
    {
        this.GetComponentInParent<MeshFromJsonGenerator>().selected = "empty";
    }
    void OnMouseUp()
    {
        UpdateMeshInfromation();
    }
    void UpdateMeshInfromation()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = new Vector3[mesh.vertices.Length];
        originalVertices = mesh.vertices;
        rotatedVertices = new Vector3[ originalVertices.Length];
        this.GetComponentInParent<MeshFromJsonGenerator>().selected = this.name;
    }
    
    void CentralizeVertices(Vector3 centerOfMass)
    {
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
    void LogVertices(Vector3[] vertices) 
    {
        foreach(Vector3 vertex in vertices)
        {
            Debug.Log(vertex);
        }
    } 
}

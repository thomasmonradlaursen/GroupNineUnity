using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Mesh mesh;
    public LineRenderer lineRenderer;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();
    void FixedUpdate()
    {
        if (this.name.Equals(this.GetComponentInParent<MeshFromJsonGenerator>().selected))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RotateMesh((1 * Mathf.PI) / 180);   //counter-clockwise
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                RotateMesh(-(1 * Mathf.PI) / 180);  //clockwise
            }
        }
    }
    public void RotateMesh(float rotationIntervalAndDirection)
    {
        Vector3 centroid = GetComponent<PieceInfo>().centroid;
        CentralizeVertices(centroid);
        float rotationTheta = rotationIntervalAndDirection;
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationTheta) + originalVertices[index].y * Mathf.Cos(rotationTheta);
        }
        RestorePositionOfVertices(centroid);
        mesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        originalVertices = mesh.vertices;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void OnMouseDown()
    {

        // set the currently selected piece as previously selected piece
        var currentlySelected = this.GetComponentInParent<MeshFromJsonGenerator>().selected;
        var currentlySelectedObject = this.GetComponentInParent<MeshFromJsonGenerator>().selectedObject;

        if (currentlySelectedObject != null && currentlySelected != this.name)
        {
            var renderer1 = currentlySelectedObject.GetComponent<MeshRenderer>();
            var materials1 = renderer1.materials;
            materials1[0].color = Color.blue;
            this.GetComponentInParent<MeshFromJsonGenerator>().previousSelected = currentlySelected;
            this.GetComponentInParent<MeshFromJsonGenerator>().previousSelectedObject = currentlySelectedObject;
        }


        // Set the new piece as currently selected piece 
        currentlySelected = this.name;
        currentlySelectedObject = this.gameObject;
        this.GetComponentInParent<MeshFromJsonGenerator>().selected = currentlySelected;
        this.GetComponentInParent<MeshFromJsonGenerator>().selectedObject = currentlySelectedObject;
        if (currentlySelectedObject != null && currentlySelected == this.name)
        {
            var renderer2 = this.GetComponent<MeshRenderer>();
            var materials2 = renderer2.materials;
            materials2[0].color = Color.red;
        }
    }
    void OnMouseUp()
    {
        UpdateMeshInformation();
    }
    void UpdateMeshInformation()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        lineRenderer = GetComponent<LineRenderer>();
        originalVertices = new Vector3[mesh.vertices.Length];
        originalVertices = mesh.vertices;
        rotatedVertices = new Vector3[ originalVertices.Length];
        this.GetComponentInParent<MeshFromJsonGenerator>().selected = this.name;
    }
    
    void CentralizeVertices(Vector3 centroid)
    {
        for (int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x -= centroid.x;
            originalVertices[index].y -= centroid.y;
        }
    }
    void RestorePositionOfVertices(Vector3 centroid)
    {
        for (int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x += centroid.x;
            rotatedVertices[index].y += centroid.y;
        }
    }
    void LogVertices(Vector3[] vertices)
    {
        foreach (Vector3 vertex in vertices)
        {
            Debug.Log(vertex);
        }
    } 

    void LogAngles(float[] angles) 
    {
        foreach(float angle in angles)
        {
            Debug.Log(angle);
        }
    } 
}

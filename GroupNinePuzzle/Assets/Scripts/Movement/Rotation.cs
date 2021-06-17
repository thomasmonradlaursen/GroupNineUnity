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
        if (this.name.Equals(this.GetComponentInParent<PuzzleModel>().selectedObject.name))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RotateMesh((1 * Mathf.PI) / 180);  //counter-clockwise
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                RotateMesh(-(1 * Mathf.PI) / 180); //Clockwise
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                this.GetComponent<PieceInfo>().RemoveConnectionsToOtherPieces();
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
        var currentlySelectedObject = this.GetComponentInParent<PuzzleModel>().selectedObject;

        if (currentlySelectedObject != null && currentlySelectedObject.name != this.name)
        {
            var renderer1 = currentlySelectedObject.GetComponent<MeshRenderer>();
            var materials1 = renderer1.materials;
            materials1[0].color = Color.blue;
            this.GetComponentInParent<PuzzleModel>().previousSelectedObject = currentlySelectedObject;
        }
        // Set the new piece as currently selected piece 
        currentlySelectedObject = this.gameObject;
        this.GetComponentInParent<PuzzleModel>().selectedObject = currentlySelectedObject;
        if (currentlySelectedObject != null && currentlySelectedObject.name == this.name)
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
        rotatedVertices = new Vector3[originalVertices.Length];
        this.GetComponentInParent<PuzzleModel>().selectedObject.name = this.name;
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
}

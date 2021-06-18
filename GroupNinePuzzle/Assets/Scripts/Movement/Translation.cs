using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Translation : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    private Vector3 mouseOffset;
    private float mouseZcoord = -10;
    private bool pieceHasBeenMoved = false;

    void OnMouseDown()
    {
        mouseZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - MouseWorldPosition();

        // set the currently selected piece as previously selected piece
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
        pieceHasBeenMoved = false;
    }
    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + mouseOffset;
        pieceHasBeenMoved = true;
    }
    void OnMouseUp()
    {
        CalculateVerticesAfterTranslation();
        CalculateCentroidAfterTranslation();
        if(pieceHasBeenMoved){
            this.GetComponent<PieceInfo>().RemoveConnectionsToOtherPieces();
        }
        pieceHasBeenMoved = false;
    }
    private Vector3 MouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZcoord;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePoint);
        return mouseWorldPosition; 
    }
    public void CalculateVerticesAfterTranslation()
    {
        transform.position = Vector3.zero;
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + MouseWorldPosition().x + mouseOffset.x;
            translatedVertices[index].y = mesh.vertices[index].y + MouseWorldPosition().y + mouseOffset.y;
        }
        mesh.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        GetComponentInParent<MeshCollider>().sharedMesh = mesh;
    }
    public void CalculateCentroidAfterTranslation()
    {
        PieceInfo piece = GetComponentInParent<PieceInfo>();
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        Vector3 transCentroid = mM.CalculateCentroid(mesh.vertices, piece.GetComponentInParent<PieceInfo>().area);
        GetComponent<PieceInfo>().centroid = transCentroid;
    }
}
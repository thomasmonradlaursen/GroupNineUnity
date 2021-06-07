using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Translation : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    private Vector3 mouseOffset;
    private float mouseZcoord = -10;

    void OnMouseDown()
    {
        mouseZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - MouseWorldPosition();
    }
    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + mouseOffset;
    }
    void OnMouseUp()
    {
        CalculateVerticesAfterTranslation();
        CalculateCentroidAfterTranslation();
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
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + MouseWorldPosition().x + mouseOffset.x;
            translatedVertices[index].y = mesh.vertices[index].y + MouseWorldPosition().y + mouseOffset.y;
        }
        mesh.SetVertices(translatedVertices);
        GetComponentInParent<MeshCollider>().sharedMesh = mesh;
    }
    public void CalculateCentroidAfterTranslation()
    {
        PieceInfo piece = GetComponentInParent<PieceInfo>();
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        Vector3 transCentroid = mM.CalculateCentroid(mesh.vertices, piece.GetComponentInParent<PieceInfo>().area);
        GetComponent<PieceInfo>().centroid = transCentroid;
        Debug.Log("new center : "+GetComponent<PieceInfo>().centroid);
        /*
        Vector3 translatedCentroid = GetComponent<PieceInfo>().centroid;
        translatedCentroid.x += (MouseWorldPosition().x + mouseOffset.x);
        translatedCentroid.y += (MouseWorldPosition().y + mouseOffset.y);
        GetComponent<PieceInfo>().centroid = translatedCentroid;
        */
    }
    void LogVertices(Vector3[] vertices) 
    {
        foreach(Vector3 vertex in vertices)
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

        void LogSides(float[] sides) 
    {
        foreach(float side in sides)
        {
            Debug.Log(side);
        }
    }
}
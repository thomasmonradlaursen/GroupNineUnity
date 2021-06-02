using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Translation : MonoBehaviour
{
    private  Vector3 mouseOffset;
    private float mouseZcoord = -10;

    void OnMouseDown()
    {
        mouseZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouseOffset = gameObject.transform.position - MouseWorldPosition();
    }

    private Vector3 MouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZcoord;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePoint);
        return mouseWorldPosition; 
    }

    void OnMouseDrag()
    {
        transform.position = MouseWorldPosition() + mouseOffset;
    }
    void OnMouseUp() {
        CalculateVerticesAfterTranslation();
    }
    void CalculateVerticesAfterTranslation()
    {
        transform.position = Vector3.zero;
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        LogVertices(mesh.vertices, "OnMouseDown, before: ");
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + MouseWorldPosition().x + mouseOffset.x;
            translatedVertices[index].y = mesh.vertices[index].y + MouseWorldPosition().y + mouseOffset.y;
        }
        mesh.SetVertices(translatedVertices);
        LogVertices(mesh.vertices, "OnMouseUp, after: ");
        GetComponentInParent<MeshCollider>().sharedMesh = mesh;
    }

    void LogVertices(Vector3[] vertices, string label) 
    {
        foreach(Vector3 vertex in vertices)
        {
            Debug.Log(label + vertex);
        }
    }
}
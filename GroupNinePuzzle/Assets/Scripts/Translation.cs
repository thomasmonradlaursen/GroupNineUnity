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
    void OnMouseUp() {
        CalculateVerticesAfterTranslation();
    }
    private Vector3 MouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZcoord;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePoint);
        return mouseWorldPosition; 
    }
    void CalculateVerticesAfterTranslation()
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
        var angles = mM.CalculateAnglesFromMesh(mesh);
        Debug.Log("# ANGLES #");
        Debug.Log("Angles of " + this.name + " after translation:");
        LogAngles(angles);

        Debug.Log("# TRANSLATION #");
        Debug.Log("Vertices of " + this.name + " after translation:");
        LogVertices(mesh.vertices);
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
}
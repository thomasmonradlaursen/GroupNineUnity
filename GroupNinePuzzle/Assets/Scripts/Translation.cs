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
                // this.GetComponentInParent<MeshFromJsonGenerator>().selected = "empty";

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
        var angles = mM.CalculateAnglesFromMesh(mesh);
        /*
        Debug.Log("# TRANSLATION #");
        Debug.Log("Vertices of " + this.name + " after translation:");
        LogVertices(mesh.vertices);
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
}
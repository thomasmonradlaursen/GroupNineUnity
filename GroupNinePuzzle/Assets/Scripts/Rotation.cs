using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Mesh mesh;
    public LineRenderer lineRenderer;
    private Vector3[] originalVertices;
    private Vector3[] rotatedVertices;
    void FixedUpdate()
    {
        if (this.name.Equals(this.GetComponentInParent<MeshFromJsonGenerator>().selected))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RotateMesh((1 * Mathf.PI) / 180);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                RotateMesh(-(1 * Mathf.PI) / 180);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                /*
                Debug.Log("# ROTATION #");
                Debug.Log("Vertices of " + this.name + " after rotation:");
                LogVertices(mesh.vertices);
                */
                Debug.Log("# ANGLES #");
                Debug.Log("Angles of " + this.name + " after rotation:");
                LogAngles(this.GetComponent<PieceInfo>().angles);
            }
        }
    }
    void RotateMesh(float rotationIntervalAndDirection)
    {
        Vector3 centerOfMass = CalculateCenterOfMass();
        CentralizeVertices(centerOfMass);
        float rotationTheta = rotationIntervalAndDirection;
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationTheta) - originalVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationTheta) + originalVertices[index].y * Mathf.Cos(rotationTheta);
        }
        RestorePositionOfVertices(centerOfMass);
        mesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        originalVertices = mesh.vertices;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void OnMouseDown()
    {
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
    void OnMouseUp()
    {
        // mesh = GetComponent<MeshFilter>().mesh;

        // // var renderer = GetComponent<MeshRenderer>();
        // // var test = renderer.materials;
        // // test[0].color = Color.red;



        UpdateMeshInfromation();
    }
    void UpdateMeshInfromation()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        lineRenderer = GetComponent<LineRenderer>();
        originalVertices = new Vector3[mesh.vertices.Length];
        originalVertices = mesh.vertices;
        rotatedVertices = new Vector3[originalVertices.Length];


        // // set the currently selected piece as previously selected piece
        // var currentlySelected = this.GetComponentInParent<MeshFromJsonGenerator>().selected;
        // var currentlySelectedObject = this.GetComponentInParent<MeshFromJsonGenerator>().selectedObject;

        // if (currentlySelectedObject != null && currentlySelected != this.name)
        // {
        //     var renderer1 = currentlySelectedObject.GetComponent<MeshRenderer>();
        //     var materials1 = renderer1.materials;
        //     materials1[0].color = Color.blue;
        //     this.GetComponentInParent<MeshFromJsonGenerator>().previousSelected = currentlySelected;
        //     this.GetComponentInParent<MeshFromJsonGenerator>().previousSelectedObject = currentlySelectedObject;
        // }


        // // Set the new piece as currently selected piece 
        // currentlySelected = this.name;
        // currentlySelectedObject = this.gameObject;
        // this.GetComponentInParent<MeshFromJsonGenerator>().selected = currentlySelected;
        // this.GetComponentInParent<MeshFromJsonGenerator>().selectedObject = currentlySelectedObject;
        // if (currentlySelectedObject != null && currentlySelected == this.name)
        // {
        //     var renderer2 = this.GetComponent<MeshRenderer>();
        //     var materials2 = renderer2.materials;
        //     materials2[0].color = Color.red;
        // }
    }
    Vector3 CalculateCenterOfMass()
    {
        float xCoordinateForCenter = 0.0f;
        float yCoordinateForCenter = 0.0f;
        foreach (Vector3 vertex in originalVertices)
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
        for (int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x -= centerOfMass.x;
            originalVertices[index].y -= centerOfMass.y;
        }
    }
    void RestorePositionOfVertices(Vector3 centerOfMass)
    {
        for (int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x += centerOfMass.x;
            rotatedVertices[index].y += centerOfMass.y;
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Gustav Nilsson Pedersen

public class PieceInfo : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    public float[] angles;
    public List<Pair> thetaAngles;
    public float[] lengths;
    public float area;
    public Vector3 centroid;
    public Vector3[] vertices;
    public void CalculateInformation()
    {
        angles = mM.CalculateAnglesFromMesh(GetComponent<MeshFilter>().mesh);
        lengths = mM.CalculateSideLengthsFromMesh(GetComponent<MeshFilter>().mesh);
        area = mM.CalculateAreaFromMesh(GetComponent<MeshFilter>().mesh);
        centroid = mM.CalculateCentroid(GetComponent<MeshFilter>().mesh.vertices, area);
    }

    public (float, float) GetMaximumAndMinimumXCoordinate()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        float maximum = vertices[0].x;
        float minimum = vertices[0].x;
        foreach (Vector3 vertex in vertices)
        {
            if (vertex.x < minimum) minimum = vertex.x;
            if (vertex.x > maximum) maximum = vertex.x;
        }
        return (minimum, maximum);
    }

    public (float, float) GetMaximumAndMinimumYCoordinate()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        float maximum = vertices[0].y;
        float minimum = vertices[0].y;
        foreach (Vector3 vertex in vertices)
        {
            if (vertex.y < minimum) minimum = vertex.y;
            if (vertex.y > maximum) maximum = vertex.y;
        }
        return (minimum, maximum);
    }

    public void RemoveConnectionsToOtherPieces()    
    {
        // Remove existing connections
        foreach (var pieceName in GetComponentInParent<PuzzleModel>().connectedPieces[this.name])
        {
            GetComponentInParent<PuzzleModel>().connectedPieces[pieceName].Remove(this.name);
        }
        GetComponentInParent<PuzzleModel>().connectedPieces[this.name].Clear();
    }
}

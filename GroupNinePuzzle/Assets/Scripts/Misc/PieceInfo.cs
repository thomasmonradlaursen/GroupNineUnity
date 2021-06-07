using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    public float[] angles;
    public float[] lengths;
    public float area;
    public Vector3 centroid;
    public Mesh mesh;
    public MeshRenderer rend;
    public MeshCollider collie;
    public MeshFilter meshFilter;
    public Translation translation;
    public Rotation rotation;
    public SnapIntoPlace snap;

    void Start()
    {
        angles = mM.CalculateAnglesFromMesh(GetComponent<MeshFilter>().mesh); 
        lengths = mM.CalculateSideLengthsFromMesh(GetComponent<MeshFilter>().mesh);
        area = mM.CalculateAreaFromMesh(GetComponent<MeshFilter>().mesh);
        centroid = mM.CalculateCentroid(GetComponent<MeshFilter>().mesh.vertices, area);
    }
}

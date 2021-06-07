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
<<<<<<< Updated upstream:GroupNinePuzzle/Assets/Scripts/PieceInfo.cs

        meshFilter = new MeshFilter();
        mesh = meshFilter.mesh;
        rend = new MeshRenderer();
        var test = rend.materials;
        test[0].color = Color.blue;
        collie = new MeshCollider();
        translation = new Translation();
        rotation = new Rotation();
        snap = new SnapIntoPlace();
=======
        
>>>>>>> Stashed changes:GroupNinePuzzle/Assets/Scripts/Misc/PieceInfo.cs
    }
}

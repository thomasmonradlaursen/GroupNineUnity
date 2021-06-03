using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    public float[] angles {get; set;}
    public float[] lengths {get; set;}
    public float area {get; set;}
    public Vector3 centerOfMass {get; set;}


    // Start is called before the first frame update
    void Start()
    {
        angles = mM.CalculateAnglesFromMesh(GetComponent<MeshFilter>().mesh); 
        lengths = mM.CalculateSideLengthsFromMesh(GetComponent<MeshFilter>().mesh);
        area = mM.CalculateAreaFromMesh(GetComponent<MeshFilter>().mesh);
        centerOfMass = mM.CalculateCenterOfMass(GetComponent<MeshFilter>().mesh); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

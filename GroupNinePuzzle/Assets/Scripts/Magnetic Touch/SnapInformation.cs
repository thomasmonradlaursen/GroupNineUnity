//main contributor: Gustav N Pedersen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Helper object for MagneticTochAlgorithm.
// Contains information needed for attempting to snap two pieces together.
public class SnapInformation
{
    public GameObject PieceToSnapTo { get; set; }

    public float DistanceBetweenPrimaryVertices { get; set; }
    public float AngleBetweenEdges { get; set; }

    public Vector3 PrimaryVertexInSelectedPiece { get; set; }
    public Vector3 PrimaryVertexInPieceToSnapTo { get; set; }
    public Vector3 SecondaryVertexInSelectedPiece { get; set; }
    public Vector3 SecondaryVertexInPieceToSnapTo { get; set; }

    public int IndexOfPrimaryVertexInSelectedPiece { get; set; }
    public int IndexOfPrimaryVertexInPieceToSnapTo { get; set; }
    public int IndexOfSecondaryVertexInSelectedPiece { get; set; }
    public int IndexOfSecondaryVertexInPieceToSnapTo { get; set; }
    public bool SecondaryVerticeIsPreviousVertice { get; set; }
    public void DebugLogInformation()
    {
        Debug.Log("DistanceBetweenPrimaryVertices: " + DistanceBetweenPrimaryVertices);
        Debug.Log("AngleBetweenEdges: " + AngleBetweenEdges);
        Debug.Log("PrimaryVertexInSelectedPiece: " + PrimaryVertexInSelectedPiece);
        Debug.Log("PrimaryVertexInPieceToSnapTo: " + PrimaryVertexInPieceToSnapTo);
        Debug.Log("PreviousVertexInSelectedPiece: " + SecondaryVertexInSelectedPiece);
        Debug.Log("PreviousVertexInPieceToSnapTo: " + SecondaryVertexInPieceToSnapTo);
    }

}

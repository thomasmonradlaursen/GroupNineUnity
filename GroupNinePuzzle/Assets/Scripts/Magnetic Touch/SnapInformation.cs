using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Helper object for MagneticTochAlgorithm.
// Contains information needed for attempting to snap two pieces together.
public class SnapInformation
{
    public GameObject PieceToSnapTo { get; set; }

    public float DistanceBetweenPrimaryVertices { get; set; }
    public float DistanceBetweenPreviousVertices { get; set; }

    public Vector3 PrimaryVertexInSelectedPiece { get; set; }
    public Vector3 PrimaryVertexInPieceToSnapTo { get; set; }
    public Vector3 PreviousVertexInSelectedPiece { get; set; }
    public Vector3 PreviousVertexInPieceToSnapTo { get; set; }

    public int IndexOfPrimaryVertexInSelectedPiece { get; set; }
    public int IndexOfPrimaryVertexInPieceToSnapTo { get; set; }
    public int IndexOfPreviousVertexInSelectedPiece { get; set; }
    public int IndexOfPreviousVertexInPieceToSnapTo { get; set; }
    public void DebugLogInformation()
    {

        Debug.Log("DistanceBetweenPrimaryVertices: " + DistanceBetweenPrimaryVertices);
        Debug.Log("DistanceBetweenPreviousVertices: " + DistanceBetweenPreviousVertices);
        Debug.Log("PrimaryVertexInSelectedPiece: " + PrimaryVertexInSelectedPiece);
        Debug.Log("PrimaryVertexInPieceToSnapTo: " + PrimaryVertexInPieceToSnapTo);
        Debug.Log("PreviousVertexInSelectedPiece: " + PreviousVertexInSelectedPiece);
        Debug.Log("PreviousVertexInPieceToSnapTo: " + PreviousVertexInPieceToSnapTo);
    }

}

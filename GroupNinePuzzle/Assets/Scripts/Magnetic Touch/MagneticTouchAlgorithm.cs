using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchAlgorithm : MonoBehaviour
{
    public List<GameObject> pieces;
    public (GameObject, List<GameObject>) possibleSnaps = (null, new List<GameObject>());

    private void Start()
    {
        //Debug.Log("MagneticTouchAlgorithm - Start()");
        pieces = GetComponentInParent<PieceController>().pieces;
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("MagneticTouchAlgorithm - TestingImplementation()");
            float margin = 0.075f;
            FindCandidatesForSnap(GetComponentInParent<MeshFromJsonGenerator>().selectedObject, margin);
            LogPossibleSnaps();
            if (possibleSnaps.Item2.Count > 0)
            {
                FindClosestVertexPair();
            }
        }
    }

    void FindClosestVertexPair()
    {
        // List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece = new List<(GameObject, Vector3, int, float)>();
        // Vector3 centroidOfSelectedPiece = possibleSnaps.Item1.GetComponent<PieceInfo>().centroid;
        // closestVerticesToSelectedPiece = GetComponentInParent<FindClosestVertex>().FindClosestVertexToCentroidOfSelectedPiece(possibleSnaps, centroidOfSelectedPiece);
        // GetComponentInParent<FindClosestVertex>().LogVertexAndDistance(closestVerticesToSelectedPiece);

        SnapInformation snapInformation;
        snapInformation = GetComponentInParent<FindClosestVertex>().FindClosestVertexToSelectedPiece(possibleSnaps);
        // pieceWithClosestVertex = GetComponentInParent<FindClosestVertex>().SelectPieceToSnapTo(closestVerticesToSelectedPiece);
        // Vector3 closestVertexOfMagnet = pieceWithClosestVertex.Item1.GetComponent<MeshFilter>().mesh.vertices[pieceWithClosestVertex.Item2];
        GameObject selectedPiece = GetComponentInParent<MeshFromJsonGenerator>().selectedObject;

        if (this.name.Equals(selectedPiece.name))
        {
            // snapInformation.DebugLogInformation();
            // Debug.Log(selectedPiece.name);
            // Debug.Log("this: " + this.name);
            // Debug.Log("this.name.Equals(selectedPiece.name");
            // int closestVertexInSelectedPiece = GetComponentInParent<FindClosestVertex>().FindClosestVertexInSelectedPiece(selectedPiece, closestVertexOfMagnet);
            
            // Todo: maybe return index in SnapInformation result instead of finding it in this kinda odd way.
            // int indexOfClosestVertexInSelectedPiece = GetComponentInParent<FindClosestVertex>().FindClosestVertexInSelectedPiece(selectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo);
            // int indexOfClosestVertexInPieceToSnapTo = GetComponentInParent<FindClosestVertex>().FindClosestVertexInSelectedPiece(snapInformation.PieceToSnapTo, snapInformation.PrimaryVertexInSelectedPiece);
            int indexOfClosestVertexInSelectedPiece = snapInformation.IndexOfPrimaryVertexInSelectedPiece;
            int indexOfClosestVertexInPieceToSnapTo = snapInformation.IndexOfPrimaryVertexInPieceToSnapTo;
            int indexOfNeighborVertexInSelectedPiece = snapInformation.IndexOfPreviousVertexInSelectedPiece;
            int indexOfNeighborVertexInPieceToSnapTo = snapInformation.IndexOfPreviousVertexInPieceToSnapTo;
            // int indexOfClosestVertexInPieceToSnapTo = GetComponentInParent<FindClosestVertex>().FindIndexOfVertexInPiece(snapInformation.PieceToSnapTo, snapInformation.PrimaryVertexInPieceToSnapTo);
            // Debug.LogFormat("index of vertex in selected piece: " + indexOfClosestVertexInSelectedPiece);
            // Debug.LogFormat("vertex in selected piece: " + selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfClosestVertexInSelectedPiece]);
            Vector3 displacement = GetComponentInParent<FindClosestVertex>().CalculateDisplacementForSnap(selectedPiece, snapInformation.PieceToSnapTo, indexOfClosestVertexInSelectedPiece, indexOfClosestVertexInPieceToSnapTo);
            float rotation = GetComponentInParent<FindClosestVertex>().CalculateRotationForSnap(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.PreviousVertexInSelectedPiece, snapInformation.PreviousVertexInPieceToSnapTo);
            Debug.Log("Displacement: " + displacement);
            Debug.Log("Rotation: " + rotation);
            GetComponentInParent<FindClosestVertex>().CalculateVerticesAfterSnapTranslation(selectedPiece, displacement);
            GetComponentInParent<FindClosestVertex>().CalculateVerticesAfterSnapRotation(selectedPiece, rotation, snapInformation.PrimaryVertexInSelectedPiece);
        }
    }

    (float, float) ConstructBoundBox(GameObject piece, float margin)
    {
        (float, float) minMax = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumXCoordinate();
        return (minMax.Item1 - margin, minMax.Item2 + margin);
    }

    void FindCandidatesForSnap(GameObject selectedPiece, float margin)
    {
        possibleSnaps.Item1 = selectedPiece;
        possibleSnaps.Item2.Clear();
        (float, float) boundBoxForSelectedPiece = ConstructBoundBox(selectedPiece, margin);
        float minimumForSelected = boundBoxForSelectedPiece.Item1;
        float maximumForSelected = boundBoxForSelectedPiece.Item2;
        float minimumForCompare;
        float maximumForCompare;
        // Debug.Log("MinimumForSelected: " + minimumForSelected + ", MaximumForSelected: " + maximumForSelected);
        foreach (GameObject piece in pieces)
        {
            (float, float) boundBoxForNext = ConstructBoundBox(piece, margin);
            minimumForCompare = boundBoxForNext.Item1;
            maximumForCompare = boundBoxForNext.Item2;
            // Debug.Log("Minimum: " + minimumForCompare + ", maximum: " + maximumForCompare);
            if (minimumForSelected < maximumForCompare && maximumForSelected > minimumForCompare)
            {
                if (!(piece.name.Equals(GetComponentInParent<MeshFromJsonGenerator>().selected)))
                {
                    possibleSnaps.Item2.Add(piece);
                }
            }
        }
    }

    void LogPossibleSnaps()
    {
        // Debug.Log("Number of possible snaps " + possibleSnaps.Item2.Count);
        if (possibleSnaps.Item2.Count > 0)
        {
            string printString = "";
            for (int index = 0; index < possibleSnaps.Item2.Count; index++)
            {
                if (index == possibleSnaps.Item2.Count - 1)
                {
                    printString = printString + possibleSnaps.Item2[index].name + ". ";
                }
                else
                {
                    printString = printString + possibleSnaps.Item2[index].name + ", ";
                }
            }
            Debug.Log(possibleSnaps.Item1.name + " can possibly snap to the following pieces: " + printString);
        }
        else
        {
            Debug.Log(possibleSnaps.Item1.name + " cannot snap to any other pieces at the moment.");
        }
    }
}

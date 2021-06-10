using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchAlgorithm : MonoBehaviour
{
    public List<GameObject> pieces;
    public (GameObject, List<GameObject>) possibleSnaps = (null, new List<GameObject>());

    private void Start()
    {
        Debug.Log("MagneticTouchAlgorithm - Start()");
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
        SnapInformation snapInformation;
        snapInformation = GetComponentInParent<FindClosestVertex>().FindClosestVertexToSelectedPiece(possibleSnaps);
        GameObject selectedPiece = GetComponentInParent<MeshFromJsonGenerator>().selectedObject;

        if (this.name.Equals(selectedPiece.name))
        {
            // snapInformation.DebugLogInformation();
            // Debug.Log(selectedPiece.name);
            // Debug.Log("this: " + this.name);
            // Debug.Log("this.name.Equals(selectedPiece.name");

            int indexOfClosestVertexInSelectedPiece = snapInformation.IndexOfPrimaryVertexInSelectedPiece;
            int indexOfClosestVertexInPieceToSnapTo = snapInformation.IndexOfPrimaryVertexInPieceToSnapTo;
            int indexOfNeighborVertexInSelectedPiece = snapInformation.IndexOfPreviousVertexInSelectedPiece;
            int indexOfNeighborVertexInPieceToSnapTo = snapInformation.IndexOfPreviousVertexInPieceToSnapTo;

            // Debug.LogFormat("index of vertex in selected piece: " + indexOfClosestVertexInSelectedPiece);
            // Debug.LogFormat("vertex in selected piece: " + selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfClosestVertexInSelectedPiece]);

            Vector3 displacement = GetComponentInParent<FindClosestVertex>().CalculateDisplacementForSnap(selectedPiece, snapInformation.PieceToSnapTo, indexOfClosestVertexInSelectedPiece, indexOfClosestVertexInPieceToSnapTo);
            float rotation = GetComponentInParent<FindClosestVertex>().CalculateRotationForSnap(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.PreviousVertexInSelectedPiece, snapInformation.PreviousVertexInPieceToSnapTo);

            // Debug.Log("Displacement: " + displacement);
            // Debug.Log("Rotation: " + rotation);

            GetComponentInParent<FindClosestVertex>().CalculateVerticesAfterSnapTranslationAndRotation(selectedPiece, displacement, rotation, snapInformation.IndexOfPrimaryVertexInSelectedPiece);
        }
    }

    ((float, float), (float, float)) ConstructBoundBox(GameObject piece, float margin)
    {
        (float, float) minMaxX = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumXCoordinate();
        (float, float) minMaxY = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumYCoordinate();
        return ((minMaxX.Item1 - margin, minMaxX.Item2 + margin), (minMaxY.Item1 - margin, minMaxY.Item2 + margin));
    }

    void FindCandidatesForSnap(GameObject selectedPiece, float margin)
    {
        possibleSnaps.Item1 = selectedPiece;
        possibleSnaps.Item2.Clear();
        var boundBoxForSelectedPiece = ConstructBoundBox(selectedPiece, margin);
        float minimumXForSelected = boundBoxForSelectedPiece.Item1.Item1;
        float maximumXForSelected = boundBoxForSelectedPiece.Item1.Item2;
        float minimumXForCompare;
        float maximumXForCompare;
        float minimumYForSelected = boundBoxForSelectedPiece.Item2.Item1;
        float maximumYForSelected = boundBoxForSelectedPiece.Item2.Item2;
        float minimumYForCompare;
        float maximumYForCompare;
        // Debug.Log("MinimumForSelected: " + minimumForSelected + ", MaximumForSelected: " + maximumForSelected);
        foreach (GameObject piece in pieces)
        {
            var boundBoxForNext = ConstructBoundBox(piece, margin);
            minimumXForCompare = boundBoxForNext.Item1.Item1;
            maximumXForCompare = boundBoxForNext.Item1.Item2;
            minimumYForCompare = boundBoxForNext.Item2.Item1;
            maximumYForCompare = boundBoxForNext.Item2.Item2;
            // Debug.Log("Minimum: " + minimumForCompare + ", maximum: " + maximumForCompare);
            if (minimumXForSelected < maximumXForCompare && maximumXForSelected > minimumXForCompare
                && minimumYForSelected < maximumYForCompare && maximumYForSelected > minimumYForCompare)
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

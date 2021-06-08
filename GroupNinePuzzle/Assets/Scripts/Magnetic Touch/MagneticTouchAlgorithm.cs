using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchAlgorithm : MonoBehaviour
{

    public List<GameObject> pieces;
    (string, List<string>) possibleSnaps = ("empty", new List<string>());

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
            float margin = 0.3f;
            FindCandidatesForSnap(GetComponentInParent<MeshFromJsonGenerator>().selectedObject, margin);
            LogPossibleSnaps();
        }
    }

    (float, float) ConstructBoundBox(GameObject piece, float margin)
    {
        (float, float) minMax = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumXCoordinate();
        return (minMax.Item1 - margin, minMax.Item2 + margin);
    }

    void FindCandidatesForSnap(GameObject selectedPiece, float margin)
    {
        possibleSnaps.Item1 = selectedPiece.name;
        possibleSnaps.Item2.Clear();
        (float, float) boundBoxForSelectedPiece = ConstructBoundBox(selectedPiece, margin);
        float minimumForSelected = boundBoxForSelectedPiece.Item1;
        float maximumForSelected = boundBoxForSelectedPiece.Item2;
        float minimumForCompare;
        float maximumForCompare;
        foreach (GameObject piece in pieces)
        {
            (float, float) boundBoxForNext = ConstructBoundBox(piece, margin);
            minimumForCompare = boundBoxForNext.Item1;
            maximumForCompare = boundBoxForNext.Item2;
            if (minimumForSelected < maximumForCompare && maximumForSelected > minimumForCompare)
            {
                if (!(piece.name.Equals(GetComponentInParent<MeshFromJsonGenerator>().selected)))
                {
                    possibleSnaps.Item2.Add(piece.name);
                }
            }
        }
    }

    void LogPossibleSnaps()
    {
        Debug.Log("Number of possible snaps " + possibleSnaps.Item2.Count);
        if (possibleSnaps.Item2.Count > 0)
        {
            string printString = "";
            for (int index = 0; index < possibleSnaps.Item2.Count; index++)
            {
                if (index == possibleSnaps.Item2.Count - 1)
                {
                    printString = printString + possibleSnaps.Item2[index] + ". ";
                }
                else
                {
                    printString = printString + possibleSnaps.Item2[index] + ", ";
                }
            }
            Debug.Log(possibleSnaps.Item1 + " can possibly snap to the following pieces: " + printString);
        }
        else
        {
            Debug.Log(possibleSnaps.Item1 + " cannot snap to any other pieces at the moment.");
        }
    }
}

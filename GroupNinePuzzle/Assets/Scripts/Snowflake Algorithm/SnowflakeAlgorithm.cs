using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeAlgorithm : MonoBehaviour

{
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();
    AreaSorting areaSorting = new AreaSorting();
    LengthsAndAnglesSorting lengthsAndAnglesSorting = new LengthsAndAnglesSorting();
    JSONPuzzle puzzle;
    (bool, string) resultAndMessage = (true, "success");
    List<Vector2> piecesWithIdenticalArea;
    List<Vector2> piecesWithIdenticalLengthsAndAngles;
    List<float> areaOfPieces;
    List<float[]> lengthsOfPieces;
    List<float[]> anglesOfPieces;

    void Start()
    {
        puzzle = GetComponentInParent<PieceController>().puzzle;
        List<GameObject> pieces = GetComponent<PieceController>().pieces;

        areaOfPieces = areaSorting.GetAreaOfPieces(pieces);
        lengthsOfPieces = lengthsAndAnglesSorting.GetLengthsOfPieces(pieces);
        anglesOfPieces = lengthsAndAnglesSorting.GetAnglesOfPieces(pieces);

        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces, puzzle);
        
        LogResult();
    }
    public void LogResult()
    {
        DetermineSnowflakeism();
        if (resultAndMessage.Item1)
        {
            Debug.Log("Snowflakeism for puzzle: True");
        }
        else
        {
            Debug.Log("Snowflakeism for puzzle: False");
            DetermineReasonForFailure();
        }
    }

    void DetermineSnowflakeism()
    {
        DetermineSnowflakeismByArea();
    }

    void DetermineSnowflakeismByArea()
    {
        if (piecesWithIdenticalArea.Count != 0)
        {
            resultAndMessage.Item1 = false;
            resultAndMessage.Item2 = "area";
        }
    }

    void DetermineSnowflakeismByLengthsAndAngles()
    {
        if (piecesWithIdenticalLengthsAndAngles.Count != 0)
        {
            resultAndMessage.Item1 = false;
            resultAndMessage.Item2 = "lengthsAndArea";
        }
    }

    void DetermineReasonForFailure()
    {
        if (resultAndMessage.Item2.Equals("area"))
        {
            Debug.Log("Reason for failure: The puzzle contains pieces with identical area");
            Debug.Log("The following pieces have identical area:");
            foreach (Vector2 pair in piecesWithIdenticalArea)
            {
                Debug.Log(string.Format("Piece {0} and Piece {1} - Identical area: {2}", pair.x, pair.y, areaOfPieces[(int)pair.x]));
            }
        }
        if (resultAndMessage.Item2.Equals("lengthsAndAngles"))
        {
            Debug.Log("Reason for failure: The puzzle contains pieces with identical lengths and angles");
            Debug.Log("The following pieces have identical lengths and angles:");
        }
    }
}
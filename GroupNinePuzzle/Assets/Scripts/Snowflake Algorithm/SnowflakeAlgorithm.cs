using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeAlgorithm : MonoBehaviour

{
    MiscellaneousMath mM = new MiscellaneousMath();
    AreaSorting areaSorting = new AreaSorting();
    JSONPuzzle puzzle;
    (bool, string) snowflakeStatus = (true, "success");
    List<Vector2> piecesWithIdenticalArea;
    List<float> areaOfPieces;

    void Start()
    {
        puzzle = GetComponentInParent<PieceController>().puzzle;
        areaOfPieces = areaSorting.GetAreaOfPieces(GetComponent<PieceController>().pieces);
        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces, puzzle);
        LogResult();
    }
    public void LogResult()
    {
        DetermineSnowflakeism();
        if (snowflakeStatus.Item1)
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
            snowflakeStatus.Item1 = false;
            snowflakeStatus.Item2 = "identicalArea";
        }
    }

    void DetermineReasonForFailure()
    {
        if (snowflakeStatus.Item2.Equals("identicalArea"))
        {
            Debug.Log("Reason for failure: The puzzle contains pieces with identical area");
            Debug.Log("The following pieces have identical area:");
            foreach (Vector2 pair in piecesWithIdenticalArea)
            {
                Debug.Log(string.Format("Piece {0} and Piece {1} - Identical area: {2}", pair.x, pair.y, areaOfPieces[(int)pair.x]));
            }
        }

    }
}

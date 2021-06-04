using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeScript : MonoBehaviour

{
    MiscellaneousMath mM = new MiscellaneousMath();
    JSONPuzzle puzzle;
    string failure;

    public void LogResult() {
        puzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;
        if(DetermineSnowflakeism()) Debug.Log("Snowflakeism for puzzle: True");
        else
        {
            Debug.Log("Snowflakeism for puzzle: False");
            DetermineReasonForFailure();
        }
    }
    bool DetermineSnowflakeism()
    {
        bool snowflakism = true;
        snowflakism = DetermineSnowflakeismByArea();
        return snowflakism;
    }

    float[] CalculateAreasOfPieces()
    {
        float[] areaOfPieces = new float[puzzle.pieces.Length];
        for(int pieceIndex = 0; pieceIndex < puzzle.pieces.Length; pieceIndex++)
        {
            areaOfPieces[pieceIndex] = mM.CalculateAreaFromMesh(GetComponent<MeshFromJsonGenerator>().meshArray[pieceIndex]);
        }
        return areaOfPieces;
    }

    List<Vector2> FindPiecesWithIdenticalArea(float[] areasOfPieces)
    {
        List<Vector2> piecesWithIdenticalArea = new List<Vector2>();
        if(areasOfPieces.Length > 1)
        {
            for(int outer = 0; outer<areasOfPieces.Length; outer++)
            {
                for(int inner = outer+1; inner<areasOfPieces.Length; inner++)
                {
                    if(areasOfPieces[outer] == areasOfPieces[inner])
                    {
                        piecesWithIdenticalArea.Add(new Vector2(puzzle.pieces[outer].piece, puzzle.pieces[inner].piece));
                    }
                }
            }
        }
        return piecesWithIdenticalArea;
    }

    bool DetermineSnowflakeismByArea()
    {
        bool snowflakeAreas = true;
        float[] areaOfPieces = CalculateAreasOfPieces();
        List<Vector2> piecesWithIdenticalArea = FindPiecesWithIdenticalArea(areaOfPieces);
        if(piecesWithIdenticalArea.Count != 0)
        {
            snowflakeAreas = false;
            //errorMessage = "The puzzle contains pieces with identical area";
        }
        return snowflakeAreas;
    }

    void DetermineReasonForFailure()
    {

    }
}

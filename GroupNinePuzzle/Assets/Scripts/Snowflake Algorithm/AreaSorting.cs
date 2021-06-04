using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AreaSorting : MonoBehaviour
{
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();
    JSONPuzzle puzzle;
    float[] CalculateAreasOfPieces()
    {
        float[] areaOfPieces = new float[puzzle.pieces.Length];
        for (int pieceIndex = 0; pieceIndex < puzzle.pieces.Length; pieceIndex++)
        {
            areaOfPieces[pieceIndex] = miscellaneousMath.CalculateAreaFromMesh(GetComponent<MeshFromJsonGenerator>().meshArray[pieceIndex]);
        }
        return areaOfPieces;
    }

    void FindPieces()
    {
        GameObject[] pieces = FindObjectsOfType<GameObject>();
        Debug.Log("Number of pieces: " + pieces.Length);
        Debug.Log("Name of first piece: " + pieces[0].name);
    }

    List<Vector2> FindPiecesWithIdenticalArea(float[] areasOfPieces)
    {
        List<Vector2> piecesWithIdenticalArea = new List<Vector2>();
        if (areasOfPieces.Length > 1)
        {
            for (int outer = 0; outer < areasOfPieces.Length; outer++)
            {
                for (int inner = outer + 1; inner < areasOfPieces.Length; inner++)
                {
                    if (areasOfPieces[outer] == areasOfPieces[inner])
                    {
                        piecesWithIdenticalArea.Add(new Vector2(puzzle.pieces[outer].piece, puzzle.pieces[inner].piece));
                    }
                }
            }
        }
        return piecesWithIdenticalArea;
    }

    (bool, string) DetermineSnowflakeismByArea()
    {  
        bool snowflakeAreas = true;
        string failureMessage = "";
        float[] areaOfPieces = CalculateAreasOfPieces();
        List<Vector2> piecesWithIdenticalArea = FindPiecesWithIdenticalArea(areaOfPieces);
        if (piecesWithIdenticalArea.Count != 0)
        {
            snowflakeAreas = false;
            failureMessage = "identicalArea";
        }
        return (snowflakeAreas, failureMessage);
    }
}

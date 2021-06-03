using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeScript : MonoBehaviour

{
    MiscellaneousMath areaCalculator = new MiscellaneousMath();
    JSONPuzzle puzzle;

    void Start() {
        puzzle = GetComponent<JSONDeserializer>().DeserializerPuzzleFromJSON("Assets/DataObjects/Classic-003-005-1331.json");
        Debug.Log("Snowflakism for puzzle: " + DetermineSnowflakism());
    }
    bool DetermineSnowflakism()
    {
        bool snowflakism = true;
        snowflakism = DetermineSnowflakismByArea();
        return snowflakism;
    }

    float[] CalculateAreasOfPieces()
    {
        float[] areaOfPieces = new float[puzzle.pieces.Length];
        for(int pieceIndex = 0; pieceIndex < puzzle.pieces.Length; pieceIndex++)
        {
            areaOfPieces[pieceIndex] = areaCalculator.CalculateAreaFromMesh(GetComponent<MeshFromJsonGenerator>().meshArray[pieceIndex]);
        }
        return areaOfPieces;
    }

    void LogAreasOfPieces(float[] areasOfPieces)
    {
        for(int index = 0; index < puzzle.pieces.Length; index++)
        {
            Debug.Log(string.Format("Piece {0} with area: {1}", puzzle.pieces[index].piece, areasOfPieces[index]));
        }
    }

    Vector2[] FindPiecesWithIdenticalArea(float[] areasOfPieces)
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
        return piecesWithIdenticalArea.ToArray();
    }

    bool DetermineSnowflakismByArea()
    {
        bool snowflakeAreas = true;
        float[] areaOfPieces = CalculateAreasOfPieces();
        Vector2[] piecesWithIdenticalArea = FindPiecesWithIdenticalArea(areaOfPieces);
        if(piecesWithIdenticalArea.Length != 0) snowflakeAreas = false;
        LogAreasOfPieces(areaOfPieces);
        return snowflakeAreas;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AreaSorting
{
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();

    public List<float> GetAreaOfPieces(List<GameObject> pieces)
    {
        Debug.Log("AreaSorting: FindAreaOfPieces");
        List<float> areaOfPieces = new List<float>();
        for (int index = 0; index < pieces.Count; index++)
        {
            areaOfPieces.Add(pieces[index].GetComponent<PieceInfo>().area);
        }
        return areaOfPieces;
    }

    public List<Vector2> FindPiecesWithIdenticalArea(List<float> areasOfPieces, JSONPuzzle puzzle)
    {
        Debug.Log("AreaSorting: FindPiecesWithIdenticalArea");
        List<Vector2> piecesWithIdenticalArea = new List<Vector2>();
        if (areasOfPieces.Count > 1)
        {
            for (int outer = 0; outer < areasOfPieces.Count; outer++)
            {
                for (int inner = outer + 1; inner < areasOfPieces.Count; inner++)
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class LengthsAndAnglesSorting
{
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();

    public List<float[]> GetLengthsOfPieces(List<GameObject> pieces)
    {
        // Debug.Log("LNASorting: FindAreaOfPieces");
        List<float[]> lengthsOfPieces = new List<float[]>();
        for (int index = 0; index < pieces.Count; index++)
        {
            lengthsOfPieces.Add(pieces[index].GetComponent<PieceInfo>().lengths);
        }
        return lengthsOfPieces;
    }
    public List<float[]> GetAnglesOfPieces(List<GameObject> pieces)
    {
        // Debug.Log("LNASorting: FindAreaOfPieces");
        List<float[]> anglesOfPieces = new List<float[]>();
        for (int index = 0; index < pieces.Count; index++)
        {
            anglesOfPieces.Add(pieces[index].GetComponent<PieceInfo>().angles);
        }
        return anglesOfPieces;
    }

    public List<Vector2> FindPiecesWithIdenticalLengthsAndAngles(List<float[]> lengthsOfPieces, List<float[]> anglesOfPieces, JSONPuzzle puzzle)
    {
        // Debug.Log("LNASorting: FindPiecesWithIdenticalArea");
        List<Vector2> piecesWithIdenticalArea = new List<Vector2>();
        if (lengthsOfPieces.Count > 1)
        {
            for (int outer = 0; outer < lengthsOfPieces.Count; outer++)
            {
                for (int inner = outer + 1; inner < lengthsOfPieces.Count; inner++)
                {
                    if (lengthsOfPieces[outer] == lengthsOfPieces[inner])
                    {
                        piecesWithIdenticalArea.Add(new Vector2(puzzle.pieces[outer].piece, puzzle.pieces[inner].piece));
                    }
                }
            }
        }
        return piecesWithIdenticalArea;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    void Start()
    {
        SetPuzzle();
        GetComponentInChildren<PieceController>().CreatePieces();
        
    }
    void SetPuzzle()
    {
        if (GetComponent<PuzzleModel>().generateRandom)
        {
            GetComponent<PuzzleModel>().puzzle = GetComponentInChildren<DivisionModel>().puzzle;
        }
        else
        {
            string puzzleFromFile = GetComponent<PuzzleModel>().locationOfFile + GetComponent<PuzzleModel>().fileName;
            GetComponent<PuzzleModel>().puzzle = GetComponent<JSONDeserializer>().DeserializerPuzzleFromJSON(puzzleFromFile);
        }
    }
}

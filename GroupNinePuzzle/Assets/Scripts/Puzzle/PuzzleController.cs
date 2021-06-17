using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    void Start()
    {
        SetPuzzle();
        GetComponentInChildren<PieceController>().CreatePieces();
        GetComponentInChildren<FitCameraToPuzzle>().FitCamera();
        
    }
    void SetPuzzle()
    {
        if (GetComponent<PuzzleModel>().generateRandom)
        {
            GetComponentInChildren<DivisionController>().SetupRandomPuzzle();
            GetComponent<PuzzleModel>().puzzle = GetComponentInChildren<DivisionModel>().puzzle;
        }
        else
        {
            string puzzleFromFile = GetComponent<PuzzleModel>().locationOfFile + GetComponent<PuzzleModel>().fileName;
            GetComponent<PuzzleModel>().puzzle = GetComponent<JSONDeserializer>().DeserializerPuzzleFromJSON(puzzleFromFile);
        }
    }
    public void EnableRandomlyGeneratedPuzzled()
    {
        GetComponent<PuzzleModel>().generateRandom = true;
    }
    public void DisableRandomlyGeneratedPuzzled()
    {
        GetComponent<PuzzleModel>().generateRandom = false;
    }
}

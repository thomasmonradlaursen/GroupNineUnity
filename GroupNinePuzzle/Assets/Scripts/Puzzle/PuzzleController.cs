using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Thomas Monrad Laursen

public class PuzzleController : MonoBehaviour
{
    void Start()
    {
        SetPuzzle();
        GetComponentInChildren<PieceController>().CreatePieces();
        DetermineAndDisplaySnowflakeism();
        GetComponentInChildren<FitCameraToPuzzle>().FitCamera();
        GetComponent<BoardOutlineGenerator>().DrawBoard();
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
    public void DetermineAndDisplaySnowflakeism()
    {
        if(!(GetComponentInChildren<SnowflakeAlgorithm>().DetermineSnowflakeism()))
        {
            GetComponent<PuzzleModel>().displayForSnowflake.SetActive(true);
        }
    }
}

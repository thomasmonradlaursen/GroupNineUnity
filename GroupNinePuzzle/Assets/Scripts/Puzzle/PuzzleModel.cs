using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

// Author: Thomas Monrad Laursen

public class PuzzleModel : MonoBehaviour
{
    public JSONPuzzle puzzle = new JSONPuzzle();
    public string fileName = "puzzle_11_auto.json";
    public string locationOfFile = "Data/";
    public List<GameObject> pieces;
    public Dictionary<string, List<string>> connectedPieces;
    public bool generateRandom = false;
    public GameObject selectedObject = null;
    public GameObject previousSelectedObject = null;
    public GameObject displayForSnowflake = null;
}

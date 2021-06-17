using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PuzzleModel : MonoBehaviour
{
    public JSONPuzzle puzzle = new JSONPuzzle();
    public string fileName = "Classic-003-005-1331.json";
    public string locationOfFile = "Assets/DataObjects/";
    public List<GameObject> pieces;
    public Dictionary<string, List<string>> connectedPieces;
    public bool generateRandom = false;
    public GameObject selectedObject = null;
    public GameObject previousSelectedObject = null;
    public GameObject displayForSnowflake = null;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PuzzleModel : MonoBehaviour
{
    public JSONPuzzle puzzle;
    public List<GameObject> pieces;
    public Dictionary<string, List<string>> connectedPieces;
    public bool generateRandom = false;
    public GameObject selectedObject = null;
    public GameObject previousSelectedObject = null;
}

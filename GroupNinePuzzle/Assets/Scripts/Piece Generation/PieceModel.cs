using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceModel : MonoBehaviour
{
    public List<GameObject> pieces;
    public Dictionary<string, List<string>> connectedPieces;
    public JSONPuzzle puzzle;
    public bool generateRandom = false;
    public List<Mesh> meshes = new List<Mesh>();
    public int[] triangles;
}
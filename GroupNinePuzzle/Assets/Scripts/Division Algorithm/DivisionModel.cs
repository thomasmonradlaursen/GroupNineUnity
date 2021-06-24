using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DivisionTriangles;
using JSONPuzzleTypes;

//Author: Thomas Monrad Laursen
public class DivisionModel : MonoBehaviour
{
    public Vector2 boardSize = new Vector2(5, 3);
    public int numberOfPieces = 5;
    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> corners = new List<Vector3>();
    public List<DivisionTriangle> triangles = new List<DivisionTriangle>();
    public int triangleId = 1;
    public string nameOfPuzzle = "RandomlyGeneratedPuzzle";
    public JSONPuzzle puzzle = new JSONPuzzle();
}

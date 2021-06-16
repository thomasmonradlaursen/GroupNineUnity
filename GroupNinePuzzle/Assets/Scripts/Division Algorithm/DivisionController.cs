using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DivisionTriangles;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DivisionController : MonoBehaviour
{
    void Start()
    {
        SetCorners();
        CreatePoints();
        RunTriangulation();
        ConstructPuzzle();
    }
    void SetCorners()
    {
        Vector2 boardSize = GetComponent<DivisionModel>().boardSize;
        List<Vector3> corners = new List<Vector3>();
        corners.Add(new Vector3(0.0f, 0.0f, 0.0f));
        corners.Add(new Vector3(boardSize.x, 0.0f, 0.0f));
        corners.Add(new Vector3(boardSize.x, boardSize.y, 0.0f));
        corners.Add(new Vector3(0.0f, boardSize.y, 0.0f));
        GetComponent<DivisionModel>().corners = corners;
    }
    void CreatePoints()
    {
        int numberOfPieces = GetComponent<DivisionModel>().numberOfPieces;
        Vector2 boardSize = GetComponent<DivisionModel>().boardSize;
        List<Vector3> points = GetComponent<PointGenerator>().GenerateRandomPoints(numberOfPieces, boardSize);
        GetComponent<DivisionModel>().points = points;
    }
    void RunTriangulation()
    {
        GetComponent<DivisionModel>().triangles = GetComponent<Triangulation>().BowyerWatsonTriangulate(GetComponent<DivisionModel>().points);
    }
    void ConstructPuzzle()
    {
        GetComponent<DivisionModel>().puzzle = GetComponent<JSONSerializer>().CreatePuzzle();
    }
    public void SetNumberOfPieces(string inputNumber)
    {
        GetComponent<DivisionModel>().numberOfPieces = int.Parse(inputNumber);
    }
    public void SetLengthOfBoard(string inputLength)
    {
        GetComponent<DivisionModel>().boardSize.y = int.Parse(inputLength);
    }
    public void SetWidthOfBoard(string inputWidth)
    {
        GetComponent<DivisionModel>().boardSize.x = int.Parse(inputWidth);
    }
}

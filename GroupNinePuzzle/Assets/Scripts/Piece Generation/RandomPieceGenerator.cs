﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPieceGenerator : MonoBehaviour
{
    Circumscribed circumscriber = new Circumscribed();
    public Vector2 boardSize = new Vector2(5, 3);
    public int numberOfPieces = 12;
    public List<Vector3> randomPieces;
    public GameObject pieceDot = null;
    public GameObject centerDot = null;
    void Start()
    {
        randomPieces = GenerateRandomPointsForPieces(numberOfPieces);
        RendererResults();
        (Vector3, float) circumcircle = circumscriber.GetCircumcenterAndCircumradius(randomPieces[0], randomPieces[1], randomPieces[2]);
        Instantiate(centerDot, circumcircle.Item1, Quaternion.identity);
        Debug.Log("Radius of circle: " + circumcircle.Item2);
        Debug.Log("Center of circle: " + circumcircle.Item1);
    }
    List<Vector3> GenerateRandomPointsForPieces(int numberOfPieces)
    {
        List<Vector3> randomPieces = new List<Vector3>();
        for (int pieceNumber = 0; pieceNumber < numberOfPieces; pieceNumber++)
        {
            float randomXCoordinate = Random.Range(0.0f, (float)boardSize.x);
            float randomYCoordinate = Random.Range(0.0f, (float)boardSize.y);
            randomPieces.Add(new Vector3(randomXCoordinate, randomYCoordinate, 0.0f));
        }
        randomPieces.Add(new Vector3(0.0f, 0.0f, 0.0f));
        randomPieces.Add(new Vector3(boardSize.x, 0.0f, 0.0f));
        randomPieces.Add(new Vector3(boardSize.x, boardSize.y, 0.0f));
        randomPieces.Add(new Vector3(0.0f, boardSize.y, 0.0f));
        return randomPieces;
    }
    void DrawBorder(Vector2 boardSize)
    {
        List<Vector3> corners = new List<Vector3>();
        for(int index = randomPieces.Count-4; index < randomPieces.Count; index++)
        {
            corners.Add(randomPieces[index]);
        }
        GetComponent<LineRenderer>().SetPositions(corners.ToArray());
    }
    void RendererResults()
    {
        DrawBorder(boardSize);
        foreach (Vector3 piece in randomPieces)
        {
            Instantiate(pieceDot, piece, Quaternion.identity);
        }
    }
    
}

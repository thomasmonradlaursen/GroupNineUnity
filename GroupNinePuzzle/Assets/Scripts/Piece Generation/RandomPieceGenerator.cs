using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPieceGenerator : MonoBehaviour
{
    public Vector2 boardSize = new Vector2(5, 3);
    public int numberOfPieces = 12;
    public List<Vector3> randomPieces;
    public List<Vector3> corners;
    public GameObject pieceDot = null;
    public GameObject centerDot = null;
    public List<Vector3> GetPoints()
    {
        randomPieces = GenerateRandomPointsForPieces(numberOfPieces);
        corners = SetupCorners(boardSize);
        //RendererResults();
        return randomPieces;
    }
    List<Vector3> GenerateRandomPointsForPieces(int numberOfPieces)
    {
        List<Vector3> randomPieces = new List<Vector3>();
        for (int pieceNumber = 0; pieceNumber < numberOfPieces; pieceNumber++)
        {
            float randomXCoordinate = Random.Range(((float)boardSize.x) / 10.0f, (float)boardSize.x - ((float)boardSize.x) / 10.0f);
            float randomYCoordinate = Random.Range(((float)boardSize.y) / 10.0f, (float)boardSize.y - ((float)boardSize.y) / 10.0f);
            randomPieces.Add(new Vector3(randomXCoordinate, randomYCoordinate, 0.0f));
        }
        return randomPieces;
    }
    List<Vector3> SetupCorners(Vector2 boardSize)
    {
        List<Vector3> corners = new List<Vector3>();
        corners.Add(new Vector3(0.0f, 0.0f, 0.0f));
        corners.Add(new Vector3(boardSize.x, 0.0f, 0.0f));
        corners.Add(new Vector3(boardSize.x, boardSize.y, 0.0f));
        corners.Add(new Vector3(0.0f, boardSize.y, 0.0f));
        return corners;
    }
    void RendererResults()
    {
        GetComponent<LineRenderer>().SetPositions(corners.ToArray());
        foreach (Vector3 piece in randomPieces)
        {
            Instantiate(pieceDot, piece, Quaternion.identity);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPieceGenerator : MonoBehaviour
{
    public Vector2 boardSize = new Vector2(5, 3);
    public int numberOfPieces = 12;
    public List<Vector3> randomPieces;
    public GameObject prefabDot = null;
    void Start()
    {
        DrawBorder(boardSize);
        randomPieces = GenerateRandomPointsForPieces(numberOfPieces);
        foreach (Vector3 piece in randomPieces)
        {
            Instantiate(prefabDot, piece, Quaternion.identity);
        }
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
        return randomPieces;
    }
    void DrawBorder(Vector2 boardSize)
    {
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(0.0f, 0.0f, 0.0f);
        corners[1] = new Vector3(boardSize.x, 0.0f, 0.0f);
        corners[2] = new Vector3(boardSize.x, boardSize.y, 0.0f);
        corners[3] = new Vector3(0.0f, boardSize.y, 0.0f);
        GetComponent<LineRenderer>().SetPositions(corners);
    }
}

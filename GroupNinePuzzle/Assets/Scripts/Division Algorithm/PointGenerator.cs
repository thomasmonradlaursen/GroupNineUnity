using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGenerator : MonoBehaviour
{
    public List<Vector3> GenerateRandomPoints(int numberOfPieces, Vector2 boardSize)
    {
        int numberOfDots = CalculateDotsFromPieces(numberOfPieces);
        List<Vector3> randomPoints = new List<Vector3>();
        for (int dotNumber = 0; dotNumber < numberOfDots; dotNumber++)
        {
            float randomXCoordinate = Random.Range(((float)boardSize.x) / 50.0f, (float)boardSize.x - ((float)boardSize.x) / 50.0f);
            float randomYCoordinate = Random.Range(((float)boardSize.y) / 50.0f, (float)boardSize.y - ((float)boardSize.y) / 50.0f);
            randomPoints.Add(new Vector3(randomXCoordinate, randomYCoordinate, 0.0f));
        }
        return randomPoints;
    }
    int CalculateDotsFromPieces(int numberOfPieces)
    {
        return (numberOfPieces / 2) - 1;
    }
}

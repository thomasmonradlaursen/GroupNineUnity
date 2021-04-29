using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JSONDeserializer : MonoBehaviour 
{
    public GameObject prefabDot = null;
    public void Start()
    {
        JSONPuzzle puzzleFromJSON = deserializerPuzzleFromJSON("Assets/DataObjects/Classic-One-Piece.json");
        logPuzzleInformation(puzzleFromJSON);
        instanciatePuzzle(puzzleFromJSON);
        float area = calculateAreaFromCoords(puzzleFromJSON.pieces[0].corners);
        Debug.Log("Area of piece: " + area);
    }

    public JSONPuzzle deserializerPuzzleFromJSON(String pathToPuzzle)
    {
        string fileContent = System.IO.File.ReadAllText(pathToPuzzle);
        JSONPuzzle puzzleFromJSON = JsonUtility.FromJson<JSONPuzzle>(fileContent);
        return puzzleFromJSON;
    }

    public void instanciatePuzzle(JSONPuzzle puzzle)
    {
        for(int i = 0; i<puzzle.nPieces; i++)
        {
            for(int j = 0; j<puzzle.pieces[i].corners.Length; j++)
            {
                float x = puzzle.pieces[i].corners[j].coord.x;
                float y = puzzle.pieces[i].corners[j].coord.y;
                Vector3 newCoordinates = new Vector3(x,y,0);
                Instantiate(prefabDot, newCoordinates, Quaternion.identity);
            }
        }
    }

    public void logPuzzleInformation(JSONPuzzle puzzle)
    {
        Debug.Log(" - Loaded puzzle from JSON - ");
        Debug.Log("Name: " + puzzle.name);
        Debug.Log("Number of pieces: " + puzzle.nPieces);
        Debug.Log(String.Format("Form: {0} X {1}", puzzle.puzzle.form[0].coord.x, puzzle.puzzle.form[0].coord.y));
    }

    public float calculateAreaFromCoords(Corner[] vertices)
    {
        float a = 0.0f;
        float p = 0.0f;
        float x = vertices[0].coord.x;
        float y = vertices[0].coord.y;
        int i = 0;

        while(i < vertices.Length)
        {
            a += vertices[i].coord.x * y - vertices[i].coord.y * x;
			p += Math.Abs((vertices[i].coord.x) - x + (vertices[i].coord.y - y));
			x = vertices[i].coord.x;
			y = vertices[i].coord.y;
			i++;
        }
        return Math.Abs(a/2.0f);
    }
}

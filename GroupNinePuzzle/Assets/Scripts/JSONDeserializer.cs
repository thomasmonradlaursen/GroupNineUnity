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
        JSONPuzzle puzzleFromJSON = DeserializerPuzzleFromJSON("Assets/DataObjects/Classic-003-005-1331.json");
        LogPuzzleInformation(puzzleFromJSON);
        InstanciatePuzzle(puzzleFromJSON);
    }
    public JSONPuzzle DeserializerPuzzleFromJSON(String pathToPuzzle)
    {
        string fileContent = System.IO.File.ReadAllText(pathToPuzzle);
        JSONPuzzle puzzleFromJSON = JsonUtility.FromJson<JSONPuzzle>(fileContent);
        return puzzleFromJSON;
    }
    public void InstanciatePuzzle(JSONPuzzle puzzle)
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
    public void LogPuzzleInformation(JSONPuzzle puzzle)
    {
        Debug.Log(" - Loaded puzzle from JSON - ");
        Debug.Log("Name: " + puzzle.name);
        Debug.Log("Number of pieces: " + puzzle.nPieces);
        Vector2 form = FindFormOfPuzzle(puzzle);
        Debug.Log(String.Format("Form: {0} × {1}", form.x, form.y));
    }
    Vector2 FindFormOfPuzzle(JSONPuzzle puzzle)
    {
        float x = 0.0f; float y = 0.0f;
        foreach(Form coordinateSet in puzzle.puzzle.form)
        {
            if(coordinateSet.coord.x > x) x = coordinateSet.coord.x;
            if(coordinateSet.coord.y > y) y = coordinateSet.coord.y;
        }
        return new Vector2(x,y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using JSONPuzzleTypes;

public class JSONDeserializer : MonoBehaviour 
{
    public JSONPuzzle Puzzle {get; set;}
    public GameObject prefabDot = null;
    public string fileName = "Classic-003-005-1331.json";
    public string locationOfFile = "Assets/DataObjects/";

    public JSONPuzzle DeserializerPuzzleFromJSON(String pathToPuzzle)
    {
        string fileContent = System.IO.File.ReadAllText(pathToPuzzle);
        Puzzle = JsonUtility.FromJson<JSONPuzzle>(fileContent);
        return Puzzle;
    }

    public void LogPuzzleInformation()
    {
        // Debug.Log("Name: " + Puzzle.name);
        // Debug.Log("Number of pieces: " + Puzzle.nPieces);
        Vector2 form = FindFormOfPuzzle();
        // Debug.Log(String.Format("Form: {0} × {1}", form.x, form.y));
    }

Vector2 FindFormOfPuzzle()
    {
        float x = 0.0f; float y = 0.0f;
        foreach(Form coordinateSet in Puzzle.puzzle.form)
        {
            if(coordinateSet.coord.x > x) x = coordinateSet.coord.x;
            if(coordinateSet.coord.y > y) y = coordinateSet.coord.y;
        }
        return new Vector2(x,y);
    }
}


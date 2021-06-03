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
    public void Start()
    {
        DeserializerPuzzleFromJSON("Assets/DataObjects/Classic-003-005-1331.json");
    }

    public JSONPuzzle DeserializerPuzzleFromJSON(String pathToPuzzle)
    {
        string fileContent = System.IO.File.ReadAllText(pathToPuzzle);
        Puzzle = JsonUtility.FromJson<JSONPuzzle>(fileContent);
        LogPuzzleInformation();
        return Puzzle;
    }

    public void InstantiatePuzzle()
    {
        for(int i = 0; i<Puzzle.nPieces; i++)
        {
            for(int j = 0; j<Puzzle.pieces[i].corners.Length; j++)
            {
                float x = Puzzle.pieces[i].corners[j].coord.x;
                float y = Puzzle.pieces[i].corners[j].coord.y;
                Vector3 newCoordinates = new Vector3(x,y,0);
                Instantiate(prefabDot, newCoordinates, Quaternion.identity);
            }
        }
    }

    public void LogPuzzleInformation()
    {
        Debug.Log(" - Loaded puzzle from JSON - ");
        Debug.Log("Name: " + Puzzle.name);
        Debug.Log("Number of pieces: " + Puzzle.nPieces);
        Vector2 form = FindFormOfPuzzle();
        Debug.Log(String.Format("Form: {0} × {1}", form.x, form.y));
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


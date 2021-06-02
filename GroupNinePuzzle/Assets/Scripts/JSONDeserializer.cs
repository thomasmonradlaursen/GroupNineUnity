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
        JSONPuzzle puzzleFromJSON = deserializerPuzzleFromJSON("Assets/DataObjects/Classic-003-005-1331.json");
        logPuzzleInformation(puzzleFromJSON);
        instanciatePuzzle(puzzleFromJSON);
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
        Debug.Log(String.Format("Form: {0} X {1}", puzzle.puzzle.form[3].coord.x, puzzle.puzzle.form[3].coord.y));
    }
}

//Author: Louise Noer Kolborg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using JSONPuzzleTypes;

public class JSONDeserializer : MonoBehaviour 
{
    public JSONPuzzle DeserializerPuzzleFromJSON(String pathToPuzzle)
    {
        string fileContent = System.IO.File.ReadAllText(pathToPuzzle);
        return JsonUtility.FromJson<JSONPuzzle>(fileContent);
    }
    public void SelectGame(string inputName)
    {
        GetComponent<PuzzleModel>().fileName = inputName;
    }
}
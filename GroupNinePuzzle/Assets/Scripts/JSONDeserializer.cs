using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JSONDeserializer : MonoBehaviour {
    public void Start(){
  
        //Deserialize json file 
        string test = System.IO.File.ReadAllText("Assets/DataObjects/144Solutions.json");
        JSONPuzzle testPuzzle = JsonUtility.FromJson<JSONPuzzle>(test);

        Debug.Log("Name: " + testPuzzle.name);
        Debug.Log("Number of pieces: " + testPuzzle.nPieces);
        Debug.Log("Form: " + testPuzzle.puzzle.form[1].coord.y);
        Debug.Log("Piece ID: " + testPuzzle.pieces[4].piece);

        for (int i = 0; i<4; i++){
            Debug.Log(String.Format("x-coordinate {0}: {1}", i, testPuzzle.pieces[4].corners[i].coord.y));
        }
        Debug.Log("JSON String: " + test);
    }
}

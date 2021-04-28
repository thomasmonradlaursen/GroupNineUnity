using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TestParser : MonoBehaviour {
    public void Start(){
  
        //Deserialize json file 
        string test = System.IO.File.ReadAllText("Assets/DataObjects/SmallPuzzle.json");
        Puzzle testPuzzle = JsonUtility.FromJson<Puzzle>(test);

        Debug.Log("Name: " + testPuzzle.name);
        Debug.Log("Number of pieces: " + testPuzzle.nPieces);
        Debug.Log("Form: " + testPuzzle.form[0].coord1.x);

        for (int i = 0; i<4; i++){
            Debug.Log(String.Format("x-coordinate {0}: {1}", i, testPuzzle.pieces[0].corners[i].coord.x));
        }
        Debug.Log(test);
        //Debug.Log(testPuzzle.result[0].name);


    }
}

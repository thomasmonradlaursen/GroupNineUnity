using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JSONTester : MonoBehaviour
{
    private void Start() {
        PiecePlayer piece = new PiecePlayer();
        piece.pieceName = "John";
        piece.pieceId = 1;

        string pieceAsJSON = JsonUtility.ToJson(piece);
        Debug.Log("JSON test: " + pieceAsJSON);
    }
}

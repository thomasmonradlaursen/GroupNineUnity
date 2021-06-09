using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AutoSolveAlgorithm : MonoBehaviour
{
    public List<GameObject> pieces;
    JSONPuzzle puzzle;
    List<GameObject> potentialPieces;
    List<List<GameObject>> placedPieces;
    float theta = 90.0f;
    float xAxisLength;
    float yAxisLength;
    Vector3 upperLeftCorner;
    Vector3 upperRightCorner;
    Vector3 lowerRightCorner;
    Vector3 lowerLeftCorner;
    
    void start(){
        puzzle = GetComponentInParent<PieceController>().puzzle;
        pieces = GetComponent<PieceController>().pieces;

        findCorners();
        xAxisLength = Vector3.Distance(upperLeftCorner, upperRightCorner);
        yAxisLength = Vector3.Distance(lowerLeftCorner, upperLeftCorner);

    }

    void findCorners(){
        var form = puzzle.puzzle.form;
        List<Vector3> corners = new List<Vector3>();
        int n = 0;
        while(n<form.Length){
            Vector3 vector = new Vector3(form[n].coord.x, form[n].coord.y, 0);
            corners.Add(vector);
            n++;
        }

        lowerLeftCorner = corners[0];
        n = 1;
        while(n<form.Length){
            if(corners[n].x < lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }
            if(corners[n].x == lowerLeftCorner.x && corners[n].y < lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(lowerLeftCorner);
        
        upperLeftCorner = corners[0];
        n = 1;
        while(n<form.Length){
            if(corners[n].x < lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }else if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperLeftCorner);

        upperRightCorner = corners[0];
        n = 1;
        while(n<form.Length){
            if(corners[n].x > lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }else if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperRightCorner);

        lowerRightCorner = corners[0];
    }
    void calculateNextAngle(){

    }
    void findPotentialPieces(){
        foreach(GameObject piece in pieces){
            float[] angles = piece.GetComponent<PieceInfo>().angles;
            foreach(float angle in angles){
                if(angle == theta){
                    potentialPieces.Add(piece);
                    break;
                }
            }
        }
    }
    bool isPlacedCorrectly(){
        bool placedCorrectly = true;

        return placedCorrectly;
    }


}

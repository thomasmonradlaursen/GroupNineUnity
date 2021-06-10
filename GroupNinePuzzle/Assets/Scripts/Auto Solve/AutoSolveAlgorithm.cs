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
    Vector3 upperLeftCorner = new Vector3(0,0,0);
    Vector3 upperRightCorner = new Vector3(0,0,0);
    Vector3 lowerRightCorner = new Vector3(0,0,0);
    Vector3 lowerLeftCorner = new Vector3(0,0,0);
    
    void Start(){
        
    }
    public void Calculate(){
        /*
        puzzle = GetComponentInParent<PieceController>().puzzle;
        pieces = GetComponentInParent<PieceController>().pieces;
        */
        puzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;

        findCorners();
        Debug.Log("Upper left corner: ("+ upperLeftCorner[0]+","+upperLeftCorner[1]+")");
        Debug.Log("Upper right corner: ("+ upperRightCorner[0]+","+upperRightCorner[1]+")");
        Debug.Log("Lower left corner: ("+ lowerRightCorner[0]+","+lowerRightCorner[1]+")");
        Debug.Log("Lower right corner: ("+ lowerLeftCorner[0]+","+lowerLeftCorner[1]+")");
        xAxisLength = Vector3.Distance(upperLeftCorner, upperRightCorner);
        yAxisLength = Vector3.Distance(lowerLeftCorner, upperLeftCorner);

    }

    void findCorners(){
        var form = puzzle.puzzle.form;
        List<Vector3> corners = new List<Vector3>();
        int n = 0;
        while(n<form.Length){
            Vector3 vector = new Vector3(form[n].coord.x, form[n].coord.y, 0);
            Debug.Log("Corner "+n+": ("+vector[0]+","+vector[1]+")");
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
        Debug.Log("Lower left corner: ("+ lowerRightCorner[0]+","+lowerRightCorner[1]+")");
        corners.Remove(lowerLeftCorner);

        upperLeftCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x < lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }else if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        Debug.Log("Upper left corner: ("+ upperLeftCorner[0]+","+upperLeftCorner[1]+")");
        corners.Remove(upperLeftCorner);

        upperRightCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x > lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }else if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        Debug.Log("Upper right corner: ("+ upperRightCorner[0]+","+upperRightCorner[1]+")");
        corners.Remove(upperRightCorner);

        lowerRightCorner = corners[0];
        Debug.Log("Lower right corner: ("+ lowerRightCorner[0]+","+lowerRightCorner[1]+")");
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

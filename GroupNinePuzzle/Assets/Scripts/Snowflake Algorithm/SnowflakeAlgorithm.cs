using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeAlgorithm : MonoBehaviour
{
    AreaSorting areaSorting = new AreaSorting();
    LengthsAndAnglesSorting lengthsAndAnglesSorting = new LengthsAndAnglesSorting();
    JSONPuzzle puzzle;
    (bool, string) resultAndMessage = (true, "success");
    List<Vector2> piecesWithIdenticalArea;  //(pieceName1, pieceName2) 
    List<float> areaOfPieces;
    List<float[]> lengthsOfPieces;
    List<float[]> anglesOfPieces;
    List<GameObject> pieces;

    void Start()
    {
        Debug.Log("SnowflakeAlgorithm Start()");
        puzzle = GetComponentInParent<PieceController>().puzzle;
        pieces = GetComponent<PieceController>().pieces;

        areaOfPieces = areaSorting.GetAreaOfPieces(pieces);
        lengthsOfPieces = lengthsAndAnglesSorting.GetLengthsOfPieces(pieces);
        anglesOfPieces = lengthsAndAnglesSorting.GetAnglesOfPieces(pieces);

        //check areas
        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces, puzzle);

        //check angles and side lengths
        if(piecesWithIdenticalArea.Count != 0){
            for(int i = 0; i < piecesWithIdenticalArea.Count; i++){
                if(DetermineSnowflakeismByAnglesAndSides(piecesWithIdenticalArea[i]) == false){
                   piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[i]);
               }else{
                   Debug.Log("Pieces "+ piecesWithIdenticalArea[i]+" have identical sideslengths or angles");
               }
            }
        }
        //output result of snowflakeism
        if(piecesWithIdenticalArea.Count != 0){
            Debug.Log("This puzzle contains identical pieces");
        }
    }
    void DetermineSnowflakeismByArea()
    {
        if (piecesWithIdenticalArea.Count != 0)
        {
            resultAndMessage.Item1 = false;
            resultAndMessage.Item2 = "area";
        }
    }

    bool DetermineSnowflakeismByAnglesAndSides(Vector2 piecesToCompare)          //return true if identical
    {
        bool areIdentical = true;
        float[] sidesA = lengthsOfPieces[(int) piecesToCompare[0]];
        float[] sidesB = lengthsOfPieces[(int) piecesToCompare[1]];

        float[] anglesA = anglesOfPieces[(int) piecesToCompare[0]];
        float[] anglesB = anglesOfPieces[(int) piecesToCompare[1]];

        
        int n = 0;
        foreach(float side in sidesA){
            Debug.Log("A: side "+n+": "+side);
            n++;
        }
        n = 0;
        foreach(float side in sidesB){
            Debug.Log("B: side "+n+": "+side);
            n++;
        }
        

        bool collision = false;
        int collisionPoint = 0;
        
        for(int j = 0; j<anglesB.Length; j++){
            if(anglesA[0] == anglesB[j]){
                collision = true;
                collisionPoint = j;
                break;
            }
        }
        
        if(collision == true){
            sidesB = alignArray(sidesB, collisionPoint);
            anglesB = alignArray(anglesB, collisionPoint);
            collision = true;
            int counter;
            if(anglesA.Length <= anglesB.Length){
                counter = anglesA.Length;
            }else{
                counter = anglesB.Length;
                float[] tempAngle = anglesA;
                float[] tempSide = sidesA;
                anglesA = anglesB;
                sidesA = sidesB;
                anglesB = tempAngle;
                sidesB = tempSide;
            }
            float carryA = 0; float carryB = 0;
            int i = 0; int j = 0;
            while(i < counter && areIdentical == true){
                if(anglesA[i] == (int) 180 && anglesB[j] != (int) 180){
                    carryA += sidesA[i];
                    //Debug.Log("Caught 180 angle in A");
                    i++;
                }
                if(anglesB[j] == (int) 180 && anglesA[i] != (int) 180){
                    carryB += sidesB[j];
                    //Debug.Log("Caught 180 angle in B");
                    j++;
                }
                if( (anglesA[i] != anglesB[j] || carryA != carryB) && anglesA[i] != (int) 180 && anglesB[j] != (int) 180 ){
                        /*
                        Debug.Log("Piece A, angle "+i+": "+anglesA[i]);
                        Debug.Log("Piece B, angle "+j+": "+anglesB[j]);
                        Debug.Log("carryA: "+ carryA);
                        Debug.Log("carryB: "+ carryB);
                        */
                        collision = false;
                        areIdentical = false;
                }
                
                carryA += sidesA[i];
                carryB += sidesB[j];
                j++;
                i++;
            }

            if(j < anglesB.Length && areIdentical == true){     //if we're not through the second angle array when the first is empty
                while(j<anglesB.Length){    //we need to check if the remaining angles are all 180. 
                    if(anglesB[j] != 180){
                        areIdentical = false;
                    }
                    j++;
                }
            }
        }
        return areIdentical;
    }

    float[] alignArray(float[] arrayToAlign, int newStart){
        float[] aligned = new float[arrayToAlign.Length];
        for(int i = 0; i < arrayToAlign.Length; i++){
            if(newStart >= arrayToAlign.Length){
                newStart = 0;
            }
            aligned[i] = arrayToAlign[newStart];
            newStart++;
        }
        return aligned;
    }

    void DetermineReasonForFailure()
    {
        if (resultAndMessage.Item2.Equals("area"))
        {
            Debug.Log("Reason for failure: The puzzle contains pieces with identical area");
            Debug.Log("The following pieces have identical area:");
            foreach (Vector2 pair in piecesWithIdenticalArea)
            {
                Debug.Log(string.Format("Piece {0} and Piece {1} - Identical area: {2}", pair.x, pair.y, areaOfPieces[(int)pair.x]));
            }
        }
        if (resultAndMessage.Item2.Equals("lengthsAndAngles"))
        {
            Debug.Log("Reason for failure: The puzzle contains pieces with identical lengths and angles");
            Debug.Log("The following pieces have identical lengths and angles:");
        }
    }

}
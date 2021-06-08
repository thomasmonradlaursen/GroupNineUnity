using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class SnowflakeAlgorithm : MonoBehaviour

{
    MiscellaneousMath miscellaneousMath = new MiscellaneousMath();
    AreaSorting areaSorting = new AreaSorting();
    LengthsAndAnglesSorting lengthsAndAnglesSorting = new LengthsAndAnglesSorting();
    JSONPuzzle puzzle;
    (bool, string) resultAndMessage = (true, "success");
    List<Vector2> piecesWithIdenticalArea;  //(pieceName1, pieceName2)
    List<Vector2> piecesWithIdenticalLengthsAndAngles;   
    List<float> areaOfPieces;
    List<float[]> lengthsOfPieces;
    List<float[]> anglesOfPieces;
    List<GameObject> pieces;

    void Start()
    {
        puzzle = GetComponentInParent<PieceController>().puzzle;
        pieces = GetComponent<PieceController>().pieces;

        areaOfPieces = areaSorting.GetAreaOfPieces(pieces);
        lengthsOfPieces = lengthsAndAnglesSorting.GetLengthsOfPieces(pieces);
        anglesOfPieces = lengthsAndAnglesSorting.GetAnglesOfPieces(pieces);

        //check areas
        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces, puzzle);

        //check angles
        if(piecesWithIdenticalArea.Count != 0){
            for(int i = 0; i < piecesWithIdenticalArea.Count; i++){
               if(DetermineSnowflakeismByAngles(piecesWithIdenticalArea[i]) == false){
                   piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[i]);
               }
            }
        }
        //check side lengths
        if(piecesWithIdenticalArea.Count != 0){
            for(int i = 0; i < piecesWithIdenticalArea.Count; i++){
                if(DetermineSnowflakeismBySides(piecesWithIdenticalArea[i]) == false){
                   piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[i]);
               }
            }
        }
        //output result of snowflakeism
        if(piecesWithIdenticalArea.Count != 0){
            Debug.Log("This puzzle contains identical pieces");
        }
    }
    /*
    public void LogResult()
    {
        DetermineSnowflakeism();
        if (resultAndMessage.Item1)
        {
            Debug.Log("Snowflakeism for puzzle: True");
        }
        else
        {
            Debug.Log("Snowflakeism for puzzle: False");
            DetermineReasonForFailure();
        }
    }
   

    void DetermineSnowflakeism()
    {
        DetermineSnowflakeismByArea();
    }
    */
    void DetermineSnowflakeismByArea()
    {
        if (piecesWithIdenticalArea.Count != 0)
        {
            resultAndMessage.Item1 = false;
            resultAndMessage.Item2 = "area";
        }
    }

    bool DetermineSnowflakeismBySides(Vector2 piecesToCompare)          //return true if identical
    {
        bool areIdentical = true;
        float[] sidesA = lengthsOfPieces[(int) piecesToCompare[0]];
        float[] sidesB = lengthsOfPieces[(int) piecesToCompare[1]];

        bool collision = false;
        int collisionPoint = 0;
        
        for(int j = 0; j<sidesB.Length; j++){
            if(sidesA[0] == sidesB[j]){
                collision = true;
                collisionPoint = j;
                break;
            }
        }
        
        if(collision == true){
            sidesB = alignArray(sidesB, collisionPoint);
            collision = true;

            for(int i = 0; i<sidesA.Length; i++){
                for(int j = 0; j<sidesB.Length; j++){
                    if(sidesA[i] != sidesB[j]){
                        collision = false;
                        //Debug.Log("Pieces "+piecesToCompare[0]+ " and "+ piecesToCompare[1]+" are different");
                        areIdentical = false;
                        i = sidesA.Length;
                        break;
                    }
                }
            }
            if(collision == true){
                Debug.Log("Pieces "+piecesToCompare[0]+ " and "+ piecesToCompare[1]+" have identical sideslengths");
                areIdentical = true;
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
    bool DetermineSnowflakeismByAngles(Vector2 piecesToCompare)         //return true if identical 
    {
        bool areIdentical = true;
        float[] anglesA = anglesOfPieces[(int) piecesToCompare[0]];
        float[] anglesB = anglesOfPieces[(int) piecesToCompare[1]];

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
            anglesB = alignArray(anglesB, collisionPoint);
            collision = true;
            int counter;
            if(anglesA.Length <= anglesB.Length){
                counter = anglesA.Length;
            }else{
                counter = anglesB.Length;
                float[] temp = anglesA;
                anglesA = anglesB;
                anglesB = temp;
            }
            int i = 0; int j = 0;
            while(i < counter){
                if(anglesA[i] != anglesB[j] && (anglesA[i] != (int) 180 && anglesB[j] != (int) 180)){
                        collision = false;
                        Debug.Log("Pieces "+piecesToCompare[0]+ " and "+ piecesToCompare[1]+" have different angles:");
                        Debug.Log("Piece "+piecesToCompare[0]+ " angle "+ i+" : " +anglesA[i]);
                        Debug.Log("Piece "+piecesToCompare[1]+ " angle "+ j+" : " +anglesB[j]);
                        areIdentical = false;
                }
                if(anglesA[i] == (int) 180){
                    j--;
                }
                if(anglesB[j] == (int) 180){
                    i--;
                }
                j++;
                i++;
            }
            /*
            for(int i = 0; i<anglesA.Length; i++){
                for(int j = 0; j<anglesB.Length; j++){
                    if(anglesA[i] != anglesB[j] && (anglesA[i] != (int) 180 && anglesB[j] != (int) 180)){
                        collision = false;
                        Debug.Log("Pieces "+piecesToCompare[0]+ " and "+ piecesToCompare[1]+" have different angles:");
                        Debug.Log("Piece "+piecesToCompare[0]+ " angle "+ i+" : " +anglesA[i]);
                        Debug.Log("Piece "+piecesToCompare[1]+ " angle "+ j+" : " +anglesB[j]);
                        areIdentical = false;
                        i = anglesA.Length; //break outer loop
                        break;              //break inner loop
                    }
                }
            }
            */
            if(collision == true){
                Debug.Log("Pieces "+piecesToCompare[0]+ " and "+ piecesToCompare[1]+" have identical angles");
                areIdentical = true;
            }
        }
        return areIdentical;

    }

    void specialCase180(float suspectAngle, float[] anglesA, float[] anglesB){
        if((int) suspectAngle == 180){

        }
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
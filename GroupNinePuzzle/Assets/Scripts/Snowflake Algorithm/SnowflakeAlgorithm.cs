﻿using System.Collections;
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
        puzzle = GetComponentInParent<PuzzleModel>().puzzle;
        pieces = GetComponentInParent<PuzzleModel>().pieces;
<<<<<<< Updated upstream
        foreach(GameObject piece in pieces){
            Debug.Log("piece "+piece.GetComponent<PieceInfo>().name);
=======
        /*
        foreach (GameObject piece in pieces)
        {
            Debug.Log("piece " + piece.GetComponent<PieceInfo>().name);
>>>>>>> Stashed changes
        }
        */
        areaOfPieces = areaSorting.GetAreaOfPieces(pieces);
        lengthsOfPieces = lengthsAndAnglesSorting.GetLengthsOfPieces(pieces);
        anglesOfPieces = lengthsAndAnglesSorting.GetAnglesOfPieces(pieces);

        //check areas
        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces);

        //check angles and side lengths
<<<<<<< Updated upstream
        if(piecesWithIdenticalArea.Count != 0){
            Debug.Log("Number of pairs with identical areas: "+ piecesWithIdenticalArea.Count);
            
            while(piecesWithIdenticalArea.Count != 0){
                if(DetermineSnowflakeismRoundTwo(piecesWithIdenticalArea[0]) == false){
                   piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[0]);
                   Debug.Log("removed a pair of pieces, pairs left to compare: "+piecesWithIdenticalArea.Count);
               }else{
                   Debug.Log("Pieces "+ piecesWithIdenticalArea[0]+" have identical sideslengths or angles");
                   break;
               }
=======
        if (piecesWithIdenticalArea.Count != 0)
        {
            //Debug.Log("Number of pairs with identical areas: " + piecesWithIdenticalArea.Count);

            while (piecesWithIdenticalArea.Count != 0)
            {
                if (DetermineSnowflakeismRoundTwo(piecesWithIdenticalArea[0]) == false)
                {
                    piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[0]);
                    //Debug.Log("removed a pair of pieces, pairs left to compare: " + piecesWithIdenticalArea.Count);
                }
                else
                {
                    //Debug.Log("Pieces " + piecesWithIdenticalArea[0] + " have identical sideslengths or angles");
                    break;
                }
>>>>>>> Stashed changes
            }
        }
        //output result of snowflakeism
        if(piecesWithIdenticalArea.Count != 0){
            Debug.Log("This puzzle contains identical pieces");
        }else{
            Debug.Log("This puzzle has unique pieces");
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

        List<int> indicesOfIdenticalAnglesInA = new List<int>();
        int numberOfIdenticalAnglesInA = 0; int n = 0;
        foreach(float angle in anglesA){
            if(angle > anglesA[0]-0.01 && angle < anglesA[0]+0.01){
                numberOfIdenticalAnglesInA++;
                indicesOfIdenticalAnglesInA.Add(n);
                n++;
            }
        }
        /*
        foreach(float side in sidesA){
            Debug.Log("A: side "+n+": "+side);
        }
        foreach(float side in sidesB){
            Debug.Log("B: side "+n+": "+side);
        }
        */
        int index = 0;
        while(index < numberOfIdenticalAnglesInA){
            bool collision = false;
            int collisionPoint = indicesOfIdenticalAnglesInA[index];
            float[] anglesAtemp = alignArray(anglesA, index);
            float[] sidesAtemp = alignArray(sidesA, index);

            for(int j = 0; j<anglesB.Length; j++){      //aligning arrays, start
                if(anglesAtemp[0] == anglesB[j]){
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
                        Debug.Log("Caught 180 angle in A");
                        i++;
                    }
                    if(anglesB[j] == (int) 180 && anglesA[i] != (int) 180){
                        carryB += sidesB[j];
                        Debug.Log("Caught 180 angle in B");
                        j++;
                    }
                    if( (anglesA[i] != anglesB[j] || carryA != carryB) && anglesA[i] != (int) 180 && anglesB[j] != (int) 180 ){
                        
                        Debug.Log("Piece A, angle "+i+": "+anglesA[i]);
                        Debug.Log("Piece B, angle "+j+": "+anglesB[j]);
                        Debug.Log("carryA: "+ carryA);
                        Debug.Log("carryB: "+ carryB);
                        
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
                }else{
                areIdentical = false;
                }
            } 
        }    //end of outer loop
        return areIdentical;
    }

    bool DetermineSnowflakeismRoundTwo(Vector2 piecesToCompare){
        bool areIdentical = false;
        float[] sidesA = lengthsOfPieces[(int) piecesToCompare[0]];
        float[] sidesB = lengthsOfPieces[(int) piecesToCompare[1]];

        float[] anglesA = anglesOfPieces[(int) piecesToCompare[0]];
        float[] anglesB = anglesOfPieces[(int) piecesToCompare[1]];
        
        if(anglesA.Length > anglesB.Length){
            float[] temp = anglesA;
            anglesA = anglesB;
            anglesB = temp;

            temp = sidesA;
            sidesA = sidesB;
            sidesB = temp;
        }
<<<<<<< Updated upstream
        
=======
        /*
>>>>>>> Stashed changes
        Debug.Log("sides lists:");
        foreach(float side in sidesA){
            Debug.Log("A: "+side);
        }
        foreach(float side in sidesB){
            Debug.Log("B: "+side);
        }
<<<<<<< Updated upstream
        
=======
        */
>>>>>>> Stashed changes

        List<int> indicesOfIdenticalAnglesInA = new List<int>();
        int numberOfIdenticalAnglesInA = 0; int n = 0;
        foreach(float angle in anglesA){
            if(angle > anglesA[0]-0.01 && angle < anglesA[0]+0.01){
                numberOfIdenticalAnglesInA++;
                indicesOfIdenticalAnglesInA.Add(n);
            }
            n++;
        }

<<<<<<< Updated upstream
        while(indicesOfIdenticalAnglesInA.Count > 0){
            Debug.Log("index: "+indicesOfIdenticalAnglesInA[0]);
=======
        while (indicesOfIdenticalAnglesInA.Count > 0)
        {
            //Debug.Log("While at 112");
            //Debug.Log("index: " + indicesOfIdenticalAnglesInA[0]);
>>>>>>> Stashed changes
            float[] tempAngles = anglesA;
            float[] tempSides = sidesA;
            tempAngles = alignArray(anglesA, indicesOfIdenticalAnglesInA[0]);
            tempSides = alignArray(sidesA, indicesOfIdenticalAnglesInA[0]);
            /*
            Debug.Log("SIDES IN A:");
            foreach(float side in tempSides){
                Debug.Log(side+"; ");
            }
            Debug.Log("SIDES IN B:");
            foreach(float side in sidesB){
                Debug.Log(side+"; ");
            }
<<<<<<< Updated upstream
            if(IdenticalAnglesLists(tempAngles, anglesB, tempSides, sidesB)){
=======
            */
            if (IdenticalAnglesLists(tempAngles, anglesB, tempSides, sidesB))
            {
>>>>>>> Stashed changes
                areIdentical = true; break;
            }else{
                indicesOfIdenticalAnglesInA.Remove(indicesOfIdenticalAnglesInA[0]);
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
    bool IdenticalAnglesLists(float[] shortListAngles, float[] longListAngles, float[] shortListSides, float[] longListSides)
    {
        float carryA =0; float carryB =0; 
        int j = 0;
        for(int i = 0; i<shortListAngles.Length; i++){
            //Debug.Log("Angle in A: "+ shortListAngles[i]+", Angle in B: "+ longListAngles[j]);
<<<<<<< Updated upstream
            Debug.Log("i = "+i+" , j = "+j);
=======
            //Debug.Log("i = " + i + " , j = " + j);
>>>>>>> Stashed changes
            //if angles are not identical
            if(!(longListAngles[j] >= shortListAngles[i]-0.01 && longListAngles[j] <= shortListAngles[i]+0.01)){
                //Debug.Log("angles not identical:");
                //Debug.Log("A: "+shortListAngles[i]);
                //Debug.Log("B: "+longListAngles[j]);
                //if angleA is 180
<<<<<<< Updated upstream
                if((shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && !(shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99)){
                    Debug.Log("caught 180 degree angle in A");
                    if(i != 0){
                        carryA = shortListSides[(shortListSides.Length-1)];
                    }else{
                        carryA = shortListSides[i-1];
=======
                if ((shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && !(shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99))
                {
                    //Debug.Log("caught 180 degree angle in A");
                    if (i != 0)
                    {
                        carryA = shortListSides[(shortListSides.Length - 1)];
                    }
                    else
                    {
                        carryA = shortListSides[i - 1];
>>>>>>> Stashed changes
                    }
                    j--; continue;
                }
                //if angleB is 180
<<<<<<< Updated upstream
                else if(!(shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && (shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99)){
                    Debug.Log("caught 180 degree angle in B");
                    if(j != 0){
                        carryB = longListSides[(longListSides.Length-1)];
                    }else{
                        carryB = longListSides[j-1];
=======
                else if (!(shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && (shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99))
                {
                    //Debug.Log("caught 180 degree angle in B");
                    if (j != 0)
                    {
                        carryB = longListSides[(longListSides.Length - 1)];
                    }
                    else
                    {
                        carryB = longListSides[j - 1];
>>>>>>> Stashed changes
                    }
                    i--; continue;
                } else{
                    //Debug.Log("angles not identical and neither are 180");
                    return false;
                }
            }
            //if angles are identical, check sides as well
<<<<<<< Updated upstream
            if((longListAngles[j] >= shortListAngles[i]-0.01 && longListAngles[j] <= shortListAngles[i]+0.01)){
                Debug.Log("Found identical angles: (A, B) = "+"("+shortListAngles[i]+", "+longListAngles[j]+")");
=======
            if ((longListAngles[j] >= shortListAngles[i] - 0.01 && longListAngles[j] <= shortListAngles[i] + 0.01))
            {
                //Debug.Log("Found identical angles: (A, B) = " + "(" + shortListAngles[i] + ", " + longListAngles[j] + ")");
>>>>>>> Stashed changes
                bool identicalSides = IdenticalSidesLists(shortListSides, longListSides, i, j, carryA, carryB);
                if(!identicalSides){
                    return false;
                } else{
                    carryA = 0;
                    carryB = 0;
                }
            }
            j++;
        }    

        //if we get to the end and the longList is still not through,
        //we need to check if the remaining angles are all 180. 
<<<<<<< Updated upstream
        if(longListAngles.Length-1 > j){
            while(j<longListAngles.Length){    
                if(!(longListAngles[j] <= 180.01 && longListAngles[j] >= 179.99)){
=======
        if (longListAngles.Length - 1 > j)
        {
            while (j < longListAngles.Length)
            {
                if (!(longListAngles[j] <= 180.01 && longListAngles[j] >= 179.99))
                {
>>>>>>> Stashed changes
                    return false;
                }
                j++;
            }
        }
        return true;
    }

<<<<<<< Updated upstream
    bool IdenticalSidesLists(float[] shortList, float[] longList, int i, int j, float carryA, float carryB){
        Debug.Log("checking sides now!!!! i: "+i+" , j: "+j);
        Debug.Log("SideA = "+shortList[i] +" + "+ carryA);
        Debug.Log("SideB = "+longList[i] +" + "+ carryB);
=======
    bool IdenticalSidesLists(float[] shortList, float[] longList, int i, int j, float carryA, float carryB)
    {
        //Debug.Log("checking sides now!!!! i: " + i + " , j: " + j);
        //Debug.Log("SideA = " + shortList[i] + " + " + carryA);
        //Debug.Log("SideB = " + longList[i] + " + " + carryB);
>>>>>>> Stashed changes
        float sideA = shortList[i] + carryA;
        float sideB = longList[j] + carryB;
        if(sideA >= sideB-0.01 && sideA <= sideB+0.01){
            return true;
<<<<<<< Updated upstream
        }else{
            Debug.Log("sides not identical:");
                Debug.Log("A: "+ sideA);
                Debug.Log("B: "+ sideB);
=======
        }
        else
        {
            //Debug.Log("sides not identical:");
            //Debug.Log("A: " + sideA);
            //Debug.Log("B: " + sideB);
>>>>>>> Stashed changes
        }
        return false;
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
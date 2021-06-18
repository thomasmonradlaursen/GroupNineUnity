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

    public bool DetermineSnowflakeism()
    {
        // This is fixed
        Debug.Log("SnowflakeAlgorithm Start()");
        puzzle = GetComponentInParent<PuzzleModel>().puzzle;
        pieces = GetComponentInParent<PuzzleModel>().pieces;
        foreach (GameObject piece in pieces)
        {
            Debug.Log("piece " + piece.GetComponent<PieceInfo>().name);
        }

        areaOfPieces = areaSorting.GetAreaOfPieces(pieces);
        lengthsOfPieces = lengthsAndAnglesSorting.GetLengthsOfPieces(pieces);
        anglesOfPieces = lengthsAndAnglesSorting.GetAnglesOfPieces(pieces);

        //check areas
        piecesWithIdenticalArea = areaSorting.FindPiecesWithIdenticalArea(areaOfPieces);

        //check angles and side lengths
        if (piecesWithIdenticalArea.Count != 0)
        {
            Debug.Log("Number of pairs with identical areas: " + piecesWithIdenticalArea.Count);

            while (piecesWithIdenticalArea.Count != 0)
            {
                Debug.Log("While at 42");
                if (DetermineSnowflakeismRoundTwo(piecesWithIdenticalArea[0]) == false)
                {
                    piecesWithIdenticalArea.Remove(piecesWithIdenticalArea[0]);
                    Debug.Log("removed a pair of pieces, pairs left to compare: " + piecesWithIdenticalArea.Count);
                }
                else
                {
                    Debug.Log("Pieces " + piecesWithIdenticalArea[0] + " have identical sideslengths or angles");
                    break;
                }
            }
        }
        //output result of snowflakeism
        if (piecesWithIdenticalArea.Count != 0)
        {
            Debug.Log("This puzzle contains identical pieces");
            return false;
        }
        else
        {
            Debug.Log("This puzzle has unique pieces");
            return true;
        }
    }

    bool DetermineSnowflakeismRoundTwo(Vector2 piecesToCompare)
    {
        bool areIdentical = false;
        float[] sidesA = lengthsOfPieces[(int)piecesToCompare[0]];
        float[] sidesB = lengthsOfPieces[(int)piecesToCompare[1]];

        float[] anglesA = anglesOfPieces[(int)piecesToCompare[0]];
        float[] anglesB = anglesOfPieces[(int)piecesToCompare[1]];

        if (anglesA.Length > anglesB.Length)
        {
            float[] temp = anglesA;
            anglesA = anglesB;
            anglesB = temp;

            temp = sidesA;
            sidesA = sidesB;
            sidesB = temp;
        }

        Debug.Log("sides lists:");
        foreach (float side in sidesA)
        {
            Debug.Log("A: " + side);
        }
        foreach (float side in sidesB)
        {
            Debug.Log("B: " + side);
        }


        List<int> indicesOfIdenticalAnglesInA = new List<int>();
        int numberOfIdenticalAnglesInA = 0; int n = 0;
        foreach (float angle in anglesA)
        {
            if (angle > anglesA[0] - 0.01 && angle < anglesA[0] + 0.01)
            {
                numberOfIdenticalAnglesInA++;
                indicesOfIdenticalAnglesInA.Add(n);
            }
            n++;
        }

        while (indicesOfIdenticalAnglesInA.Count > 0)
        {
            Debug.Log("While at 112");
            Debug.Log("index: " + indicesOfIdenticalAnglesInA[0]);
            float[] tempAngles = anglesA;
            float[] tempSides = sidesA;
            tempAngles = alignArray(anglesA, indicesOfIdenticalAnglesInA[0]);
            tempSides = alignArray(sidesA, indicesOfIdenticalAnglesInA[0]);

            Debug.Log("SIDES IN A:");
            foreach (float side in tempSides)
            {
                Debug.Log(side + "; ");
            }
            Debug.Log("SIDES IN B:");
            foreach (float side in sidesB)
            {
                Debug.Log(side + "; ");
            }
            if (IdenticalAnglesLists(tempAngles, anglesB, tempSides, sidesB))
            {
                areIdentical = true; break;
            }
            else
            {
                indicesOfIdenticalAnglesInA.Remove(indicesOfIdenticalAnglesInA[0]);
            }
        }
        return areIdentical;
    }
    float[] alignArray(float[] arrayToAlign, int newStart)
    {
        float[] aligned = new float[arrayToAlign.Length];
        for (int i = 0; i < arrayToAlign.Length; i++)
        {
            if (newStart >= arrayToAlign.Length)
            {
                newStart = 0;
            }
            aligned[i] = arrayToAlign[newStart];
            newStart++;
        }
        return aligned;
    }
    bool IdenticalAnglesLists(float[] shortListAngles, float[] longListAngles, float[] shortListSides, float[] longListSides)
    {
        float carryA = 0; float carryB = 0;
        int j = 0;
        for (int i = 0; i < shortListAngles.Length; i++)
        {
            //Debug.Log("Angle in A: "+ shortListAngles[i]+", Angle in B: "+ longListAngles[j]);
            Debug.Log("i = " + i + " , j = " + j);
            //if angles are not identical
            if (!(longListAngles[j] >= shortListAngles[i] - 0.01 && longListAngles[j] <= shortListAngles[i] + 0.01))
            {
                //Debug.Log("angles not identical:");
                //Debug.Log("A: "+shortListAngles[i]);
                //Debug.Log("B: "+longListAngles[j]);
                //if angleA is 180
                if ((shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && !(shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99))
                {
                    Debug.Log("caught 180 degree angle in A");
                    if (i != 0)
                    {
                        carryA = shortListSides[(shortListSides.Length - 1)];
                    }
                    else
                    {
                        carryA = shortListSides[i - 1];
                    }
                    j--; continue;
                }
                //if angleB is 180
                else if (!(shortListAngles[i] <= 180.01 && shortListAngles[i] >= 179.99) && (shortListAngles[j] <= 180.01 && shortListAngles[j] >= 179.99))
                {
                    Debug.Log("caught 180 degree angle in B");
                    if (j != 0)
                    {
                        carryB = longListSides[(longListSides.Length - 1)];
                    }
                    else
                    {
                        carryB = longListSides[j - 1];
                    }
                    i--; continue;
                }
                else
                {
                    //Debug.Log("angles not identical and neither are 180");
                    return false;
                }
            }
            //if angles are identical, check sides as well
            if ((longListAngles[j] >= shortListAngles[i] - 0.01 && longListAngles[j] <= shortListAngles[i] + 0.01))
            {
                Debug.Log("Found identical angles: (A, B) = " + "(" + shortListAngles[i] + ", " + longListAngles[j] + ")");
                bool identicalSides = IdenticalSidesLists(shortListSides, longListSides, i, j, carryA, carryB);
                if (!identicalSides)
                {
                    return false;
                }
                else
                {
                    carryA = 0;
                    carryB = 0;
                }
            }
            j++;
        }

        //if we get to the end and the longList is still not through,
        //we need to check if the remaining angles are all 180. 
        if (longListAngles.Length - 1 > j)
        {
            while (j < longListAngles.Length)
            Debug.Log("While at 224");
            {
                if (!(longListAngles[j] <= 180.01 && longListAngles[j] >= 179.99))
                {
                    return false;
                }
                j++;
            }
        }
        return true;
    }

    bool IdenticalSidesLists(float[] shortList, float[] longList, int i, int j, float carryA, float carryB)
    {
        Debug.Log("checking sides now!!!! i: " + i + " , j: " + j);
        Debug.Log("SideA = " + shortList[i] + " + " + carryA);
        Debug.Log("SideB = " + longList[i] + " + " + carryB);
        float sideA = shortList[i] + carryA;
        float sideB = longList[j] + carryB;
        if (sideA >= sideB - 0.01 && sideA <= sideB + 0.01)
        {
            return true;
        }
        else
        {
            Debug.Log("sides not identical:");
            Debug.Log("A: " + sideA);
            Debug.Log("B: " + sideB);
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
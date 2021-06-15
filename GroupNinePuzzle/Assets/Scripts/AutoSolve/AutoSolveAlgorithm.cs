using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AutoSolveAlgorithm : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    public List<GameObject> pieces;
    JSONPuzzle puzzle;
    List<GameObject> potentialPieces = new List<GameObject>();
    List<List<GameObject>> placedPieces = new List<List<GameObject>>();
    List<(int,int,GameObject)> testedPieces = new List<(int,int,GameObject)>();
    float theta = 90.0f; int indexOfTheta;
    float xAxisLength; float yAxisLength;
    Vector3 upperLeftCorner; Vector3 upperRightCorner;
    Vector3 lowerRightCorner; Vector3 lowerLeftCorner;
    Vector3 currentPoint; Vector3 nextPoint; GameObject activePiece; int currentRow;
    
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("AutoSolve - TestingImplementation()");
            AutoSolve();
        }
    }

    void AutoSolve(){

    }
     void AutoSolve2(){
        puzzle = GetComponentInParent<PieceController>().puzzle;
        pieces = GetComponent<PieceController>().pieces;
        //Debug.Log("AUTOSOLVER started");

        FindCorners();
        /*
        Debug.Log("lower left corner: "+ lowerLeftCorner);
        Debug.Log("Upper left corner: "+ upperLeftCorner);
        Debug.Log("Upper right corner: "+ upperRightCorner);
        Debug.Log("lower right corner: "+ lowerRightCorner);
        */
        List<GameObject> rowList = new List<GameObject>();
        currentPoint = upperLeftCorner;
        nextPoint = upperRightCorner;
        currentRow = 0; theta = 90.0f;

        int numberOfPieces = puzzle.pieces.Length;
        int n = 0; int place = 0;
        while(n < numberOfPieces){
            if((double)currentPoint.x == (double)upperRightCorner.x){   //check for row change
                placedPieces.Add(rowList);
                currentRow++;
                place = 0;
                Debug.Log("currentRow: "+currentRow);
                findNextPoint();
                CalculateNextAngle(true);
                //Debug.Log("currentPoint updated to be: "+currentPoint);
                //Debug.Log("nextPoint updated to be: "+nextPoint);
                //Debug.Log("Theta updated to be: "+theta);
                rowList = new List<GameObject>();
            }
            List<GameObject> temp = potentialPieces;
            FindPotentialPieces(place);
            if(potentialPieces.Count == 0){         
                potentialPieces = temp;          
                potentialPieces.Remove(activePiece);
                if(potentialPieces.Count > 0){
                    activePiece = potentialPieces[0];
                }else{
                    Debug.Log("Backtracking!");
                    rowList = Backtrack(rowList, place);
                    place = rowList.Count-1;
                    FindPotentialPieces(place);
                }
            }
            UpdateIndexOfTheta(0);
            PlacePiece();

            bool overlap = OverLapsBoard();
            List<Vector3> cornersToCheck = NumberOfThetaAnglesInPiece();
            int thetaIndex = 1;
            while(overlap == true && cornersToCheck.Count > thetaIndex){
                UpdateIndexOfTheta(thetaIndex);
                PlacePiece();
                overlap = OverLapsBoard();
                thetaIndex++;
            }
            if(overlap == false){
                rowList.Add(activePiece);
                testedPieces.Add((currentRow, place, activePiece));
                Debug.Log("placed piece "+ activePiece.GetComponent<PieceInfo>().name+" at ("+currentRow+", "+place+")");
                place++;
                pieces.Remove(activePiece);
                updateCurrentPoint();
                //Debug.Log("currentPoint updated to: "+currentPoint);
                CalculateNextAngle(false);
                //Debug.Log("theta updated to: "+theta);
                
            }else{
                potentialPieces.Remove(activePiece);
                if(potentialPieces.Count > 0){
                    activePiece = potentialPieces[0];
                }else{  //backtrack
                    Debug.Log("Backtracking at line 102!");
                    rowList = Backtrack(rowList, place);
                    place = rowList.Count-1;
                    FindPotentialPieces(place);
                }
            }
            n++;
        }
    }
    List<GameObject> Backtrack(List<GameObject> rowList, int place){   //we reach this point when potentialPieces is empty and nothing fits
        if(place > 0){
            GameObject toBeRemoved = rowList[place-1];  
            rowList.Remove(toBeRemoved);
            testedPieces.Add((currentRow, place, toBeRemoved));
            pieces.Add(toBeRemoved);
            if(rowList.Count > 0){
                activePiece = rowList[rowList.Count-1];     //piece causing trouble is in middle of a row
            }else{
                currentRow--;
                rowList = placedPieces[currentRow];
                activePiece = rowList[rowList.Count-1];     //piece causing trouble is at start of new row
            }
        }else{
            currentRow--;                                   //piece causing trouble is at the end of a row
            rowList = placedPieces[currentRow];
            GameObject toBeRemoved = rowList[place-1];  
            rowList.Remove(toBeRemoved);
            pieces.Add(toBeRemoved);
            testedPieces.Add((currentRow, place, toBeRemoved));
            activePiece = rowList[rowList.Count-1];
        }
        return rowList;
    }
    
    void PlacePiece(){
        Vector3 displacement = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]-currentPoint;
        //Debug.Log("Vertex of theta: "+ potentialPieces[0].GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]);
        AutoTranslate(displacement);
        
        Vector3 pointToBeAligned;
        if(indexOfTheta != 0){
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta-1];
        }else{
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[activePiece.GetComponent<MeshFilter>().mesh.vertices.Length-1];
        }
        float rotationAngle = CalculateRotationAngle(pointToBeAligned);
        /*
        Debug.Log("pointToBeAligned: " + pointToBeAligned);
        Debug.Log("nextPoint: " + nextPoint);
        Debug.Log("currentPoint: " + currentPoint);
        Debug.Log("rotationAngle, radians: " + rotationAngle+ " rotationAngle, degrees: "+(rotationAngle*180)/Mathf.PI);
        
        foreach(Vector3 ver in selectedPiece.GetComponent<MeshFilter>().mesh.vertices){
            Debug.Log(ver);
        }
        */
        AutoRotate(rotationAngle);
        
        
    }
    
    void FindPotentialPieces(int place){
        Debug.Log("Number of pieces: "+pieces.Count);
        potentialPieces = new List<GameObject>();
        //Debug.Log("Theta: "+theta);
        foreach(GameObject piece in pieces){
            if(testedPieces.Contains((currentRow, place, piece))){ 
                continue; 
            }
            float[] angles = piece.GetComponent<PieceInfo>().angles;
            foreach(float angle in angles){
                //Debug.Log("Angle: "+angle);
                if((int) angle == (int) theta){
                    potentialPieces.Add(piece);
                    break;
                }
            }
        }
        if(potentialPieces.Count != 0){
            activePiece = potentialPieces[0];
        }
    }
    List<Vector3> NumberOfThetaAnglesInPiece(){
        List<Vector3> thetaAngleVertices = new List<Vector3>();
        int n = 0;
        foreach(float angle in activePiece.GetComponent<PieceInfo>().angles){
            if(angle == theta){
                thetaAngleVertices.Add(activePiece.GetComponent<MeshFilter>().mesh.vertices[n]);
            }
            n++;
        }
        return thetaAngleVertices;
    }
    bool OverLapsBoard(){
        bool overlapExists = false;
        Vector3[] vertices = activePiece.GetComponent<MeshFilter>().mesh.vertices;
        float smallestX = vertices[0].x; float biggestX = vertices[0].x;
        float smallestY = vertices[0].y; float biggestY = vertices[0].y;

        foreach(Vector3 vertex in vertices){
            if(vertex.x < smallestX){
                smallestX = vertex.x;
            }
            if(vertex.x > biggestX){
                biggestX = vertex.x;
            }
            if(vertex.y < smallestY){
                smallestY = vertex.y;
            }
            if(vertex.y > biggestY){
                biggestY = vertex.y;
            }
        }
        /*
        Debug.Log("smallestX : "+smallestX);
        Debug.Log("biggestX : "+biggestX);
        Debug.Log("smallestY : "+smallestY);
        Debug.Log("biggestY : "+biggestY);
        */
        if(smallestX < (lowerLeftCorner.x - 0.1) || biggestX > (lowerRightCorner.x + 0.1) 
           || smallestY < (lowerLeftCorner.y - 0.1) || biggestY > (upperRightCorner.y + 0.1) ){
               overlapExists = true;
           }
        Debug.Log("OVERLAP : " + overlapExists);
        return overlapExists;
    }
    void updateCurrentPoint(){
        Vector3[] vertices = activePiece.GetComponent<MeshFilter>().mesh.vertices;
        Vector3 temp = vertices[0];
        //Debug.Log("updating current point ********************");
        //Debug.Log(temp);
        int n = 1;
        while(n<vertices.Length){
            //Debug.Log("testing: "+ vertices[n]);
            if(vertices[n].x > temp.x && vertices[n].y >= (temp.y-0.1)){
                temp = vertices[n];
            }
            n++;
        }
        currentPoint = temp;

    }
    void findNextPoint(){
        //find lower left corner in piece0
        List<GameObject> inner = placedPieces[currentRow-1];
        //Debug.Log("inner accessible");
        //Debug.Log("length of inner: "+ inner.Count);
        Vector3[] vertices = inner[0].GetComponent<MeshFilter>().mesh.vertices;
        Vector3 llc = vertices[0];
        int index = 0;
        int n = 0;
        while(n < vertices.Length){
            if(llc.x > vertices[n].x){
                llc = vertices[n];
                index = n;
            }
            if(llc.x >= vertices[n].x && llc.y > vertices[n].y){
                llc = vertices[n];
                index = n;
            }
            n++;
        }
        currentPoint = vertices[index];
        if(index < vertices.Length-1){
            nextPoint = vertices[index+1];
        }else{
            nextPoint = vertices[0];
        }
        
    }
    void AutoRotate(float rotationAngle){
        Mesh meshForActivePiece = activePiece.GetComponent<MeshFilter>().mesh;
        /*
        Debug.Log("Input mesh for rotation ************************************")
        foreach(Vector3 ver in meshForActivePiece.vertices){
            Debug.Log(ver);
        }
        */
        LineRenderer lineRenderer = activePiece.GetComponent<LineRenderer>();
        
        Vector3[] rotatedVertices = new Vector3[meshForActivePiece.vertices.Length];
        Vector3[] originalVertices = meshForActivePiece.vertices;
        
        float area = mM.CalculateAreaFromMesh(meshForActivePiece);
        //Vector3 centroid = mM.CalculateCentroid(meshForActivePiece.vertices, area);
        //Debug.Log("centroid: "+ centroid);
        
        originalVertices = CentralizeVertices(currentPoint, originalVertices);
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationAngle) - originalVertices[index].y * Mathf.Sin(rotationAngle);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationAngle) + originalVertices[index].y * Mathf.Cos(rotationAngle);
        }
        //rotatedVertices = RestorePositionOfVertices(activePiece.GetComponent<PieceInfo>().centroid, rotatedVertices);
        rotatedVertices = RestorePositionOfVertices(currentPoint, rotatedVertices);
        /*
        Debug.Log("ouput mesh after rotation ************************************");
        foreach(Vector3 ver in rotatedVertices){
            Debug.Log(ver);
        }
        */
        activePiece.GetComponent<MeshFilter>().mesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        activePiece.GetComponent<MeshCollider>().sharedMesh = activePiece.GetComponent<MeshFilter>().mesh;     
    }

    void UpdateIndexOfTheta(int n){
        while(n < activePiece.GetComponent<PieceInfo>().angles.Length){
            if(activePiece.GetComponent<PieceInfo>().angles[n] == theta){
                indexOfTheta = n;
                break;
            }
            n++;
        }
    }
    void CalculateNextAngle(bool shiftingRows){
        if(shiftingRows == false){
            Vector3[] vertices = activePiece.GetComponent<MeshFilter>().mesh.vertices;
            int n = 0;
            while(currentPoint != vertices[n]){
                n++;
            }
            Vector3 nextVertex; 
            if(n == 0){
                nextVertex = vertices[vertices.Length-1];
            }else{
                nextVertex = vertices[n-1];
            }
            theta = Vector3.SignedAngle(currentPoint-nextVertex, nextPoint - currentPoint , Vector3.up);
            //Debug.Log("new theta: "+angle);
            //Debug.Log("found vertex: "+nextVertex);
        }else{
            theta = Vector3.SignedAngle(currentPoint-nextPoint, currentPoint - lowerLeftCorner, Vector3.up);
        }
    }

    float CalculateRotationAngle(Vector3 pointToBeAligned){
        float angle = Vector3.SignedAngle(pointToBeAligned-currentPoint, nextPoint-currentPoint, Vector3.up);
        //Debug.Log("*************************************************************************************");
        //Debug.Log("rotation angle: "+angle);
        if(pointToBeAligned.y > currentPoint.y){
            angle = angle*(-1);
        }
        return (Mathf.PI*angle)/180.0f;
    }
    void AutoTranslate(Vector3 displacement){
        Mesh meshForActivePiece = activePiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = activePiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForActivePiece.vertices.Length];
        for(int index = 0; index < meshForActivePiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForActivePiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForActivePiece.vertices[index].y - displacement.y;
            //Debug.Log("translated vertices: "+translatedVertices[index]);
        }
        activePiece.GetComponent<MeshFilter>().mesh.SetVertices(translatedVertices);
        meshForActivePiece.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        
        float area = mM.CalculateAreaFromMesh(meshForActivePiece);
        //Debug.Log("area: " + area);
        activePiece.GetComponent<PieceInfo>().centroid = mM.CalculateCentroid(translatedVertices, area);
        //Debug.Log("centroid: "+ selectedPiece.GetComponent<PieceInfo>().centroid);
    }
    void FindCorners(){
        var form = puzzle.puzzle.form;
        List<Vector3> corners = new List<Vector3>();
        int n = 0;
        while(n<form.Length){
            Vector3 vector = new Vector3(form[n].coord.x, form[n].coord.y, 0);
            //Debug.Log("Corner "+n+": " +vector);
            corners.Add(vector);
            n++;
        }

        lowerLeftCorner = corners[0];
        n = 1;
        while(n<corners.Count){

            if(corners[n].x < lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }
            if(corners[n].x <= lowerLeftCorner.x && corners[n].y < lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(lowerLeftCorner);

        upperLeftCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x < upperLeftCorner.x){
                upperLeftCorner = corners[n];
            }
            if(corners[n].x == upperLeftCorner.x && corners[n].y > upperLeftCorner.y){
                upperLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperLeftCorner);

        upperRightCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x > upperRightCorner.x){
                upperRightCorner = corners[n];
            }
            if(corners[n].x == upperRightCorner.x && corners[n].y > upperRightCorner.y){
                upperRightCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperRightCorner);

        lowerRightCorner = corners[0];
    }
    Vector3[] CentralizeVertices(Vector3 centroid, Vector3[] originalVertices)
    {
        for (int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x = originalVertices[index].x - centroid.x;
            originalVertices[index].y = originalVertices[index].y - centroid.y;
        }
        return originalVertices;
    }
    // Todo: already exists in Rotation.cs
    Vector3[] RestorePositionOfVertices(Vector3 centroid, Vector3[] rotatedVertices)
    {
        for (int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x = rotatedVertices[index].x + centroid.x;
            rotatedVertices[index].y = rotatedVertices[index].y + centroid.y;
        }
        return rotatedVertices;
    }
}



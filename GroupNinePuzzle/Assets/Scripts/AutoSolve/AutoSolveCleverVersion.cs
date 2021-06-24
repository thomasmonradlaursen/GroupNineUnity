
//Author: Louise Noer Kolborg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AutoSolveCleverVersion : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    JSONPuzzle puzzle; public List<GameObject> pieces;
    List<GameObject> potentialPieces = new List<GameObject>();
    List<Quadruple> placedPieces = new List<Quadruple>();     //(row, col, piece)
    List<Triple> testedPieces = new List<Triple>();
    float theta; int indexOfTheta;
    Vector3 upperLeftCorner; Vector3 upperRightCorner; Vector3 lowerRightCorner; Vector3 lowerLeftCorner;
    Vector3 currentPoint; Vector3 nextPoint; GameObject activePiece; int currentRow; int currentColumn;
    
    //SKURKEN ER I FINDPOTENTIALPIECES OG / ELLER BACKTRACKING. måske

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Autosolve started");
            AutoSolve();
        }
    }
     public void AutoSolve(){
        puzzle = GetComponentInParent<PuzzleModel>().puzzle;
        pieces = GetComponentInParent<PuzzleModel>().pieces;
        FindCorners();
        
        currentPoint = upperLeftCorner;     //we start in the upper left corner of the board
        nextPoint = upperRightCorner;       //and move right
        currentRow = 0; currentColumn = 0; theta = 90.0f; indexOfTheta = 0;     //starting theta is always 90 degrees
        int numberOfPieces = pieces.Count;


        while(placedPieces.Count < numberOfPieces){  
            Debug.Log("*********************************************************");
            Debug.Log("row and column: ("+currentRow+", "+currentColumn+")");
            bool changedRows = false;
            if(currentRow == 0 && currentColumn == 0){
                FindPotentialPieces();
                if(potentialPieces.Count == 0){
                    Debug.Log("no solutions were found");
                }
                SetActivePiece();           //thetaangles in piece are calculated here
                PlacePiece();             
            }else{
                changedRows = CheckForRowChange();
                FindPotentialPieces();
                if(potentialPieces.Count == 0){
                    Debug.Log("no potential pieces were found");
                    Backtrack();
                }
                SetActivePiece();
                PlacePiece();
            }   
                        
            bool overlap = OverLapsBoard(); 
            while(overlap == true){
                Debug.Log("Overlap detected!");
                //if piece holds more angles matching theta
                //we test the next angle
                if(activePiece.GetComponent<PieceInfo>().thetaAngles.Count > 1){ 
                    Pair temp = activePiece.GetComponent<PieceInfo>().thetaAngles[0];
                    activePiece.GetComponent<PieceInfo>().thetaAngles.Remove(temp);
                    indexOfTheta = activePiece.GetComponent<PieceInfo>().thetaAngles[0].index;     
                    PlacePiece();
                    overlap = OverLapsBoard();
                    continue;
                //if the piece only had one theta angle
                //we'll remove this piece as the active and move on to other potential pieces
                }else if(activePiece.GetComponent<PieceInfo>().thetaAngles.Count == 1){
                    Pair temp = activePiece.GetComponent<PieceInfo>().thetaAngles[0];
                    activePiece.GetComponent<PieceInfo>().thetaAngles.Remove(temp);
                    overlap = OverLapsBoard();
                    continue;
                }else{
                    potentialPieces.Remove(activePiece);
                    testedPieces.Add(new Triple(currentRow, currentColumn, activePiece));
                    if(potentialPieces.Count > 0){
                        SetActivePiece();
                        PlacePiece();
                        overlap = OverLapsBoard();
                    }else{
                        break;
                    }
                }
            }
            if(overlap == false){
            placedPieces.Add(new Quadruple(currentRow, currentColumn, currentPoint, activePiece));
            testedPieces.Add(new Triple(currentRow, currentColumn, activePiece));
            Debug.Log("PLACED "+ activePiece.GetComponent<PieceInfo>().name);
            pieces.Remove(activePiece);
            updateCurrentPoint(changedRows);
            currentColumn++;
            }else{
                Backtrack();
            }
        }
    }
    bool CheckForRowChange(){
        if(currentPoint.x >= upperRightCorner.x-0.01 && currentPoint.x <= upperRightCorner.x+0.01){   //check for row change
                currentRow++;
                currentColumn = 0;
                updateCurrentPoint(true);
                findNextPoint();
                CalculateNextAngle();
            return true;
        } else {
            findNextPoint();
            CalculateNextAngle();
            return false;
        }
    }
    void SetActivePiece(){
        activePiece = potentialPieces[0];
        Pair temp = activePiece.GetComponent<PieceInfo>().thetaAngles[0];
        indexOfTheta = temp.index;
        activePiece.GetComponent<PieceInfo>().thetaAngles.Remove(temp);
    }
    void Backtrack(){   //we reach this point when potentialPieces is empty and nothing fits
        Debug.Log("BACKTRACKING!");
        if(placedPieces.Count == 0){
            Debug.Log("THERE ARE NO SOLUTIONS TO THIS PUZZLE!");
        }else{
            Quadruple pieceToRemove = placedPieces[placedPieces.Count-1];
            currentRow = pieceToRemove.row;
            currentColumn = pieceToRemove.column;
            GameObject piece = pieceToRemove.piece;
            currentPoint = pieceToRemove.currentPoint;

            placedPieces.Remove(pieceToRemove);
            pieces.Add(piece);
            
            Debug.Log("Removed "+piece.GetComponent<PieceInfo>().name);
            if(placedPieces.Count == 0){
                nextPoint = upperRightCorner;
                theta = 90.0f;
                FindPotentialPieces();
            }else{
                activePiece = placedPieces[placedPieces.Count-1].piece;
                findNextPoint();
                CalculateNextAngle();
                FindPotentialPieces();
                Debug.Log("upon backtracking "+potentialPieces.Count+" potential pieces were found");
                if(potentialPieces.Count == 0){
                    Backtrack();
                }
            }
        }
    }

    void FindCorners(){
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
    void PlacePiece(){
        Vector3 displacement = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]-currentPoint;
        AutoTranslate(displacement);
        
        Vector3 pointToBeAligned;
        if(indexOfTheta != 0){
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta-1];
        }else{
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[activePiece.GetComponent<MeshFilter>().mesh.vertices.Length-1];
        }
        float rotationAngle = CalculateRotationAngle(pointToBeAligned);
        
        AutoRotate(rotationAngle);
    }   
    void AutoTranslate(Vector3 displacement){
        Mesh meshForActivePiece = activePiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = activePiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForActivePiece.vertices.Length];
        for(int index = 0; index < meshForActivePiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForActivePiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForActivePiece.vertices[index].y - displacement.y;
        }
        activePiece.GetComponent<MeshFilter>().mesh.SetVertices(translatedVertices);
        meshForActivePiece.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        activePiece.GetComponent<PieceInfo>().vertices = translatedVertices;
        
        float area = mM.CalculateAreaFromMesh(meshForActivePiece);
        activePiece.GetComponent<PieceInfo>().centroid = mM.CalculateCentroid(translatedVertices, area);
    }
    float CalculateRotationAngle(Vector3 pointToBeAligned){

        Vector2 vec1 = new Vector2(pointToBeAligned.x-currentPoint.x, pointToBeAligned.y-currentPoint.y);
        Vector2 vec2 = new Vector2(nextPoint.x-currentPoint.x, nextPoint.y-currentPoint.y);
        float angle = Vector2.SignedAngle(vec1, vec2);
        return (Mathf.PI*angle)/180.0f;
    }
    void AutoRotate(float rotationAngle){
        Mesh meshForActivePiece = activePiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = activePiece.GetComponent<LineRenderer>();
        
        Vector3[] rotatedVertices = new Vector3[meshForActivePiece.vertices.Length];
        Vector3[] originalVertices = meshForActivePiece.vertices;
        
        float area = mM.CalculateAreaFromMesh(meshForActivePiece);
        
        originalVertices = CentralizeVertices(currentPoint, originalVertices);
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationAngle) - originalVertices[index].y * Mathf.Sin(rotationAngle);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationAngle) + originalVertices[index].y * Mathf.Cos(rotationAngle);
        }
        rotatedVertices = RestorePositionOfVertices(currentPoint, rotatedVertices);

        activePiece.GetComponent<MeshFilter>().mesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        activePiece.GetComponent<MeshCollider>().sharedMesh = activePiece.GetComponent<MeshFilter>().mesh;     
        activePiece.GetComponent<PieceInfo>().vertices = rotatedVertices;
    }
    void CalculateNextAngle(){
        if(currentColumn > 0){
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
            theta = Vector3.SignedAngle(currentPoint-nextPoint, currentPoint-nextVertex, Vector3.down);
        }else{
            theta = Vector3.SignedAngle(currentPoint-nextPoint, currentPoint - lowerLeftCorner, Vector3.up);
        }
    }
    void FindPotentialPieces(){
        potentialPieces = new List<GameObject>();       //reset potPieces 
        foreach(GameObject testPiece in pieces){
            float[] angles = testPiece.GetComponent<PieceInfo>().angles;
            foreach(float angle in angles){
                if(angle <= theta+0.01 && angle >= theta-0.01){
                    testPiece.GetComponent<PieceInfo>().thetaAngles = ThetaAnglesInPiece(testPiece);
                    potentialPieces.Add(testPiece);
                    break;
                }
            }
        }
        //remove previously tested pieces
        for(int i = 0; i < potentialPieces.Count; i++){
            for(int j = 0; j < testedPieces.Count; j++){
                if(potentialPieces[i] == testedPieces[j].piece
                    && testedPieces[j].row ==currentRow 
                    && testedPieces[j].column == currentColumn){
                    potentialPieces.Remove(potentialPieces[i]);
                    i--;
                    break;
                }
            }
        }
    }
    List<Pair> ThetaAnglesInPiece(GameObject piece){
        List<Pair> thetaAngleVertices = new List<Pair>();
        int n = 0;
        foreach(float angle in piece.GetComponent<PieceInfo>().angles){
            if(angle >= theta-0.01 && angle <= theta+0.01){
                thetaAngleVertices.Add(new Pair(n, piece.GetComponent<MeshFilter>().mesh.vertices[n]));
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
            if((double)vertex.x < (double)smallestX){
                smallestX = vertex.x;
            }
            if((double)vertex.x > (double)biggestX){
                biggestX = vertex.x;
            }
            if((double)vertex.y < (double)smallestY){
                smallestY = vertex.y;
            }
            if((double)vertex.y > (double)biggestY){
                biggestY = vertex.y;
            }
        }
        if(smallestX < (lowerLeftCorner.x - 0.1) || biggestX > (lowerRightCorner.x + 0.1) 
           || smallestY < (lowerLeftCorner.y - 0.1) || biggestY > (upperRightCorner.y + 0.1) ){
               overlapExists = true;
           }
        return overlapExists;
    }
    void updateCurrentPoint(bool changedRows){
        if(changedRows == false){
           Vector3[] vertices = activePiece.GetComponent<MeshFilter>().mesh.vertices; 
           Vector3 temp = vertices[0];
            int n = 1;
            while(n<vertices.Length){
                //find the upper left corner of the active piece
                if(vertices[n].x > temp.x && vertices[n].y >= (temp.y-0.01)){
                    temp = vertices[n];
                }
                n++;
            }
            currentPoint = temp;

        }else{
            GameObject pieceAbove = new GameObject(); 
            foreach(Quadruple trip in placedPieces){
                if(trip.row == currentRow-1 && trip.column == currentColumn){
                    pieceAbove = trip.piece;
                }
            }
            Vector3[] verticesInPieceAbove = pieceAbove.GetComponent<MeshFilter>().mesh.vertices; 
            Vector3[] verticesInActivePiece = activePiece.GetComponent<MeshFilter>().mesh.vertices; 
            Vector3 temp = currentPoint;
            int n = 1;
            while(n<verticesInPieceAbove.Length){
                int i = 0;
                while(i<verticesInActivePiece.Length){
                    if(verticesInPieceAbove[n] == verticesInActivePiece[i] && verticesInActivePiece[i].x > temp.x){
                        temp = verticesInActivePiece[i];
                    }
                    i++;
                }
                n++;
            }
            currentPoint = temp;
        }
    }   
    void findNextPoint(){
        if(currentRow == 0){                                //case: first row
            nextPoint = upperRightCorner;
        }else if(currentColumn != 0){                       //case: middle or end of a row
            GameObject nextPieceAbove = new GameObject();
            for(int i = 0; i < placedPieces.Count; i++){
                if(placedPieces[i].row == currentRow-1 && placedPieces[i].column == (currentColumn)){
                    nextPieceAbove = placedPieces[i].piece;
                    break;
                }
            }
            Vector3[] vertices = nextPieceAbove.GetComponent<MeshFilter>().mesh.vertices;
            int index = 0;
            foreach(Vector3 vertex in vertices){
                if(vertex == currentPoint){
                    break;
                }
                index++;
            }
            if(index < vertices.Length-1){
                nextPoint = vertices[index+1];
            }else{
                nextPoint = vertices[0];
            }
            
        }else if(currentColumn == 0){                       //case: start of new row
            GameObject pieceAbove = new GameObject();
            for(int i = 0; i < placedPieces.Count; i++){
                if(placedPieces[i].row == currentRow-1 && placedPieces[i].column == currentColumn){     //bug here! probably
                    pieceAbove = placedPieces[i].piece;
                    break;
                }
            }
            Vector3[] vertices = pieceAbove.GetComponent<MeshFilter>().mesh.vertices;
            Vector3 llc = vertices[0];
            int n = 0; int index = 0;
            while(n < vertices.Length){
                if(llc.x > vertices[n].x-0.01){
                    llc = vertices[n];
                    index = n;
                }
                if(llc.x >= vertices[n].x-0.01 && llc.y > vertices[n].y-0.01){
                    llc = vertices[n];
                    index = n;
                }
                n++;
            }
            currentPoint = llc;
            if(index < vertices.Length-1){
                nextPoint = vertices[index+1];
            }else{
                nextPoint = vertices[0];
            }
        }
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


public class Quadruple {
    public int row;
    public int column;
    public Vector3 currentPoint;
    public GameObject piece;
    public Quadruple(int row, int column, Vector3 currentPoint, GameObject piece){
        this.row = row;
        this.column = column;
        this.currentPoint = currentPoint;
        this.piece = piece;
    }
}
public class Triple {
    public int row;
    public int column;
    public GameObject piece;
    public Triple(int row, int column, GameObject piece){
        this.row = row;
        this.column = column;
        this.piece = piece;
    }
}

public class Pair {
    public int index;
    public Vector3 vertex;
    public Pair(int index, Vector3 vertex){
        this.index = index;
        this.vertex = vertex;
    }
}
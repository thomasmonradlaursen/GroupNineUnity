using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class AutoSolveCleverVersion : MonoBehaviour
{
    MiscellaneousMath mM = new MiscellaneousMath();
    JSONPuzzle puzzle; public List<GameObject> pieces;
    List<GameObject> potentialPieces = new List<GameObject>();
    List<Triple> placedPieces = new List<Triple>();
    List<Triple> testedPieces = new List<Triple>();
    float theta; int indexOfTheta;
    Vector3 upperLeftCorner; Vector3 upperRightCorner; Vector3 lowerRightCorner; Vector3 lowerLeftCorner;
    Vector3 currentPoint; Vector3 nextPoint; GameObject activePiece; int currentRow; int currentColumn;
    
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AutoSolve();
        }
    }
     void AutoSolve(){
        puzzle = GetComponentInParent<PuzzleModel>().puzzle;
        pieces = GetComponent<PuzzleModel>().pieces;
        FindCorners();
        /*
        Debug.Log("lower left corner: "+ lowerLeftCorner);
        Debug.Log("Upper left corner: "+ upperLeftCorner);
        Debug.Log("Upper right corner: "+ upperRightCorner);
        Debug.Log("lower right corner: "+ lowerRightCorner);
        */
        currentPoint = upperLeftCorner;
        nextPoint = upperRightCorner;
        currentRow = 0; currentColumn = 0; theta = 90.0f; indexOfTheta = 0;
        int numberOfPieces = pieces.Count;

        while(placedPieces.Count < numberOfPieces){     //not done with this loop (but basics ok)
            bool changedRows = CheckForRowChange();
            FindPotentialPieces();
            Debug.Log("Found "+potentialPieces.Count+" potential pieces.");
            SetActivePiece();
            PlacePiece();
        
            while(OverLapsBoard() == true && activePiece.GetComponent<PieceInfo>().thetaAngles.Count > 0){
                Pair temp = activePiece.GetComponent<PieceInfo>().thetaAngles[0];
                activePiece.GetComponent<PieceInfo>().thetaAngles.Remove(temp);
                indexOfTheta = activePiece.GetComponent<PieceInfo>().thetaAngles[0].index;
                PlacePiece();
            }
            if(OverLapsBoard() == false){
                testedPieces.Add(new Triple(currentRow, currentColumn, activePiece));
                placedPieces.Add(new Triple(currentRow, currentColumn, activePiece));
                pieces.Remove(activePiece);
                potentialPieces.Remove(activePiece);
                updateCurrentPoint();
                CalculateNextAngle(changedRows);
                currentColumn++;
            } else {
                testedPieces.Add(new Triple(currentRow, currentColumn, activePiece));
                potentialPieces.Remove(activePiece);
                if(potentialPieces.Count == 0){
                    Backtrack();
                } else {
                    //try next potential piece 
                }
            }
        }
    }
    bool CheckForRowChange(){
        if((double)currentPoint.x == (double)upperRightCorner.x){   //check for row change
                currentRow++;
                currentColumn++;
                //Debug.Log("currentRow: "+currentRow);
                findNextPoint();
                CalculateNextAngle(true);
                //Debug.Log("currentPoint updated to be: "+currentPoint);
                //Debug.Log("nextPoint updated to be: "+nextPoint);
                //Debug.Log("Theta updated to be: "+theta);
            return true;
        } else {
            return false;
        }
    }
    void SetActivePiece(){
        activePiece = potentialPieces[0];
        activePiece.GetComponent<PieceInfo>().thetaAngles = ThetaAnglesInPiece();
        Pair temp = activePiece.GetComponent<PieceInfo>().thetaAngles[0];
        indexOfTheta = activePiece.GetComponent<PieceInfo>().thetaAngles[0].index;
        activePiece.GetComponent<PieceInfo>().thetaAngles.Remove(temp);
    }
    void Backtrack(){   //we reach this point when potentialPieces is empty and nothing fits
        Debug.Log("backtracking!");
        if(currentRow > 0){
            if(currentColumn == 0){
                int temp = placedPieces.Count-1;
                GameObject pieceToRemove = placedPieces[temp].piece;
            }
        }
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
    float CalculateRotationAngle(Vector3 pointToBeAligned){
        float angle = Vector3.SignedAngle(pointToBeAligned-currentPoint, nextPoint-currentPoint, Vector3.up);
        //Debug.Log("*************************************************************************************");
        //Debug.Log("rotation angle: "+angle);
        if(pointToBeAligned.y > currentPoint.y){
            angle = angle*(-1);
        }
        return (Mathf.PI*angle)/180.0f;
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
    void FindPotentialPieces(){
        Debug.Log("Number of pieces: "+pieces.Count);
        potentialPieces = new List<GameObject>();       //reset potPieces 
        foreach(GameObject testPiece in pieces){
            float[] angles = testPiece.GetComponent<PieceInfo>().angles;
            foreach(float angle in angles){
                if(angle == theta){
                    potentialPieces.Add(testPiece);
                    break;
                }
            }
        }
        //remove previously tested pieces
        for(int i = 0; i < testedPieces.Count; i++){
            for(int j = 0; j < potentialPieces.Count; j++){
                if(potentialPieces[j] == testedPieces[i].piece){
                    potentialPieces.Remove(potentialPieces[j]);
                }
            }
        }
    }
    List<Pair> ThetaAnglesInPiece(){
        List<Pair> thetaAngleVertices = new List<Pair>();
        int n = 0;
        foreach(float angle in activePiece.GetComponent<PieceInfo>().angles){
            if(angle == theta){
                thetaAngleVertices.Add(new Pair(n, activePiece.GetComponent<MeshFilter>().mesh.vertices[n]));
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
        if(currentRow == 0){
            nextPoint = upperRightCorner;
        }else{
            GameObject pieceAbove = new GameObject();
            for(int i = 0; i < placedPieces.Count; i++){
                if(placedPieces[i].row == currentRow-1 && placedPieces[i].column == currentColumn){     //bug here! probably
                    pieceAbove = placedPieces[i].piece;
                    break;
                }
            }
            Vector3[] vertices = pieceAbove.GetComponent<MeshFilter>().mesh.vertices;
            Vector3 llc = vertices[0];
            int index = 0;
            int n = 0;
            while(n < vertices.Length){
                if((double) llc.x > (double) vertices[n].x){
                    llc = vertices[n];
                    index = n;
                }
                if((double) llc.x >= (double) vertices[n].x && (double) llc.y > (double) vertices[n].y){
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
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
    List<List<GameObject>> placedPieces;
    float theta = 90.0f; int indexOfTheta;
    float xAxisLength; float yAxisLength;
    Vector3 upperLeftCorner; Vector3 upperRightCorner;
    Vector3 lowerRightCorner; Vector3 lowerLeftCorner;
    Vector3 currentPoint; GameObject activePiece; int currentRow = 0;
    
    public void Start(){
        Debug.Log("AutoSolveAlgori");
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("AutoSolve - TestingImplementation()");
            AutoSolve();
        }
    }
    public void Calculate(){
        puzzle = GetComponentInParent<MeshFromJsonGenerator>().puzzle;
        pieces = GetComponentInParent<PieceController>().pieces;

        FindCorners();
        Debug.Log("lower left corner: "+ lowerLeftCorner);
        Debug.Log("Upper left corner: "+ upperLeftCorner);
        Debug.Log("Upper right corner: "+ upperRightCorner);
        Debug.Log("lower right corner: "+ lowerRightCorner);
        currentPoint = upperLeftCorner;
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
    void AutoSolve(){
        Calculate();
        FindPotentialPieces();
        UpdateIndexOfTheta(0);
        PlacePiece();
        /*
        if(OverLapsBoard() == false){
            placedPieces[currentRow].Add(activePiece);
            pieces.Remove(activePiece);
            Debug.Log("Piece fits in first try");
        }else{
            RotateToFit();
        }
        */

    }
    void RotateToFit(){
        if(NumberOfThetaAnglesInPiece().Count > 1){
            int n = 1;
            bool overlap = true;
            while(n < NumberOfThetaAnglesInPiece().Count && overlap == true){
                UpdateIndexOfTheta(n);
                PlacePiece();
                overlap = OverLapsBoard();
                n++;
            }
            if(OverLapsBoard() == false){
                placedPieces[currentRow].Add(activePiece);
                pieces.Remove(activePiece);
            }else{
                potentialPieces.Remove(activePiece);
            }
        }
    }
    void PlacePiece(){
        Vector3 displacement = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]-currentPoint;
        //Debug.Log("Vertex of theta: "+ potentialPieces[0].GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]);
        //Debug.Log("Current point: "+ currentPoint);
        AutoTranslate(displacement);
        
        Vector3 pointToBeAligned;
        if(indexOfTheta != 0){
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta-1];
        }else{
            pointToBeAligned = activePiece.GetComponent<MeshFilter>().mesh.vertices[activePiece.GetComponent<MeshFilter>().mesh.vertices.Length-1];
        }
        float rotationAngle = CalculateRotationAngle(pointToBeAligned);
        //Debug.Log("pointToBeAligned: " + pointToBeAligned);
        //Debug.Log("rotationAngle, radians: " + rotationAngle+ " rotationAngle, degrees: "+(rotationAngle*180)/Mathf.PI);
        /*
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
            //Debug.Log(translatedVertices[index]);
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
        float angle = Vector3.SignedAngle(pointToBeAligned-currentPoint, upperRightCorner-currentPoint, Vector3.up);
        //Debug.Log("*************************************************************************************");
        //Debug.Log("rotation angle: "+angle);
        return (-Mathf.PI*angle)/180.0f;
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
        Vector3 centroid = mM.CalculateCentroid(meshForActivePiece.vertices, area);
        //Debug.Log("centroid: "+ centroid);
        
        originalVertices = CentralizeVertices(currentPoint, originalVertices);
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationAngle) - originalVertices[index].y * Mathf.Sin(rotationAngle);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationAngle) + originalVertices[index].y * Mathf.Cos(rotationAngle);
        }
        //rotatedVertices = RestorePositionOfVertices(activePiece.GetComponent<PieceInfo>().centroid, rotatedVertices);
        rotatedVertices = RestorePositionOfVertices(currentPoint, rotatedVertices);
        Debug.Log("ouput mesh after rotation ************************************");
        foreach(Vector3 ver in rotatedVertices){
            Debug.Log(ver);
        }
        activePiece.GetComponent<MeshFilter>().mesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        activePiece.GetComponent<MeshCollider>().sharedMesh = activePiece.GetComponent<MeshFilter>().mesh;     
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

    void UpdateIndexOfTheta(int n){
        while(n < activePiece.GetComponent<PieceInfo>().angles.Length){
            if(activePiece.GetComponent<PieceInfo>().angles[n] == theta){
                indexOfTheta = n;
                break;
            }
            n++;
        }
    }
    void CalculateNextAngle(){

    }
    void FindPotentialPieces(){
        //Debug.Log("Number of pieces: "+pieces.Count);
        //Debug.Log("Theta: "+theta);
        foreach(GameObject piece in pieces){
            float[] angles = piece.GetComponent<PieceInfo>().angles;
            foreach(float angle in angles){
                //Debug.Log("Angle: "+angle);
                if((int) angle == (int) theta){
                    potentialPieces.Add(piece);
                    break;
                }
            }
        }
        activePiece = potentialPieces[0];
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
        
        Debug.Log("smallestX : "+smallestX);
        Debug.Log("biggestX : "+biggestX);
        Debug.Log("smallestY : "+smallestY);
        Debug.Log("biggestY : "+biggestY);
        
        if(smallestX < lowerLeftCorner.x || biggestX > lowerRightCorner.x 
           || smallestY < lowerLeftCorner.y || biggestY > upperRightCorner.y ){
               overlapExists = true;
           }
        Debug.Log("OVERLAP : "+overlapExists);
        return overlapExists;
    }
}

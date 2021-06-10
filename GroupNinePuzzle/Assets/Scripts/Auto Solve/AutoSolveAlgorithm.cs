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
    Vector3 currentPoint; GameObject selectedPiece;
    
    void Start(){
        
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("AutoSolve - TestingImplementation()");
            Calculate();
            PlacePiece();
        }
    }
    public void Calculate(){
        puzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;
        pieces = GetComponentInParent<PieceController>().pieces;

        FindCorners();
        Debug.Log("lower left corner: "+ lowerLeftCorner);
        Debug.Log("Upper left corner: "+ upperLeftCorner);
        Debug.Log("Upper right corner: "+ upperRightCorner);
        Debug.Log("lower right corner: "+ lowerRightCorner);
        xAxisLength = Vector3.Distance(upperLeftCorner, upperRightCorner);
        yAxisLength = Vector3.Distance(lowerLeftCorner, upperLeftCorner);
        currentPoint = upperLeftCorner;
        FindPotentialPieces();
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
            if(corners[n].x == lowerLeftCorner.x && corners[n].y < lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(lowerLeftCorner);

        upperLeftCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x < lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }
            if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperLeftCorner);

        upperRightCorner = corners[0];
        n = 1;
        while(n<corners.Count){
            if(corners[n].x > lowerLeftCorner.x){
                lowerLeftCorner = corners[n];
            }
            if(corners[n].x == lowerLeftCorner.x && corners[n].y > lowerLeftCorner.y){
                lowerLeftCorner = corners[n];
            }
            n++;
        }
        corners.Remove(upperRightCorner);

        lowerRightCorner = corners[0];
    }
    void PlacePiece(){
        UpdateIndexOfTheta();
        Vector3 displacement = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]-currentPoint;
        //Debug.Log("Vertex of theta: "+ potentialPieces[0].GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]);
        //Debug.Log("Current point: "+ currentPoint);
        AutoTranslate(displacement);
        
        Vector3 pointToBeAligned;
        if(indexOfTheta != 0){
            pointToBeAligned = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta-1];
        }else{
            pointToBeAligned = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[selectedPiece.GetComponent<MeshFilter>().mesh.vertices.Length-1];
        }
        float rotationAngle = CalculateRotationAngle(pointToBeAligned);
        foreach(Vector3 ver in selectedPiece.GetComponent<MeshFilter>().mesh.vertices){
            Debug.Log(ver);
        }
        AutoRotate(rotationAngle);
        
    }
    void AutoTranslate(Vector3 displacement){
        Mesh meshForSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = selectedPiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];
        for(int index = 0; index < meshForSelectedPiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForSelectedPiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForSelectedPiece.vertices[index].y - displacement.y;
            //Debug.Log(translatedVertices[index]);
        }
        selectedPiece.GetComponent<MeshFilter>().mesh.SetVertices(translatedVertices);
        meshForSelectedPiece.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        
        float area = mM.CalculateAreaFromMesh(meshForSelectedPiece);
        //Debug.Log("area: " + area);
        selectedPiece.GetComponent<PieceInfo>().centroid = mM.CalculateCentroid(translatedVertices, area);
        //Debug.Log("centroid: "+ selectedPiece.GetComponent<PieceInfo>().centroid);
    }
    float CalculateRotationAngle(Vector3 pointToBeAligned){
        float angle = Vector3.SignedAngle(currentPoint-pointToBeAligned, currentPoint-upperRightCorner, Vector3.up);
        //Debug.Log("*************************************************************************************");
        //Debug.Log("rotation angle: "+angle);
        return angle;
    }
    void AutoRotate(float rotationAngle){
        Mesh meshForSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh;
        foreach(Vector3 ver in meshForSelectedPiece.vertices){
            Debug.Log(ver);
        }
        LineRenderer lineRenderer = selectedPiece.GetComponent<LineRenderer>();
        
        Vector3[] rotatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];
        Vector3[] originalVertices = meshForSelectedPiece.vertices;
        
        float area = mM.CalculateAreaFromMesh(meshForSelectedPiece);
        Vector3 centroid = mM.CalculateCentroid(meshForSelectedPiece.vertices, area);
        Debug.Log("centroid: "+ centroid);
        
        originalVertices = CentralizeVertices(centroid, originalVertices);
        for (int index = 0; index < originalVertices.Length; index++)
        {
            rotatedVertices[index].x = originalVertices[index].x * Mathf.Cos(rotationAngle) - originalVertices[index].y * Mathf.Sin(rotationAngle);
            rotatedVertices[index].y = originalVertices[index].x * Mathf.Sin(rotationAngle) + originalVertices[index].y * Mathf.Cos(rotationAngle);
        }
        rotatedVertices = RestorePositionOfVertices(selectedPiece.GetComponent<PieceInfo>().centroid, rotatedVertices);
        meshForSelectedPiece.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        selectedPiece.GetComponent<MeshCollider>().sharedMesh = meshForSelectedPiece;
        
    }

     Vector3[] CentralizeVertices(Vector3 centroid, Vector3[] originalVertices)
    {
        for (int index = 0; index < originalVertices.Length; index++)
        {
            // we add 100000 because we want all coordinates to be positive.
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
            // we add 100000 because we want all coordinates to be positive.
            rotatedVertices[index].x = rotatedVertices[index].x + centroid.x;
            rotatedVertices[index].y = rotatedVertices[index].y + centroid.y;
        }
        return rotatedVertices;
    }

    void UpdateIndexOfTheta(){
        GameObject piece = potentialPieces[0];
        int n = 0;
        while(n < piece.GetComponent<PieceInfo>().angles.Length){
            if(piece.GetComponent<PieceInfo>().angles[n] == theta){
                indexOfTheta = n;
                break;
            }
            n++;
        }
    }
    void CalculateNextAngle(){

    }
    void FindPotentialPieces(){
        Debug.Log("Number of pieces: "+pieces.Count);
        Debug.Log("Theta: "+theta);
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
        selectedPiece = potentialPieces[0];
    }
    bool IsPlacedCorrectly(){
        bool placedCorrectly = true;

        return placedCorrectly;
    }


}

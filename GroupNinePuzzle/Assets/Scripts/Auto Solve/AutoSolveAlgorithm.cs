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
        selectedPiece = potentialPieces[0];
        Vector3 displacement = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]-currentPoint;
        //Debug.Log("Vertex of theta: "+ potentialPieces[0].GetComponent<MeshFilter>().mesh.vertices[indexOfTheta]);
        //Debug.Log("Current point: "+ currentPoint);
        
        Mesh meshForSelectedPiece = potentialPieces[0].GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = potentialPieces[0].GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];
        for(int index = 0; index < meshForSelectedPiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForSelectedPiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForSelectedPiece.vertices[index].y - displacement.y;
        }
        meshForSelectedPiece.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        //GetComponent<MeshCollider>().sharedMesh = meshForSelectedPiece;
        
        float area = mM.CalculateAreaFromMesh(GetComponent<MeshFilter>().mesh);
        selectedPiece.GetComponent<PieceInfo>().centroid = mM.CalculateCentroid(GetComponent<MeshFilter>().mesh.vertices, area);
        selectedPiece.GetComponent<Rotation>().originalVertices = translatedVertices;

        Vector3 pointToBeAligned;
        if(indexOfTheta != 0){
            pointToBeAligned = meshForSelectedPiece.vertices[indexOfTheta-1];
        }else{
            pointToBeAligned = meshForSelectedPiece.vertices[meshForSelectedPiece.vertices.Length-1];
        }
        float rotationAngle = CalculateRotationAngle(pointToBeAligned);

        selectedPiece.GetComponent<Rotation>().AutoRotate((rotationAngle*Mathf.PI) / 180, potentialPieces[0]);
    }
    float CalculateRotationAngle(Vector3 pointToBeAligned){
        float angle = Vector3.SignedAngle(currentPoint-pointToBeAligned, currentPoint-upperRightCorner, Vector3.up);
        //Debug.Log("*************************************************************************************");
        //Debug.Log("rotation angle: "+angle);
        return angle;
    }
    void RotatePiece(){

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
                Debug.Log("Angle: "+angle);
                if((int) angle == (int) theta){
                    potentialPieces.Add(piece);
                    break;
                }
            }
        }
    }
    bool IsPlacedCorrectly(){
        bool placedCorrectly = true;

        return placedCorrectly;
    }


}

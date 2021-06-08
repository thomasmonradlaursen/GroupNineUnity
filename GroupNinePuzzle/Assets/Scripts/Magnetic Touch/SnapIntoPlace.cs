using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapIntoPlace : MonoBehaviour
{
    private Vector3 mouseOffset;
    private MiscellaneousMath mM;
    private PieceInfo closestPiece;
    void OnMouseUp()
    {
        closestPiece = AutoTranslate();
        Debug.Log("Closest piece: " + closestPiece.name);
    }
    
    void FixedUpdate(){
        if (Input.GetKeyDown(KeyCode.S)){
            CalculateVerticesAfterTranslation(closestPiece.centroid);
            GetComponent<PieceInfo>().centroid = closestPiece.GetComponent<PieceInfo>().centroid;
        }
    }

    PieceInfo AutoTranslate(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3 center = GetComponent<PieceInfo>().centroid;

        PieceInfo[] pieces = FindObjectsOfType<PieceInfo>();  //locate all pieces
        
        closestPiece = pieces[0];

        foreach(PieceInfo piece in pieces){
            Vector3 closestPoint;
            if(piece.GetComponent<MeshFilter>().mesh == mesh){continue;}
            Debug.Log("Piece #"+piece.GetComponent<PieceInfo>().name+" center: "+piece.GetComponent<PieceInfo>().centroid);
            Vector3 tempCenter = piece.centroid;

            float testDist = Vector3.Distance(center, tempCenter);
            float currDist = Vector3.Distance(center, closestPiece.GetComponent<PieceInfo>().centroid);
            
            if(testDist < currDist){
                closestPoint = tempCenter;
                closestPiece = piece;
            }
        }
        Debug.Log("Center: "+ center);
        Debug.Log("Closest point: " + closestPiece.GetComponent<PieceInfo>().centroid);
        return closestPiece;
        
    }

    void CalculateVerticesAfterTranslation(Vector3 closestPoint)
    {
        //transform.position = Vector3.zero;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + closestPoint[0];
            translatedVertices[index].y = mesh.vertices[index].y + closestPoint[1];
            Debug.Log("moving x: " +mesh.vertices[index].x + " + " +closestPoint[0] + " = "+ translatedVertices[index].x);
            Debug.Log("moving y: " +mesh.vertices[index].y + " + " +closestPoint[1] + " = "+ translatedVertices[index].y);
        }
        mesh.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void CalculateCentroidAfterTranslation()
    {
        PieceInfo piece = GetComponentInParent<PieceInfo>();
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        Vector3 transCentroid = mM.CalculateCentroid(mesh.vertices, piece.GetComponentInParent<PieceInfo>().area);
        GetComponent<PieceInfo>().centroid = transCentroid;
    }

}

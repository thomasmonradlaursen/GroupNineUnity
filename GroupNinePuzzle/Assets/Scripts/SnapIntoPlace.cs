using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapIntoPlace : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 mouseOffset;
    private MiscellaneousMath mM;
    private PieceInfo closestPiece;
    void OnMouseUp()
    {
        closestPiece = AutoTranslate();
        Debug.Log("Closest piece: " + closestPiece.name);
        //CalculateVerticesAfterTranslation(closestPoint);
        //CalculateCentroidAfterTranslation();
    }
    
    void FixedUpdate(){
        if (Input.GetKey(KeyCode.RightArrow)){
            CalculateVerticesAfterTranslation(closestPiece.centroid);
            GetComponent<PieceInfo>().centroid = closestPiece.GetComponent<PieceInfo>().centroid;
        }
    }

    PieceInfo AutoTranslate(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3 center = GetComponent<PieceInfo>().centroid;

        PieceInfo[] pieces = FindObjectsOfType<PieceInfo>();  //locate all pieces
        
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, 0);
        closestPiece = GetComponent<PieceInfo>();

        foreach(PieceInfo piece in pieces){
            if(piece.GetComponent<MeshFilter>().mesh == mesh){continue;}
            Debug.Log("Piece #"+piece.GetComponent<PieceInfo>().name+" center: "+piece.GetComponent<PieceInfo>().centroid);
            Vector3 tempCenter = piece.centroid;

            float testDist = Vector3.Distance(center, tempCenter);
            float currDist = Vector3.Distance(center, closestPoint);
            
            if(testDist < currDist){
                closestPoint = tempCenter;
                closestPiece = piece;
            }
        }
        Debug.Log("Center: "+ center);
        Debug.Log("Closest point: "+closestPoint);
        return closestPiece;
        
    }

    void CalculateVerticesAfterTranslation(Vector3 closestPoint)
    {
        //transform.position = Vector3.zero;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + closestPoint[0];
            translatedVertices[index].y = mesh.vertices[index].y + closestPoint[1];
            Debug.Log("moving x: " +mesh.vertices[index].x + " + " +closestPoint[0] + " = "+ translatedVertices[index].x);
            Debug.Log("moving y: " +mesh.vertices[index].y + " + " +closestPoint[1] + " = "+ translatedVertices[index].y);
        }
        mesh.SetVertices(translatedVertices);
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

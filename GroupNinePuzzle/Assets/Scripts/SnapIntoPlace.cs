using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapIntoPlace : MonoBehaviour
{
    // Start is called before the first frame update
    void OnMouseUp()
    {
        AutoTranslate();
    }

    Vector3 FindCenterOfMassInPiece(Mesh piece){
        float sumX = 0;
        float sumY = 0;

        foreach(var vertex in piece.vertices){
            sumX += vertex.x;
            sumY += vertex.y;
        }

        var centerOfMass = new Vector3();
        centerOfMass.x = sumX/piece.vertices.Length;
        centerOfMass.y = sumY/piece.vertices.Length;

        return centerOfMass;
    }

    float FindAreaOfPossiblePieces(Mesh piece, Vector3 centerOfMass){
        float greatestDistance = 0;

        foreach(var vertex in piece.vertices){
            var distanceFromCenterToVertex = Mathf.Sqrt(Mathf.Pow(vertex.x - centerOfMass.x, 2) + Mathf.Pow(vertex.y - centerOfMass.y, 2));
            if(distanceFromCenterToVertex > greatestDistance){
                greatestDistance = distanceFromCenterToVertex;
            }
        }

        return greatestDistance + 1;
    }

    void AutoTranslate(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3 center = GetComponent<PieceInfo>().centroid;
        float max = FindAreaOfPossiblePieces(mesh, center);

        GameObject[] pieces = FindObjectsOfType<GameObject>();  //locate all pieces
        
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, 0);
        foreach(GameObject piece in pieces){
            if(piece.GetComponent<MeshFilter>().mesh == mesh){continue;}
            Debug.Log("found piece: "+ piece.GetComponent<PieceInfo>().name);
            Vector3 tempCenter = piece.GetComponent<PieceInfo>().centroid;
            float xt = center[0]-tempCenter[0];
            float yt = center[1]-tempCenter[1];
            float testDist = Mathf.Sqrt(xt*xt+yt*yt);

            float xc = center[0] - closestPoint[0];
            float yc = center[1] - closestPoint[1];
            float currDist = Mathf.Sqrt(xc*xc+yc*yc);
            if(testDist < currDist){
                closestPoint = tempCenter;
                Debug.Log("new closest point: "+ closestPoint);
            }
        }
        //move piece, doesnt work... 
        CalculateVerticesAfterTranslation(closestPoint);
        CalculateCentroidAfterTranslation(closestPoint);
        gameObject.transform.position = closestPoint;
    }

    void CalculateVerticesAfterTranslation(Vector3 closestPoint)
    {
        transform.position = Vector3.zero;
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
        Vector3[] translatedVertices = new Vector3[mesh.vertices.Length];
        for(int index = 0; index < mesh.vertices.Length; index++)
        {
            translatedVertices[index].x = mesh.vertices[index].x + closestPoint[0];
            translatedVertices[index].y = mesh.vertices[index].y + closestPoint[1];
        }
        mesh.SetVertices(translatedVertices);
        GetComponentInParent<MeshCollider>().sharedMesh = mesh;
    }
    void CalculateCentroidAfterTranslation(Vector3 closestPoint)
    {
        Vector3 translatedCentroid = GetComponent<PieceInfo>().centroid;
        translatedCentroid.x += (closestPoint[0]);
        translatedCentroid.y += (closestPoint[1]);
        GetComponent<PieceInfo>().centroid = translatedCentroid;
    }

}

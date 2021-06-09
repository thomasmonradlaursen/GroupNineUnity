using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosestVertex : MonoBehaviour
{
    public List<(GameObject, Vector3, int, float)> FindClosestVertexToCentroidOfSelectedPiece((GameObject, List<GameObject>) possibleSnaps, Vector3 baseVertex)
    {
        List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece = new List<(GameObject, Vector3, int, float)>();
        foreach (GameObject candidateForSnap in possibleSnaps.Item2)
        {
            Vector3[] verticesOfCandicate = candidateForSnap.GetComponent<MeshFilter>().mesh.vertices;
            float distanceFromClosestVertexToSelectedPiece = Vector3.Distance(verticesOfCandicate[0], baseVertex);
            Vector3 clostestVertexForCurrentPiece = verticesOfCandicate[0];
            int indexForVertex = -1;
            for (int index = 0; index < verticesOfCandicate.Length; index++)
            {
                float distanceFromCurrentVertex = Vector3.Distance(verticesOfCandicate[index], baseVertex);
                if (distanceFromCurrentVertex <= distanceFromClosestVertexToSelectedPiece)
                {
                    clostestVertexForCurrentPiece = verticesOfCandicate[index];
                    distanceFromClosestVertexToSelectedPiece = distanceFromCurrentVertex;
                    indexForVertex = index;
                }
            }
            closestVerticesToSelectedPiece.Add((candidateForSnap, clostestVertexForCurrentPiece, indexForVertex, distanceFromClosestVertexToSelectedPiece));
        }
        return closestVerticesToSelectedPiece;
    }
    public int FindClosestVertexInSelectedPiece(GameObject selectedPiece, Vector3 closestVertexInOtherPiece)
    {
        Vector3[] verticesOfSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
        float smallestDistance = Vector3.Distance(closestVertexInOtherPiece, verticesOfSelectedPiece[0]);
        int indexForClosestVertex = -1;
        for(int index = 0; index < verticesOfSelectedPiece.Length; index++)
        {
            float currentDistance = Vector3.Distance(closestVertexInOtherPiece, verticesOfSelectedPiece[index]);
            if(smallestDistance <= currentDistance)
            {
                indexForClosestVertex = index;
                smallestDistance = currentDistance;
            }
        }
        return indexForClosestVertex;
    }
    public Vector3 CalculateDisplacementForSnap(GameObject selectedPiece, GameObject magnetPiece, int indexForVertexOfSelected, int indexForVertexOfMagnet)
    {
        Vector3 selectedVertex = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfSelected];
        Vector3 magnetVertex = magnetPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfMagnet];
        Debug.Log(selectedVertex);
        Debug.Log(magnetVertex);
        return selectedVertex-magnetVertex;
    }
    public void CalculateVerticesAfterSnapTranslation(GameObject selectedPiece, Vector3 displacement)
    {
        Mesh meshForSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = selectedPiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];
        for(int index = 0; index < meshForSelectedPiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForSelectedPiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForSelectedPiece.vertices[index].y - displacement.y;
        }
        meshForSelectedPiece.SetVertices(translatedVertices);
        lineRenderer.SetPositions(translatedVertices);
        GetComponentInParent<MeshCollider>().sharedMesh = meshForSelectedPiece;
    }
    // In case a piece can snap to more than one piece at once, select the closest and snap to it.
    public (GameObject, int, float) SelectPieceToSnapTo(List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece)
    {
        GameObject pieceWithClosestVertex = closestVerticesToSelectedPiece[0].Item1;
        int indexForVertex = closestVerticesToSelectedPiece[0].Item3;
        float distanceForClosestVertex = closestVerticesToSelectedPiece[0].Item4;
        foreach ((GameObject, Vector3, int, float) currentVertex in closestVerticesToSelectedPiece)
        {
            if (currentVertex.Item4 < distanceForClosestVertex)
            {
                pieceWithClosestVertex = currentVertex.Item1;
                indexForVertex = currentVertex.Item3;
                distanceForClosestVertex = currentVertex.Item4;
            }
        }
        Debug.LogFormat("Closest : {0}. Index for vertex: {1}. Distance: {2}.", pieceWithClosestVertex.name, indexForVertex, distanceForClosestVertex);
        return (pieceWithClosestVertex, indexForVertex, distanceForClosestVertex);
    }
    public void LogVertexAndDistance(List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece)
    {
        foreach ((GameObject, Vector3, int, float) closestVertex in closestVerticesToSelectedPiece)
        {
            Debug.LogFormat("For : {0}. Vertex at index: {1} Value of vertex: {2}.", closestVertex.Item1.name, closestVertex.Item3, closestVertex.Item2);
        }
    }
    int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }
}

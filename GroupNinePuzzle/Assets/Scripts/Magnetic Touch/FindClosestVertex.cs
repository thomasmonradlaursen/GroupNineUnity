using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosestVertex : MonoBehaviour
{
    // public List<(GameObject, Vector3, int, float)> FindClosestVertexToCentroidOfSelectedPiece((GameObject, List<GameObject>) possibleSnaps, Vector3 baseVertex)
    // {
    //     List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece = new List<(GameObject, Vector3, int, float)>();
    //     foreach (GameObject candidateForSnap in possibleSnaps.Item2)
    //     {
    //         Vector3[] verticesOfCandicate = candidateForSnap.GetComponent<MeshFilter>().mesh.vertices;
    //         float distanceFromClosestVertexToSelectedPiece = Vector3.Distance(verticesOfCandicate[0], baseVertex);
    //         Vector3 clostestVertexForCurrentPiece = verticesOfCandicate[0];
    //         int indexForVertex = -1;
    //         for (int index = 0; index < verticesOfCandicate.Length; index++)
    //         {
    //             float distanceFromCurrentVertex = Vector3.Distance(verticesOfCandicate[index], baseVertex);
    //             if (distanceFromCurrentVertex <= distanceFromClosestVertexToSelectedPiece)
    //             {
    //                 clostestVertexForCurrentPiece = verticesOfCandicate[index];
    //                 distanceFromClosestVertexToSelectedPiece = distanceFromCurrentVertex;
    //                 indexForVertex = index;
    //             }
    //         }
    //         closestVerticesToSelectedPiece.Add((candidateForSnap, clostestVertexForCurrentPiece, indexForVertex, distanceFromClosestVertexToSelectedPiece));
    //     }
    //     return closestVerticesToSelectedPiece;
    // }
    // public int FindClosestVertexInSelectedPiece(GameObject selectedPiece, Vector3 closestVertexInOtherPiece)
    // {
    //     Vector3[] verticesOfSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
    //     float smallestDistance = Vector3.Distance(closestVertexInOtherPiece, verticesOfSelectedPiece[0]);
    //     int indexForClosestVertex = -1;
    //     for (int index = 0; index < verticesOfSelectedPiece.Length; index++)
    //     {
    //         float currentDistance = Vector3.Distance(closestVertexInOtherPiece, verticesOfSelectedPiece[index]);
    //         if (smallestDistance <= currentDistance)
    //         {
    //             indexForClosestVertex = index;
    //             smallestDistance = currentDistance;
    //         }
    //     }
    //     return indexForClosestVertex;
    // }

    // public int FindIndexOfVertexInPiece(GameObject selectedPiece, Vector3 vertex)
    // {
    //     Vector3[] verticesOfSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
    //     int indexForVertex = -1;
    //     for (int index = 0; index < verticesOfSelectedPiece.Length; index++)
    //     {
    //         if (verticesOfSelectedPiece[index] == vertex)
    //         {
    //             indexForVertex = index;
    //         }
    //     }
    //     return indexForVertex;
    // }

    public Vector3 CalculateDisplacementForSnap(GameObject selectedPiece, GameObject magnetPiece, int indexForVertexOfSelected, int indexForVertexOfMagnet)
    {
        Vector3 selectedVertex = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfSelected];
        Vector3 magnetVertex = magnetPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfMagnet];
        // Debug.Log(selectedVertex);
        // Debug.Log(magnetVertex);
        return selectedVertex - magnetVertex;
    }

    public float CalculateRotationForSnap(Vector3 primaryVertexInSelected, Vector3 primaryVertexInPieceToSnapTo, Vector3 previousVertexInSelected, Vector3 previousVertexInPieceToSnapTo)
    {
        Vector3 lineToVertexInSelected = previousVertexInSelected - primaryVertexInSelected;
        Vector3 lineToVertexInPieceToSnapTo = previousVertexInPieceToSnapTo - primaryVertexInPieceToSnapTo;

        Vector3 unitVectorSelected = lineToVertexInSelected / Vector3.Magnitude(lineToVertexInSelected);
        Vector3 unitVectorInPieceToSnapTo = lineToVertexInPieceToSnapTo / Vector3.Magnitude(lineToVertexInPieceToSnapTo);

        // Debug.Log("lines and rotation:");
        // Debug.Log(lineToVertexInSelected);
        // Debug.Log(lineToVertexInPieceToSnapTo);

        var radiansToRotate = 0f;
        if (unitVectorSelected != unitVectorInPieceToSnapTo)
        {
            var radiansSelected = Mathf.Atan2(unitVectorSelected.y, unitVectorSelected.x);
            var radiansPieceToSnapTo = Mathf.Atan2(unitVectorInPieceToSnapTo.y, unitVectorInPieceToSnapTo.x);
            radiansToRotate = radiansPieceToSnapTo - radiansSelected;
            // Debug.Log("radiansSelected " + radiansSelected * Mathf.Rad2Deg);
            // Debug.Log("radiansPieceToSnapTo " + radiansPieceToSnapTo * Mathf.Rad2Deg);
            Debug.Log("radiansToRotate " + radiansToRotate * Mathf.Rad2Deg);
        }

        return radiansToRotate;
    }
    public void CalculateVerticesAfterSnapTranslationAndRotation(GameObject selectedPiece, Vector3 displacement, float rotationAngle, int indexOfVertexToRotateAbout)
    {
        Mesh meshForSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = selectedPiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];

        for (int index = 0; index < meshForSelectedPiece.vertices.Length; index++)
        {
            translatedVertices[index].x = meshForSelectedPiece.vertices[index].x - displacement.x;
            translatedVertices[index].y = meshForSelectedPiece.vertices[index].y - displacement.y;
            Debug.Log("index: " + index + "   x: " + translatedVertices[index].x + "   y:" + translatedVertices[index].y);
        }

        Vector3[] rotatedVertices = new Vector3[meshForSelectedPiece.vertices.Length];
        Vector3 vertexToRotateAbout = translatedVertices[indexOfVertexToRotateAbout];

        translatedVertices = CentralizeVertices(vertexToRotateAbout, translatedVertices);

        float rotationTheta = rotationAngle;

        for (int index = 0; index < translatedVertices.Length; index++)
        {
            rotatedVertices[index].x = translatedVertices[index].x * Mathf.Cos(rotationTheta) - translatedVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = translatedVertices[index].x * Mathf.Sin(rotationTheta) + translatedVertices[index].y * Mathf.Cos(rotationTheta);
        }

        rotatedVertices = RestorePositionOfVertices(vertexToRotateAbout, rotatedVertices);

        meshForSelectedPiece.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        selectedPiece.GetComponent<MeshCollider>().sharedMesh = meshForSelectedPiece;
    }

    // Todo: already exists in Rotation.cs (not exactly like this though)
    Vector3[] CentralizeVertices(Vector3 centroid, Vector3[] originalVertices)
    {
        for (int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x = originalVertices[index].x - centroid.x;
            originalVertices[index].y = originalVertices[index].y - centroid.y;
        }
        return originalVertices;
    }

    // Todo: already exists in Rotation.cs (not exactly like this though)
    Vector3[] RestorePositionOfVertices(Vector3 centroid, Vector3[] rotatedVertices)
    {
        for (int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x = rotatedVertices[index].x + centroid.x;
            rotatedVertices[index].y = rotatedVertices[index].y + centroid.y;
        }
        return rotatedVertices;
    }

    public void LogVertexAndDistance(List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece)
    {
        foreach ((GameObject, Vector3, int, float) closestVertex in closestVerticesToSelectedPiece)
        {
            Debug.LogFormat("For : {0}. Vertex at index: {1} Value of vertex: {2}.", closestVertex.Item1.name, closestVertex.Item3, closestVertex.Item2);
        }
    }
    public int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }

    public SnapInformation FindClosestVertexToSelectedPiece((GameObject, List<GameObject>) possibleSnaps)
    {
        List<(GameObject, Vector3, int, float)> closestVerticesToSelectedPiece = new List<(GameObject, Vector3, int, float)>();
        Vector3[] verticesOfSelected = possibleSnaps.Item1.GetComponent<MeshFilter>().mesh.vertices;
        (float, float) smallestDistance = (100000f, 100000f);
        Vector3 primaryVertexInSelectedPiece = new Vector3();
        Vector3 primaryVertexInPieceToSnapTo = new Vector3();
        Vector3 previousVertexInSelectedPiece = new Vector3();
        Vector3 previousVertexInPieceToSnapTo = new Vector3();

        int indexOfPrimaryVertexInSelectedPiece = 0;
        int indexOfPrimaryVertexInPieceToSnapTo = 0;
        int indexOfPreviousVertexInSelectedPiece = 0;
        int indexOfPreviousVertexInPieceToSnapTo = 0;

        GameObject pieceWithShortestDistance = null;
        foreach (GameObject candidateForSnap in possibleSnaps.Item2)
        {
            Vector3[] verticesOfCandicate = candidateForSnap.GetComponent<MeshFilter>().mesh.vertices;
            
            var result = CheckDistancesBetweenVerticesOfTwoPieces(verticesOfSelected, verticesOfCandicate);


            // Todo: Not correct. smallest distance might be item2, if the distance between previous vertice and other piece is smaller than distance between next vertex and other piece.
            if (result.Item1.Item1 < smallestDistance.Item1)
            {
                smallestDistance = result.Item1;
                primaryVertexInPieceToSnapTo = result.Item2.Item1.Item2;
                primaryVertexInSelectedPiece = result.Item2.Item1.Item1;
                previousVertexInPieceToSnapTo = result.Item2.Item2.Item2;
                previousVertexInSelectedPiece = result.Item2.Item2.Item1;
                indexOfPrimaryVertexInSelectedPiece = result.Item3.Item1.Item1;
                indexOfPrimaryVertexInPieceToSnapTo = result.Item3.Item1.Item2;
                indexOfPreviousVertexInSelectedPiece = result.Item3.Item2.Item1;
                indexOfPreviousVertexInPieceToSnapTo = result.Item3.Item2.Item2;
                pieceWithShortestDistance = candidateForSnap;
            }
        }

        // Debug.Log("DistanceBetweenPrimaryVertices: " + smallestDistance.Item2);
        // Debug.Log("DistanceBetweenPreviousVertices: " + smallestDistance.Item1);
        // Debug.Log("PrimaryVertexInSelectedPiece: " + primaryVertexInSelectedPiece);
        // Debug.Log("PrimaryVertexInPieceToSnapTo: " + primaryVertexInPieceToSnapTo);
        // Debug.Log("PreviousVertexInSelectedPiece: " + previousVertexInSelectedPiece);
        // Debug.Log("PreviousVertexInPieceToSnapTo: " + previousVertexInPieceToSnapTo);

        var snapInformation = new SnapInformation()
        {
            PieceToSnapTo = pieceWithShortestDistance,
            DistanceBetweenPreviousVertices = smallestDistance.Item1,
            DistanceBetweenPrimaryVertices = smallestDistance.Item2,
            PrimaryVertexInPieceToSnapTo = primaryVertexInPieceToSnapTo,
            PrimaryVertexInSelectedPiece = primaryVertexInSelectedPiece,
            PreviousVertexInPieceToSnapTo = previousVertexInPieceToSnapTo,
            PreviousVertexInSelectedPiece = previousVertexInSelectedPiece,
            IndexOfPrimaryVertexInSelectedPiece = indexOfPrimaryVertexInSelectedPiece,
            IndexOfPrimaryVertexInPieceToSnapTo = indexOfPrimaryVertexInPieceToSnapTo,
            IndexOfPreviousVertexInSelectedPiece = indexOfPreviousVertexInSelectedPiece,
            IndexOfPreviousVertexInPieceToSnapTo = indexOfPreviousVertexInPieceToSnapTo,
        };

        return snapInformation;
    }


    public ((float, float), ((Vector3, Vector3), (Vector3, Vector3)), ((int, int), (int, int))) CheckDistancesBetweenVerticesOfTwoPieces(Vector3[] piece1, Vector3[] piece2)
    {

        // item1 is distance from vertice i in piece1 to vertice k in piece2.
        // item2 is distance from vertice i+1 in piece1 to vertice k+1 in piece2.
        (float, float) smallestDistance = (100000f, 100000f);

        // Previous vertix in piece1 paired with previous vertex in piece2
        // This is then paired with the current vertex in piece1 paired with the current vertex in piece2
        ((Vector3, Vector3), (Vector3, Vector3)) verticesInShortestDistance = ((new Vector3(), new Vector3()), (new Vector3(), new Vector3()));

        ((int, int), (int, int)) indexesOfVerticesInShortestDistance = ((0, 0), (0, 0));

        for (int i = 0; i < piece1.Length; i++)
        {
            var vertex_piece1 = piece1[i];
            var previousVertex_piece1 = piece1[GetWrappingIndex(i - 1, piece1.Length)];
            var nextVertex_piece1 = piece1[GetWrappingIndex(i + 1, piece1.Length)];
            for (int k = 0; k < piece2.Length; k++)
            {
                var vertex_piece2 = piece2[k];
                var previousVertex_piece2 = piece2[GetWrappingIndex(k + 1, piece2.Length)]; // + because we need to go the other way around compared to piece1.
                var nextVertex_piece2 = piece2[GetWrappingIndex(k - 1, piece2.Length)]; // - because the same thing.

                var distBetweenVertices = Vector3.Distance(vertex_piece1, vertex_piece2);
                if (distBetweenVertices < smallestDistance.Item1)
                {
                    var distPreviousVertices = Vector3.Distance(previousVertex_piece1, previousVertex_piece2);
                    var distNextVertices = Vector3.Distance(nextVertex_piece1, nextVertex_piece2);

                    if (distPreviousVertices <= distNextVertices)
                    {
                        smallestDistance = (distBetweenVertices, distPreviousVertices);
                        verticesInShortestDistance = ((vertex_piece1, vertex_piece2), (previousVertex_piece1, previousVertex_piece2));
                        indexesOfVerticesInShortestDistance = ((i, k), (GetWrappingIndex(i - 1, piece1.Length), GetWrappingIndex(k + 1, piece2.Length)));
                    }
                    else
                    {
                        smallestDistance = (distBetweenVertices, distNextVertices);
                        verticesInShortestDistance = ((vertex_piece1, vertex_piece2), (nextVertex_piece1, nextVertex_piece2));
                        indexesOfVerticesInShortestDistance = ((i, k), (GetWrappingIndex(i + 1, piece1.Length), GetWrappingIndex(k - 1, piece2.Length)));
                    }
                }
            }
        }

        return (smallestDistance, verticesInShortestDistance, indexesOfVerticesInShortestDistance);
    }
}

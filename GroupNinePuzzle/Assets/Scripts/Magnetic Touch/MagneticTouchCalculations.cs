using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchCalculations : MonoBehaviour
{
    /* Calculate displacement vector that the selected piece has to be translated
     with in order to fit together with the other piece. The translation is based
     on the two vertices closest two each other (one from each piece)  */
    public Vector3 CalculateDisplacementForSnapToPiece(GameObject selectedPiece, GameObject magnetPiece, int indexForVertexOfSelected, int indexForVertexOfMagnet)
    {
        Vector3 selectedVertex = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfSelected];
        Vector3 magnetVertex = magnetPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfMagnet];
        // Debug.Log(selectedVertex);
        // Debug.Log(magnetVertex);
        return selectedVertex - magnetVertex;
    }

    public float CalculateRotation(Vector3 primaryVertexInSelected, Vector3 primaryVertexToSnapTo, Vector3 previousVertexInSelected, Vector3 previousVertexToSnapTo)
    {
        Vector3 lineInSelected = previousVertexInSelected - primaryVertexInSelected;
        Vector3 lineToSnapTo = previousVertexToSnapTo - primaryVertexToSnapTo;

        Vector3 unitVectorSelected = lineInSelected / Vector3.Magnitude(lineInSelected);
        Vector3 unitVectorInPieceToSnapTo = lineToSnapTo / Vector3.Magnitude(lineToSnapTo);

        // Debug.Log("lines and rotation:");
        // Debug.Log(lineToVertexInSelected);
        // Debug.Log(lineToVertexInPieceToSnapTo);

        var radiansToRotate = 0f;
        if (unitVectorSelected != unitVectorInPieceToSnapTo)
        {
            var radiansSelected = Mathf.Atan2(unitVectorSelected.y, unitVectorSelected.x); // Angle between line and x-axis
            var radiansPieceToSnapTo = Mathf.Atan2(unitVectorInPieceToSnapTo.y, unitVectorInPieceToSnapTo.x); // Angle between line and x-axis
            radiansToRotate = radiansPieceToSnapTo - radiansSelected; // Angle between between lines

            // Debug.Log("radiansSelected " + radiansSelected * Mathf.Rad2Deg);
            // Debug.Log("radiansPieceToSnapTo " + radiansPieceToSnapTo * Mathf.Rad2Deg);
            Debug.Log("Degrees to rotate: " + radiansToRotate * Mathf.Rad2Deg);
        }

        return radiansToRotate;
    }
    public void TranslateAndRotatePiece(GameObject selectedPiece, Vector3 displacement, float rotationAngle, int indexOfVertexToRotateAbout)
    {
        Mesh selectedPieceMesh = selectedPiece.GetComponent<MeshFilter>().mesh;
        Vector3[] pieceVertices = selectedPieceMesh.vertices;
        LineRenderer lineRenderer = selectedPiece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[pieceVertices.Length];

        // Translate vertices
        for (int index = 0; index < pieceVertices.Length; index++)
        {
            translatedVertices[index].x = pieceVertices[index].x - displacement.x;
            translatedVertices[index].y = pieceVertices[index].y - displacement.y;
            // Debug.Log("index: " + index + "   x: " + translatedVertices[index].x + "   y:" + translatedVertices[index].y);
        }

        Vector3[] rotatedVertices = new Vector3[pieceVertices.Length];
        Vector3 vertexToRotateAbout = translatedVertices[indexOfVertexToRotateAbout];

        // Set vertexToRotateAbout as (0,0) and translate other vertices accordingly
        translatedVertices = CentralizeVertices(vertexToRotateAbout, translatedVertices);

        // Rotate vertices
        float rotationTheta = rotationAngle;
        for (int index = 0; index < translatedVertices.Length; index++)
        {
            rotatedVertices[index].x = translatedVertices[index].x * Mathf.Cos(rotationTheta) - translatedVertices[index].y * Mathf.Sin(rotationTheta);
            rotatedVertices[index].y = translatedVertices[index].x * Mathf.Sin(rotationTheta) + translatedVertices[index].y * Mathf.Cos(rotationTheta);
        }

        // Restore vertices by previous displacement where vertexToRotateAbout was set to (0,0)
        rotatedVertices = RestorePositionOfVertices(vertexToRotateAbout, rotatedVertices);

        // Update vertices in piece
        selectedPieceMesh.SetVertices(rotatedVertices);
        lineRenderer.SetPositions(rotatedVertices);
        selectedPiece.GetComponent<MeshCollider>().sharedMesh = selectedPieceMesh;
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

    // Todo: also exists in other files
    public int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }

    public SnapInformation FindPieceToSnapToAndSnapInformation((GameObject, List<GameObject>) possibleSnaps)
    {
        Vector3[] verticesOfSelected = possibleSnaps.Item1.GetComponent<MeshFilter>().mesh.vertices;
        (float, float) smallestDistance = (100000f, 100000f);

        var snapInformation = new SnapInformation();

        foreach (GameObject candidateForSnap in possibleSnaps.Item2)
        {
            Vector3[] verticesOfCandicate = candidateForSnap.GetComponent<MeshFilter>().mesh.vertices;

            var snapInformationResult = CalculateSnapInformation(verticesOfSelected, verticesOfCandicate);

            // Todo: Not correct. smallest distance might be item2, if the distance between previous vertice and other piece is smaller than distance between next vertex and other piece.
            if (snapInformationResult.DistanceBetweenPrimaryVertices < smallestDistance.Item1)
            {
                snapInformation = snapInformationResult;
                snapInformation.PieceToSnapTo = candidateForSnap;
            }
        }

        return snapInformation;
    }


    public SnapInformation CalculateSnapInformation(Vector3[] piece1, Vector3[] piece2)
    {
        SnapInformation snapInformation = new SnapInformation();
        snapInformation.DistanceBetweenPrimaryVertices = 100000f;
        snapInformation.DistanceBetweenSecondaryVertices = 100000f;

        for (int i = 0; i < piece1.Length; i++) // Iterate through piece 1
        {
            var vertex_piece1 = piece1[i];
            var previousVertex_piece1 = piece1[GetWrappingIndex(i - 1, piece1.Length)];
            var nextVertex_piece1 = piece1[GetWrappingIndex(i + 1, piece1.Length)];

            for (int k = 0; k < piece2.Length; k++) // Iterate through piece 2
            {
                var vertex_piece2 = piece2[k];
                var previousVertex_piece2 = piece2[GetWrappingIndex(k + 1, piece2.Length)]; // + because we need to go the other way around compared to piece1.
                var nextVertex_piece2 = piece2[GetWrappingIndex(k - 1, piece2.Length)]; // - because the same thing.

                var distBetweenVertices = Vector3.Distance(vertex_piece1, vertex_piece2);
                if (distBetweenVertices < snapInformation.DistanceBetweenPrimaryVertices)
                {
                    snapInformation.DistanceBetweenPrimaryVertices = distBetweenVertices;
                    
                    snapInformation.PrimaryVertexInSelectedPiece = vertex_piece1;
                    snapInformation.PrimaryVertexInPieceToSnapTo = vertex_piece2;
                    
                    snapInformation.IndexOfPrimaryVertexInSelectedPiece = i;
                    snapInformation.IndexOfPrimaryVertexInPieceToSnapTo = k;

                    var distPreviousVertices = Vector3.Distance(previousVertex_piece1, previousVertex_piece2);
                    var distNextVertices = Vector3.Distance(nextVertex_piece1, nextVertex_piece2);

                    if (distPreviousVertices <= distNextVertices)
                    {
                        snapInformation.DistanceBetweenSecondaryVertices = distPreviousVertices;

                        snapInformation.SecondaryVertexInSelectedPiece = previousVertex_piece1;
                        snapInformation.SecondaryVertexInPieceToSnapTo = previousVertex_piece2;

                        snapInformation.IndexOfSecondaryVertexInSelectedPiece = GetWrappingIndex(i - 1, piece1.Length);
                        snapInformation.IndexOfSecondaryVertexInPieceToSnapTo = GetWrappingIndex(k + 1, piece2.Length);

                        snapInformation.SecondaryVerticeIsPreviousVertice = true;
                    }
                    else
                    {
                        snapInformation.DistanceBetweenSecondaryVertices = distNextVertices;

                        snapInformation.SecondaryVertexInSelectedPiece = nextVertex_piece1;
                        snapInformation.SecondaryVertexInPieceToSnapTo = nextVertex_piece2;

                        snapInformation.IndexOfSecondaryVertexInSelectedPiece = GetWrappingIndex(i + 1, piece1.Length);
                        snapInformation.IndexOfSecondaryVertexInPieceToSnapTo = GetWrappingIndex(k - 1, piece2.Length);

                        snapInformation.SecondaryVerticeIsPreviousVertice = false;
                    }
                }
            }
        }

        return snapInformation;
    }
}

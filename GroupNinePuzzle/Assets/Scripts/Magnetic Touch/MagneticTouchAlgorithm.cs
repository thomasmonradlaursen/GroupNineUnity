using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MagneticTouchCalculations;

public class MagneticTouchAlgorithm : MonoBehaviour
{
    //Todo: probably move these fields somewhere else
    private List<GameObject> pieces;
    private Vector3 lowerLeftCorner;
    private Vector3 upperLeftCorner;
    private Vector3 lowerRightCorner;
    private Vector3 upperRightCorner;

    private (GameObject, List<GameObject>) possibleSnaps = (null, new List<GameObject>()); // Item1 is selected piece and Item 2 is a list of pieces that might be possible to snap to

    private void Start()
    {
        pieces = GetComponentInParent<PuzzleModel>().pieces;
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S)) // Snap to other piece
        {
            if (pieces.Count == 0)
            {
                pieces = GetComponentInParent<PuzzleModel>().pieces;
            }
            FindCandidatesForSnap(GetComponentInParent<PuzzleModel>().selectedObject);
            if (possibleSnaps.Item2.Count > 0)
            {
                SnapPiecesTogether();
            }
        }
        else if (Input.GetKeyDown(KeyCode.K)) // Snap to corner of board
        {
            FindCorners(); // Todo: move to some BoardModel/PuzzleModel object or something
            SnapToCorner(GetComponentInParent<PuzzleModel>().selectedObject);
        }
    }

    #region Snap Pieces Together
    private void SnapPiecesTogether()
    {
        SnapInformation snapInformation;
        snapInformation = FindPieceToSnapToAndSnapInformation(possibleSnaps);

        GameObject selectedPiece = GetComponentInParent<PuzzleModel>().selectedObject;

        Vector3 displacement = CalculateDisplacementForSnapToPiece(selectedPiece, snapInformation.PieceToSnapTo, snapInformation.IndexOfPrimaryVertexInSelectedPiece, snapInformation.IndexOfPrimaryVertexInPieceToSnapTo);

        float rotation = CalculateRotation(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.SecondaryVertexInSelectedPiece, snapInformation.SecondaryVertexInPieceToSnapTo);

        var test1 = CalculateConstantsForLineThroughTwoVertices(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.SecondaryVertexInSelectedPiece);
        var test2 = CalculateConstantsForLineThroughTwoVertices(snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.SecondaryVertexInPieceToSnapTo);

        // if (rotation > 0.35 || rotation < -0.35)
        // {
        //     Debug.Log("Angle is too big.");
        //     return;
        // }
        Vector3[] originalVertices = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
        TranslateAndRotatePiece(selectedPiece, displacement, rotation, snapInformation.IndexOfPrimaryVertexInSelectedPiece);


        if (CheckIfPiecesOverlap(selectedPiece, snapInformation.PieceToSnapTo))
        {
            Debug.Log("Cannot snap pieces together. They overlap.");
            RestoreVertices(selectedPiece, originalVertices);
        }
        else
        {
            // Establish new connections
            foreach (var pieceName in GetComponentInParent<PuzzleModel>().connectedPieces[snapInformation.PieceToSnapTo.name])
            {
                GetComponentInParent<PuzzleModel>().connectedPieces[selectedPiece.name].Add(pieceName);

                if (!GetComponentInParent<PuzzleModel>().connectedPieces[pieceName].Contains(selectedPiece.name))
                {
                    GetComponentInParent<PuzzleModel>().connectedPieces[pieceName].Add(selectedPiece.name);
                }
            }
            if (!GetComponentInParent<PuzzleModel>().connectedPieces[snapInformation.PieceToSnapTo.name].Contains(selectedPiece.name))
            {
                GetComponentInParent<PuzzleModel>().connectedPieces[snapInformation.PieceToSnapTo.name].Add(selectedPiece.name);
            }
            GetComponentInParent<PuzzleModel>().connectedPieces[selectedPiece.name].Add(snapInformation.PieceToSnapTo.name);
        }
    }



    private void FindCandidatesForSnap(GameObject selectedPiece)
    {
        possibleSnaps.Item1 = selectedPiece;
        possibleSnaps.Item2.Clear();
        var boundBoxForSelectedPiece = ConstructBoundBox(selectedPiece);
        float minimumXForSelected = boundBoxForSelectedPiece.Item1.Item1;
        float maximumXForSelected = boundBoxForSelectedPiece.Item1.Item2;
        float minimumYForSelected = boundBoxForSelectedPiece.Item2.Item1;
        float maximumYForSelected = boundBoxForSelectedPiece.Item2.Item2;
        foreach (GameObject piece in pieces)
        {
            var boundBoxForNext = ConstructBoundBox(piece);
            float minimumXForCompare = boundBoxForNext.Item1.Item1;
            float maximumXForCompare = boundBoxForNext.Item1.Item2;
            float minimumYForCompare = boundBoxForNext.Item2.Item1;
            float maximumYForCompare = boundBoxForNext.Item2.Item2;
            if (minimumXForSelected < maximumXForCompare && maximumXForSelected > minimumXForCompare
                && minimumYForSelected < maximumYForCompare && maximumYForSelected > minimumYForCompare)
            {
                if (!(piece.name.Equals(GetComponentInParent<PuzzleModel>().selectedObject.name)))
                {
                    possibleSnaps.Item2.Add(piece);
                }
            }
        }
    }

    private void LogPossibleSnaps()
    {
        if (possibleSnaps.Item2.Count > 0)
        {
            string printString = "";
            for (int index = 0; index < possibleSnaps.Item2.Count; index++)
            {
                if (index == possibleSnaps.Item2.Count - 1)
                {
                    printString = printString + possibleSnaps.Item2[index].name + ". ";
                }
                else
                {
                    printString = printString + possibleSnaps.Item2[index].name + ", ";
                }
            }
        }
    }
    #endregion


    #region Check for overlaps

    private bool CheckIfPiecesOverlap(GameObject selectedPiece, GameObject pieceToSnapTo)
    {
        Vector3[] verticesSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] verticesSnapPiece = pieceToSnapTo.GetComponent<MeshFilter>().mesh.vertices;
        var meshTrianglesSelected = selectedPiece.GetComponent<MeshFilter>().mesh.triangles;
        var meshTrianglesSnapPiece = pieceToSnapTo.GetComponent<MeshFilter>().mesh.triangles;

        //     Debug.Log("Vertices in selected piece");
        // foreach(var vertex in verticesSelectedPiece){
        //     Debug.Log(vertex);
        // }

        #region Check for overlap with piece we are trying to snap to

        var linesInSelectedPiece = GetLinesInPiece(verticesSelectedPiece);
        var linesInSnapPiece = GetLinesInPiece(verticesSnapPiece);

        if (AnyIntersectionsBetweenLines(verticesSelectedPiece, verticesSnapPiece, linesInSelectedPiece, linesInSnapPiece))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return true;
        }

        Debug.Log("container piece is snap piece");
        if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSelectedPiece, verticesSnapPiece, meshTrianglesSnapPiece))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return true;
        }

        Debug.Log("container piece is selected piece");
        if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSnapPiece, verticesSelectedPiece, meshTrianglesSelected))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return true;
        }

        #endregion


        #region Check for overlaps with pieces already snapped together with the piece we are trying to snap to

        List<string> connectedPiecesNames = pieceToSnapTo.GetComponentInParent<PuzzleModel>().connectedPieces[pieceToSnapTo.name];
        var connectedPieces = new List<GameObject>();
        foreach (var piece in pieceToSnapTo.GetComponentInParent<PuzzleModel>().pieces)
        {
            if (connectedPiecesNames.Contains(piece.name) && piece.name != selectedPiece.name)
            {
                connectedPieces.Add(piece);
            }
        }

        foreach (var connectedPiece in connectedPieces)
        {
            Vector3[] verticesConnectedPiece = connectedPiece.GetComponent<MeshFilter>().mesh.vertices;
            var meshTrianglesConnected = connectedPiece.GetComponent<MeshFilter>().mesh.triangles;

            var linesInConnectedPiece = GetLinesInPiece(verticesConnectedPiece);

            if (AnyIntersectionsBetweenLines(verticesSelectedPiece, verticesConnectedPiece, linesInSelectedPiece, linesInConnectedPiece))
            {
                Debug.Log("Overlap with connected piece: " + connectedPiece.name);
                return true;
            }

            Debug.Log("container piece is connected piece");
            if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSelectedPiece, verticesConnectedPiece, meshTrianglesConnected))
            {
                Debug.Log("Overlap with connected piece: " + connectedPiece.name);
                return true;
            }

            Debug.Log("container piece is selected piece");
            if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesConnectedPiece, verticesSelectedPiece, meshTrianglesSelected))
            {
                Debug.Log("Overlap with connected piece: " + connectedPiece.name);
                return true;
            }
        }

        #endregion

        return false;
    }

    private bool CheckIfAnyPointIsContainedInTheOtherPiece(Vector3[] pointPieceVertices, Vector3[] containerPieceVertices, int[] triangles)
    {
        for (int i = 0; i < pointPieceVertices.Length; i++)
        {
            // if (IsVertexContainedInOtherPiece(pointPieceVertices[i], containerPieceVertices, triangles))
            if (IsPointInPiece(pointPieceVertices[i], containerPieceVertices, triangles))
            {
                return true;
            }
        }

        return false;
    }

    private bool AnyIntersectionsBetweenLines(Vector3[] verticesPiece1, Vector3[] verticesPiece2, List<(float, float)> linesPiece1, List<(float, float)> linesPiece2)
    {
        // Check if any lines from piece1 intersects with any lines from piece2.
        // If we encounter an intersection, we return true.
        var idx1 = 0;
        foreach (var line1 in linesPiece1)
        {
            var idx2 = 0;
            foreach (var line2 in linesPiece2)
            {
                var linesIntersect = CheckIfLinesIntersectInLineSegment(line1,
                        line2,
                        verticesPiece1[idx1],
                        verticesPiece1[GetWrappingIndex(idx1 + 1, verticesPiece1.Length)],
                        verticesPiece2[idx2],
                        verticesPiece2[GetWrappingIndex(idx2 + 1, verticesPiece2.Length)]
                        );
                if (linesIntersect)
                {
                    Debug.Log("line1: " + line1);
                    Debug.Log("line2: " + line2);
                    Debug.Log("verticesPiece1: " + verticesPiece1[idx1]);
                    Debug.Log("verticesPiece1: " + verticesPiece1[GetWrappingIndex(idx1 + 1, verticesPiece1.Length)]);
                    Debug.Log("verticesPiece2: " + verticesPiece2[idx2]);
                    Debug.Log("verticesPiece2: " + verticesPiece2[GetWrappingIndex(idx2 + 1, verticesPiece2.Length)]);
                    return true;
                }
                idx2++;
            }

            idx1++;
        }

        return false;
    }

    #endregion


    #region Snap to Corner

    // Todo: maybe move to other class?
    private void FindCorners()
    {
        var form = GetComponentInParent<PuzzleModel>().puzzle.puzzle.form;

        var corners = new List<Vector3>();

        foreach (var item in form)
        {
            Vector3 vector = new Vector3(item.coord.x, item.coord.y, 0);
            corners.Add(vector);
        }

        float minX = corners[0].x;
        float maxX = corners[0].x;
        float minY = corners[0].y;
        float maxY = corners[0].y;

        foreach (var corner in corners)
        {
            if (corner.x < minX)
            {
                minX = corner.x;
            }
            if (corner.x > maxX)
            {
                maxX = corner.x;
            }
            if (corner.y < minY)
            {
                minY = corner.y;
            }
            if (corner.y > maxY)
            {
                maxY = corner.y;
            }
        }

        lowerLeftCorner = new Vector3(minX, minY, 0);
        upperLeftCorner = new Vector3(minX, maxY, 0);
        lowerRightCorner = new Vector3(maxX, minY, 0);
        upperRightCorner = new Vector3(maxX, maxY, 0);
    }


    private void SnapToCorner(GameObject selectedPiece)
    {
        var cornerVertices = new Vector3[] { lowerLeftCorner, upperLeftCorner, lowerRightCorner, upperRightCorner };
        var pieceVertices = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;

        var findVertexToSnapToCornerResult = FindVertexToSnapToCorner(pieceVertices);
        var distanceFromVertexToCorner = findVertexToSnapToCornerResult.Item1;
        var indexOfVertexToSnapToCorner = findVertexToSnapToCornerResult.Item2;
        var cornerToSnapTo = findVertexToSnapToCornerResult.Item3;
        var neighboringCorners = findVertexToSnapToCornerResult.Item4;


        if (distanceFromVertexToCorner > margin * 3)
        {
            Debug.Log("Too far away from corner");
            Debug.Log("Distance: " + distanceFromVertexToCorner);
            return;
        }

        var vertexToSnapToCorner = pieceVertices[indexOfVertexToSnapToCorner];
        var borderEdge1 = (cornerToSnapTo, neighboringCorners.Item1);
        var borderEdge2 = (cornerToSnapTo, neighboringCorners.Item2);

        // Todo: ??? Is it a problem if vertex closest to the border is on the "wrong" side of the border?
        var smallestDistanceToBorderResult = SmallestDistanceToBorder(pieceVertices, vertexToSnapToCorner, borderEdge1, borderEdge2);

        var distanceFromVertexToBorder = smallestDistanceToBorderResult.Item1;
        var indexOfVertexWithShortestDistanceToBorder = smallestDistanceToBorderResult.Item2;
        var edgeInBorder = smallestDistanceToBorderResult.Item3;

        var vertexToMoveToBorder = pieceVertices[indexOfVertexWithShortestDistanceToBorder];
        var otherCornerOfEdgeToSnapTo = edgeInBorder.Item2;

        Vector3 displacement = vertexToSnapToCorner - cornerToSnapTo;
        float rotation = CalculateRotation(vertexToSnapToCorner, cornerToSnapTo, vertexToMoveToBorder, otherCornerOfEdgeToSnapTo);

        // // Angle is too big.
        // // Either the rotation is not as intended or the player should try harder to place the pieces accurately
        // if (rotation > 0.35 || rotation < -0.35)
        // {
        //     Debug.Log("Angle is too big.");
        //     return;
        // }

        var linesInPiece = GetLinesInPiece(pieceVertices);
        var linesInBorder = GetLinesInPiece(cornerVertices);

        TranslateAndRotatePiece(selectedPiece, displacement, rotation, indexOfVertexToSnapToCorner);
    }

    // Finds vertex with shortest distance to a corner of the board and information about snapping to the corner 
    private (float, int, Vector3, (Vector3, Vector3)) FindVertexToSnapToCorner(Vector3[] pieceVertices)
    {
        float shortestDistance = 100000f;
        int indexOfVertex = -1;
        Vector3 cornerToSnapTo = new Vector3();
        (Vector3, Vector3) neighboringCorners = (new Vector3(), new Vector3());

        int idx = 0;
        foreach (var vertex in pieceVertices)
        {

            var distanceToLowerLeftCorner = Vector3.Distance(vertex, lowerLeftCorner);
            var distanceToUpperLeftCorner = Vector3.Distance(vertex, upperLeftCorner);
            var distanceToLowerRightCorner = Vector3.Distance(vertex, lowerRightCorner);
            var distanceToUpperRightCorner = Vector3.Distance(vertex, upperRightCorner);

            if (distanceToLowerLeftCorner < shortestDistance)
            {
                shortestDistance = distanceToLowerLeftCorner;
                indexOfVertex = idx;
                cornerToSnapTo = lowerLeftCorner;
                neighboringCorners = (upperLeftCorner, lowerRightCorner);
            }
            if (distanceToUpperLeftCorner < shortestDistance)
            {
                shortestDistance = distanceToUpperLeftCorner;
                indexOfVertex = idx;
                cornerToSnapTo = upperLeftCorner;
                neighboringCorners = (lowerLeftCorner, upperRightCorner);
            }
            if (distanceToLowerRightCorner < shortestDistance)
            {
                shortestDistance = distanceToLowerRightCorner;
                indexOfVertex = idx;
                cornerToSnapTo = lowerRightCorner;
                neighboringCorners = (lowerLeftCorner, upperRightCorner);
            }
            if (distanceToUpperRightCorner < shortestDistance)
            {
                shortestDistance = distanceToUpperRightCorner;
                indexOfVertex = idx;
                cornerToSnapTo = upperRightCorner;
                neighboringCorners = (upperLeftCorner, lowerRightCorner);
            }

            idx++;
        }

        return (shortestDistance, indexOfVertex, cornerToSnapTo, neighboringCorners);
    }

    private (float, int, (Vector3, Vector3)) SmallestDistanceToBorder(Vector3[] pieceVertices, Vector3 vertexToSnapToCorner, (Vector3, Vector3) borderEdge1, (Vector3, Vector3) borderEdge2)
    {
        float shortestDistance = 100000f;
        int indexOfVertex = -1;
        (Vector3, Vector3) edge = (new Vector3(), new Vector3());

        int idx = 0;
        foreach (var vertex in pieceVertices)
        {

            if (vertex == vertexToSnapToCorner)
            {
                idx++;
                continue;
            }

            var distanceToEdge1 = CalculateDistanceFromPointToLine(vertex, borderEdge1);
            var distanceToEdge2 = CalculateDistanceFromPointToLine(vertex, borderEdge2);

            if (distanceToEdge1 < shortestDistance)
            {
                shortestDistance = distanceToEdge1;
                indexOfVertex = idx;
                edge = borderEdge1;
            }
            if (distanceToEdge2 < shortestDistance)
            {
                shortestDistance = distanceToEdge2;
                indexOfVertex = idx;
                edge = borderEdge2;
            }
            idx++;
        }

        return (shortestDistance, indexOfVertex, edge);
    }

    #endregion

    private SnapInformation FindPieceToSnapToAndSnapInformation((GameObject, List<GameObject>) possibleSnaps)
    {
        Vector3[] verticesOfSelected = possibleSnaps.Item1.GetComponent<MeshFilter>().mesh.vertices;
        float smallestDistance = 100000f;
        float smallestAngleForVertexWithSmallestDistance = 100000f;

        var snapInformation = new SnapInformation();

        foreach (GameObject candidateForSnap in possibleSnaps.Item2)
        {
            Vector3[] verticesOfCandicate = candidateForSnap.GetComponent<MeshFilter>().mesh.vertices;

            var snapInformationResult = CalculateSnapInformation(verticesOfSelected, verticesOfCandicate);

            if (RoundToXDecimals(snapInformationResult.DistanceBetweenPrimaryVertices, 2) < RoundToXDecimals(smallestDistance, 2)
                || (RoundToXDecimals(snapInformationResult.DistanceBetweenPrimaryVertices, 2) == RoundToXDecimals(smallestDistance, 2)
                    && Mathf.Abs(snapInformationResult.AngleBetweenEdges) < Mathf.Abs(smallestAngleForVertexWithSmallestDistance)))
            {
                snapInformation = snapInformationResult;
                snapInformation.PieceToSnapTo = candidateForSnap;
                smallestDistance = snapInformationResult.DistanceBetweenPrimaryVertices;
            }
        }

        return snapInformation;
    }

    private List<(float, float)> GetLinesInPiece(Vector3[] verticesInPiece, List<int> indexesOfStartingVerticesToExclude = null)
    {
        var linesInPiece = new List<(float, float)>();

        if (indexesOfStartingVerticesToExclude == null)
        {
            indexesOfStartingVerticesToExclude = new List<int>();
        }

        for (int i = 0; i < verticesInPiece.Length; i++)
        {
            if (indexesOfStartingVerticesToExclude.Contains(i)) continue;

            var vertex_i = verticesInPiece[i];
            var vertex_i_plus_1 = verticesInPiece[GetWrappingIndex(i + 1, verticesInPiece.Length)];
            linesInPiece.Add(CalculateConstantsForLineThroughTwoVertices(vertex_i, vertex_i_plus_1));
        }

        return linesInPiece;
    }
}
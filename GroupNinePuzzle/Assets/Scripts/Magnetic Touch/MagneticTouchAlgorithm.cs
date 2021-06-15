using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchAlgorithm : MonoBehaviour
{
    public List<GameObject> pieces;
    private Vector3 lowerLeftCorner;
    private Vector3 upperLeftCorner;
    private Vector3 lowerRightCorner;
    private Vector3 upperRightCorner;
    private float margin = 0.175f;

    public (GameObject, List<GameObject>) possibleSnaps = (null, new List<GameObject>());

    private void Start()
    {
        //Debug.Log("MagneticTouchAlgorithm - Start()");
        pieces = GetComponentInParent<PieceController>().pieces;
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Debug.Log("MagneticTouchAlgorithm - TestingImplementation()");
            FindCandidatesForSnap(GetComponentInParent<MeshFromJsonGenerator>().selectedObject);
            LogPossibleSnaps();
            if (possibleSnaps.Item2.Count > 0)
            {
                FindClosestVertexPair();
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            FindCorners();
            SnapToCorner(GetComponentInParent<MeshFromJsonGenerator>().selectedObject);
        }
    }

    void FindClosestVertexPair()
    {
        SnapInformation snapInformation;
        snapInformation = GetComponentInParent<FindClosestVertex>().FindClosestVertexToSelectedPiece(possibleSnaps);
        GameObject selectedPiece = GetComponentInParent<MeshFromJsonGenerator>().selectedObject;

        if (this.name.Equals(selectedPiece.name))
        {
            // snapInformation.DebugLogInformation();
            // Debug.Log(selectedPiece.name);
            // Debug.Log("this: " + this.name);
            // Debug.Log("this.name.Equals(selectedPiece.name");

            int indexOfClosestVertexInSelectedPiece = snapInformation.IndexOfPrimaryVertexInSelectedPiece;
            int indexOfClosestVertexInPieceToSnapTo = snapInformation.IndexOfPrimaryVertexInPieceToSnapTo;
            int indexOfNeighborVertexInSelectedPiece = snapInformation.IndexOfPreviousVertexInSelectedPiece;
            int indexOfNeighborVertexInPieceToSnapTo = snapInformation.IndexOfPreviousVertexInPieceToSnapTo;

            // Debug.LogFormat("index of vertex in selected piece: " + indexOfClosestVertexInSelectedPiece);
            // Debug.LogFormat("vertex in selected piece: " + selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexOfClosestVertexInSelectedPiece]);

            Vector3 displacement = GetComponentInParent<FindClosestVertex>().CalculateDisplacementForSnap(selectedPiece, snapInformation.PieceToSnapTo, indexOfClosestVertexInSelectedPiece, indexOfClosestVertexInPieceToSnapTo);
            float rotation = GetComponentInParent<FindClosestVertex>().CalculateRotationForSnap(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.PreviousVertexInSelectedPiece, snapInformation.PreviousVertexInPieceToSnapTo);

            // Angle is too big.
            // Either the rotation is not as intenden or the player should try harder to place the pieces accurately
            if (rotation > 0.35 || rotation < -0.35)
            {
                Debug.Log("Angle is too big.");
                return;
            }

            // Debug.Log("Displacement: " + displacement);
            // Debug.Log("Rotation: " + rotation);
            Vector3[] originalVertices = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;


            GetComponentInParent<FindClosestVertex>().CalculateVerticesAfterSnapTranslationAndRotation(selectedPiece, displacement, rotation, snapInformation.IndexOfPrimaryVertexInSelectedPiece);

            if (CheckIfPiecesOverlap(selectedPiece, snapInformation.PieceToSnapTo))
            {
                Debug.Log("Cannot snap pieces together. They overlap.");
                RestoreVertices(selectedPiece, originalVertices);
            }
            else
            {
                // Establish new connections
                foreach (var pieceName in GetComponentInParent<PieceController>().connectedPieces[snapInformation.PieceToSnapTo.name])
                {
                    GetComponentInParent<PieceController>().connectedPieces[selectedPiece.name].Add(pieceName);

                    if (!GetComponentInParent<PieceController>().connectedPieces[pieceName].Contains(selectedPiece.name))
                    {
                        GetComponentInParent<PieceController>().connectedPieces[pieceName].Add(selectedPiece.name);
                    }
                }
                if (!GetComponentInParent<PieceController>().connectedPieces[snapInformation.PieceToSnapTo.name].Contains(selectedPiece.name))
                {
                    GetComponentInParent<PieceController>().connectedPieces[snapInformation.PieceToSnapTo.name].Add(selectedPiece.name);
                }
                GetComponentInParent<PieceController>().connectedPieces[selectedPiece.name].Add(snapInformation.PieceToSnapTo.name);
            }

            Debug.Log("========= Connected Pieces =========");

            Debug.Log("ConnectedWith: " + selectedPiece.name);

            foreach (var item in GetComponentInParent<PieceController>().connectedPieces[selectedPiece.name])
            {
                Debug.Log("Next: " + item);
                foreach (var item2 in GetComponentInParent<PieceController>().connectedPieces[item])
                {
                    Debug.Log(item2);
                }
            }

        }
    }

    void RestoreVertices(GameObject piece, Vector3[] originalVertices)
    {
        Mesh meshForPiece = piece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = piece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForPiece.vertices.Length];

        meshForPiece.SetVertices(originalVertices);
        lineRenderer.SetPositions(originalVertices);
        piece.GetComponent<MeshCollider>().sharedMesh = meshForPiece;
    }

    ((float, float), (float, float)) ConstructBoundBox(GameObject piece)
    {
        (float, float) minMaxX = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumXCoordinate();
        (float, float) minMaxY = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumYCoordinate();
        return ((minMaxX.Item1 - margin, minMaxX.Item2 + margin), (minMaxY.Item1 - margin, minMaxY.Item2 + margin));
    }

    void FindCandidatesForSnap(GameObject selectedPiece)
    {
        possibleSnaps.Item1 = selectedPiece;
        possibleSnaps.Item2.Clear();
        var boundBoxForSelectedPiece = ConstructBoundBox(selectedPiece);
        float minimumXForSelected = boundBoxForSelectedPiece.Item1.Item1;
        float maximumXForSelected = boundBoxForSelectedPiece.Item1.Item2;
        float minimumXForCompare;
        float maximumXForCompare;
        float minimumYForSelected = boundBoxForSelectedPiece.Item2.Item1;
        float maximumYForSelected = boundBoxForSelectedPiece.Item2.Item2;
        float minimumYForCompare;
        float maximumYForCompare;
        // Debug.Log("MinimumForSelected: " + minimumForSelected + ", MaximumForSelected: " + maximumForSelected);
        foreach (GameObject piece in pieces)
        {
            var boundBoxForNext = ConstructBoundBox(piece);
            minimumXForCompare = boundBoxForNext.Item1.Item1;
            maximumXForCompare = boundBoxForNext.Item1.Item2;
            minimumYForCompare = boundBoxForNext.Item2.Item1;
            maximumYForCompare = boundBoxForNext.Item2.Item2;
            // Debug.Log("Minimum: " + minimumForCompare + ", maximum: " + maximumForCompare);
            if (minimumXForSelected < maximumXForCompare && maximumXForSelected > minimumXForCompare
                && minimumYForSelected < maximumYForCompare && maximumYForSelected > minimumYForCompare)
            {
                if (!(piece.name.Equals(GetComponentInParent<MeshFromJsonGenerator>().selected)))
                {
                    possibleSnaps.Item2.Add(piece);
                }
            }
        }
    }

    void LogPossibleSnaps()
    {
        // Debug.Log("Number of possible snaps " + possibleSnaps.Item2.Count);
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
            Debug.Log(possibleSnaps.Item1.name + " can possibly snap to the following pieces: " + printString);
        }
        else
        {
            Debug.Log(possibleSnaps.Item1.name + " cannot snap to any other pieces at the moment.");
        }
    }

    bool CheckIfPiecesOverlap(GameObject selectedPiece, GameObject pieceToSnapTo/*, Vector3 snapVertexSelectedPiece*/)
    {
        // Check that two or more vertices in succession in the two pieces are identical.
        // Any identical vertices that are not in succession means the two pieces don't fit together (in the current orientation).
        // The above method assumes that a long straight line will only fit with a short straight line if
        // the long straight line is made up of multiple shorter straight lines.

        // Use PolygonTriangulation methods to determine if pieces overlap?
        // Maybe check all lines from one piece for intersections with any lines in the other piece?
        // If we do the line thing we should maybe exclude the lines that the two pieces share?

        // Line intersection:
        Vector3[] verticesSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] verticesSnapPiece = pieceToSnapTo.GetComponent<MeshFilter>().mesh.vertices;
        if (AnyIntersectionsBetweenLines(verticesSelectedPiece, verticesSnapPiece))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return true;
        }

        List<string> connectedPiecesNames = pieceToSnapTo.GetComponentInParent<PieceController>().connectedPieces[pieceToSnapTo.name];
        var connectedPieces = new List<GameObject>();
        foreach (var piece in pieceToSnapTo.GetComponentInParent<PieceController>().pieces)
        {
            if (connectedPiecesNames.Contains(piece.name) && piece.name != selectedPiece.name)
            {
                connectedPieces.Add(piece);
            }
        }

        foreach (var connectedPiece in connectedPieces)
        {
            Vector3[] verticesConnectedPiece = connectedPiece.GetComponent<MeshFilter>().mesh.vertices;
            if (AnyIntersectionsBetweenLines(verticesSelectedPiece, verticesConnectedPiece))
            {
                Debug.Log("Overlap with piece: " + connectedPiece.name);
                return true;
            }
        }

        return false;
    }

    bool AnyIntersectionsBetweenLines(Vector3[] verticesPiece1, Vector3[] verticesPiece2)
    {
        var linesPiece1 = new List<(float, float)>();
        var linesPiece2 = new List<(float, float)>();

        // Calculate lines in piece 1
        for (int i = 0; i < verticesPiece1.Length; i++)
        {
            var vertex_i = verticesPiece1[i];
            var vertex_i_plus_1 = verticesPiece1[GetWrappingIndex(i + 1, verticesPiece1.Length)];
            linesPiece1.Add(CalculateConstantsForLineThroughTwoVertices(vertex_i, vertex_i_plus_1));
        }

        // Calculate lines in piece 2
        for (int i = 0; i < verticesPiece2.Length; i++)
        {
            var vertex_i = verticesPiece2[i];
            var vertex_i_plus_1 = verticesPiece2[GetWrappingIndex(i + 1, verticesPiece2.Length)];
            linesPiece2.Add(CalculateConstantsForLineThroughTwoVertices(vertex_i, vertex_i_plus_1));
        }

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
                    // Debug.Log("===== Intersection =====");
                    // Debug.Log("");
                    // Debug.Log(verticesPiece1[idx1]);
                    // Debug.Log(verticesPiece1[GetWrappingIndex(idx1 + 1, verticesPiece1.Length)]);
                    // Debug.Log(verticesPiece2[idx2]);
                    // Debug.Log(verticesPiece2[GetWrappingIndex(idx2 + 1, verticesPiece2.Length)]);

                    return true;
                }
                idx2++;
            }
            idx1++;
        }

        return false;
    }

    bool CheckIfLinesIntersectInLineSegment((float, float) line1, (float, float) line2, Vector3 point1Line1, Vector3 point2Line1, Vector3 point1Line2, Vector3 point2Line2)
    {
        if (line1.Item1 == line2.Item1)
        { // if the slope is the same
            return false;
        }

        var intersectionX = (line2.Item2 - line1.Item2) / (line1.Item1 - line2.Item1);



        // var intersectionY = line1.Item1 * intersectionX + line1.Item2;

        // if the x-value of the intersection point is in both line segments, the two line segments intersect.
        if (((RoundTo3Decimals(point1Line1.x) < intersectionX - 0.01 && intersectionX + 0.01 < RoundTo3Decimals(point2Line1.x))
            || (RoundTo3Decimals(point1Line1.x) > intersectionX + 0.01 && intersectionX - 0.01 > RoundTo3Decimals(point2Line1.x)))
            && ((RoundTo3Decimals(point1Line2.x) < intersectionX - 0.01 && intersectionX + 0.01 < RoundTo3Decimals(point2Line2.x))
            || (RoundTo3Decimals(point1Line2.x) > intersectionX + 0.01 && intersectionX - 0.01 > RoundTo3Decimals(point2Line2.x))))
        {
            Debug.Log("===== Intersection =====");
            Debug.Log("Top: " + (line2.Item2 - line1.Item2));
            Debug.Log("Bottom: " + (line1.Item1 - line2.Item1));
            Debug.Log("");
            Debug.Log("intersectionX: " + intersectionX);
            Debug.Log("intersectionX rounded: " + RoundTo3Decimals(intersectionX));
            Debug.Log("line1 point1: " + RoundTo3Decimals(point1Line1.x));
            Debug.Log("line1 point2: " + RoundTo3Decimals(point2Line1.x));
            Debug.Log("line2 point1: " + RoundTo3Decimals(point1Line2.x));
            Debug.Log("line2 point2: " + RoundTo3Decimals(point2Line2.x));
            return true;
        }

        return false;
    }

    float RoundTo3Decimals(float toRound)
    {
        return Mathf.Round(toRound * 100f) / 100f;
    }

    (float, float) CalculateConstantsForLineThroughTwoVertices(Vector3 vertex1, Vector3 vertex2)
    {

        float lineSlope = (vertex2.y - vertex1.y) / (vertex2.x - vertex1.x);
        float lineIntersectionWithYAxis = vertex1.y - lineSlope * vertex1.x;

        (float, float) lineConstants = (lineSlope, lineIntersectionWithYAxis);

        return lineConstants;
    }

    void FindCorners()
    {
        var form = GetComponentInParent<PieceController>().puzzle.puzzle.form;

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


    public void SnapToCorner(GameObject selectedPiece)
    {
        var cornerVertices = new Vector3[] { lowerLeftCorner, upperLeftCorner, lowerRightCorner, upperRightCorner };
        var pieceVertices = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;

        var findVertexToSnapToCornerResult = FindVertexToSnapToCorner(pieceVertices);
        var distanceFromVertexToCorner = findVertexToSnapToCornerResult.Item1;
        var indexOfVertexToSnapToCorner = findVertexToSnapToCornerResult.Item2;
        var cornerToSnapTo = findVertexToSnapToCornerResult.Item3;
        var neighboringCorners = findVertexToSnapToCornerResult.Item4;


        if(distanceFromVertexToCorner > margin*3){
            Debug.Log("Too far away from corner");
            Debug.Log("Distance: " + distanceFromVertexToCorner);
            return;
        }

        var vertexToSnapToCorner = pieceVertices[indexOfVertexToSnapToCorner];
        var borderEdge1 = (cornerToSnapTo, neighboringCorners.Item1);
        var borderEdge2 = (cornerToSnapTo, neighboringCorners.Item2);

        // ??? Is it a problem if vertex closest to the border is on the "wrong" side of the border?
        var smallestDistanceToBorderResult = SmallestDistanceToBorder(pieceVertices, vertexToSnapToCorner, borderEdge1, borderEdge2);

        var distanceFromVertexToBorder = smallestDistanceToBorderResult.Item1;
        var indexOfVertexWithShortestDistanceToBorder = smallestDistanceToBorderResult.Item2;
        var edgeInBorder = smallestDistanceToBorderResult.Item3;

        var vertexToMoveToBorder = pieceVertices[indexOfVertexWithShortestDistanceToBorder];
        var otherCornerOfEdgeToSnapTo = edgeInBorder.Item2;

        Vector3 displacement = vertexToSnapToCorner - cornerToSnapTo;
        float rotation = GetComponentInParent<FindClosestVertex>().CalculateRotationForSnap(vertexToSnapToCorner, cornerToSnapTo, vertexToMoveToBorder, otherCornerOfEdgeToSnapTo);

        // Angle is too big.
        // Either the rotation is not as intended or the player should try harder to place the pieces accurately
        if (rotation > 0.35 || rotation < -0.35)
        {
            Debug.Log("Angle is too big.");
            return;
        }

        if(AnyIntersectionsBetweenLines(pieceVertices, cornerVertices)){
            Debug.Log("Piece overlaps with border.");
        }

        GetComponentInParent<FindClosestVertex>().CalculateVerticesAfterSnapTranslationAndRotation(selectedPiece, displacement, rotation, indexOfVertexToSnapToCorner);
    }

    public (float, int, Vector3, (Vector3, Vector3)) FindVertexToSnapToCorner(Vector3[] pieceVertices)
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

    public (float, int, (Vector3, Vector3)) SmallestDistanceToBorder(Vector3[] pieceVertices, Vector3 vertexToSnapToCorner, (Vector3, Vector3) borderEdge1, (Vector3, Vector3) borderEdge2)
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

    private float CalculateDistanceFromPointToLine(Vector3 point, (Vector3, Vector3) edge)
    {
        var edgePoint1 = edge.Item1;
        var edgePoint2 = edge.Item2;

        var numerator = Mathf.Abs((edgePoint2.x - edgePoint1.x) * (edgePoint1.y - point.y) - (edgePoint1.x - point.x) * (edgePoint2.y - edgePoint1.y));
        var denominator = Mathf.Sqrt(Mathf.Pow((edgePoint2.x - edgePoint1.x), 2) + Mathf.Pow((edgePoint2.y - edgePoint1.y), 2));

        var distance = numerator / denominator;
        return distance;
    }

    public int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }

}




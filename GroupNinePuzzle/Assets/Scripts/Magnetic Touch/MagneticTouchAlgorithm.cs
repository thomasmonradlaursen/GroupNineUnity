using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchAlgorithm : MonoBehaviour
{
    //Todo: probably move these fields somewhere else
    public List<GameObject> pieces;
    private Vector3 lowerLeftCorner;
    private Vector3 upperLeftCorner;
    private Vector3 lowerRightCorner;
    private Vector3 upperRightCorner;
    private float margin = 0.175f;

    public (GameObject, List<GameObject>) possibleSnaps = (null, new List<GameObject>()); // Item1 is selected piece and Item 2 is a list of pieces that might be possible to snap to

    private void Start()
    {
        pieces = GetComponentInParent<PuzzleModel>().pieces;
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S)) // Snap to other piece
        {
            FindCandidatesForSnap(GetComponentInParent<PuzzleModel>().selectedObject);
            // LogPossibleSnaps();
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
    void SnapPiecesTogether()
    {
        SnapInformation snapInformation;
        snapInformation = GetComponentInParent<MagneticTouchCalculations>().FindPieceToSnapToAndSnapInformation(possibleSnaps);
        GameObject selectedPiece = GetComponentInParent<PuzzleModel>().selectedObject;

        if (this.name.Equals(selectedPiece.name))
        {
            Vector3 displacement = GetComponentInParent<MagneticTouchCalculations>().CalculateDisplacementForSnapToPiece(selectedPiece, snapInformation.PieceToSnapTo, snapInformation.IndexOfPrimaryVertexInSelectedPiece, snapInformation.IndexOfPrimaryVertexInPieceToSnapTo);

            float rotation = GetComponentInParent<MagneticTouchCalculations>().CalculateRotation(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.SecondaryVertexInSelectedPiece, snapInformation.SecondaryVertexInPieceToSnapTo);

            var test1 = CalculateConstantsForLineThroughTwoVertices(snapInformation.PrimaryVertexInSelectedPiece, snapInformation.SecondaryVertexInSelectedPiece);
            var test2 = CalculateConstantsForLineThroughTwoVertices(snapInformation.PrimaryVertexInPieceToSnapTo, snapInformation.SecondaryVertexInPieceToSnapTo);
            // Debug.Log("line selected: " + test1.Item1 + "*x + " + test1.Item2);
            // Debug.Log("line to snap to: " + test2.Item1 + "*x + " + test2.Item2);
            // Debug.Log("selected Point1: " + snapInformation.PrimaryVertexInSelectedPiece);
            // Debug.Log("selected Point2: " + snapInformation.SecondaryVertexInSelectedPiece);
            // Debug.Log("snap to Point1: " + snapInformation.PrimaryVertexInPieceToSnapTo);
            // Debug.Log("snap to Point2: " + snapInformation.SecondaryVertexInPieceToSnapTo);


            if (rotation > 0.35 || rotation < -0.35)
            {
                Debug.Log("Angle is too big.");
                return;
            }
            Vector3[] originalVertices = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
            GetComponentInParent<MagneticTouchCalculations>().TranslateAndRotatePiece(selectedPiece, displacement, rotation, snapInformation.IndexOfPrimaryVertexInSelectedPiece);


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
            var test4 = GetComponentInParent<PuzzleModel>();
            var test5 = selectedPiece.name;
            var test6 = test4.connectedPieces[selectedPiece.name];
            foreach (var item in GetComponentInParent<PuzzleModel>().connectedPieces[selectedPiece.name])
            {
                foreach (var item2 in GetComponentInParent<PuzzleModel>().connectedPieces[item])
                {
                    // Debug.Log(item2);
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
        foreach (GameObject piece in pieces)
        {
            var boundBoxForNext = ConstructBoundBox(piece);
            minimumXForCompare = boundBoxForNext.Item1.Item1;
            maximumXForCompare = boundBoxForNext.Item1.Item2;
            minimumYForCompare = boundBoxForNext.Item2.Item1;
            maximumYForCompare = boundBoxForNext.Item2.Item2;
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

    void LogPossibleSnaps()
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
            // Debug.Log(possibleSnaps.Item1.name + " can possibly snap to the following pieces: " + printString);
        }
        else
        {
            // Debug.Log(possibleSnaps.Item1.name + " cannot snap to any other pieces at the moment.");
        }
    }
    #endregion


    #region Check for overlaps

    bool CheckIfPiecesOverlap(GameObject selectedPiece, GameObject pieceToSnapTo/*, Vector3 snapVertexSelectedPiece*/)
    {
        Vector3[] verticesSelectedPiece = selectedPiece.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] verticesSnapPiece = pieceToSnapTo.GetComponent<MeshFilter>().mesh.vertices;
        var meshTrianglesSelected = selectedPiece.GetComponent<MeshFilter>().mesh.triangles;
        var meshTrianglesSnapPiece = pieceToSnapTo.GetComponent<MeshFilter>().mesh.triangles;


        #region Check for overlap with piece we are trying to snap to

        var linesInSelectedPiece = GetLinesInPiece(verticesSelectedPiece);
        var linesInSnapPiece = GetLinesInPiece(verticesSnapPiece);

        if (AnyIntersectionsBetweenLines(verticesSelectedPiece, verticesSnapPiece, linesInSelectedPiece, linesInSnapPiece, precision: 0.1f, decimals: 3))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return true;
        }

        Debug.Log("container piece is snap piece");
        if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSelectedPiece, verticesSnapPiece))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return false;
        }

        Debug.Log("container piece is selected piece");
        if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSnapPiece, verticesSelectedPiece))
        {
            Debug.Log("Overlap with piece (first): " + pieceToSnapTo.name);
            return false;
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
            if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesSelectedPiece, verticesConnectedPiece))
            {
                Debug.Log("Overlap with connected piece: " + connectedPiece.name);
                return true;
            }

            Debug.Log("container piece is selected piece");
            if (CheckIfAnyPointIsContainedInTheOtherPiece(verticesConnectedPiece, verticesSelectedPiece))
            {
                Debug.Log("Overlap with connected piece: " + connectedPiece.name);
                return true;
            }
        }

        #endregion

        return false;
    }

    bool CheckIfAnyPointIsContainedInTheOtherPiece(Vector3[] pointPieceVertices, Vector3[] containerPieceVertices)
    {
        for (int i = 0; i < pointPieceVertices.Length; i++)
        {
            if (IsVertexContainedInOtherPiece(pointPieceVertices[i], containerPieceVertices))
            {
                return true;
            }
        }

        return false;
    }

    bool IsVertexContainedInOtherPiece(Vector3 vertexToCheck, Vector3[] containerPieceVertices)
    {

        for (int i = 0; i < containerPieceVertices.Length; i++)
        {
            var vertex1InLine = containerPieceVertices[i];
            var vertex2InLine = containerPieceVertices[GetWrappingIndex(i + 1, containerPieceVertices.Length)];

            var distance = CalculateDistanceFromPointToLine(vertexToCheck, (vertex1InLine, vertex2InLine));
            // If the distance is this small it's within the acceptable uncertainty of being on the border of the other piece
            if (distance < 0.05f)
            {
                // Debug.Log("distance smaller than 0.05: " + distance);
                return false;
            }
            // Debug.Log("Distance greater than 0.05: " + distance);
        }

        for (int i = 0; i < containerPieceVertices.Length; i++)
        {
            var vertex1InLine = containerPieceVertices[i];
            var vertex2InLine = containerPieceVertices[GetWrappingIndex(i + 1, containerPieceVertices.Length)];

            var projectionOntoLine = CalculateRightAngledProjectionFromPointToLine(vertexToCheck, vertex1InLine, vertex2InLine);
            var intersectionWithLine = vertex1InLine + projectionOntoLine;

            // Debug.Log("intersectionWithLine: " + intersectionWithLine);
            if (IsIntersectionPointInLineSegment(intersectionWithLine, vertex1InLine, vertex2InLine, 1, 0.1f))
            {
                // Debug.Log("IsIntersectionPointInLineSegment: true");

                if (IsPointOnInsideOfLine(vertexToCheck, intersectionWithLine, vertex1InLine, vertex2InLine))
                {
                    // Debug.Log("IsPointOnInsideOfLine: true");

                    var lineFromPointToIntersection = new List<(float, float)>() { CalculateConstantsForLineThroughTwoVertices(vertexToCheck, intersectionWithLine) };
                    var linesInPiece = GetLinesInPiece(containerPieceVertices);
                    var verticesIntersectionLine = new Vector3[] { vertexToCheck, intersectionWithLine };
                    // Debug.Log("lineFromPointToIntersection: " + lineFromPointToIntersection[0]);

                    if (!AnyIntersectionsBetweenLines(verticesIntersectionLine, containerPieceVertices, lineFromPointToIntersection, linesInPiece, -0.05f, 5))
                    { // not any intersections between line from point to intersection point and lines in container piece

                        if (!AnyIntersectionsBetweenLineAndVertices(verticesIntersectionLine, containerPieceVertices, lineFromPointToIntersection[0]))
                        { // not any intersections between line from point to intersection point and vertices in container piece
                            // Debug.Log("AnyIntersectionsBetweenPoints: false");
                            // Debug.Log("vertexToCheck: " + vertexToCheck);
                            // Debug.Log("index of containerpiece vertice: " + i);
                            // Debug.Log("vertex1InLine: " + vertex1InLine);
                            // Debug.Log("vertex2InLine: " + vertex2InLine);
                            // Debug.Log("projectionOntoLine: " + projectionOntoLine);
                            // Debug.Log("intersectionWithLine: " + intersectionWithLine);

                            Debug.Log("Point is in other piece: " + vertexToCheck);
                            return true;
                        }
                    }
                    else
                    {
                        // Debug.Log("AnyIntersectionsBetweenLines: false");
                    }
                }
                else
                {
                    // Debug.Log("IsPointOnInsideOfLine: false");
                }
            }
            else
            {
                // Debug.Log("IsIntersectionPointInLineSegment: false");
            }

        }

        return false;
    }



    bool AnyIntersectionsBetweenLines(Vector3[] verticesPiece1, Vector3[] verticesPiece2, List<(float, float)> linesPiece1, List<(float, float)> linesPiece2, float precision = 0.03f, int decimals = 3)
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
                        verticesPiece2[GetWrappingIndex(idx2 + 1, verticesPiece2.Length)],
                        precision,
                        decimals
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


    bool AnyIntersectionsBetweenLineAndVertices(Vector3[] verticesLineSegment1, Vector3[] verticesPiece, (float, float) line, float precision = 0.1f)
    {
        Debug.Log("line: " + line);

        // Check if any vertices from the piece intersects with the line segment.
        // If we encounter an intersection, we return true.
        var idx = 0;
        foreach (var vertex in verticesPiece)
        {
            Debug.Log("vertex: " + idx + "     " + verticesPiece[idx]);

            if (float.IsPositiveInfinity(line.Item1))
            { // vertical line
                if (RoundToXDecimals(verticesLineSegment1[0].x, 1) == RoundToXDecimals(vertex.x, 1))
                {
                    Debug.Log("Intersection between line and vertice. Vertical line.");
                    // Debug.Log("verticesLine: " + verticesLineSegment1[0]);
                    // Debug.Log("verticesLine: " + verticesLineSegment1[1]);
                    // Debug.Log("verticesPiece: " + verticesPiece[idx]);
                    // Debug.Log("verticesPiece: " + verticesPiece[GetWrappingIndex(idx + 1, verticesPiece.Length)]);
                    return true;
                }
            }

            var yValueForXOnLine = line.Item1 * vertex.x + line.Item2;
            var difference = Mathf.Abs(vertex.y - yValueForXOnLine);
            var intersection = difference < precision;

            // Debug.Log("yValueForXOnLine: " + yValueForXOnLine);
            // Debug.Log("difference: " + difference);
            // Debug.Log("intersection: " + intersection);


            if (intersection)
            {
                Debug.Log("Intersection between line and vertice.");
                // Debug.Log("verticesLine: " + verticesLineSegment1[0]);
                // Debug.Log("verticesLine: " + verticesLineSegment1[1]);
                // Debug.Log("verticesPiece: " + verticesPiece[idx]);
                // Debug.Log("verticesPiece: " + verticesPiece[GetWrappingIndex(idx + 1, verticesPiece.Length)]);
                return true;
            }

            idx++;
        }

        return false;
    }

    #endregion


    #region Snap to Corner

    // Todo: maybe move to other class?
    void FindCorners()
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


    public void SnapToCorner(GameObject selectedPiece)
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
        float rotation = GetComponentInParent<MagneticTouchCalculations>().CalculateRotation(vertexToSnapToCorner, cornerToSnapTo, vertexToMoveToBorder, otherCornerOfEdgeToSnapTo);

        // Angle is too big.
        // Either the rotation is not as intended or the player should try harder to place the pieces accurately
        if (rotation > 0.35 || rotation < -0.35)
        {
            Debug.Log("Angle is too big.");
            return;
        }

        var linesInPiece = GetLinesInPiece(pieceVertices);
        var linesInBorder = GetLinesInPiece(cornerVertices);

        // Removed this check because they always intersect in current implementation. It's not that important either.
        // if (AnyIntersectionsBetweenLines(pieceVertices, cornerVertices, linesInPiece, linesInBorder, precision: 0.075f))
        // {
        //     Debug.Log("Piece overlaps with border.");
        //     return;
        // }

        GetComponentInParent<MagneticTouchCalculations>().TranslateAndRotatePiece(selectedPiece, displacement, rotation, indexOfVertexToSnapToCorner);
    }

    // Finds vertex with shortest distance to a corner of the board and information about snapping to the corner 
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

    #endregion


    #region Calculations
    // Formula from https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line under section "Line defined by two points"
    private float CalculateDistanceFromPointToLine(Vector3 point, (Vector3, Vector3) edge)
    {
        var edgePoint1 = edge.Item1;
        var edgePoint2 = edge.Item2;

        var numerator = Mathf.Abs((edgePoint2.x - edgePoint1.x) * (edgePoint1.y - point.y) - (edgePoint1.x - point.x) * (edgePoint2.y - edgePoint1.y));
        var denominator = Mathf.Sqrt(Mathf.Pow((edgePoint2.x - edgePoint1.x), 2) + Mathf.Pow((edgePoint2.y - edgePoint1.y), 2));

        var distance = numerator / denominator;
        return distance;
    }

    // Wrapping index means that for example index 20 with an array of length 8 will return 4 (and it has thereby wrapped around the array twice)
    public int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }

    bool CheckIfLinesIntersectInLineSegment((float, float) line1, (float, float) line2, Vector3 point1Line1, Vector3 point2Line1, Vector3 point1Line2, Vector3 point2Line2, float precision = 0.01f, int decimals = 3)
    {
        if (float.IsPositiveInfinity(line1.Item1) && float.IsPositiveInfinity(line2.Item1))
        { // slope is infinity for both lines
            return false;
        }

        var intersectionX = 0f;
        var intersectionY = 0f;

        if (float.IsPositiveInfinity(line1.Item1))
        { // slope is infinity if x is constant for line1
            intersectionX = point1Line1.x;
            intersectionY = line2.Item1 * intersectionX + line2.Item2;
            // intersection x-value is between the two points in line segment of line 2.
            // intersection y-value is between the two points in line segment of line 1.
            if ((IsValueInInterval(intersectionX, point1Line2.x, point2Line2.x, precision, decimals)
                    || IsValueInInterval(intersectionX, point2Line2.x, point1Line2.x, precision, decimals))
                && (IsValueInInterval(intersectionY, point1Line1.y, point2Line1.y, precision, decimals)
                    || IsValueInInterval(intersectionY, point2Line1.y, point1Line1.y, precision, decimals)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (float.IsPositiveInfinity(line2.Item1))
        { // slope is infinity if x is constant for line2
            intersectionX = point1Line2.x;
            intersectionY = line1.Item1 * intersectionX + line1.Item2;
            // intersection x-value is between the two points in line segment of line 1.
            // intersection y-value is between the two points in line segment of line 2.
            if ((IsValueInInterval(intersectionX, point1Line1.x, point2Line1.x, precision, decimals)
                    || IsValueInInterval(intersectionX, point2Line1.x, point1Line1.x, precision, decimals))
                && (IsValueInInterval(intersectionY, point1Line2.y, point2Line2.y, precision, decimals)
                    || IsValueInInterval(intersectionY, point2Line2.y, point1Line2.y, precision, decimals)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (RoundToXDecimals(line1.Item1, 2) == RoundToXDecimals(line2.Item1, 2))
        { // if the slope is the same
            return false;
        }

        intersectionX = (line2.Item2 - line1.Item2) / (line1.Item1 - line2.Item1);

        // if the x-value of the intersection point is in both line segments, the two line segments intersect.
        if ((IsValueInInterval(intersectionX, point1Line1.x, point2Line1.x, precision, decimals)
                || IsValueInInterval(intersectionX, point2Line1.x, point1Line1.x, precision, decimals))
            && (IsValueInInterval(intersectionX, point1Line2.x, point2Line2.x, precision, decimals)
                || IsValueInInterval(intersectionX, point2Line2.x, point1Line2.x, precision, decimals)))
        {
            // Debug.Log("rounded decimal" + RoundToXDecimals(point1Line1.x, decimals));
            return true;
        }

        return false;
    }

    // Round a float value to a given number of decimals
    float RoundToXDecimals(float toRound, int decimals)
    {
        var multiplier = Mathf.Pow(10, decimals);
        return Mathf.Round(toRound * multiplier) / multiplier;
    }

    //Item1 is slope, Item2 is intersection with Y-axis
    (float, float) CalculateConstantsForLineThroughTwoVertices(Vector3 vertex1, Vector3 vertex2)
    {
        if (RoundToXDecimals(vertex2.x, 3) == RoundToXDecimals(vertex1.x, 3))
        {
            return (float.PositiveInfinity, float.PositiveInfinity);
        }
        if (RoundToXDecimals(vertex2.y, 3) == RoundToXDecimals(vertex1.y, 3))
        {
            return (0, vertex1.y);
        }

        float lineSlope = (vertex2.y - vertex1.y) / (vertex2.x - vertex1.x);
        float lineIntersectionWithYAxis = vertex1.y - lineSlope * vertex1.x;

        (float, float) lineConstants = (lineSlope, lineIntersectionWithYAxis);

        return lineConstants;
    }

    bool IsIntersectionPointInLineSegment(Vector3 intersectionPoint, Vector3 point1InLine, Vector3 point2InLine, int decimals, float precision)
    {
        // Debug.Log("intersectionPoint.x: " + (RoundToXDecimals(intersectionPoint.x, decimals)));
        // Debug.Log("point1line.x: " + (RoundToXDecimals(point1InLine.x, decimals)));
        // Debug.Log("point2line.x: " + (RoundToXDecimals(point2InLine.x, decimals)));
        var result = IsValueInInterval(intersectionPoint.x, point1InLine.x, point2InLine.x, precision, decimals) || IsValueInInterval(intersectionPoint.x, point2InLine.x, point1InLine.x, precision, decimals);
        return result;
    }

    bool IsValueInInterval(float value, float intervalStart, float intervalEnd, float precision = 0.1f, int decimals = 3)
    {
        return RoundToXDecimals(intervalStart, decimals) < value - precision && value + precision < RoundToXDecimals(intervalEnd, decimals);
    }

    Vector3 CalculateRightAngledProjectionFromPointToLine(Vector3 vertexToCheck, Vector3 vertex1InLine, Vector3 vertex2InLine)
    {
        var line = vertex2InLine - vertex1InLine;
        var translatedVertice = vertexToCheck - vertex1InLine;
        var projection = (Vector3.Dot(line, translatedVertice) / Vector3.Dot(line, line)) * line;
        return projection;
    }

    bool IsPointOnInsideOfLine(Vector3 vertexToCheck, Vector3 intersectionPoint, Vector3 vertex1InLine, Vector3 vertex2InLine)
    {
        // vertexToCheck.y has to be larger than f(vertexToCheck.x) when the vector from vertex1InLine to vertex2InLine has positive x.
        // vertexToCheck.y has to be smaller than f(vertexToCheck.x) when the vector from vertex1InLine til vertex2InLine has negative x.

        var diffOfYValuesForVertexAndPointOnLine = vertexToCheck.y - intersectionPoint.y;

        var diffOfXValuesForPointsOnLine = vertex2InLine.x - vertex1InLine.x;
        var diffOfYValuesForPointsOnLine = vertex2InLine.y - vertex1InLine.y;

        if (RoundToXDecimals(diffOfYValuesForVertexAndPointOnLine, 2) == 0.00f)
        { // point is on the line
            return false;
        }

        if (diffOfXValuesForPointsOnLine > -0.05 && diffOfXValuesForPointsOnLine < 0.05
            && (diffOfYValuesForPointsOnLine > 0.05 || diffOfYValuesForPointsOnLine < -0.05))
        { // vertical line

            if (diffOfYValuesForPointsOnLine > 0 && vertexToCheck.x - vertex1InLine.x < 0)
            { // VertexToCheck is to the left of the line and therefore on the inside
                return true;
            }
            if (diffOfYValuesForPointsOnLine < 0 && vertexToCheck.x - vertex1InLine.x > 0)
            { // VertexToCheck is to the right of the line and therefore on the outside
                return false;
            }
        }


        bool yGreaterThanYOnLine = diffOfYValuesForVertexAndPointOnLine > 0;
        bool positiveChangeInX = diffOfXValuesForPointsOnLine > 0;

        if (positiveChangeInX && yGreaterThanYOnLine || !positiveChangeInX && !yGreaterThanYOnLine)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    List<(float, float)> GetLinesInPiece(Vector3[] verticesInPiece)
    {
        var linesInPiece = new List<(float, float)>();

        for (int i = 0; i < verticesInPiece.Length; i++)
        {
            var vertex_i = verticesInPiece[i];
            var vertex_i_plus_1 = verticesInPiece[GetWrappingIndex(i + 1, verticesInPiece.Length)];
            linesInPiece.Add(CalculateConstantsForLineThroughTwoVertices(vertex_i, vertex_i_plus_1));
        }

        return linesInPiece;
    }

    #endregion

}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTouchCalculations
{
    public static float margin = 0.175f;

    /* Calculate displacement vector that the selected piece has to be translated
     with in order to fit together with the other piece. The translation is based
     on the two vertices closest two each other (one from each piece)  */
    public static Vector3 CalculateDisplacementForSnapToPiece(GameObject selectedPiece, GameObject magnetPiece, int indexForVertexOfSelected, int indexForVertexOfMagnet)
    {
        Vector3 selectedVertex = selectedPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfSelected];
        Vector3 magnetVertex = magnetPiece.GetComponent<MeshFilter>().mesh.vertices[indexForVertexOfMagnet];
        // Debug.Log(selectedVertex);
        // Debug.Log(magnetVertex);
        return selectedVertex - magnetVertex;
    }

    public static float CalculateRotation(Vector3 primaryVertexInSelected, Vector3 primaryVertexToSnapTo, Vector3 previousVertexInSelected, Vector3 previousVertexToSnapTo)
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
            // Debug.Log("Degrees to rotate: " + radiansToRotate * Mathf.Rad2Deg);
        }

        // We use the smallest angle possible to reach the same rotation (because we have restrictions on maximum angle size elsewhere)
        if (Mathf.Abs(radiansToRotate) > Mathf.PI)
        {
            radiansToRotate = (Mathf.Sign(radiansToRotate) * (-1)) * (Mathf.Abs(radiansToRotate) - Mathf.PI * 2);
        }
        Debug.Log("Degrees to rotate: " + radiansToRotate * Mathf.Rad2Deg);


        return radiansToRotate;
    }
    public static void TranslateAndRotatePiece(GameObject selectedPiece, Vector3 displacement, float rotationAngle, int indexOfVertexToRotateAbout)
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
    public static Vector3[] CentralizeVertices(Vector3 centroid, Vector3[] originalVertices)
    {
        for (int index = 0; index < originalVertices.Length; index++)
        {
            originalVertices[index].x = originalVertices[index].x - centroid.x;
            originalVertices[index].y = originalVertices[index].y - centroid.y;
        }
        return originalVertices;
    }

    // Todo: already exists in Rotation.cs (not exactly like this though)
    public static Vector3[] RestorePositionOfVertices(Vector3 centroid, Vector3[] rotatedVertices)
    {
        for (int index = 0; index < rotatedVertices.Length; index++)
        {
            rotatedVertices[index].x = rotatedVertices[index].x + centroid.x;
            rotatedVertices[index].y = rotatedVertices[index].y + centroid.y;
        }
        return rotatedVertices;
    }

    public static void RestoreVertices(GameObject piece, Vector3[] originalVertices)
    {
        Mesh meshForPiece = piece.GetComponent<MeshFilter>().mesh;
        LineRenderer lineRenderer = piece.GetComponent<LineRenderer>();
        Vector3[] translatedVertices = new Vector3[meshForPiece.vertices.Length];

        meshForPiece.SetVertices(originalVertices);
        lineRenderer.SetPositions(originalVertices);
        piece.GetComponent<MeshCollider>().sharedMesh = meshForPiece;
    }

    public static ((float, float), (float, float)) ConstructBoundBox(GameObject piece)
    {
        (float, float) minMaxX = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumXCoordinate();
        (float, float) minMaxY = piece.GetComponent<PieceInfo>().GetMaximumAndMinimumYCoordinate();
        return ((minMaxX.Item1 - margin, minMaxX.Item2 + margin), (minMaxY.Item1 - margin, minMaxY.Item2 + margin));
    }

    public static Vector3 CalculateRightAngledProjectionFromPointToLine(Vector3 vertexToCheck, Vector3 vertex1InLine, Vector3 vertex2InLine)
    {
        var line = vertex2InLine - vertex1InLine;
        var translatedVertice = vertexToCheck - vertex1InLine;
        var projection = (Vector3.Dot(line, translatedVertice) / Vector3.Dot(line, line)) * line;
        return projection;
    }

    //Item1 is slope, Item2 is intersection with Y-axis
    public static (float, float) CalculateConstantsForLineThroughTwoVertices(Vector3 vertex1, Vector3 vertex2)
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

    public static bool IsIntersectionPointInLineSegment(Vector3 intersectionPoint, Vector3 point1InLine, Vector3 point2InLine, int decimals, float precision)
    {
        // Debug.Log("intersectionPoint.x: " + (RoundToXDecimals(intersectionPoint.x, decimals)));
        // Debug.Log("point1line.x: " + (RoundToXDecimals(point1InLine.x, decimals)));
        // Debug.Log("point2line.x: " + (RoundToXDecimals(point2InLine.x, decimals)));
        var result = IsValueInInterval(intersectionPoint.x, point1InLine.x, point2InLine.x, precision, decimals) || IsValueInInterval(intersectionPoint.x, point2InLine.x, point1InLine.x, precision, decimals);
        return result;
    }

    public static bool IsPointOnInsideOfLine(Vector3 vertexToCheck, Vector3 intersectionPoint, Vector3 vertex1InLine, Vector3 vertex2InLine)
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

    // Formula from https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line under section "Line defined by two points"
    public static float CalculateDistanceFromPointToLine(Vector3 point, (Vector3, Vector3) edge)
    {
        var edgePoint1 = edge.Item1;
        var edgePoint2 = edge.Item2;

        var numerator = Mathf.Abs((edgePoint2.x - edgePoint1.x) * (edgePoint1.y - point.y) - (edgePoint1.x - point.x) * (edgePoint2.y - edgePoint1.y));
        var denominator = Mathf.Sqrt(Mathf.Pow((edgePoint2.x - edgePoint1.x), 2) + Mathf.Pow((edgePoint2.y - edgePoint1.y), 2));

        var distance = numerator / denominator;
        return distance;
    }

    public static bool CheckIfLinesIntersectInLineSegment((float, float) line1, (float, float) line2,
                        Vector3 point1Line1, Vector3 point2Line1, Vector3 point1Line2, Vector3 point2Line2, float precision = 0.01f, int decimals = 3)
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

    // Wrapping index means that for example index 20 with an array of length 8 will return 4 (and it has thereby wrapped around the array twice)
    public static int GetWrappingIndex(int index, int lengthOfArray)
    {
        return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
    }

    // Round a float value to a given number of decimals
    public static float RoundToXDecimals(float toRound, int decimals)
    {
        var multiplier = Mathf.Pow(10, decimals);
        return Mathf.Round(toRound * multiplier) / multiplier;
    }

    public static bool IsValueInInterval(float value, float intervalStart, float intervalEnd, float precision = 0.1f, int decimals = 3)
    {
        return RoundToXDecimals(intervalStart, decimals) < value - precision && value + precision < RoundToXDecimals(intervalEnd, decimals);
    }

    public static SnapInformation CalculateSnapInformation(Vector3[] piece1, Vector3[] piece2)
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Below code is based on https://gamedev.stackexchange.com/questions/167317/scale-camera-to-fit-screen-size-unity
// [ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FitCameraToPuzzle : MonoBehaviour
{

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth;
    public float sceneHeight;

    Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
        var jsonPuzzle = GetComponentInParent<MeshFromJsonGenerator>().Puzzle;
        var shape = jsonPuzzle.puzzle.form;

        var lowestXValueOfPieces = 0f;
        var highestXValueOfPieces = 0f;
        var lowestYValueOfPieces = 0f;
        var highestYValueOfPieces = 0f;

        // Find lowest and highest x- and y-value among all pieces.
        foreach (var piece in jsonPuzzle.pieces)
        {
            foreach (var corner in piece.corners)
            {
                if (corner.coord.x < lowestXValueOfPieces) lowestXValueOfPieces = corner.coord.x;
                if (corner.coord.x > highestXValueOfPieces) highestXValueOfPieces = corner.coord.x;
                if (corner.coord.y < lowestYValueOfPieces) lowestYValueOfPieces = corner.coord.y;
                if (corner.coord.y > highestYValueOfPieces) highestYValueOfPieces = corner.coord.y;
            }
        }

        // Area that pieces are distributed over initially.
        var widthOfPieceDistribution = highestXValueOfPieces - lowestXValueOfPieces;
        var heightOfPieceDistribution = highestYValueOfPieces - lowestYValueOfPieces;

        // Board should be centered in the center of the distribution of pieces
        var newCenterOfBoardX = lowestXValueOfPieces + widthOfPieceDistribution / 2;
        var newCenterOfBoardY = lowestYValueOfPieces + heightOfPieceDistribution / 2;


        var widthOfBoard = shape[2].coord.x - shape[0].coord.x;
        var heightOfBoard = shape[2].coord.y - shape[0].coord.y;

        var distanceToMoveBoardX = newCenterOfBoardX - shape[0].coord.x - widthOfBoard / 2;
        var distanceToMoveBoardY = newCenterOfBoardY - shape[0].coord.y - heightOfBoard / 2;

        var idx = 0;
        foreach (var point in shape)
        {
            shape[idx].coord.x = point.coord.x + distanceToMoveBoardX;
            shape[idx].coord.y = point.coord.y + distanceToMoveBoardY;
            idx++;
        }

        jsonPuzzle.puzzle.form = shape;

        var lowestXValueTotal = lowestXValueOfPieces < shape[0].coord.x ? lowestXValueOfPieces : shape[0].coord.x;
        var highestXValueTotal = highestXValueOfPieces > shape[2].coord.x ? highestXValueOfPieces : shape[2].coord.x;
        var lowestYValueTotal = lowestYValueOfPieces < shape[0].coord.y ? lowestYValueOfPieces : shape[0].coord.y;
        var highestYValueTotal = highestYValueOfPieces > shape[2].coord.y ? highestYValueOfPieces : shape[2].coord.y;

        sceneHeight = (highestYValueTotal - lowestYValueTotal) * 1.3f;
        // Debug.Log("height: " + sceneHeight);
        sceneWidth = (highestXValueTotal - lowestXValueTotal) * 1.3f;
        // Debug.Log("width: " + sceneWidth);

        float unitsPerPixelWidth = sceneWidth / Screen.width;
        float unitsPerPixelHeight = sceneHeight / Screen.height;

        if (unitsPerPixelWidth > unitsPerPixelHeight)
        {
            float desiredHalfHeight = 0.5f * unitsPerPixelWidth * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
            // Debug.Log("width > height");
        }
        else
        {
            float desiredHalfHeight = 0.5f * unitsPerPixelHeight * Screen.height;
            _camera.orthographicSize = desiredHalfHeight;
            // Debug.Log("height > width");
        }

        var newCameraPosition = new Vector3(newCenterOfBoardX, newCenterOfBoardY, -10);
        _camera.transform.position =  newCameraPosition;
    }
}
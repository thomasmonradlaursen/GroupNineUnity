using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;
using DTriangle;

public class CreateJSONFromRandom : MonoBehaviour
{
    public JSONPuzzle randomPuzzle = new JSONPuzzle();
    public List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();
    public JSONPuzzle CreatePuzzle()
    {
        triangles = GetComponent<DelaunayTriangulation>().RunDelaunayTriangulation();
        SetupName(randomPuzzle, "RandomlyGeneratedPuzzle");
        SetupForm(randomPuzzle);
        SetupPieces(randomPuzzle);
        return randomPuzzle;
    }
    void SetupForm(JSONPuzzle puzzle)
    {
        Debug.Log("Entered SetupForm");
        List<Vector3> corners = GetComponent<RandomPieceGenerator>().corners;
        Debug.Log("This is corners: " + corners.Count);
        randomPuzzle.puzzle.form = new Form[corners.Count];
        for (int index = 0; index < corners.Count; index++)
        {
            Debug.Log(index);
            randomPuzzle.puzzle.form[index] = new Form();
            randomPuzzle.puzzle.form[index].coord = new Coord();
            randomPuzzle.puzzle.form[index].coord.x = corners[index].x;
            randomPuzzle.puzzle.form[index].coord.y = corners[index].y;
        }
    }
    void SetupName(JSONPuzzle puzzle, string name)
    {
        Debug.Log("Entered SetupName");
        randomPuzzle.name = name;
    }
    void SetupPieces(JSONPuzzle puzzle)
    {
        Debug.Log("Entered SetupPieces");
        randomPuzzle.nPieces = triangles.Count;
        randomPuzzle.pieces = new Piece[triangles.Count];
        Debug.Log("Number of pieces: " + randomPuzzle.pieces.Length);
        for (int pieceIndex = 0; pieceIndex < triangles.Count; pieceIndex++)
        {
            randomPuzzle.pieces[pieceIndex] = new Piece();
            randomPuzzle.pieces[pieceIndex].corners = new Corner[triangles[pieceIndex].vertices.Length];
            randomPuzzle.pieces[pieceIndex].piece = pieceIndex;
            for (int cornerIndex = 0; cornerIndex < triangles[pieceIndex].vertices.Length; cornerIndex++)
            {
                randomPuzzle.pieces[pieceIndex].corners[cornerIndex] = new Corner();
                randomPuzzle.pieces[pieceIndex].corners[cornerIndex].coord = new Coord();
                randomPuzzle.pieces[pieceIndex].corners[cornerIndex].coord.x = triangles[pieceIndex].vertices[cornerIndex].x;
                randomPuzzle.pieces[pieceIndex].corners[cornerIndex].coord.y = triangles[pieceIndex].vertices[cornerIndex].y;
            }
        }
    }
}

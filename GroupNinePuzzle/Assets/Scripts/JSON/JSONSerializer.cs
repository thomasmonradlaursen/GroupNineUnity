using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;
using DTriangle;

public class JSONSerializer : MonoBehaviour
{
    public JSONPuzzle CreatePuzzle()
    {
        JSONPuzzle puzzle = new JSONPuzzle();
        SetName(puzzle, GetComponent<DivisionModel>().nameOfPuzzle);
        SetForm(puzzle);
        SetPieces(puzzle, GetComponent<DivisionModel>().triangles);
        return puzzle;
    }
    void SetForm(JSONPuzzle puzzle)
    {
        List<Vector3> corners = GetComponent<DivisionModel>().corners;
        puzzle.puzzle = new Puzzle();
        puzzle.puzzle.form = new Form[corners.Count];
        for (int index = 0; index < corners.Count; index++)
        {
            puzzle.puzzle.form[index] = new Form();
            puzzle.puzzle.form[index].coord = new Coord();
            puzzle.puzzle.form[index].coord.x = corners[index].x;
            puzzle.puzzle.form[index].coord.y = corners[index].y;
        }
    }
    void SetName(JSONPuzzle puzzle, string name)
    {
        puzzle.name = name;
    }
    void SetPieces(JSONPuzzle puzzle, List<DelaunayTriangle> triangles)
    {
        puzzle.nPieces = triangles.Count;
        puzzle.pieces = new Piece[triangles.Count];
        for (int pieceIndex = 0; pieceIndex < triangles.Count; pieceIndex++)
        {
            puzzle.pieces[pieceIndex] = new Piece();
            puzzle.pieces[pieceIndex].corners = new Corner[triangles[pieceIndex].vertices.Length];
            puzzle.pieces[pieceIndex].piece = pieceIndex;
            for (int cornerIndex = 0; cornerIndex < triangles[pieceIndex].vertices.Length; cornerIndex++)
            {
                puzzle.pieces[pieceIndex].corners[cornerIndex] = new Corner();
                puzzle.pieces[pieceIndex].corners[cornerIndex].coord = new Coord();
                puzzle.pieces[pieceIndex].corners[cornerIndex].coord.x = triangles[pieceIndex].vertices[cornerIndex].x;
                puzzle.pieces[pieceIndex].corners[cornerIndex].coord.y = triangles[pieceIndex].vertices[cornerIndex].y;
            }
        }
    }
}

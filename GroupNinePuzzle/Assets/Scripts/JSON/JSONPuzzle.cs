using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

namespace JSONPuzzleTypes
{
    [Serializable]
    public class JSONPuzzle
    {
        public Puzzle puzzle;
        public string name;
        [FormerlySerializedAs("no. of pieces")]
        public int nPieces;
        public Piece[] pieces;
    }

    [Serializable]
    public class Puzzle
    {
        public Form[] form;
    }

    [Serializable]
    public class Form
    {
        public Coord coord;
    }
    [Serializable]
    public class Piece
    {
        public int piece;
        public Corner[] corners;
    }

    [Serializable]
    public class Coord
    {
        public float x;
        public float y;
    }
    [Serializable]
    public class Corner
    {
        public Coord coord;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 

[Serializable]
public class JSONPuzzle {
    public Puzzle puzzle;
    public string name;
    public int nPieces;
    public Piece[] pieces;
}

[Serializable]
public class Puzzle {
    public Form[] form;
}

[Serializable]
public class Form {
    public Coord coord;
}
[Serializable]
public class Piece{
    public int piece;
    public Corner[] corners;
}

[Serializable]
public class Coord{
        public double x;
        public double y;
    }
[Serializable]
public class Corner{
    public Coord coord;
}

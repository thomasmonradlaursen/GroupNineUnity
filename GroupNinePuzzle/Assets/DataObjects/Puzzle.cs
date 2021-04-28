using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
namespace Tester{

[Serializable]
public class Puzzle{
    public PuzzleData[] result;
}

public class PuzzleData {
    public string name;
    public int nPieces;
    public Form[] form;  //List<Corner>
    public Piece[] pieces; //List<Piece>
}

[Serializable]
public class Form{
    public Corner[] form;

}
[Serializable]
public class Piece{
    public string piece;
    public Corner[] corners;
}

[Serializable]
public class Coord{
        public double x;
        public double y;
    }
[Serializable]
public class Corner{
    public Coord Coord;
}
}
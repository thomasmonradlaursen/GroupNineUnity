using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Puzzle{
    private string name {get; set;}
    private int nPieces {get; set;}
    private List<Corner> form {get; set;}
    private List<Piece> pieces {get; set;}
}
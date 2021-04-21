[Serializable]
public class Puzzle
{
    public string name {get; set;}
    public Corners form {get; set;}
    public int nPieces {get; set;}
    public List<Piece> pieces {get; set;}
}
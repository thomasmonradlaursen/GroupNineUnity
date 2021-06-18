using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceController : MonoBehaviour
{
    public void CreatePieces()
    {
        // this is fixed also
        if (GetComponentInParent<PuzzleModel>().generateRandom)
        {
            GetComponentInChildren<MeshGenerator>().MeshesFromRandom();
        }
        else
        {
            GetComponentInChildren<MeshGenerator>().GenerateMeshes(GetComponentInParent<PuzzleModel>().puzzle);
        }
        List<Mesh> meshes = GetComponentInChildren<MeshModel>().meshes;

        GetComponentInParent<PuzzleModel>().connectedPieces = new Dictionary<string, List<string>>();

        int idx = 0;
        foreach (Mesh mesh in meshes)
        {
            var newPiece = new GameObject("Piece " + GetComponentInParent<PuzzleModel>().puzzle.pieces[idx].piece);
            newPiece.AddComponent<MeshFilter>();
            newPiece.GetComponent<MeshFilter>().mesh = mesh;
            newPiece.AddComponent<MeshRenderer>();
            newPiece.AddComponent<MeshCollider>();
            newPiece.AddComponent<Translation>();
            newPiece.AddComponent<Rotation>();
            newPiece.AddComponent<PieceInfo>();
            newPiece.GetComponent<PieceInfo>().CalculateInformation();
            newPiece.GetComponent<PieceInfo>().vertices = mesh.vertices;
            newPiece.AddComponent<MagneticTouchAlgorithm>();
<<<<<<< Updated upstream
            PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices);
=======
            if(GetComponentInParent<PuzzleModel>().puzzle.puzzle.form != null)
            {
                PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices, GetComponentInParent<PuzzleModel>().puzzle.puzzle.form[2].coord.x, GetComponentInParent<PuzzleModel>().puzzle.puzzle.form[2].coord.y);
            }
            else 
            {
                PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices, 3.0f, 3.0f);
            }
>>>>>>> Stashed changes
            newPiece.transform.parent = this.transform;

            var renderer = newPiece.GetComponent<MeshRenderer>();
            var materials = renderer.materials;
            materials = new Material[]
            {
                new Material(Shader.Find("Sprites/Default"))
                };
            materials[0].color = Color.blue;
            renderer.materials = materials;

            GetComponentInParent<PuzzleModel>().pieces.Add(newPiece);
            GetComponentInParent<PuzzleModel>().connectedPieces.Add(newPiece.name, new List<string>());

            idx++;
        }
    }
}

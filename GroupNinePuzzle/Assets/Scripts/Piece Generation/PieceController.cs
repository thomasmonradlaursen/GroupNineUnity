using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceController : MonoBehaviour
{
    public void CreatePieces()
    {
        // Select mode for mesh generation
        if (GetComponentInParent<PuzzleModel>().generateRandom)
        {
            GetComponentInChildren<MeshGenerator>().MeshesFromRandom();
        }
        else
        {
            GetComponentInChildren<MeshGenerator>().GenerateMeshes(GetComponentInParent<PuzzleModel>().puzzle);
        }

        GetComponentInParent<PuzzleModel>().connectedPieces = new Dictionary<string, List<string>>();

        int idx = 0;
        foreach (Mesh mesh in GetComponentInChildren<MeshModel>().meshes)
        {
            // Create a new piece
            GameObject newPiece = new GameObject("Piece " + GetComponentInParent<PuzzleModel>().puzzle.pieces[idx].piece);

            // Add components
            newPiece.AddComponent<MeshFilter>();
            newPiece.GetComponent<MeshFilter>().mesh = mesh;
            newPiece.AddComponent<MeshRenderer>();
            newPiece.AddComponent<MeshCollider>();
            newPiece.AddComponent<Translation>();
            newPiece.AddComponent<Rotation>();
            newPiece.AddComponent<PieceInfo>();

            // Set relevant information
            newPiece.GetComponent<PieceInfo>().CalculateInformation();
            newPiece.GetComponent<PieceInfo>().vertices = mesh.vertices;

            // Draw outlines
            if(GetComponentInParent<PuzzleModel>().puzzle.puzzle.form != null)
            {
                PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices, GetComponentInParent<PuzzleModel>().puzzle.puzzle.form[2].coord.x, GetComponentInParent<PuzzleModel>().puzzle.puzzle.form[2].coord.y);
            }
            else 
            {
                PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices, 3.0f, 3.0f);
            }

            // Setup materials for the piece
            var renderer = newPiece.GetComponent<MeshRenderer>();
            var materials = renderer.materials;
            materials = new Material[] { new Material(Shader.Find("Sprites/Default")) };
            materials[0].color = Color.blue;
            renderer.materials = materials;

            // Maintain the structure
            newPiece.transform.parent = this.transform;
            GetComponentInParent<PuzzleModel>().pieces.Add(newPiece);
            GetComponentInParent<PuzzleModel>().connectedPieces.Add(newPiece.name, new List<string>());

            idx++;
        }
    }
}

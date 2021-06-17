using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceController : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<PuzzleModel>().generateRandom)
        {
            GetComponent<PuzzleModel>().puzzle = GetComponentInChildren<DivisionModel>().puzzle;
        }
        else
        {
            JSONDeserializer deserializer = GetComponent<JSONDeserializer>();
            GetComponent<PuzzleModel>().puzzle = deserializer.DeserializerPuzzleFromJSON(deserializer.locationOfFile + deserializer.fileName);
        }
        CreatePieces();
    }
    void CreatePieces()
    {
        if (GetComponent<PuzzleModel>().generateRandom)
        {
            GetComponent<MeshGenerator>().MeshesFromRandom();
        }
        else
        {
            GetComponent<MeshGenerator>().GenerateMeshes(GetComponent<PuzzleModel>().puzzle);
        }
        List<Mesh> meshes = GetComponent<MeshModel>().meshes;

        GetComponent<PuzzleModel>().connectedPieces = new Dictionary<string, List<string>>();

        int idx = 0;
        foreach (Mesh mesh in meshes)
        {
            var newPiece = new GameObject("Piece " + GetComponent<PuzzleModel>().puzzle.pieces[idx].piece);
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
            PieceOutlineGenerator.GenerateOutline(newPiece, mesh.vertices);
            newPiece.transform.parent = this.transform;

            var renderer = newPiece.GetComponent<MeshRenderer>();
            var materials = renderer.materials;
            materials = new Material[]
            {
                new Material(Shader.Find("Sprites/Default"))
                };
            materials[0].color = Color.blue;
            renderer.materials = materials;

            GetComponent<PuzzleModel>().pieces.Add(newPiece);
            GetComponent<PuzzleModel>().connectedPieces.Add(newPiece.name, new List<string>());

            idx++;
        }
    }
    public void EnableRandomlyGeneratedPuzzled()
    {
        GetComponent<PuzzleModel>().generateRandom = true;
    }
    public void DisableRandomlyGeneratedPuzzled()
    {
        GetComponent<PuzzleModel>().generateRandom = false;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceController : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<PieceModel>().generateRandom)
        {
            GetComponent<PieceModel>().puzzle = GetComponentInChildren<DivisionModel>().puzzle;
        }
        else
        {
            JSONDeserializer deserializer = GetComponent<JSONDeserializer>();
            GetComponent<PieceModel>().puzzle = deserializer.DeserializerPuzzleFromJSON(deserializer.locationOfFile + deserializer.fileName);
        }
        CreatePieces();
    }
    void CreatePieces()
    {
        if (GetComponent<PieceModel>().generateRandom)
        {
            GetComponent<MeshGenerator>().MeshesFromRandom();
        }
        else
        {
            GetComponent<MeshGenerator>().GenerateMeshes(GetComponent<PieceModel>().puzzle);
        }
        List<Mesh> meshes = GetComponent<PieceModel>().meshes;

        GetComponent<PieceModel>().connectedPieces = new Dictionary<string, List<string>>();

        int idx = 0;
        foreach (Mesh mesh in meshes)
        {
            var newPiece = new GameObject("Piece " + GetComponent<PieceModel>().puzzle.pieces[idx].piece);
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

            GetComponent<PieceModel>().pieces.Add(newPiece);
            GetComponent<PieceModel>().connectedPieces.Add(newPiece.name, new List<string>());

            idx++;
        }
    }
    public void EnableRandomlyGeneratedPuzzled()
    {
        GetComponent<PieceModel>().generateRandom = true;
    }
    public void DisableRandomlyGeneratedPuzzled()
    {
        GetComponent<PieceModel>().generateRandom = false;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

public class PieceController : MonoBehaviour
{
    public List<GameObject> pieces;
    public JSONPuzzle puzzle;
    void Start()
    {
        JSONDeserializer deserializer = GetComponent<JSONDeserializer>();
        puzzle = deserializer.DeserializerPuzzleFromJSON(deserializer.locationOfFile + deserializer.fileName);
        Debug.Log("PieceGenerator: Start()");
        CreatePieces();
        
    }
    void CreatePieces()
    {
        List<Mesh> meshes = GetComponent<MeshFromJsonGenerator>().meshArray;

        int idx = 0;
        foreach (Mesh mesh in meshes)
        {
            var newPiece = new GameObject("Piece " + puzzle.pieces[idx].piece);
            newPiece.AddComponent<MeshFilter>();
            newPiece.GetComponent<MeshFilter>().mesh = mesh;
            newPiece.AddComponent<MeshRenderer>();
            newPiece.AddComponent<MeshCollider>();
            newPiece.AddComponent<Translation>();
            newPiece.AddComponent<Rotation>();
            newPiece.AddComponent<PieceInfo>();
            newPiece.GetComponent<PieceInfo>().CalculateInformation();
            newPiece.AddComponent<SnapIntoPlace>();
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

            pieces.Add(newPiece);

            idx++;
        }
    }
}
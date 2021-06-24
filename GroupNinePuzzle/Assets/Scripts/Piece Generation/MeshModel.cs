using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSONPuzzleTypes;

// Author: Thomas Monrad Laursen

public class MeshModel : MonoBehaviour
{
    public List<Mesh> meshes = new List<Mesh>();
    public int[] triangles;
    public Vector2[] newUV;
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DivisionTriangles
{
    [Serializable]
    public class DivisionTriangle
    {
        public int id;
        public Vector3[] vertices;
        public Edge[] edges;
        public Vector3 circumcenter;
        public float circumradius;
    }

    [Serializable]
    public class Edge
    {
        public (Vector3, Vector3) innerHalf;
        public (Vector3, Vector3) outerHalf;
    }
}

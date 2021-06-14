using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DTriangle
{
    [Serializable]
    public class DelaunayTriangle
    {
        public Vector3[] vertices;
        public Edge[] edges;
        public Vector3 circumcenter;
        public float circumradius;
    }

    [Serializable]
    public class Edge
    {
        public (Vector3, Vector3) coordinates;
        public float length;
    }
}

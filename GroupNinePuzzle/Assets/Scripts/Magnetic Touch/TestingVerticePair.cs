// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TestingVerticePair
// {
//     public static int GetWrappingIndex(int index, int lengthOfArray)
//     {
//         return ((index % lengthOfArray) + lengthOfArray) % lengthOfArray;
//     }

//     public static void CheckDistancesBetweenVerticesOfTwoPieces(Vector3[] piece1, Vector3[] piece2)
//     {

//         // item1 is distance from vertice i in piece1 to vertice k in piece2.
//         // item2 is distance from vertice i+1 in piece1 to vertice k+1 in piece2.
//         (float, float) smallestDistance = (100000f, 100000f);

//         // Previous vertix in piece1 paired with previous vertex in piece2
//         // This is then paired with the current vertex in piece1 paired with the current vertex in piece2
//         ((Vector3, Vector3), (Vector3, Vector3)) verticesInShortestDistance;

//         for (int i = 0; i < piece1.Length; i++)
//         {
//             var vertex_piece1 = piece1[i];
//             var previousVertex_piece1 = piece1[GetWrappingIndex(i - 1, piece1.Length)];
//             var nextVertex_piece1 = piece1[GetWrappingIndex(i + 1, piece1.Length)];
//             for (int k = 0; k < piece2.Length; k++)
//             {
//                 var vertex_piece2 = piece2[k];
//                 var previousVertex_piece2 = piece2[GetWrappingIndex(k - 1, piece2.Length)];
//                 var nextVertex_piece2 = piece2[GetWrappingIndex(k + 1, piece2.Length)];

//                 var distBetweenVertices = DistanceBetweenVertices(vertex_piece1, vertex_piece2);
//                 if (distBetweenVertices < smallestDistance.Item1)
//                 {
//                     var distPreviousVertices = DistanceBetweenVertices(previousVertex_piece1, previousVertex_piece2);
//                     var distNextVertices = DistanceBetweenVertices(previousVertex_piece1, previousVertex_piece2);

//                     if (distPreviousVertices <= distNextVertices)
//                     {
//                         smallestDistance = (distBetweenVertices, distPreviousVertices);
//                         verticesInShortestDistance = ((previousVertex_piece1, previousVertex_piece2), (vertex_piece1, vertex_piece2));
//                     }
//                     else
//                     {
//                         smallestDistance = (distBetweenVertices, distNextVertices);
//                         verticesInShortestDistance = ((vertex_piece1, vertex_piece2), (nextVertex_piece1, nextVertex_piece2));
//                     }
//                 }
//             }
//         }

//         return (smallestDistance, verticesInShortestDistance);
//     }

//     public static float DistanceBetweenVertices(Vector3 vertex1, Vector3 vertex2)
//     {
//         var xDist = vertex2.x - vertex1.x;
//         var yDist = vertex2.y - vertex1.y;
//         var distance = Mathf.Sqrt(Mathf.Pow(xDist, 2) + Mathf.Pow(yDist, 2));
//         return distance;
//     }

// }

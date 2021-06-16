using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModel : MonoBehaviour
{
    public int numberOfClicks = 0;

    public void ChooseMode()
    {
        Debug.Log("Mode has been set to random");
        GetComponentInParent<PieceController>().puzzleFromRandom = true;
    }
}

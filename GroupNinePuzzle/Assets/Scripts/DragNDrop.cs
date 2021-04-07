using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragNDrop : MonoBehaviour
{
    
    bool selected;

    private void OnMouseDown() {
        if(selected)
        {selected = false;}
        else
        {selected = true;}   
    }

    private void Update() {
        if(selected){
            
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);

        }
    }  
    
}
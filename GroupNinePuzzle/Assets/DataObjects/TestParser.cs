using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TestParser{
    public void Start(){
        string jsonString = File.ReadAllText(144Solutions.json);
        Puzzle test = JsonSerializer.Deserialize<Puzzle>(jsonString);
    }
}

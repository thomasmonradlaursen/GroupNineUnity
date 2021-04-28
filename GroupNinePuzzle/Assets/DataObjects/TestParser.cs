using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Text.Json;

[Serializable]
public class Player
{
    public string playerId;
    public string playerLoc;
    public string playerNick;
}
public class TestParser : MonoBehaviour {
    public void Start(){
        //string jsonString = File.ReadAllText(144Solutions.json);
        //Puzzle test = JsonSerializer.Deserialize<Puzzle>(jsonString);

        Player playerInstance = new Player();
        playerInstance.playerId = "8484239823";
        playerInstance.playerLoc = "Powai";
        playerInstance.playerNick = "Random Nick";

        //Convert to JSON
        string playerToJson = JsonUtility.ToJson(playerInstance, true);
        Debug.Log(playerToJson);

        string jsonString2 = "{\"playerId\":\"8484239823\",\"playerLoc\":\"Powai\",\"playerNick\":\"Random Nick\"}";
        Player player = JsonUtility.FromJson<Player>(jsonString2);
        Debug.Log(player.playerLoc);
        Console.WriteLine(jsonString2);

        //Deserialize json file 
        string test = System.IO.File.ReadAllText("Assets\\DataObjects\\SmallPuzzle.json");
        JavaScriptSerializer jss = new JavaScriptSerializer();
        Tester.Puzzle testPuzzle = JsonHelper.FromJson<Tester.Puzzle>(test);

        Debug.Log(testPuzzle.result);
        //Debug.Log(testPuzzle.result[0].name);


    }
}

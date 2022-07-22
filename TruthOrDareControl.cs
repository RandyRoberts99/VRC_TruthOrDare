
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class TruthOrDareControl : UdonSharpBehaviour
{
    [Header("Game Objects")]
    public Collider gameTrigger;

    [Header("UI Elements")]
    public TextMeshProUGUI questionScreen;

    // Udon Synced Variables
    [UdonSynced, NonSerialized]
    public string question;

    [UdonSynced, NonSerialized]
    public bool isTruth = false;

    // Local game variables
    private int questionIndex = 0;
    private int randomIndex = 0;
    private int sampleNameIndex = 0;

    // TODO: Replace these with avatar image
    public string[] truthQuestions = new string[10];
    public string[] dareQuestions = new string[10];

    // For when there are not enough people to fully fill in the random names
    public string[] sampleNames = new string[20];

    void Start()
    {
        
    }

    public override void OnDeserialization() => DisplayQuestion();

    private void DisplayQuestion()
    {
        questionScreen.text = question;
    }

    public void _RequestQuestion(bool isTruth)
    {
        VRCPlayerApi[] playerList = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(playerList);

        VRCPlayerApi[] activeList = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        int numPlayers = 0;

        foreach (var player in playerList)
        {
            var pos = player.GetPosition();
            if (_inRange(gameTrigger.ClosestPoint(pos), pos))
            {
                activeList[numPlayers] = player;
                numPlayers++;
            }
        }

        if (numPlayers <= 0)
        {
            Debug.Log(numPlayers);
            questionScreen.text = "You need atleast two people to start!";
            return;
        }

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        int questionsLength = (isTruth) ? truthQuestions.Length : dareQuestions.Length;
        questionIndex = (UnityEngine.Random.Range(1, questionsLength) + questionIndex) % questionsLength;
        question = (isTruth) ? truthQuestions[questionIndex] : dareQuestions[questionIndex];

        question = question.Replace("[RANDOM_NAME_1]", GetRandomPlayerName(activeList, numPlayers));
        question = question.Replace("[RANDOM_NAME_2]", GetRandomPlayerName(activeList, numPlayers));
        question = question.Replace("[RANDOM_NAME_3]", GetRandomPlayerName(activeList, numPlayers));
        question = question.Replace("[RANDOM_NAME_4]", GetRandomPlayerName(activeList, numPlayers));

        RequestSerialization();
        DisplayQuestion();
    }

    private string GetRandomPlayerName(VRCPlayerApi[] activeList, int numActivePlayers)
    {
        if (numActivePlayers <= 4)
        {
            sampleNameIndex = (UnityEngine.Random.Range(1, sampleNames.Length) + sampleNameIndex) % sampleNames.Length;
            return sampleNames[sampleNameIndex];
        }
        else
        {
            randomIndex = (UnityEngine.Random.Range(1, activeList.Length) + randomIndex) % activeList.Length;
            return activeList[randomIndex].displayName;
        }
    }

    private bool _inRange(Vector3 pos1, Vector3 pos2)
    {
        if (Vector3.Distance(pos1, pos2) <= 0.1)
        {
            return true;
        }

        return false;
    }
}

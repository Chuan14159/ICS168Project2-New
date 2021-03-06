﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DodgeballNetworkManager : NetworkManager {
    #region Attributes
    [SerializeField]
    private GameObject preMatch;    // The objects to only enable before the server starts
    [SerializeField]
    private List<GameObject> balls; // The balls to spawn
    [SerializeField]
    private GameObject gameInfo;    // All of the game's numbers         
    #endregion

    #region Properties

    #endregion

    #region Event Functions
    public override void OnStartServer ()
    {
        base.OnStartServer();
        preMatch.SetActive(false);
        StartCoroutine(Utils.DoAfter(SpawnObjects, Utils.ServerActive));
    }

    public override void OnStartClient (NetworkClient client)
    {
        base.OnStartClient(client);
        preMatch.SetActive(false);
    }

    public override void OnStopServer ()
    {
        base.OnStopServer();
        preMatch.SetActive(true);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        preMatch.SetActive(true);
    }

    public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (numPlayers >= matchSize)
        {
            Scoreboard s = gameInfo.GetComponent<Scoreboard>();
            gameInfo.GetComponent<Timer>().StartTimer();
            s.ResetScores();
            s.SetWin("");
        }
    }

    // Awake is called before Start
    private void Awake ()
	{
		
	}

	// Use this for initialization
	private void Start () 
	{
		
	}
	
	// Update is called once per frame
	private void Update () 
	{
		
	}
	#endregion
	
	#region Methods
	// Spawn in the balls
    private void SpawnObjects ()
    {
        foreach (GameObject g in balls)
        {
            float pos = Random.Range(0f, 8f);
            GameObject r = Instantiate(g, new Vector3(pos, 4), Quaternion.identity);
            NetworkServer.Spawn(r);
            GameObject l = Instantiate(g, new Vector3(-pos, 4), Quaternion.identity);
            NetworkServer.Spawn(l);
        }
    }

    // Set the number of players in the match with a string
    public void SetMaxPlayers (string input)
    {
        matchSize = uint.Parse(input);
    }
	#endregion
	
	#region Coroutines

	#endregion
}

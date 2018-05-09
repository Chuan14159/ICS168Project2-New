using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour {
    #region Attributes
    public static TeamSelection instance;       // The instance to reference
    #endregion

    #region Properties
    public PlayerControl Player { get; set; }   // The current player to modify
	#endregion

	#region Event Functions
	// Awake is called before Start
	private void Awake ()
	{
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
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
	// Choose the team for the player
    public void ChooseTeam (int team)
    {
        Player.CmdChooseTeam(team);
        gameObject.SetActive(false);
    }
	#endregion
	
	#region Coroutines
	
	#endregion
}

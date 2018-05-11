using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Scoreboard : NetworkBehaviour {
    #region Attributes
    public static Scoreboard instance;  // The instance to reference
    [SerializeField]
    private int scoreLimit;             // The max score before winning
    [SerializeField]
    private Text scoreDisplay1;         // The score1 display
    [SerializeField]
    private Text scoreDisplay2;         // The score2 display
    [SyncVar(hook = "SetScore1")]
    private int score1;                 // The first score
    [SyncVar(hook = "SetScore2")]
    private int score2;                 // The second score
    #endregion

    #region Properties
    // Returns score1
    public int Score1
    {
        get
        {
            return score1;
        }
    }

    // Returns score2
    public int Score2
    {
        get
        {
            return score2;
        }
    }

    // Returns whether the game is over
    public bool GameOver
    {
        get
        {
            return score1 >= scoreLimit || score2 >= scoreLimit;
        }
    }
    #endregion

    #region Event Functions
    public override void OnStartClient ()
    {
        base.OnStartClient();
        SetScore1(score1);
        SetScore2(score2);
    }

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
	// Reset the scores
    public void ResetScores ()
    {
        SetScore1(0);
        SetScore2(0);
    }

    // Set the first score
    public void SetScore1 (int value)
    {
        score1 = value;
        if (scoreDisplay1 != null)
        {
            scoreDisplay1.text = string.Format("{0:00}", score1);
        }
    }

    // Set the second score
    public void SetScore2 (int value)
    {
        score2 = value;
        if (scoreDisplay2 != null)
        {
            scoreDisplay2.text = string.Format("{0:00}", score1);
        }
    }
    #endregion

    #region Coroutines

    #endregion
}

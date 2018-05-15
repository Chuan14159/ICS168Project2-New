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
    [SerializeField]
    private Text winDisplay;            // The win screen display
    [SyncVar(hook = "SetScore1")]
    private int score1;                 // The first score
    [SyncVar(hook = "SetScore2")]
    private int score2;                 // The second score
    [SyncVar(hook = "SetWin")]
    private string winText;             // The text on the win screen
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

    // Return the team in the lead, or -1 if tied
    public int WinningTeam
    {
        get
        {
            if (score1 > score2)
            {
                return 0;
            }
            else if (score1 < score2)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    #endregion

    #region Event Functions
    public override void OnStartClient ()
    {
        base.OnStartClient();
        SetScore1(score1);
        SetScore2(score2);
        SetWin(winText);
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
            scoreDisplay1.text = string.Format("\n\n\n {0:00}", score1);
        }
        if (GameOver)
        {
            DeclareWinner();
        }
    }

    // Set the second score
    public void SetScore2 (int value)
    {
        score2 = value;
        if (scoreDisplay2 != null)
        {
            scoreDisplay2.text = string.Format("\n\n\n {0:00}", score2);
        }
        if (GameOver)
        {
            DeclareWinner();
        }
    }

    // Set the win text
    public void SetWin (string value)
    {
        winText = value;
        winDisplay.text = value;
    }

    // Declare the winner
    public void DeclareWinner ()
    {
        switch (WinningTeam)
        {
            case -1:
                SetWin("DRAW");
                break;
            case 0:
                SetWin("RED TEAM WINS");
                break;
            case 1:
                SetWin("BLUE TEAM WINS");
                break;
        }
    }
    #endregion

    #region Coroutines

    #endregion
}

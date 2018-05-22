using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Timer : NetworkBehaviour {
    #region Attributes
    public Task after;              // What to do when the timer expires
    [SerializeField]
    private int time;               // The time to set for the timer
    [SerializeField]
    private Text text;              // The text to show the time
    private Coroutine routine;      // The current timer routine
    [SyncVar(hook = "SetSeconds")]
    public int seconds;            // The time left in seconds
    #endregion

    #region Properties

    #endregion

    #region Event Functions
    public override void OnStartClient ()
    {
        base.OnStartClient();
        SetSeconds(seconds);
    }

    // Awake is called before Start
    private void Awake ()
	{
		
	}

	// Use this for initialization
	private void Start () 
	{
        after = Scoreboard.instance.DeclareWinner;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		
	}
	#endregion
	
	#region Methods
	// Start a timer and stop the other one
    public void StartTimer ()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(TimeRoutine(time));
    }

    // Set the seconds
    private void SetSeconds (int sec)
    {
        seconds = sec;
        if (text != null)
        {
            text.text = string.Format("0{0}:{1:00}", seconds / 60, seconds % 60);
        }
    }
	#endregion
	
	#region Coroutines
    // Start a timer with certain seconds
	public IEnumerator TimeRoutine (int sec)
    {
        SetSeconds(sec);
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1);
            SetSeconds(seconds - 1);
        }
        if (after != null)
        {
            after();
        }
    }
	#endregion
}

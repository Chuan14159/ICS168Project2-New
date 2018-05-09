using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    #region Attributes
    public static GameController instance;  // The instance to reference
    public GameObject teamSelectionUI;      // The team selection screen
    #endregion

    #region Properties

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
	
	#endregion
	
	#region Coroutines
	
	#endregion
}

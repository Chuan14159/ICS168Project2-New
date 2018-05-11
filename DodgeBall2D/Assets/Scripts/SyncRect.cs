using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SyncRect : NetworkBehaviour {
    #region Attributes
    [SerializeField]
    private RectTransform rectTransform;    // The Rect Transform component
    [SyncVar]
    [SerializeField]
    private Vector2 minAnchor;              // The minimum anchor points
    [SyncVar]
    [SerializeField]
    private Vector2 maxAnchor;              // The maximum anchor points

    #endregion

    #region Properties

    #endregion

    #region Event Functions
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
        rectTransform.SetAnchors(minAnchor, maxAnchor);
    }
    #endregion

    #region Methods

	#endregion
	
	#region Coroutines
	
	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void Task();
public delegate bool Condition();

public static class Utils {
    #region Methods
    // Returns whether the server is active or not
    public static bool ServerActive ()
    {
        return NetworkServer.active;
    }
    #endregion

    #region Coroutines
    // Waits until a condition is met then does something
    public static IEnumerator DoAfter (Task task, Condition condition)
    {
        while (!condition())
        {
            yield return null;
        }
        task();
    }
	#endregion
}

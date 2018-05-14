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

    // Sets the anchors and zeroes out the sizes of a rect transform
    public static void SetAnchors (this RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }

    // Returns a color with a different alpha
    public static Color SetAlpha (this Color c, float value)
    {
        return new Color(c.r, c.g, c.b, value);
    }
    #endregion

    #region Coroutines
    // Waits until a condition is met then does a task
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

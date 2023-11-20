using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string linkToOpen;

    // Call this method from the Button click event
    public void OpenURL()
    {
        Application.OpenURL(linkToOpen);
    }
}

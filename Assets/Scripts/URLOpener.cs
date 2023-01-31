
using UnityEngine;

public class URLOpener : MonoBehaviour
{
    private string URL = "http://lucky-kat.com";

    public void OpenURL()
    {
        Application.OpenURL(URL);
    }
}

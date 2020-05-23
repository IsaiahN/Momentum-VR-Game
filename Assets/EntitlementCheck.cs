using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Platform;

    public class EntitlementCheck: MonoBehaviour
{

    void Awake()
    {
        try
        {
            Core.AsyncInitialize();
            Entitlements.IsUserEntitledToApplication().OnComplete(GetEntitlementCallback);
        }
        catch (UnityException e)
        {
            Debug.LogError("Platform failed to initialize due to exception");
            Debug.LogException(e);
            UnityEngine.Application.Quit();
        }
    }

    void GetEntitlementCallback (Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("You are NOT entitled to use this app.");
            UnityEngine.Application.Quit();
        }
        else
        {
            Debug.Log("You are entitled to use this app");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
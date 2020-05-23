using UnityEngine;
using TMPro;

public class controllerTest : MonoBehaviour {
	public TextMeshProUGUI warningText;
	
	public void Update() {
        OVRInput.Update();
        // If a GearVR/oculus go controller is not found, throw error;
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
        {
            warningText.SetText("");
            //Debug.Log("Controller Found");

        } else
        {
            warningText.SetText("Note: You will need to connect your controller to continue");
            //Debug.Log("Controller Missing");
        }
	}
}

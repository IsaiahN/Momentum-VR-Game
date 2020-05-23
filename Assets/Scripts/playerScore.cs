using UnityEngine;
using UnityEngine.UI;

public class playerScore : MonoBehaviour {

    public Transform player;
    public Text scoreText;

    private void Start()
    {
        scoreText.text = " ";
    }
	
	// Update is called once per frame
	void Update () {

    /*     if (FindObjectOfType<GameManager>().gameHasEnded == false)
         {
             scoreText.text = player.position.z.ToString("0") + "M";
         }
         */
        /*
         * DEBUG - OVR Touchpad Input
        OVRInput.FixedUpdate();
        scoreText.text = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).ToString();
        */

    }

    
}

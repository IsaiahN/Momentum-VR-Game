using UnityEngine;

public class rotateskybox : MonoBehaviour {

    public float multiplyRotationSpeed = 2f;
	
	// Update is called once per frame
	void Update () {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * multiplyRotationSpeed);
}
}

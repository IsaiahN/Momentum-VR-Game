using UnityEngine;

public class playercollision : MonoBehaviour {

    public PlayerMovement movement;
    public float upforce = 3000f;

	void OnCollisionEnter(Collision collisioninfo)

    {

    if (collisioninfo.collider.tag == "obstacle")
        {
            //Debug.Log("we hit an obstacle called " + collisioninfo.collider.name);
            movement.enabled = false;
            FindObjectOfType<GameManager>().EndGame();
            
        }

        if (collisioninfo.collider.tag == "wall")
        {

            movement.rb.AddForce(0, upforce * Time.deltaTime, 0, ForceMode.VelocityChange);

        }
    }
	
}

using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider collisioninfo)
    {

        if (collisioninfo.tag == "Player")
        {
            gameManager.EndLevel();
        }
    }
}
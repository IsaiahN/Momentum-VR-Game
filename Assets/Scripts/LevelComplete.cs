using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour {

    public void LoadNextLevel()
    {
        //Get Next Scene or Level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

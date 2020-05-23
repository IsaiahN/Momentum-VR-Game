using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

//Gamemanager Purpose: Manages gamestates

public class GameManager : MonoBehaviour {

    public bool gameHasEnded = false;
    public bool timedLevel = false;
    public float levelMinutes = 0;
    public bool levelTimerCompleted = false;
    public static bool GamePaused = false;
    public float restartDelay = 0.9f;
    public GameObject completeLevelUI;
    public GameObject PauseOptionsUI;
    public AudioMixer audioMixer;
    public static int sceneLevel;
    private static int pauseGameIndex = 0;
    public bool isLevel = false;
    private GameObject playerObject;
    private GameObject remoteOne;
    private GameObject remoteTwo;
    private GameObject skyboxScript;
    private GameObject ringPointer;
    private Vector3 shrinkControllers = new Vector3(0, 0, 0);
    private Vector3 growControllers = new Vector3(1, 1, 1);

    public void Start()
    {
        //Set Frame Rate to 72Hz
        OVRManager.display.displayFrequency = 72.0f;
        pauseGameIndex = SceneManager.sceneCountInBuildSettings - 1;
        playerObject = GameObject.Find("Player");
        ringPointer = GameObject.Find("GazePointerRing");
        if (SceneManager.GetActiveScene().buildIndex != pauseGameIndex)
        {
            sceneLevel = SceneManager.GetActiveScene().buildIndex;
        }

        // If Scene is a level, Hide Controllers
        if (SceneManager.GetActiveScene().name.ToLower().Contains("level"))
        {
            //Debug.Log("Level Found");
            remoteOne = GameObject.Find("TrackedRemote (1)");
            remoteTwo = GameObject.Find("TrackedRemote (2)");
            remoteOne.transform.localScale = shrinkControllers;
            remoteTwo.transform.localScale = shrinkControllers;
            isLevel = true;
        }
        else if (sceneLevel == 0)
        {
            isLevel = true;
        }
        skyboxScript = GameObject.Find("Rotate Skybox");

        // Create Timed Levels
        if (timedLevel)
        {
            Invoke("EndingLevelFlag", levelMinutes * 60f);
        }
    }

    void Update()
    {
        if (PauseOptionsUI.activeInHierarchy)
        {
            PositionPanel(PauseOptionsUI, new Vector3(0, 6, 7));
        }
        if (completeLevelUI.activeInHierarchy)
        {
            //PositionPanel(completeLevelUI, new Vector3(0, 0, 0));
            PositionPanel(completeLevelUI.transform.GetChild(0).gameObject, new Vector3(0, 3, 1));
            PositionPanel(completeLevelUI.transform.GetChild(1).gameObject, new Vector3(0, 3, 1));
        }
        
        OVRInput.Update();

        if (OVRInput.GetDown(OVRInput.Button.Back) || OVRInput.Get(OVRInput.RawButton.Back) || Input.GetKey("p"))
        {
            if (GamePaused == false)
            {
                if (!IsInvoking("Pause"))
                {
                    Invoke("Pause", 0.1f);
                }
            }
            else
            {
                if (!IsInvoking("Resume"))
                {
                    Invoke("Resume", 0.1f);
                }
            }
        } else if (Input.GetKey("r")) { Resume(); }

        // If headset is off, trigger pause
        if ((OVRPlugin.userPresent == false || OVRManager.hasInputFocus == false) && isLevel == true && GamePaused == false) {
             Pause();
        }
        

        // If Player Falls Beneath Ground, End game
        if (playerObject.transform.position.y < -1f)
        {
            EndGame();
        }
    }


    public void PositionPanel(GameObject panel, Vector3 offset)
    {
        panel.transform.position = playerObject.transform.position + offset;
        panel.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public void Resume(){
        // Hide Controller Models
        if (isLevel)
        {
            remoteOne.transform.localScale = shrinkControllers;
            remoteTwo.transform.localScale = shrinkControllers;
        }

        // Remove RingPointer
        ringPointer.SetActive(false);

        // Hide Pause Menu
        PauseOptionsUI.SetActive(false);

        // Resume Skybox Rotation
        skyboxScript.SetActive(true);

        // Resume Music
        AudioListener.pause = false;

        // Resume Player Motion
        playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Set Game State to Active
        GamePaused = false;
        
    }
    public void Pause() {
        // Pause Music
        AudioListener.pause = true;

        // Freeze Player Motion
        playerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        // Activate RingPointer
        ringPointer.SetActive(true);

        // Stop Skybox Rotation
        skyboxScript.SetActive(false);

        // Show Controller Models
        if (isLevel)
        { 
            remoteOne.transform.localScale = growControllers;
            remoteTwo.transform.localScale = growControllers;
        }
        
        // Show Pause Menu
        PauseOptionsUI.SetActive(true);


        // Set Game State to Paused
        GamePaused = true;
    }
    // Creates Flag to End Level Generation & Spawn Level End Block/Event
    public void EndingLevelFlag()
    {
        if (levelTimerCompleted == false)
        {
            levelTimerCompleted = true;
        }
    }
    public void EndLevel()
    {
        if (gameHasEnded == false)
        {
            //Debug.Log("END LEVEL");
            gameHasEnded = true;
            completeLevelUI.transform.GetChild(1).gameObject.SetActive(true);
            ringPointer.SetActive(false);
            completeLevelUI.SetActive(true);
            Invoke("StartGame", restartDelay);

        }
    }
    public void EndGame() {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            completeLevelUI.transform.GetChild(0).gameObject.SetActive(true);
            ringPointer.SetActive(false);
            completeLevelUI.SetActive(true);
            //Debug.Log("GAME OVER");

            Invoke("Restart", restartDelay);
        }
    }

    public void Restart() { 
            //Reload the Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Exits the Game
    public void QuitGame()
    {
        //Debug.Log("Quit Game");
        UnityEngine.Application.Quit();
    }

    // Hard Resets the Game
    public void ResetGame()
    {
        //Debug.Log("Reset Game");
        SceneManager.LoadScene(1);

        // Resume Music
        AudioListener.pause = false;

        // Set Game State to Active
        GamePaused = false;

    }


    // Starts New Game
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Handle Audio
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    //Set Quality
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}



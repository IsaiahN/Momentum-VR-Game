using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This Class helps to create/destroy objects autogenerating an endless level
 * that can be stopped via timer, and vary obstacle prefabs types based on World/Level.
 * 
 * For Blocktype Reference:
 * Behind Player uses: custom offset 
 * Ahead of Player uses: default floor scale z
 * SpawnAroundPlayer uses: default floor scale z
 * SpawnAhead Of Player uses: default floor scale z
 * 
 */
public class ContainFollowPlayer : MonoBehaviour
{

    public Transform player;
    public GameObject spawner;
    public enum BlockType
    {
        BehindPlayer = 1,
        AheadOfPlayer = 2,
        SpawnAroundPlayer = 3,
        SpawnAheadOfPlayer = 4
    }
    public BlockType followType;
    public Vector3 offset;
    public float secsPerObstacle = 5;
    public float secsPerDestroy = 3;
    public float secsWallDelay = 0.6f;
    public GameObject[] Obstacles;
    public GameObject[] Powerups;
    public GameObject[] Overhangs;
    GameObject[] destroyQueue;
    Transform floor;
    Transform itemClone;
    GameObject obstacleClone;
    Vector3 blockP;
    Vector3 SpawnPoint;
    float initialStartDistance;
    int randObjectIndex = 0;
    float floor_x;

    private void Start()
    {

        floor = GameObject.FindWithTag("AutoGen Floor").transform;
        //If there is no offset for Z, use floor scale z for offset.
        if (offset.z == 0)
        {
            offset.z = floor.lossyScale.z;
        }
        floor_x = floor.lossyScale.x;
        blockP = transform.position;
        initialStartDistance = offset.z / 2;

        if ((int)followType == 3 || (int)followType == 4)
        {
            SpawnPoint = spawner.transform.position;
        }
     
    }

    void OnCollisionEnter (Collision collision)
    {

        if (((int) followType == 1) && (collision.gameObject.tag == "Destroy"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        } 
    }
   

    // Update is called once per frame
    void FixedUpdate()
    {
        blockP = transform.position;
        if ((int)followType == 1)
        {
            // Behind Player / Starting Block
            //Debug.Log("BEHIND - Distance: " + (player.position.z - blockP.z) + "| Player Z: " + player.position.z + " | Block Z: " + blockP.z);
            //Debug.Log("BEHIND PLAYER / START BLOCK");
            if ((player.position.z - blockP.z) >= offset.z)
            {

                blockP.z = player.position.z - offset.z;
                transform.position = blockP;
            }
        }
        else if ((int)followType == 2)
        {
            //Ahead of Player / End Level Block
            //Debug.Log("AHEAD - Distance: " + (blockP.z - player.position.z) + "| Player Z: " + player.position.z + " | Block Z: " + blockP.z);
            //Debug.Log("AHEAD OF PLAYER/ ENDING BLOCK");
            if (((player.position.z >= SpawnPoint.z / 2) && (blockP.z - player.position.z) <= offset.z) && (FindObjectOfType<GameManager>().levelTimerCompleted == false))
            {
                transform.SetPositionAndRotation(new Vector3(transform.position.x,transform.position.y, player.position.z + offset.z), Quaternion.identity);
              
            }
        }
        else if (((int)followType == 3)  && (FindObjectOfType<GameManager>().levelTimerCompleted == false))
        {
           
            // Spawn Around Player / Auto Generate Walls or Floor
            //If past previous spawn point, spawn more objects
            if (player.position.z >= SpawnPoint.z)
            {
                MoveSpawnPoint();
                if (!IsInvoking("SpawnWrapper"))
                    Invoke("SpawnWrapper", secsWallDelay);
            }
        }
        else if ((int)followType == 4)
        {
            // Spawn Ahead Player / Auto Generate Obstacle Prefabs
            // If player position is over half the floor distance (initial start distance) and leveltimer is ongoing, allow for spawning
            // then move spawner forward by that start distance for the entire duration of timer.
            if ((player.position.z >= initialStartDistance) && (FindObjectOfType<GameManager>().levelTimerCompleted == false))
            {
                blockP.z = player.position.z + initialStartDistance;
                transform.position = blockP;
                //If past previous spawn point, spawn more objects

                if (player.position.z >= SpawnPoint.z)
                {
                    MoveSpawnPoint();
                    if (!IsInvoking("SpawnObstacles"))
                    {
                        Invoke("SpawnObstacles", secsPerObstacle * 0f);
                        Invoke("SpawnObstacles", secsPerObstacle * .3f);
                        Invoke("SpawnObstacles", secsPerObstacle * 0.9f);
                        Invoke("SpawnObstacles", secsPerObstacle * 1.5f);
                        Invoke("SpawnObstacles", secsPerObstacle * 2.1f);
                    }
                    if (!IsInvoking("SpawnOverhang"))
                        Invoke("SpawnOverhang", secsPerObstacle * 2.5f);
                    if (!IsInvoking("SpawnPowerUps"))
                        Invoke("SpawnPowerUps", secsPerObstacle * 6);
                }
            }
        }
        //Cleaner Script: Removes Obstacles With Destroy Tag, Once Per Few Seconds
        Invoke("DestroyBehindObject", secsPerDestroy);
    }

    public void MoveSpawnPoint()
    {
        // Put a distance of floor scale z between previous spawntrigger position and the new position
        SpawnPoint.z = SpawnPoint.z + offset.z;
    }
    public void SpawnWrapper()
    {

        // Clones Object, and sets distance to previous object end and rotation
        itemClone = Instantiate(transform, blockP, Quaternion.identity);
        transform.gameObject.GetComponent<ContainFollowPlayer>().enabled = false;
        transform.gameObject.tag = "Destroy";

        // gets previous object end position
        itemClone.position = new Vector3(floor.position.x, Mathf.Round(floor.position.y), Mathf.Round(floor.position.z + floor.lossyScale.z));
        itemClone.name = transform.name;
        

    }
    public void SpawnObstacles()
    {

        randObjectIndex = Random.Range(0, Obstacles.Length);
        if (Obstacles.Length > 0 && Obstacles[randObjectIndex])
        {
            // Spawning obstacles from prefab gameobject array, at 
            //Debug.Log("Obstacles - Min X: " + (((floor_x - Obstacles[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "MAX x: " + (((floor_x - Obstacles[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "Item x:" + Obstacles[randObjectIndex].transform.lossyScale.x + " floor x: " + floor_x);
            
            obstacleClone = Instantiate(Obstacles[randObjectIndex], new Vector3(Obstacles[randObjectIndex].transform.position.x, Obstacles[randObjectIndex].transform.lossyScale.y / 2, spawner.transform.position.z), Quaternion.identity);
            obstacleClone.gameObject.tag = "Destroy";
        }
    }
    public void SpawnOverhang()
    {

        randObjectIndex = Random.Range(0, Overhangs.Length);
        if (Overhangs.Length > 0 && Overhangs[randObjectIndex])
        {
            //Debug.Log("Overhangs - Min X: " + (((floor_x - Overhangs[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "MAX x: " + (((floor_x - Overhangs[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "Item x:" + Overhangs[randObjectIndex].transform.lossyScale.x + " floor x: " + floor_x);
            obstacleClone = Instantiate(Overhangs[randObjectIndex], new Vector3(0, Overhangs[randObjectIndex].transform.lossyScale.y / 2, spawner.transform.position.z), Quaternion.identity);
            obstacleClone.gameObject.tag = "Destroy";
        }
    }
    public void SpawnPowerUps()
    {

        randObjectIndex = Random.Range(0, Powerups.Length);
        if (Powerups.Length > 0 && Powerups[randObjectIndex])
        {
            //Debug.Log("Powerups - Min X: " + (((floor_x - Powerups[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "MAX x: " + (((floor_x - Powerups[randObjectIndex].transform.lossyScale.x) / 2) * -1) + "Item x:" + Powerups[randObjectIndex].transform.lossyScale.x + " floor x: " + floor_x);
            obstacleClone = Instantiate(Powerups[randObjectIndex], new Vector3(0, Powerups[randObjectIndex].transform.lossyScale.y, spawner.transform.position.z), Quaternion.identity);
            obstacleClone.gameObject.tag = "Destroy";
        }
    }

    // Create separate destroy script attached to prefab obstacles that is based on distance from object boundary position
    public void DestroyBehindObject ()
    {
        destroyQueue = GameObject.FindGameObjectsWithTag("Destroy");
        for(var i=0; i < destroyQueue.Length; i++)
        {
            // Destroys object if its 2 floor/wall offset distances behind player 
            //Debug.Log("if (" + player.transform.position.z + " - " + destroyQueue[i].transform.position.z + ") >= (" + offset.z * 1.5 + ")");
            if ((player.transform.position.z - destroyQueue[i].transform.position.z) >= (offset.z * 1.5))
            {
                Destroy(destroyQueue[i]);
            }

        }
    }

}
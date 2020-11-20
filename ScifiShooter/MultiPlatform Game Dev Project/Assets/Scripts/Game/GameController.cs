using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/* 
    GameController.cs handles player progression through the level, key management, and level completion / failure.
*/

public class GameController : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject key;
    public GameObject enemy;
    private GameObject player;
    public GameObject[] spawnPoints;
    private int currentWaveLevel;
    public Text YouWonText;
    public Text YouLostText;
    private bool isGameFinished = false;
    private bool allKeysCollected = false;
    private int spawnedKeys;
    #endregion


    [NonSerialized]
    private int currentEnemyCount = 0;
    //public float countdown = 10;


    // On start, set everything to normal.
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentWaveLevel = 0;
        YouLostText.text = "";
        YouWonText.text = "";
        isGameFinished = false;
        currentEnemyCount = 0;
        allKeysCollected = false;
        spawnedKeys = 0;
    }

    // Generating keys at random spawn points.
    public void SpawnKey(int keysToSpawn)
    {
        // Store spawn points in a list of coordinates
        List<Vector3> coord = new List<Vector3>();
        Vector3 keyPosition = new Vector3(0, 0, 0);

        coord.Add(new Vector3(-18f, 5.391f, 33f));
        coord.Add(new Vector3(32f, 5.391f, 36f));
        coord.Add(new Vector3(37f, 1f, 1.23f));
        coord.Add(new Vector3(-16.18f, 1f, 30.6f));
        coord.Add(new Vector3(48f, 1f, 48f));
        coord.Add(new Vector3(14f, 3.742f, 6.5f));
        coord.Add(new Vector3(26f, 1f, 36f));

        // Randomly choose spawn points for every key
        for (int i = 0; i < keysToSpawn; i++) 
        {
            var index = UnityEngine.Random.Range(0, coord.Count);
            Instantiate(key, coord.ElementAt(index), Quaternion.identity);            
            spawnedKeys++;
            Debug.Log("spawned " + spawnedKeys);
            coord.Remove(coord.ElementAt(index));
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Reset game if player has won or lost the game.
        if (isGameFinished && Input.GetKeyDown(KeyCode.Space))
        {
            isGameFinished = true;
            Start();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void FixedUpdate()
    {
        if (!isGameFinished && currentEnemyCount == 0 && currentWaveLevel >= 0 && currentWaveLevel < 3)
        {
            StartLevel(ref currentWaveLevel);
            Debug.Log(currentWaveLevel + " spawned");
        }

        if (!isGameFinished && currentEnemyCount == 0 && currentWaveLevel >= 3)
        {
            if (allKeysCollected)
            {
                GameWon();
            }
            
            player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel);
            
        }
    }

    // Handles logic relating to starting the current level
    private void StartLevel(ref int currentWaveLevel)
    {
        currentWaveLevel++;

        player.GetComponent<PlayerStatController>().SetPlayerLevel(currentWaveLevel);
        //SpawnEnemies();
        foreach (var sp in spawnPoints)
        {
            if (sp == spawnPoints.First() || (UnityEngine.Random.Range(0, 1f) < 0.3 * currentWaveLevel))
            {
                var ene = Instantiate(enemy, sp.transform.position, sp.transform.rotation);
                ene.GetComponent<EnemyController>().Init(currentWaveLevel);
                currentEnemyCount++;
            }
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        //We might decrease the count of different enemy types.
        currentEnemyCount--;
        if (currentEnemyCount == 0 && currentWaveLevel == 3)
        {
            currentWaveLevel++;
            player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel);
        }
    }

    public void OnKeysCollected()
    {
        allKeysCollected = true;
    }

    private void GameWon()
    {
        player.GetComponent<PlayerStatController>().FreezePlayer();

        YouWonText.text = "You Won!\n" + "Time Spent: " + Time.timeSinceLevelLoad + "\n" +
            "Press SPACE TO TRY AGAIN!";
        isGameFinished = true;

    }

    public void GameLost()
    {
        player.GetComponent<PlayerStatController>().FreezePlayer();
        
        YouLostText.text = "You Lost!\nPress SPACE TO TRY AGAIN!";
        //currentWaveLevel = 0;
        isGameFinished = true;
    }
}

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
    public int NumberOfTurrets;
    private GameObject player;
    public GameObject[] spawnPointsEnemies;
    public GameObject[] spawnPointsKey;
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
        Vector3 keyPosition = new Vector3(0, 0, 0);
        var sp = spawnPointsKey.ToList();
        // Randomly choose spawn points for every key
        for (int i = 0; i < keysToSpawn; i++) 
        {
            var index = UnityEngine.Random.Range(0, sp.Count());
            Instantiate(key, sp.ElementAt(index).transform.position, Quaternion.identity);            
            spawnedKeys++;
            Debug.Log("spawned " + spawnedKeys);
            sp.Remove(sp.ElementAt(index));
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


    // Commented for now
    private void FixedUpdate()
    {
        if (!isGameFinished && currentEnemyCount == 0 && currentWaveLevel >= 0 && currentWaveLevel < 3)
        {
            StartLevel(ref currentWaveLevel);
            Debug.Log(currentWaveLevel + " spawned");
        }

        if (!isGameFinished && currentEnemyCount <= 0 && currentWaveLevel >= 3)
        {
            if (currentEnemyCount < 0) 
            {
                Debug.Log("Check the enemy count it is bugged!");
            }

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
        foreach (var sp in spawnPointsEnemies)
        {
            if (sp == spawnPointsEnemies.First() || (UnityEngine.Random.Range(0, 1f) < 0.5 * currentWaveLevel))
            {
                var ene = Instantiate(enemy, sp.transform.position, sp.transform.rotation);
                ene.GetComponent<EnemyController>().StartEnemies(currentWaveLevel);
                currentEnemyCount++;
            }
        }

    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        //We might decrease the count of different enemy types.
        if (enemy.tag == "Turret")
        {
            return; //No need to decrease enemy count if it is a turret
        }

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

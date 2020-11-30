using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour
{
    #region Variables
    [Header("Player Statistics")]
    public int Health;
    public int Level;
    public float Armor;

    //private float[] experienceRequiredPerLevel = { 5f, 10f, 25f, 50f, 100f, 200f };   

    [Header("Text Objects")]
    public Text HealthText;
    public Text ArmorText;
    public Text LevelText;

    public AudioSource audio;

    public AudioClip takingDamage1;
    public AudioClip takingDamage2;
    public AudioClip takingDamage3;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        //SetPlayerLevel(1);
        audio = GetComponent<AudioSource>();
    }

    // Every wave, modify player statistics
    public void SetPlayerLevel(int currentWaveLevel)
    {//TODO Ertugrul: Make this upgradeable
        Level = currentWaveLevel;
        Health = Mathf.Clamp(Health + 30, 0, 100);
        Armor += Level * 0.1f;
        //gameObject.GetComponent<PlayerMove>().speed += currentWaveLevel * 0.5f;
        //This causes coupling so it is better to combine stat and movement after the prototype
     //   GameObject.FindGameObjectWithTag("Weapon").GetComponent<PistolShoot>().OnLeveledUp(currentWaveLevel);
        UpdatePlayerStatsTexts();
       
    }

    // UI Update Method
    private void UpdatePlayerStatsTexts()
    {
        HealthText.text = "♥" + Health.ToString();
        ArmorText.text = "Armor %" + Armor.ToString();
        UpdateWaveText(Level);
    }

    // Handling player taking damage
    public void TakeDamage(int damage)
    {
        // If there is armour active, use it first
        if (Armor > 0)
        {
            Health -= Mathf.RoundToInt((float)damage * (1 - Armor));
        }
        else
        {
            if ((Health - damage) >= 0)
            {
                Health -= damage;
            }
            else
            {
                Health = 0;
            }
        }


        if (UnityEngine.Random.value < 0.1f)
        {
            audio.clip = takingDamage1;

        }
        else if (UnityEngine.Random.value < 0.2f)
        {
            audio.clip = takingDamage2;
        }
        else
        {
            audio.clip = takingDamage3;
        }

        audio.Play();

        UpdatePlayerStatsTexts();

        // If player dies
        if (Health <= 0)
        {
            Health = 0;
            Invoke(nameof(GameOver), 0.2f);
            audio.Play();
        }
    }

    public void OnHealthPickedUp()
    {
            
        Health = Mathf.Clamp(Health + 25, 0, 100);
       
        UpdatePlayerStatsTexts();
    }

    private void  GameOver()
    {        
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameLost();
        /*Destroy(gameObject);*/
    }

    public void UpdateWaveText(int level)
    {
        LevelText.text = "Waves Cleared: " + (level - 1) + " / " + 3;
    }

    public void FreezePlayer()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //Currently giving an error that there is no ridgit body attached to player
        gameObject.GetComponent<PlayerMove>().enabled = false;
        //gameObject.GetComponent<Crouch>().enabled = false;
        
        if(gameObject.GetComponentInChildren<PistolShoot>() != null)
        {
            gameObject.GetComponentInChildren<PistolShoot>().enabled = false;
        }

        if (gameObject.GetComponentInChildren<MachineGunShoot>() != null)
        {
            gameObject.GetComponentInChildren<MachineGunShoot>().enabled = false;
        }

        if (gameObject.GetComponentInChildren<ShotgunShoot>() != null)
        {
            gameObject.GetComponentInChildren<ShotgunShoot>().enabled = false;
        }

    }
    
}

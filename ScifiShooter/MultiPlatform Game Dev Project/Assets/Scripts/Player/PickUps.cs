using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PickUps : MonoBehaviour
{
    #region Variables
    // Stores amount of keys player currently has
    private int haveKey;

    // Stores amount of keys player needs
    private int needKey;

    // UI element and sound
    public Text keyCount;
    private AudioSource pickUpSound;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        haveKey = 0;
        needKey = 3;
        keyCount.text = "";
        SetCountText();
        //GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SpawnKey(needKey);
        pickUpSound = gameObject.GetComponent<AudioSource>();
    }


    // If PlayerController collides with something
    void OnTriggerEnter(Collider other)
    { 
        // Handles Key Pickup
        if (other.gameObject.tag == "PickUpKey")
        {
            other.gameObject.SetActive(false);
            haveKey++;
            SetCountText();
            if (haveKey == needKey)
            {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().OnKeysCollected();
            }
            pickUpSound.Play();
        }
        // Handles Health Pickup
        else if (other.gameObject.tag == "PickUpHealth")
        {
            other.gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatController>().OnHealthPickedUp();
            pickUpSound.Play();
        }
        // Handles Ammo Pickup
        else if (other.gameObject.tag == "PickUpAmmo")
        {
            other.gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Weapon").GetComponent<PistolShoot>().OnAmmoPickedUp();
            pickUpSound.Play();
        }

    }

    // UI Update Method
    private void SetCountText()
    {
        keyCount.text = "Keys Collected: " + haveKey.ToString() + " / "+ needKey.ToString();
    }
}

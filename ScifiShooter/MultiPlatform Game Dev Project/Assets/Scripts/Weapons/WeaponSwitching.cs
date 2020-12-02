using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] weapons = new GameObject[3];
    private int selectedWeapon = 0;
    private int maxWeaponCount = 1;

    void Start()
    {
        SelectWeapon();
    }

    
    // Update is called once per frame
    void Update()
    {
        var secondWeaponUnlocked = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SecondWeaponUnlocked;
        var thirdWeaponUnlocked = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().ThirdWeaponUnlocked;
        if (secondWeaponUnlocked)
        {
            maxWeaponCount = 2;
        }
        else if (thirdWeaponUnlocked)
        {
            maxWeaponCount = 3;
        }
        else
        {
            maxWeaponCount = 1;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {
            if (selectedWeapon == maxWeaponCount -1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) 
        {
            if (selectedWeapon == 0)
            {
                selectedWeapon = maxWeaponCount -1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && (secondWeaponUnlocked || thirdWeaponUnlocked))
        {
            selectedWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && thirdWeaponUnlocked)
        {
            selectedWeapon = 2;
        }
        SelectWeapon();
    }

    void SelectWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == selectedWeapon)
            {
                weapons[i].gameObject.SetActive(true);
            }
            else
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
        
    }
}

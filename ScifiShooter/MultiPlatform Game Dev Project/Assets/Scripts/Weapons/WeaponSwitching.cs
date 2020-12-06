using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitching : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] weapons = new GameObject[3];
    private int selectedWeapon = 0;
    private int maxWeaponCount = 1;

    public Text Weapon1Text;
    public Text Weapon2Text;
    public Text Weapon3Text;

    void Start()
    {
        SelectWeapon();
    }

    
    // Update is called once per frame
    void Update()
    {
        var secondWeaponUnlocked = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SecondWeaponUnlocked;
        var thirdWeaponUnlocked = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().ThirdWeaponUnlocked;
        if (secondWeaponUnlocked && !thirdWeaponUnlocked)
        {
            maxWeaponCount = 2;
        }
        else if (secondWeaponUnlocked && thirdWeaponUnlocked)
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

        Weapon1Text.color = Color.black;
        Weapon2Text.color = Color.black;
        Weapon3Text.color = Color.black;
        Weapon1Text.fontStyle = FontStyle.Normal;
        Weapon2Text.fontStyle = FontStyle.Normal;
        Weapon3Text.fontStyle = FontStyle.Normal;

        if (selectedWeapon == 0)
        {
            Weapon1Text.color = Color.red;
            Weapon1Text.fontStyle = FontStyle.Bold;
        }
        else if (selectedWeapon == 1)
        {
            Weapon2Text.color = Color.red;
            Weapon2Text.fontStyle = FontStyle.Bold;
        }
        else if (selectedWeapon == 2)
        {
            Weapon3Text.color = Color.red;
            Weapon3Text.fontStyle = FontStyle.Bold;
        }
    }
}

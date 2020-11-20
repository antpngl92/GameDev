using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class PistolShoot : MonoBehaviour
{
    #region Variables

    [Header("Gun Statistics")]
    public int damage;
    public int maxMagAmmo;
    public int maxReserveAmmo;
    public int currentMagAmmo;
    public int currentReserveAmmo;
    public int weaponRange;
    public float fireRate = 0.2f;
    public float recoilIntensity = 10f;
    [Space()]

    [Header("Related GameObjects")]
    public Camera camera;
    public AudioClip shootSound;
    public GameObject currentLineEffect;
    public GameObject[] allLineEffects;
    public Text magAmmoText;
    public Text reserveAmmoText;
 
    [Space()]

    private float fireDelay;

    private AudioSource audioSource;

    private Vector3 originPosition;
    private Vector3 recoilOffset;
    private Vector3 rotation;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentMagAmmo = maxMagAmmo;
        currentReserveAmmo = maxReserveAmmo;
        originPosition = transform.position;
        currentLineEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, check if there is a shooting event
        Shoot();

        // Every frame, check if the user wants to reload
        Reload();

        // Update UI elements in case they have changed
        UpdateUI();
    }

    void Shoot()
    {
        // If user presses Fire1 (LMB) and they can shoot
        if (Input.GetMouseButton(0) && Time.time > fireDelay && currentMagAmmo > 0)
        {
            // Calculate new delay timer to control firerate
            fireDelay = Time.time + fireRate;

            // Cast a ray from the middle of the camera out into the world
            Vector3 ray = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            DisplayRecoil();
            RecoilRecovery();

            StartCoroutine("ShootEffect");

            // Play Shooting Sound
            audioSource.clip = shootSound;
            audioSource.Play();

            currentMagAmmo -= 1;

            // If the ray hit something
            if (Physics.Raycast(ray, camera.transform.forward, out hit, weaponRange))
            {
                if (hit.transform.gameObject.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                    Debug.Log("Enemy hit!");
                }
                else
                {
                    Debug.Log("Hit!");
                }
            }
        }
    }

    void Reload()
    {
        // If the user has ammo to reload with, and if they have shot at least once
        if (Input.GetKeyDown(KeyCode.R) && currentReserveAmmo > 0 && currentMagAmmo != maxMagAmmo)
        {
            // Calculate how many bullets were fired from the gun
            int magDiff = Math.Abs(currentMagAmmo - maxMagAmmo);

            // If the user has some ammo left, but they don't have enough to refill the whole mag
            if (magDiff >= currentReserveAmmo)
            {
                currentMagAmmo = currentReserveAmmo;
                currentReserveAmmo = 0;
            }
            else
            {
                currentReserveAmmo -= magDiff;
                currentMagAmmo = maxMagAmmo;
            }
        }
    }

    void UpdateUI()
    {
        magAmmoText.text = currentMagAmmo.ToString();
        reserveAmmoText.text = currentReserveAmmo.ToString() + " -R-";
    }

    public void OnAmmoPickedUp()
    {
        currentMagAmmo += 20;
    }

    public void OnLeveledUp(int currentWaveLevel)
    {
        currentMagAmmo += 20;
        damage += 20;
        maxReserveAmmo += 30;
        fireRate -= 0.005f;
        recoilIntensity -= 0.005f;
        currentLineEffect = allLineEffects[currentWaveLevel - 1];
    }

    void DisplayRecoil()
    {
        // Create recoil boundaries from recoil intensity (maximum recoil kick)
        Vector3 recoilBoundaries = new Vector3(recoilIntensity, recoilIntensity, recoilIntensity);
        // Generate a randomised recoil offset 
        recoilOffset = new Vector3(-recoilBoundaries.x, UnityEngine.Random.Range(-recoilBoundaries.y, recoilBoundaries.y), UnityEngine.Random.Range(-recoilBoundaries.z, recoilBoundaries.z));
    }

    void RecoilRecovery()
    {
       // Calculate gun recoil and recovery rotation
       recoilOffset = Vector3.Lerp(recoilOffset, originPosition, 25f * Time.deltaTime);
       rotation = Vector3.Slerp(rotation, recoilOffset, 6f * Time.deltaTime);
       
       // Set recovery Rotation
       transform.localRotation = Quaternion.Euler(rotation);
    }

    IEnumerator ShootEffect()
    {
        currentLineEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        currentLineEffect.SetActive(false);
    }
}

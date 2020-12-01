using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotgunShoot : MonoBehaviour
{
    #region Variables

    [Header("Gun Statistics")]
    public string name = "Shotgun";
    public int damage;
    public int pelletCount;
    public int maxMagAmmo;
    public int maxReserveAmmo;
    public int currentMagAmmo;
    public int currentReserveAmmo;
    public int weaponRange;
    public float reloadTime = 1.0f;
    public float fireRate = 0.2f;
    public float recoilIntensity = 10f;
    [Space()]

    [Header("Related GameObjects")]
    public Camera camera;
    public AudioClip shootSound;
    public AudioClip noAmmo;

    public Text weaponNameText;
    public Text magAmmoText;
    public Text reserveAmmoText;
    public Text reloadingWeaponText;

    public Transform muzzleEnd;
    public ParticleSystem muzzleEffect;
    public Animator animator;

    [Space()]

    private float fireDelay;
    public bool isReloading = false;

    public AudioSource audioSource;

    private Vector3 originPosition;
    private Vector3 recoilOffset;
    private Vector3 rotation;
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentMagAmmo = maxMagAmmo;
        currentReserveAmmo = maxReserveAmmo;
        originPosition = transform.localPosition;
        //currentLineEffect.SetActive(false);
        muzzleEffect.Stop();
        reloadingWeaponText.enabled = false;
    }
    void OnEnable()
    {
        isReloading = false;
        audioSource.Stop();
        animator.SetBool("IsReloading", false);
        muzzleEffect.Stop();
        reloadingWeaponText.enabled = false;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.isGamePaused)
        {
            return;
        }
        Shoot();

        Reload();

        UpdateUI();
    }
    void Shoot()
    {
        if (isReloading)
        {
            return;
        }

        // If user presses Fire1 (LMB) and they can shoot
        if (Input.GetMouseButton(0) && Time.time > fireDelay)
        {
            if (currentMagAmmo <= 0)
            {
                audioSource.clip = noAmmo;
                audioSource.Play();
                return;
            }
            // Calculate new delay timer to control firerate
            fireDelay = Time.time + fireRate;

            // Calculate shotgun spread

            for (int i = 0; i < pelletCount; i++)
            {
                float xOffset = Random.Range(0f, 0.1f);
                float yOffset = Random.Range(0f, 0.1f);

                Vector3 ray = camera.ViewportToWorldPoint(new Vector3(0.5f + xOffset, 0.5f + yOffset, 0));
                //Debug.Log(ray.ToString());
                RaycastHit hit;

                // If the ray hit something
                if (Physics.Raycast(ray, camera.transform.forward, out hit, weaponRange))
                {
                    if (hit.transform.gameObject.tag == "Enemy")
                    {
                        hit.transform.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
                        //Debug.Log("Enemy hit!");
                    }
                    else if (hit.transform.gameObject.tag == "Turret")
                    {
                        hit.transform.gameObject.GetComponent<GatlingGun>().TakeDamage(damage);
                        //Debug.Log("Turret hit!");
                    }
                    else
                    {
                        //Debug.Log("Hit!");
                    }
                }
            }
            
            //DisplayRecoil();
            //RecoilRecovery();

            muzzleEffect.Play();

            // Play Shooting Sound
            audioSource.clip = shootSound;
            audioSource.Play();

            currentMagAmmo -= 1;
        }
    }
    public void OnAmmoPickedUp()
    {
        currentReserveAmmo += 20;
    }

    void Reload()
    {
        // If the user has ammo to reload with, and if they have shot at least once
        if (Input.GetKeyDown(KeyCode.R) && currentReserveAmmo > 0 && currentMagAmmo != maxMagAmmo)
        {
            Debug.Log("Reloading");
            StartCoroutine("ReloadCoroutine");
        }
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        animator.SetBool("IsReloading", true);
        // Calculate how many bullets were fired from the gun
        int magDiff = Mathf.Abs(currentMagAmmo - maxMagAmmo);

        yield return new WaitForSeconds(reloadTime - 0.25f);

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
        animator.SetBool("IsReloading", false);
        yield return new WaitForSeconds(0.25f);
        isReloading = false;
    }

    void UpdateUI()
    {
        if (isReloading)
        {
            reloadingWeaponText.enabled = true;
        }
        else if (!isReloading)
        {
            reloadingWeaponText.enabled = false;
        }
        magAmmoText.text = currentMagAmmo.ToString();
        reserveAmmoText.text = currentReserveAmmo.ToString() + " -R-";
        weaponNameText.text = name;
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
}


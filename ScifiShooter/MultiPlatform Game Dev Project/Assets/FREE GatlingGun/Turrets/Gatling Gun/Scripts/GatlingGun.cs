using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatlingGun : MonoBehaviour
{
    private GameObject gameController;

    #region Variables

    [Header("Objects")]
    public Canvas UI;
    public Text NameText;
    public Slider healthBar;

    [Header("Enemy Statistics")]
    [Range(1, 9)]
    public int Level = 1;
    public int Health;
    public float Armor;
    public int EnemyDamagePower;

    public GameObject player;

    public float AttackInterval = 1f;
    private bool alreadyAttacked;

    private AudioSource shoot;

    // target the gun will aim at
    Transform go_target;

    // Gameobjects need to control rotation and aiming
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;

    // Gun barrel rotation
    public float barrelRotationSpeed;
    float currentRotationSpeed;

    // Distance the turret can aim and fire from
    public float firingRange;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    // Used to start and stop the turret firing
    bool canFire = false;
    #endregion



    // Method that calculates enemy level scaling
    public void SetEnemyLevel(int level)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameController = GameObject.FindGameObjectWithTag("GameController");
        shoot = GetComponent<AudioSource>();

        Level = level;
        Health = 100;
        EnemyDamagePower = 100; // This could be changed based on level
        healthBar.maxValue = Health;
        healthBar.minValue = 0;
        healthBar.value = Health;
        NameText.text = "Level " + Level + " Turret";
        NameText.color = Color.white;

        // This could be changed based on level
        /* if (Level == 1)
         {
             NameText.color = Color.white;
         }
         else if (Level == 2)
         {
             NameText.color = Color.blue;
         }
         else if (Level == 3)
         {
             NameText.color = Color.red;
         }*/

    }

    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;
        SetEnemyLevel(1); // This could be changed to get the level parameter
    }

    void Update()
    {
        AimAndFire();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            go_target = other.transform;
            canFire = true;
        }

    }
    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canFire = false;
        }
    }

    void AimAndFire()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            Vector3 baseTargetPostition = new Vector3(go_target.position.x, this.transform.position.y, go_target.position.z);
            Vector3 gunBodyTargetPostition = new Vector3(go_target.position.x, go_target.position.y, go_target.position.z);

            go_baseRotation.transform.LookAt(baseTargetPostition);
            go_GunBody.transform.LookAt(gunBodyTargetPostition);
            
            if (!alreadyAttacked)
            {
                // Attack Code                  
                player.transform.gameObject.GetComponent<PlayerStatController>().TakeDamage(EnemyDamagePower);
                Debug.Log("Player is getting attacked!");
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), AttackInterval);

            }

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
                shoot.Play();
            }
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                muzzelFlash.Stop();
                shoot.Stop();
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    // Taking Damage Handling script
    public void TakeDamage(int damage)
    {
        Health -= damage;
        healthBar.value = Health;

        if (Health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    // If enemy dies
    private void DestroyEnemy()
    {
        // Randomly generate health pickups upon death
        if (UnityEngine.Random.value < 0.1f)
        {
            var healthPickUp = GameObject.FindGameObjectWithTag("PickUpHealth");
            Instantiate(healthPickUp, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.rotation);
        }
        // Randomly generate ammo pickups upon death
        else if (UnityEngine.Random.value < 0.2f)
        {
            var ammoPickUp = GameObject.FindGameObjectWithTag("PickUpAmmo");
            Instantiate(ammoPickUp, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.rotation);
        }

        /*gameController.GetComponent<GameController>().OnEnemyDestroyed(gameObject);*/
        Destroy(gameObject);
    }

}
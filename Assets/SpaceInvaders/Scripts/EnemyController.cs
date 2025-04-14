using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // movement range
    public float rangeH = 5;
    public float rangeV = 1;
   
    // speed
    public float speed = 2;

    public GameObject laserPrefab;

    public float laserSpeed = 2.0f;

    public float AttackFrequency = 0.1f;

    public GameObject model;

    public ParticleSystem dead;

    // direction
    int direction = 1;

    // accumulated movement
    float accMovement = 0;

     // available states
    enum State { MovingHorizontally, MovingVertically, Dead};

    
    // keep track of the current state
    State currState;

    // Game Manager
    public GameManager gm;

    // Enemy Manager
    EnemyManager em;

    AudioSource hitsound;

    private int health = 1;

    // Get all materials
    private Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        // initial state
        currState = State.MovingHorizontally;

        // game manager
        gm = GameObject.FindObjectOfType<GameManager>();

       

        // log error if it wasn't found
        if (gm == null)
        {
            Debug.LogError("there needs to be an GameManager in the scene");
        }

        // enemy manager
        em = gm.GetCurrentEm();

        hitsound = em.GetComponent<AudioSource>();
        
        // log error if it wasn't found
        if (em == null)
        {
            Debug.LogError("there needs to be an EnemyManager in the scene");
        }

        //Get all materials
        materials = model.GetComponent<Renderer>().materials;
        InitialColor();
    }

    // Update is called once per frame
    void Update()
    {
        // nothing happens if the enemy is dead
        if (currState == State.Dead) return;

        // calculate movement  v = d / t --> d = v * t
        float movement = speed * Time.deltaTime;

        // update accumulate movement
        accMovement += movement;

        // are we moving horizontally?
        if (currState == State.MovingHorizontally)
        {
            // if yes, then transition to moving vertically
            if(accMovement >= rangeH)
            {
                // transition to moving vertically
                currState = State.MovingVertically;

                // reverse direction (for horizontal movement)
                direction *= -1;

                // reset acc movement
                accMovement = 0;
            }
            // if not, move the invader horizontally
            else
            {
                transform.position += transform.forward * movement * direction;
            }
        }
        // this is, if we are moving vertically
        else
        {
            // if yes, then transition to moving horizontally
            if (accMovement >= rangeV)
            {
                // transition to moving horiz
                currState = State.MovingHorizontally;

                // reset acc movement
                accMovement = 0;
            }
            // if not, move the invader vertically
            else
            {
                transform.position += Vector3.down * movement;
            }
        }

        // enemy shoots at random to the player
        if(Random.value < AttackFrequency * Time.deltaTime)
        {
            OnFire();
        }
    }

    void OnFire()
    {
        // spawn a new laser
        GameObject newLaser = Instantiate(laserPrefab);

        GameObject player = GameObject.FindGameObjectWithTag("Player Body");

        // pass the game manager
        newLaser.GetComponent<EnemyLaser>().gm = gm;

        // position will be that of the enemy
        newLaser.transform.position = transform.position;

        // get rigid body
        Rigidbody laserRb = newLaser.GetComponent<Rigidbody>();

        // calculate direction towards the player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // let the laser face to the player when shoot
        newLaser.transform.LookAt(player.transform.position);

        // give the bullet velocity towards the player
        laserRb.velocity = directionToPlayer * laserSpeed;
    }

    void InitialColor()
    {
        float random = Random.value;

        if(random < em.green)
        {
            health = 1;
        }else if(random < em.green + em.blue)
        {
            health = 2;
            materials[0].color = Color.blue;
        }else
        {
            health = 3;
            materials[0].color = Color.red;
        }
    }

    void changeColor()
    {
        if(materials[0].color == Color.red)
        {
            materials[0].color = Color.blue;
        }else if(materials[0].color == Color.blue)
        {
            Color color = new Color32(0x18, 0xCC, 0x14, 0xFF);
           materials[0].color = color;
        }
    }

    public void KillEnemy()
    {
        // nothing will happen if already dead
        if (currState == State.Dead) return;

        // decrease health
        health--;

        // change color
        changeColor();

        // play sound
        hitsound.Play();

        // if still alive, return
        if (health > 0) return;


        // set the state to dead
        currState = State.Dead;

        // Instantiate dead particle
        ParticleSystem deadEffect = Instantiate(dead, transform.position, Quaternion.Euler(-90f, 0f, 0f));

        // play sound
        deadEffect.GetComponent<AudioSource>().Play();

        //[implement your own effect here]

        //[Example]
        Destroy(gameObject);
        //[End of Example]

        // decrease number of enemies
        em.numEnemies--;
        
        // check winning condition
        gm.HandleEnemyDead();
    }

    void OnTriggerEnter(Collider other)
    {
        // nothing will happen if already dead
        if (currState == State.Dead) return;

        //check if the enemy hit the player
        if (other.CompareTag("Player Body"))
        {
            gm.PlayerHP--;
            gm.SetHPText();
            gm.hurtsound.Play();
            if(gm.PlayerHP <= 0){
                gm.GameOver();
            }
        }

        //check if the enemy reached the floor
        else if (other.CompareTag("Ground"))
        { 
            gm.GameOver();
        }
    }
}

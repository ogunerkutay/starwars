using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{

    private Rigidbody2D rb; // reference to the Rigidbody2D component
    public float speed = 10f; // speed of the spaceship
    
    public GameObject fireballPrefab; // prefab of the fireball object
    public Sprite fireballSprite;
    public float fireballSpeed = 20f; // speed of the fireballs
    public KeyCode shootKey = KeyCode.Space; // key to shoot fireballs

    public GameObject asteroidPrefab; // prefab of the asteroid object
    public Sprite[] asteroidSprites; // array of asteroid sprites
    public float spawnInterval = 1f; // interval between asteroid spawns
    private float timeSinceLastSpawn = 0f; // time since last asteroid spawn
    
    public int score = 0; // score variable


    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Fireball"))
    //     {
    //         Destroy(other.gameObject);
    //         Destroy(gameObject); // destroy the asteroid
    //         score++; // increment the score
    //     }
    // }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Fireball"))
    //     {
    //         Destroy(other.gameObject);
    //     }
    // }

    void SpawnAsteroid()
    {
        
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;

        float spriteWidth = asteroidSprites[0].bounds.size.x;
        float spriteHeight = asteroidSprites[0].bounds.size.y;

        float visibleWidth = screenAspect * 2f * Camera.main.orthographicSize;
        float visibleHeight = 2f * Camera.main.orthographicSize;

        float rightEdgeX = (visibleWidth / 2f) - (spriteWidth / 2f);
        float minSpawnX = Camera.main.transform.position.x + rightEdgeX;
        float maxSpawnX = Camera.main.transform.position.x + (visibleWidth / 2f);

        float x = Random.Range(minSpawnX, maxSpawnX);
        float y = Camera.main.transform.position.y + (visibleHeight / 2f) + (spriteHeight / 2f);

        Vector2 position = new Vector2(x, y);
        GameObject asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
        SpriteRenderer spriteRenderer = asteroid.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = asteroidSprites[Random.Range(0, asteroidSprites.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Load asteroid sprites from the "asteroids" folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("asteroids");
        if (sprites.Length > 0) {
            asteroidSprites = new Sprite[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                asteroidSprites[i] = sprites[i];
            }
        } else {
            Debug.LogWarning("Not enough asteroid sprites found in 'asteroids' folder");
        }

        fireballSprite = Resources.Load<Sprite>("fireball");
        if (fireballSprite is null) {
            Debug.LogWarning("Fireball sprite not found");
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Get input from player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move the spaceship
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        rb.velocity = movement * speed;

        // Shoot fireballs
        if (Input.GetKeyDown(shootKey))
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Vector2 direction = Vector2.right; // default direction is right
            Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
            fireballRb.velocity = direction * fireballSpeed;
        }

        // Spawn asteroids
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            timeSinceLastSpawn = 0f;
            SpawnAsteroid();
        }

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(asteroidPrefab.transform.position);
        if (viewportPosition.x < 0f || viewportPosition.x > 1f || viewportPosition.y < 0f || viewportPosition.y > 1f) {
            Debug.Log("Asteroid is not visible on screen");
        }
    }
}
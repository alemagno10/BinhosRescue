using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PlayerController : Entity {
    public Rigidbody2D body;
    public float speed;
    public float jumpForce;
    public Vector2 jump;
    public bool isGrounded = true;
    public AudioClip damageSound; // AudioClip for damage sound
    public AudioClip jumpSound; // AudioClip for jump sound
    public AudioClip deathSound; // AudioClip for death sound
    public AudioClip portalSound; // AudioClip for portal sound
    public float damageSoundVolume = 0.5f; // Volume regulator for the damage sound
    public float jumpSoundVolume = 0.5f; // Volume regulator for the jump sound
    public float deathSoundVolume = 0.5f; // Volume regulator for the death sound
    public float portalSoundVolume = 0.5f; // Volume regulator for the portal sound

    private AudioSource audioSource; // AudioSource component to play the sound
    private SceneChanger sceneChanger;

    void Start() {
        speed = 7;
        jumpForce = 7;
        jump = new Vector2(0.0f, 2.0f);
        sceneChanger = FindObjectOfType<SceneChanger>();

        // Initialize the audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            // Add an AudioSource component if not already present
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update() {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);
        
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && isGrounded) {
            body.AddForce(jump * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            // Play the jump sound at the specified volume
            if (audioSource != null && jumpSound != null) {
                audioSource.PlayOneShot(jumpSound, jumpSoundVolume);
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;  // Marks as on the ground
        } else if (collision.gameObject.CompareTag("Die")) {
            handleDeath(); // Handle death when colliding with deadly objects
        }   
    } 

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Portal")) {
            if (audioSource != null && portalSound != null) {
                audioSource.PlayOneShot(portalSound, portalSoundVolume);
            }
            sceneChanger.Next("Level2-Onca"); 
        }

        if (collider.gameObject.CompareTag("Portal1")) {
            if (audioSource != null && portalSound != null) {
                audioSource.PlayOneShot(portalSound, portalSoundVolume);
            }
            sceneChanger.Next("VictoryMenu"); 
        }
    }   

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        // Play the damage sound at the specified volume
        if (audioSource != null && damageSound != null) {
            audioSource.PlayOneShot(damageSound, damageSoundVolume);
        }
    }

    public override void handleDeath() {
        if (audioSource != null && deathSound != null) {
            audioSource.PlayOneShot(deathSound, deathSoundVolume);
        } 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

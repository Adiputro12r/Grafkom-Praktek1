using UnityEngine;

public class Character : MonoBehaviour {
  [SerializeField] private GameManager gameManager;
  [SerializeField] private GameObject character;
  [SerializeField] private ParticleSystem deathParticles;
  

  // Di dalam Character.cs

  private void OnTriggerEnter(Collider other) {
    // 1. Cek apakah objek yang disentuh punya tag "Collectible"
    if (other.CompareTag("Collectible")) {
        
        // 2. Ambil skrip Collectible dari objek itu
        Collectible item = other.GetComponent<Collectible>();
        
        if (item != null) {
            // 3. Beri tahu GameManager item apa yang kita ambil
            gameManager.CollectItem(item.type);
            
            // 4. Hancurkan objek sampah agar hilang
            Destroy(other.gameObject);
        }
    }
}

  private void OnCollisionEnter(Collision collision) {
    // Only collide with vehicles if we're not already done so.
    if (collision.gameObject.CompareTag("Vehicle") && character.activeSelf) {
      Kill(collision.GetContact(0).point);
    }
  }

  public void Kill(Vector3 collisionPoint) {
    // Hide the character model
    character.SetActive(false);

    // Orient the particles relative to the collision.
    deathParticles.transform.position = collisionPoint;
    deathParticles.transform.LookAt(transform.position + Vector3.up);

    // Show the particles.
    deathParticles.Play();

    // Tell the GameManager we've collided.
    gameManager.PlayerCollision();
  }

  public void Reset() {
    // Re-enable the character model.
    character.SetActive(true);
    // Remove any left over particles.
    deathParticles.Clear();
  }
}
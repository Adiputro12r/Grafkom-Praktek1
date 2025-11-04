using UnityEngine;

public class Character : MonoBehaviour {
  [SerializeField] private GameManager gameManager;
  [SerializeField] private GameObject character;
  [SerializeField] private ParticleSystem deathParticles;

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
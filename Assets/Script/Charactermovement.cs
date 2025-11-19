using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameManager1 gameManager1;
    [SerializeField] private Transform characterModel; // Transform untuk rotasi
    
    [Header("Parameters")]
    [SerializeField] private float moveDuration = 0.2f;

    private Vector2Int characterPos;

    public void Init(Vector2Int startPos) {
        characterPos = startPos;
    }

    void Update() {
        // Hanya bergerak jika game dalam state Ready
        if (gameManager1.gameState == GameManager1.GameState.Ready) {
            Vector2Int moveDirection = Vector2Int.zero;

            // Mengganti .wasPressedThisFrame menjadi .isPressed
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) {
                characterModel.localRotation = Quaternion.Euler(0, 90, 0);
                moveDirection.y = 1;
            } else if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) {
                characterModel.localRotation = Quaternion.Euler(0, -90, 0);
                moveDirection.y = -1;
            } else if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) {
                characterModel.localRotation = Quaternion.Euler(0, 0, 0);
                moveDirection.x = -1;
            } else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) {
                characterModel.localRotation = Quaternion.Euler(0, 180, 0);
                moveDirection.x = 1;
            }

            if (moveDirection != Vector2Int.zero) {
                Vector2Int destination = characterPos + moveDirection;
                
                // Pengecekan tabrakan menggunakan data dari GameManager
                if (InStartArea(destination) || ((destination.y >= 0) && !gameManager1.obstacles[destination.y].locations.Contains(destination.x))) {
                    
                    characterPos = destination; // Update posisi grid
                    StartCoroutine(MoveCharacter());
                    
                    // Beritahu GameManager bahwa karakter telah bergerak
                    gameManager1.HandleCharacterMovement(characterPos); 
                }
            }
        }
        
        // --- Logika Camera Follow di sini jika ingin memindahkannya dari GameManager ---
        Vector3 cameraPosition = new(transform.position.x + 2, 4, transform.position.z - 3);
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, -1, 5);
        Camera.main.transform.position = cameraPosition;
    }

    private IEnumerator MoveCharacter() {
        gameManager1.gameState = GameManager1.GameState.Moving; // Ganti state game
        float elapsedTime = 0f;

        // Tentukan yHeight berdasarkan terrain (mengakses data dari GameManager)
        float yHeight = 0.2f;
        if (characterPos.y >= 0) {
            yHeight = gameManager1.obstacles[characterPos.y].terrainHeight;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = new(characterPos.x, yHeight, characterPos.y);

        Quaternion startRotation = characterModel.localRotation;

        while (elapsedTime < moveDuration) {
            float percent = elapsedTime / moveDuration;

            Vector3 newPos = Vector3.Lerp(startPos, endPos, percent);
            transform.position = newPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        characterModel.localRotation = startRotation;

        if (gameManager1.gameState == GameManager1.GameState.Moving) {
            gameManager1.gameState = GameManager1.GameState.Ready; // Kembali ke state Ready
        }
    }
    
    // Dipindahkan dari GameManager
    private bool InStartArea(Vector2Int location) {
        if ((location.y > -5) && (location.y < 0) && (location.x > -6) && (location.x < 6)) {
            return true;
        }
        return false;
    }
}

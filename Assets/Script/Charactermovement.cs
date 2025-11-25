using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour {
    [Header("References")]
    // Pastikan nama class sesuai dengan script GameManager kamu (di file yang kamu upload namanya 'GameManager')
    [SerializeField] private GameManager1 gameManager; 
    [SerializeField] private Transform characterModel; // Transform untuk rotasi visual
    [SerializeField] private Character characterScript; // Referensi untuk memanggil fungsi Kill()
    
    [Header("Parameters")]
    [SerializeField] private float moveDuration = 0.2f;

    private Vector2Int characterPos;
    private Transform currentLog; // Menyimpan log yang sedang dinaiki
    private float logOffset; // <--- TAMBAHKAN INI
    public void Init(Vector2Int startPos) {
        characterPos = startPos;
    }

    void Update() {
        // Hanya bergerak jika game dalam state Ready
        // Perhatikan: Akses enum via nama class 'GameManager'
        if (gameManager != null && gameManager.gameState == GameManager1.GameState.Ready) { 
            
            Vector2Int moveDirection = Vector2Int.zero;

            // Input Logic (Menggunakan New Input System)
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) {
                characterModel.localRotation = Quaternion.Euler(0, 90, 0); // Koreksi rotasi (biasanya 0 hadap depan)
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
                if (moveDirection.x > 0) // Jika gerak KANAN
                    {
                    characterPos.x = Mathf.FloorToInt(transform.position.x);
                    }
                else if (moveDirection.x < 0) // Jika gerak KIRI
                    {
                    characterPos.x = Mathf.CeilToInt(transform.position.x);
                    }
                else 
                    {
                    // Jika gerak MAJU/MUNDUR (Y axis), pakai Round biasa aman
                    characterPos.x = Mathf.RoundToInt(transform.position.x);
                    }
                Vector2Int destination = characterPos + moveDirection;
                
                // Cek apakah tujuan valid (bukan obstacle pohon/batu)
                if (InStartArea(destination) || ((destination.y >= 0) && !gameManager.obstacles[destination.y].locations.Contains(destination.x))) {
                    
                    characterPos = destination; // Update grid logika
                    StartCoroutine(MoveCharacter());
                    
                    // Beritahu GameManager (update score, spawn terrain baru)
                    // (Logika ini ada di Update GameManager asli, tapi jika kamu memindahkannya ke sini:)
                    gameManager.HandleCharacterMovement(characterPos); 
                }
            }
        }

        // --- LOGIKA RIDING LOG (NAIK KAYU) ---
        // Jika sedang diam (Ready) DAN kita punya pijakan Log
        if (gameManager.gameState == GameManager1.GameState.Ready && currentLog != null)
        {
            Vector3 newPos = transform.position;

            // 1. Hitung posisi target: Di mana log berada + offset kita
            float targetX = currentLog.position.x + logOffset;

            // 2. CLAMP (Kunci) posisi X pemain agar tidak lewat dari -6 atau 6
            // Ini membuat efek pemain tertahan "tembok" pinggir layar
            newPos.x = Mathf.Clamp(targetX, -5f, 5f);
            

            transform.position = newPos;

            // 4. Update posisi Grid integer
            characterPos.x = Mathf.RoundToInt(transform.position.x);

            // 5. DETEKSI JATUH
            // Cek selisih antara posisi Log asli (targetX) dengan posisi Pemain yang tertahan (newPos.x)
            // Jika selisihnya > 0.5f, berarti log sudah pergi meninggalkan pemain.
            if (Mathf.Abs(targetX - newPos.x) > 0.5f) 
            {
                currentLog = null; // Lepas dari log
                if(characterScript != null) characterScript.Kill(transform.position); // Mati
            }
        }
        
        // --- Camera Follow ---
        Vector3 cameraPosition = new(transform.position.x + 2, 4, transform.position.z - 3);
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, -1, 5);
        if (Camera.main != null) Camera.main.transform.position = cameraPosition;
    }

    private IEnumerator MoveCharacter() {
        // Ganti state ke Moving
        if(gameManager != null) gameManager.gameState = GameManager1.GameState.Moving; 
        
        // Lepaskan Log saat mulai melompat agar tidak "terseret" saat di udara
        currentLog = null;

        float elapsedTime = 0f;

        // Tentukan tinggi lompatan berdasarkan terrain target
        float yHeight = 0.2f;
        if (characterPos.y >= 0 && gameManager != null) {
            yHeight = gameManager.obstacles[characterPos.y].terrainHeight;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = new(characterPos.x, yHeight, characterPos.y);
        Quaternion startRotation = characterModel.localRotation;

        // Animasi Lompat
        while (elapsedTime < moveDuration) {
            float percent = elapsedTime / moveDuration;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, percent);
            
            // Tambahkan sedikit arc (lengkungan) ke atas saat melompat (opsional)
            // newPos.y += Mathf.Sin(percent * Mathf.PI) * 0.5f; 

            transform.position = newPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        characterModel.localRotation = startRotation;

        // --- CEK PIJAKAN SETELAH MENDARAT ---
        CheckSurface();

        

        // Kembalikan ke state Ready hanya jika belum mati
        if (gameManager != null && gameManager.gameState == GameManager1.GameState.Moving) {
            gameManager.gameState = GameManager1.GameState.Ready;
        }
    }

    private void CheckSurface() {
        // Tembakkan sinar (Raycast) dari perut karakter ke bawah
        RaycastHit hit;
        // Posisi asal + 0.5 ke atas, arah ke bawah, jarak 2 unit
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2.0f)) 
        {
            if (hit.collider.CompareTag("Log")) 
            {
                // Jika kena Log, simpan referensinya agar update loop bisa mengikutinya
                currentLog = hit.transform;
                logOffset = transform.position.x - currentLog.position.x;
            }
            else if (hit.collider.CompareTag("Water")) 
            {
                // Jika kena Air, MATI
                currentLog = null;
                if(characterScript != null) characterScript.Kill(transform.position);
            }
            else 
            {
                // Jika kena jalan biasa (Road/Grass)
                currentLog = null;
            }
        }
    }
    
    private bool InStartArea(Vector2Int location) {
        if ((location.y > -5) && (location.y < 0) && (location.x > -6) && (location.x < 6)) {
            return true;
        }
        return false;
    }
}
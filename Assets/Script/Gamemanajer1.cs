using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour {
    // --- REFERENSI KE SCRIPT LAIN (BARU) ---
    [Header("Script References")]
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private TaskSystem taskSystem;
    [SerializeField] private UIManager1 uiManager;

    [Header("Game objects")]
    [SerializeField] private Transform character;
    [SerializeField] private Transform characterModel;
    [SerializeField] private Transform terrainHolder;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    [Header("Terrain objects")]
    [SerializeField] private Grass grassPrefab;
    [SerializeField] private Road roadPrefab;
    [SerializeField] private Home homePrefab;

    [Header("Game parameters")]
    [SerializeField] private int spawnDistance = 20;
    public TaskSystem TaskSystemRef => taskSystem;

    // ENUM & STATE
    public enum GameState {
        Tutorial,
        Ready,
        Moving,
        Dead,
        Won
    }
    public GameState gameState; // Dijadikan public agar CharacterMovement bisa cek
    
    // VARIABEL LEVEL
    private int spawnLocation;
    
    // PUBLIC untuk diakses CharacterMovement
    public List<(float terrainHeight, HashSet<int> locations, GameObject obj)> obstacles = new();
    
    private int score = 0;
    private bool isGameActive = false;

    void Awake() {
        NewLevel();
    }

    /// Fungsi ini akan dipanggil oleh OnClick() Button di GameOverPanel dan WinPanel.
    public void RestartGame() {
        NewLevel();
    }

    private void NewLevel() {
        gameState = GameState.Ready;

        // Reset semua melalui UIManager dan TaskSystem
        uiManager.HideAllPanels();
        taskSystem.ResetTask();
        score = 0;
        scoreText.text = "0";

        // Reset Character
        Vector2Int startPos = new Vector2Int(0, -1);
        character.position = new Vector3(0, 0.2f, -1);
        character.GetComponent<Character>().Reset();
        characterMovement.Init(startPos); // Inisialisasi posisi di CharacterMovement

        // Remove all terrain
        obstacles.Clear();
        foreach (Transform child in terrainHolder) {
            Destroy(child.gameObject);
        }

        // Reset level, dan regenerate
        spawnLocation = 0;
        for (int i = 0; i < spawnDistance; i++) {
            SpawnObstacle();
        }

        if (isGameActive) {
            gameState = GameState.Ready;
            uiManager.HideTutorialPanel();
        } else {
            gameState = GameState.Tutorial; // Mengunci player
            uiManager.ShowTutorialPanel();
        }
    }

    public void StartGameFromTutorial() {
        if (gameState != GameState.Tutorial) return;

        gameState = GameState.Ready;
        isGameActive = true;
        uiManager.HideTutorialPanel();
    }

    private void SpawnObstacle() {
        // ... (Logika SpawnObstacle yang sama) ...
        float roadProbability = Mathf.Lerp(0.5f, 0.9f, spawnLocation / 250f);

        if (Random.value < roadProbability) {
            Road road = Instantiate(roadPrefab, terrainHolder);
            obstacles.Add((0.1f, road.Init(spawnLocation), road.gameObject));
            road.gameObject.name = $"{spawnLocation} - Road";
        } else {
            if (Random.value < 0.5f) {
                Grass grass = Instantiate(grassPrefab, terrainHolder);
                obstacles.Add((0.2f, grass.Init(spawnLocation), grass.gameObject));
                grass.gameObject.name = $"{spawnLocation} - Grass";
            } else {
                Home home = Instantiate(homePrefab, terrainHolder);
                obstacles.Add((0.2f, home.Init(spawnLocation), home.gameObject));
                home.gameObject.name = $"{spawnLocation} - Home";
            }
        }
        spawnLocation++;
    }

    // --- FUNGSI DIPANGGIL OLEH CharacterMovement ---

    // Dipanggil saat karakter bergerak
    public void HandleCharacterMovement(Vector2Int newPos) {
        // Update score
        if ((newPos.y + 1) > score) {
            score = newPos.y + 1;
            scoreText.text = $"{score}";
        }

        // Spawn baru
        while (obstacles.Count < (newPos.y + spawnDistance)) {
            SpawnObstacle();

            // Hancurkan Terrain lama
            int oldIndex = newPos.y - spawnDistance;
            if ((oldIndex >= 0) && (obstacles[oldIndex].obj != null)) {
                Destroy(obstacles[oldIndex].obj);
            }
        }

        // Cek Game Over (Mundur terlalu jauh)
        if (newPos.y < (score - 10)) {
            character.GetComponent<Character>().Kill(character.transform.position + new Vector3(0, 0.2f, 0.5f));
            PlayerCollision();
        }
    }

    // Dipanggil saat karakter bertabrakan
    public void PlayerCollision() {
        gameState = GameState.Dead;
        uiManager.ShowGameOverPanel(taskSystem.GetTaskProgress());
    }

    // Dipanggil saat TaskSystem menyatakan kemenangan
    public void WinGame() {
        gameState = GameState.Won;
        // SoundManager.Instance.Play("Win"); // Pastikan SoundManager ada
        uiManager.ShowWinPanel(taskSystem.GetTaskProgress());
    }
}

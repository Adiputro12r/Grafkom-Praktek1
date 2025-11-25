using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour {
    
    // --- STRUKTUR DATA UNTUK INSPECTOR ---
    [System.Serializable]
    public struct TerrainData {
        public string name;           // Nama biar gampang dibaca (misal: "Hutan")
        public GameObject prefab;     // Prefab (Grass/Road/Home/River)
        public float weight;          // Peluang muncul (semakin besar angka, semakin sering)
        public float height;          // Tinggi terrain (Grass/Home = 0.2, Road/River = 0.1)
        [Range(0, 1)] public float collectibleChance; // 0.0 - 1.0 (Peluang muncul sampah)
    }

    [Header("Modular Level Settings")]
    [SerializeField] private List<TerrainData> levelTerrains; // <--- ISI INI DI INSPECTOR

    [Header("Script References")]
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private TaskSystem taskSystem;
    [SerializeField] private UIManager1 uiManager;

    [Header("Game objects")]
    [SerializeField] private Transform character;
    [SerializeField] private Transform terrainHolder;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    [Header("Game parameters")]
    [SerializeField] private int spawnDistance = 20;
    
    public TaskSystem TaskSystemRef => taskSystem;

    public enum GameState { Tutorial, Ready, Moving, Dead, Won }
    public GameState gameState;
    
    private int spawnLocation;
    public List<(float terrainHeight, HashSet<int> locations, GameObject obj)> obstacles = new();
    
    private int score = 0;
    private bool isGameActive = false;

    void Awake() {
        NewLevel();
    }

    public void RestartGame() => NewLevel();

    private void NewLevel() {
        gameState = GameState.Ready;
        uiManager.HideAllPanels();
        taskSystem.ResetTask();
        score = 0;
        scoreText.text = "0";

        Vector2Int startPos = new Vector2Int(0, -1);
        character.position = new Vector3(0, 0.2f, -1);
        character.GetComponent<Character>().Reset();
        characterMovement.Init(startPos);

        obstacles.Clear();
        foreach (Transform child in terrainHolder) Destroy(child.gameObject);

        spawnLocation = 0;
        for (int i = 0; i < spawnDistance; i++) {
            SpawnObstacle();
        }

        if (isGameActive) {
            gameState = GameState.Ready;
            uiManager.HideTutorialPanel();
        } else {
            gameState = GameState.Tutorial;
            uiManager.ShowTutorialPanel();
        }
    }

    public void StartGameFromTutorial() {
        if (gameState != GameState.Tutorial) return;
        gameState = GameState.Ready;
        isGameActive = true;
        uiManager.HideTutorialPanel();
    }

    // --- FUNGSI UTAMA SPAWN ---
    private void SpawnObstacle() {
        if (levelTerrains.Count == 0) return;

        // 1. Pilih Terrain secara acak berdasarkan Weight
        TerrainData selectedData = GetRandomTerrain();

        // 2. Spawn Prefab
        GameObject newObj = Instantiate(selectedData.prefab, terrainHolder);
        newObj.name = $"{spawnLocation} - {selectedData.name}";

        HashSet<int> busyLocations = new HashSet<int>();

        // 3. Cek Script apa yang ada di prefab, lalu panggil Init dengan data dari Inspector
        if (newObj.TryGetComponent(out Grass grass)) {
            // Kirim collectibleChance dari Inspector ke Grass
            busyLocations = grass.Init(spawnLocation, selectedData.collectibleChance);
        }
        else if (newObj.TryGetComponent(out Home home)) {
            // Kirim collectibleChance dari Inspector ke Home
            busyLocations = home.Init(spawnLocation, selectedData.collectibleChance);
        }
        else if (newObj.TryGetComponent(out Road road)) {
            // Road tidak butuh chance
            busyLocations = road.Init(spawnLocation);
        }
        else if (newObj.TryGetComponent(out River river)) {
            // River tidak butuh chance
            busyLocations = river.Init(spawnLocation);
        }

        // 4. Masukkan ke list obstacles dengan Height dari Inspector
        obstacles.Add((selectedData.height, busyLocations, newObj));

        spawnLocation++;
    }

    // Algoritma memilih acak berdasarkan "Weight" (Bobot)
    private TerrainData GetRandomTerrain() {
        float totalWeight = 0;
        foreach (var t in levelTerrains) totalWeight += t.weight;

        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;

        foreach (var t in levelTerrains) {
            currentSum += t.weight;
            if (randomValue <= currentSum) {
                return t;
            }
        }
        return levelTerrains[0];
    }

    // --- FUNGSI LAIN (TIDAK BERUBAH) ---
    public void HandleCharacterMovement(Vector2Int newPos) {
        if ((newPos.y + 1) > score) {
            score = newPos.y + 1;
            scoreText.text = $"{score}";
        }

        while (obstacles.Count < (newPos.y + spawnDistance)) {
            SpawnObstacle();
            int oldIndex = newPos.y - spawnDistance;
            if ((oldIndex >= 0) && (obstacles[oldIndex].obj != null)) {
                Destroy(obstacles[oldIndex].obj);
            }
        }

        if (newPos.y < (score - 10)) {
            character.GetComponent<Character>().Kill(character.transform.position + new Vector3(0, 0.2f, 0.5f));
            PlayerCollision();
        }
    }

    public void PlayerCollision() {
        gameState = GameState.Dead;
        uiManager.ShowGameOverPanel();
    }

    public void WinGame() {
        gameState = GameState.Won;
        uiManager.ShowWinPanel();
    }
}
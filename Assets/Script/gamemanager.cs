using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
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
  [SerializeField] private float moveDuration = 0.2f;
  [SerializeField] private int spawnDistance = 20;

  [Header("Task System")]
[SerializeField] private int plasticGoal = 10;
[SerializeField] private int ironGoal = 5;
[SerializeField] private int stickGoal = 8;

private int plasticCollected = 0;
private int ironCollected = 0;
private int stickCollected = 0;

[Header("Canvas HUD Texts")]
[SerializeField] private TMPro.TextMeshProUGUI plasticTaskText; // Teks HUD
[SerializeField] private TMPro.TextMeshProUGUI ironTaskText; // Teks HUD
[SerializeField] private TMPro.TextMeshProUGUI stickTaskText; // Teks HUD

[Header("UI Panels")]
[SerializeField] private GameObject gameOverPanel;
[SerializeField] private GameObject winPanel; 
[SerializeField] private GameObject tutorialPanelContainer;
// Teks di panel Game Over (HANYA ANGKA)
[Header("Game Over Panel Texts")]
[SerializeField] private TMPro.TextMeshProUGUI gameOverPlasticText;
[SerializeField] private TMPro.TextMeshProUGUI gameOverIronText;
[SerializeField] private TMPro.TextMeshProUGUI gameOverStickText;

// Teks di panel Menang (HANYA ANGKA)
[Header("Win Panel Texts")]
[SerializeField] private TMPro.TextMeshProUGUI winPlasticText;
[SerializeField] private TMPro.TextMeshProUGUI winIronText;
[SerializeField] private TMPro.TextMeshProUGUI winStickText;


  enum GameState {
    Tutorial,
   Ready,
    Moving,
    Dead,
    Won
  }
  private GameState gameState;
  private Vector2Int characterPos;
  private int spawnLocation;
  private List<(float terrainHeight, HashSet<int> locations, GameObject obj)> obstacles = new();
  private int score = 0;
  private bool isGameActive = false;

  void Awake() {
    // Initialise all the starting state.
    NewLevel();
  }

  /// Fungsi ini akan dipanggil oleh OnClick() Button di GameOverPanel dan WinPanel.
  public void RestartGame() {
    NewLevel();
  }

  private void NewLevel() {
    gameState = GameState.Ready;

    // Hide panels
    if (gameOverPanel != null) gameOverPanel.SetActive(false);
    if (winPanel != null) winPanel.SetActive(false);

    // Reset task progress
    plasticCollected = 0;
    ironCollected = 0;
    stickCollected = 0;
    UpdateTaskUI(); // Reset teks HUD

    // Reset character position
    characterPos = new Vector2Int(0, -1);
    character.position = new Vector3(0, 0.2f, -1);
    character.GetComponent<Character>().Reset();

    // Reset the score
    score = 0;
    scoreText.text = "0";

    // Remove all terrain
    obstacles.Clear();
    foreach (Transform child in terrainHolder) {
      Destroy(child.gameObject);
    }

    // Reset level, and regenerate
    spawnLocation = 0;
    for (int i = 0; i < spawnDistance; i++) {
      SpawnObstacle();
    }
    if (isGameActive) {
      // Ini adalah RESTART, langsung main
      gameState = GameState.Ready;
      if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(false);
    } 
    else {
      // Ini adalah PERTAMA KALI MAIN, kunci player & tampilkan tutorial
      gameState = GameState.Tutorial; // <-- Mengunci player
      if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(true);
    }
  }

  public void StartGameFromTutorial() {
    if (gameState != GameState.Tutorial) return; // Hanya jalan jika game masih terkunci

    gameState = GameState.Ready;    // 1. Buka kunci player
    isGameActive = true;            // 2. Tandai tutorial selesai
    
    
  }

  private void SpawnObstacle() {
    // Spawn more roads the further we get, at 250 have 90% chance of a road.
    float roadProbability = Mathf.Lerp(0.5f, 0.9f, spawnLocation / 250f);

    if (Random.value < roadProbability)
    {
      // Create road with terrain height of 0.1f.
      Road road = Instantiate(roadPrefab, terrainHolder);
      obstacles.Add((0.1f, road.Init(spawnLocation), road.gameObject));
      road.gameObject.name = $"{spawnLocation} - Road";
    }
    else
    {
      if (Random.value < 0.5f)
      {
        // Create grass with terrain height of 0.2f.
        Grass grass = Instantiate(grassPrefab, terrainHolder);
        obstacles.Add((0.2f, grass.Init(spawnLocation), grass.gameObject));
        grass.gameObject.name = $"{spawnLocation} - Grass";
      }
      else
      {
        // Create home with terrain height of 0.2f.
        Home home = Instantiate(homePrefab, terrainHolder);
        obstacles.Add((0.2f, home.Init(spawnLocation), home.gameObject));
        home.gameObject.name = $"{spawnLocation} - Home";
      }
    }

    // Update to the next free location
    spawnLocation++;
  }

  private bool InStartArea(Vector2Int location) {
    // Movement anywhere in the starting region is allowed.
    if ((location.y > -5) && (location.y < 0) && (location.x > -6) && (location.x < 6)) {
      return true;
    }
    return false;
  }

  // Update is called once per frame
  void Update() {
    // Detect arrow key presses.
    if (gameState == GameState.Ready) {
      Vector2Int moveDirection = Vector2Int.zero;
      // Single if/else don't want to move diagonally.

      // ---- PERUBAHAN DI SINI ----
      // Mengganti .wasPressedThisFrame menjadi .isPressed
      if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) {
        character.localRotation = Quaternion.identity;
        moveDirection.y = 1;
      } else if (Keyboard.current.downArrowKey.isPressed|| Keyboard.current.sKey.isPressed) {
        character.localRotation = Quaternion.Euler(0, 180, 0);
        moveDirection.y = -1;
      } else if (Keyboard.current.leftArrowKey.isPressed|| Keyboard.current.aKey.isPressed) {
        character.localRotation = Quaternion.Euler(0, -90, 0);
        moveDirection.x = -1;
      } else if (Keyboard.current.rightArrowKey.isPressed|| Keyboard.current.dKey.isPressed) {
        character.localRotation = Quaternion.Euler(0, 90, 0);
        moveDirection.x = 1;
      }
      // --------------------------

      // If the user wants to move
      if (moveDirection != Vector2Int.zero) {
        Vector2Int destination = characterPos + moveDirection;
        // In the start area there are no obstacles so you can move anywhere.
        if (InStartArea(destination) || ((destination.y >= 0) && !obstacles[destination.y].locations.Contains(destination.x))) {
          // Update our character grid coordinate.
          characterPos = destination;
          // Call coroutine to move the character object.
          StartCoroutine(MoveCharacter());
          // Update score if necessary.
          if ((destination.y + 1) > score) {
            score = destination.y + 1;
            scoreText.text = $"{score}";
          }
        }

        // Spawn new obstacles if necessary
        while (obstacles.Count < (characterPos.y + spawnDistance)) {
          SpawnObstacle();

          // Destroy old terrain objects as we progress
          int oldIndex = characterPos.y - spawnDistance;
          if ((oldIndex >= 0) && (obstacles[oldIndex].obj != null)) {
            Destroy(obstacles[oldIndex].obj);
          }
        }

        // If we've gone back too far end the game.
        if (characterPos.y < (score - 10)) {
          character.GetComponent<Character>().Kill(character.transform.position + new Vector3(0, 0.2f, 0.5f));
        }
      }
    }

    // Can only use our shortcut to reset the level when we're dead.
    // Biarkan yang ini .wasPressedThisFrame agar tidak me-restart level terus menerus
   

    // Camera follow at (+2, 4, -3)
    Vector3 cameraPosition = new(character.position.x + 2, 4, character.position.z - 3);

    // Limit camera movement in x direction.
    // Only follow the character as it moves to -3 and +3.
    // The camera offset is +2 so that's -1 to +5 in the camera x position.
    cameraPosition.x = Mathf.Clamp(cameraPosition.x, -1, 5);

    Camera.main.transform.position = cameraPosition;
  }

  private IEnumerator MoveCharacter() {
    gameState = GameState.Moving;
    float elapsedTime = 0f;

    // The yHeight changes if we're on grass or road.
    float yHeight = 0.2f;
    if (characterPos.y >= 0) {
      yHeight = obstacles[characterPos.y].terrainHeight;
    }

    Vector3 startPos = character.position;
    Vector3 endPos = new(characterPos.x, yHeight, characterPos.y);

    Quaternion startRotation = characterModel.localRotation;

    while (elapsedTime < moveDuration) {
      // How far through the animation are we.
      float percent = elapsedTime / moveDuration;

      // Update the character position
      Vector3 newPos = Vector3.Lerp(startPos, endPos, percent);
      character.position = newPos;

      // Update the elapsed time
      elapsedTime += Time.deltaTime;

      yield return null;
    }

    // Ensure we're at the end.
    character.position = endPos;
    characterModel.localRotation = startRotation;

    // Need to check we're still in moving at the end.
    // If we're dead we don't want to go back to ready.
    if (gameState == GameState.Moving) {
      gameState = GameState.Ready;
    }
  }

  // 1. Dipanggil Character
  public void CollectItem(CollectibleType type) {
    if (gameState == GameState.Dead || gameState == GameState.Won) return;

    if (type == CollectibleType.Plastic) plasticCollected++;
    else if (type == CollectibleType.Iron) ironCollected++;
    else if (type == CollectibleType.Stick) stickCollected++;

    UpdateTaskUI();
    CheckTaskCompletion();
}

// 2. Update Teks HUD (pojok layar)
  private void UpdateTaskUI() {
    if (plasticTaskText != null) plasticTaskText.text = $"{plasticCollected}/{plasticGoal}";
    if (ironTaskText != null) ironTaskText.text = $"{ironCollected}/{ironGoal}";
    if (stickTaskText != null) stickTaskText.text = $"{stickCollected}/{stickGoal}";
  }

// 3. Cek Kemenangan
  private void CheckTaskCompletion() {
    if (gameState == GameState.Won) return;
    if (plasticCollected >= plasticGoal && ironCollected >= ironGoal && stickCollected >= stickGoal) {
        gameState = GameState.Won;
        SoundManager.Instance.Play("Win");
        ShowWinPanel();
    }
  }

// 4. Tampilkan Panel (Kalah / Menang)
  private void ShowGameOverPanel() {
    gameOverPanel.SetActive(true);
    UpdateFinalTrashText(); // Panggil fungsi skor akhir
  }

  private void ShowWinPanel() {
    winPanel.SetActive(true);
    UpdateFinalTrashText(); // Panggil fungsi skor akhir
  }

// 5. Update Teks Skor Akhir (Hanya Angka)
  private void UpdateFinalTrashText() {
    string plasticScore = $"{plasticCollected}/{plasticGoal}";
    string ironScore = $"{ironCollected}/{ironGoal}";
    string stickScore = $"{stickCollected}/{stickGoal}";

    if (gameOverPlasticText != null) gameOverPlasticText.text = plasticScore;
    if (gameOverIronText != null) gameOverIronText.text = ironScore;
    if (gameOverStickText != null) gameOverStickText.text = stickScore;

    if (winPlasticText != null) winPlasticText.text = plasticScore;
    if (winIronText != null) winIronText.text = ironScore;
    if (winStickText != null) winStickText.text = stickScore;
  }
public void PlayerCollision()
  {
    // When we collide, we'll simply update the game state.
    gameState = GameState.Dead;
    ShowGameOverPanel();
  }
}
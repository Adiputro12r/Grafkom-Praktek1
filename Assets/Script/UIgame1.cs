using UnityEngine;

public class UIManager1 : MonoBehaviour {
    [Header("Canvas HUD Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI plasticTaskText; // Teks HUD
    [SerializeField] private TMPro.TextMeshProUGUI ironTaskText; // Teks HUD
    [SerializeField] private TMPro.TextMeshProUGUI stickTaskText; // Teks HUD

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel; 
    [SerializeField] private GameObject tutorialPanelContainer;

    [Header("Game Over Panel Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI gameOverPlasticText;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverIronText;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverStickText;

    [Header("Win Panel Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI winPlasticText;
    [SerializeField] private TMPro.TextMeshProUGUI winIronText;
    [SerializeField] private TMPro.TextMeshProUGUI winStickText;

    [Header("References")]
    [SerializeField] private GameManager1 gameManager;

    public void HideAllPanels() {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(false);
    }
    
    public void HideTutorialPanel() {
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(false);
    }
    
    public void ShowTutorialPanel() {
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(true);
    }

    // Dipanggil dari TaskSystem
    public void UpdateTaskUI(TaskSystem.TaskProgress progress) {
        if (plasticTaskText != null) plasticTaskText.text = $"{progress.plastic}/{progress.plasticG}";
        if (ironTaskText != null) ironTaskText.text = $"{progress.iron}/{progress.ironG}";
        if (stickTaskText != null) stickTaskText.text = $"{progress.stick}/{progress.stickG}";
    }

    public void ShowGameOverPanel(TaskSystem.TaskProgress progress) {
        gameOverPanel.SetActive(true);
        UpdateFinalTrashText(progress);
    }

    public void ShowWinPanel(TaskSystem.TaskProgress progress) {
        SoundManager.Instance.Play("Win");
        winPanel.SetActive(true);
        UpdateFinalTrashText(progress);
    }

    // Dipanggil saat game berakhir (Menang/Kalah)
    private void UpdateFinalTrashText(TaskSystem.TaskProgress progress) {
        string plasticScore = $"{progress.plastic}/{progress.plasticG}";
        string ironScore = $"{progress.iron}/{progress.ironG}";
        string stickScore = $"{progress.stick}/{progress.stickG}";

        if (gameOverPlasticText != null) gameOverPlasticText.text = plasticScore;
        if (gameOverIronText != null) gameOverIronText.text = ironScore;
        if (gameOverStickText != null) gameOverStickText.text = stickScore;

        if (winPlasticText != null) winPlasticText.text = plasticScore;
        if (winIronText != null) winIronText.text = ironScore;
        if (winStickText != null) winStickText.text = stickScore;
    }
    
    // Fungsi ini harus dihubungkan ke Button Restart di Inspector
    public void OnRestartButtonClick()
    {
        gameManager.RestartGame();
    }
    
    // Fungsi ini harus dihubungkan ke Button Start di Tutorial Panel
    public void OnTutorialStartButtonClick()
    {
        gameManager.StartGameFromTutorial();
    }
}

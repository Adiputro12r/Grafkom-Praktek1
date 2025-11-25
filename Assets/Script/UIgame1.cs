using UnityEngine;
using UnityEngine.SceneManagement; // Wajib untuk pindah scene

public class UIManager1 : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel; 
    [SerializeField] private GameObject tutorialPanelContainer;

    [Header("Settings")]
    [SerializeField] private string nextStageName; // Nama Scene selanjutnya

    [Header("References")]
    [SerializeField] private GameManager1 gameManager;

    // --- LOGIKA PANEL (TANPA ARGUMEN) ---

    public void HideAllPanels() {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(false);
    }
    
    public void ShowTutorialPanel() {
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(true);
    }

    public void HideTutorialPanel() {
        if (tutorialPanelContainer != null) tutorialPanelContainer.SetActive(false);
    }

    // Dipanggil GameManager saat Kalah
    public void ShowGameOverPanel() {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // Dipanggil GameManager saat Menang
    public void ShowWinPanel() {
        SoundManager.Instance.Play("Win");
        if (winPanel != null) winPanel.SetActive(true);
    }

    // --- FUNGSI TOMBOL ---

    public void OnRestartButtonClick()
    {
        gameManager.RestartGame();
    }
    
    public void OnTutorialStartButtonClick()
    {
        gameManager.StartGameFromTutorial();
    }

    public void OnNextStageButtonClick()
    {
        if(!string.IsNullOrEmpty(nextStageName))
        {
            SceneManager.LoadScene(nextStageName);
        }
        else
        {
            Debug.Log("Nama Next Stage belum diisi di Inspector UIManager!");
        }
    }
}

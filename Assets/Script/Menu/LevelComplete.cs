using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    // Build Index dari scene Level Select Menu (biasanya 0)
    public int menuSceneBuildIndex = 0;
    
    // Dipanggil saat pemain menyelesaikan level
    // nextLevelToUnlock adalah ANGKA level berikutnya yang HARUS DIBUKA (misal: 2 setelah Lvl 1 selesai)
    public void SaveProgressAndGoToMenu(int nextLevelToUnlock)
    {
        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Hanya simpan jika level yang baru dibuka lebih tinggi dari yang tersimpan
        if (nextLevelToUnlock > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevelToUnlock);
            PlayerPrefs.Save();
            Debug.Log($"Level {nextLevelToUnlock} berhasil dibuka.");
        }

        // Kembali ke Level Menu (Scene Index 0)
        SceneManager.LoadScene(menuSceneBuildIndex); 
    }
}
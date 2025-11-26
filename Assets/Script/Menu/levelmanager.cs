using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Seret semua Tombol Level Anda ke array ini (misalnya, 3 tombol)
    public Button[] levelButtons; 

    void Start()
    {
        // Mendapatkan level tertinggi yang sudah dibuka (default ke 1 jika belum ada data)
        // Kita asumsikan PlayerPrefs.GetInt("UnlockedLevel") menyimpan angka level (1, 2, 3, dst.)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); 
        
        // Loop untuk memeriksa status setiap tombol
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Tombol ke-i mewakili Level (i + 1).
            int currentButtonLevel = i + 1;

            // Jika level tombol lebih besar dari level yang sudah dibuka, kunci.
            if (currentButtonLevel > unlockedLevel)
            {
                // Nonaktifkan interaksi tombol
                levelButtons[i].interactable = false;
                // Opsional: Ganti warna tombol yang terkunci
                if (levelButtons[i].GetComponent<Image>() != null)
                {
                    levelButtons[i].GetComponent<Image>().color = Color.gray;
                }
            }
        }
    }

    // Dipanggil oleh Tombol Level saat diklik (diatur di Editor)
    // levelBuildIndex adalah Build Index dari scene level yang ingin dimuat (1, 2, 3, dst.)
    public void LoadLevel(int levelBuildIndex)
    {
        SceneManager.LoadScene(levelBuildIndex);
    }

    // FUNGSI RESET BARU
    public void ResetGameProgress()
    {
        // 1. Reset nilai PlayerPrefs untuk membuka level.
        // Set nilai kembali ke 1 (hanya Level 1 yang terbuka).
        PlayerPrefs.SetInt("UnlockedLevel", 1); 
        
        // 2. Simpan perubahan secara permanen.
        PlayerPrefs.Save(); 
        
        // 3. Muat ulang scene menu saat ini untuk menerapkan perubahan.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

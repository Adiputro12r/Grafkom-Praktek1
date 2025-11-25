using UnityEngine;
using System.Collections.Generic;
using TMPro; // Wajib untuk TextMeshPro

public class TaskSystem : MonoBehaviour
{
    [System.Serializable]
    public class Mission
    {
        public string name;           // Label di Inspector
        public CollectibleType type;  // Tipe Sampah
        public GameObject itemPrefab; // Prefab sampah (Drag dari Project)
        public int goal;              // Target
        public TextMeshProUGUI uiText;// Text UI (Drag dari Canvas)
        
        [HideInInspector] public int current; // Progress otomatis
    }

    [Header("Mission Setup (Drag Prefab & UI Disini)")]
    public List<Mission> missions = new List<Mission>();

    [Header("References")]
    [SerializeField] private GameManager1 gameManager1;
    
    // UIManager tidak wajib di sini karena TaskSystem update text sendiri
    // Tapi jika butuh reset UI saat restart, bisa akses uiText langsung

    public void ResetTask()
    {
        foreach(var mission in missions)
        {
            mission.current = 0;
            if(mission.uiText != null)
                mission.uiText.text = $"0/{mission.goal}";
        }
    }

    public void CollectItem(CollectibleType type)
    {
        if (gameManager1.gameState == GameManager1.GameState.Dead || gameManager1.gameState == GameManager1.GameState.Won) return;

        foreach(var mission in missions)
        {
            if(mission.type == type)
            {
                // Tambah progress
                mission.current++;
                
                // Update UI Text LANGSUNG disini
                if(mission.uiText != null)
                {
                     mission.uiText.text = $"{mission.current}/{mission.goal}";
                }

                CheckTaskCompletion();
                return; 
            }
        }
    }

    private void CheckTaskCompletion()
    {
        if (gameManager1.gameState == GameManager1.GameState.Won) return;

        bool allComplete = true;
        foreach(var mission in missions)
        {
            if(mission.current < mission.goal)
            {
                allComplete = false;
                break;
            }
        }

        if (allComplete)
        {
            gameManager1.WinGame();
        }
    }

    // Dipanggil oleh Grass untuk spawn sampah yang dibutuhkan
    public GameObject GetNeededPrefab()
    {
        List<Mission> incomplete = new List<Mission>();
        foreach(var m in missions)
        {
            if(m.current < m.goal) incomplete.Add(m);
        }

        if (incomplete.Count > 0)
        {
            return incomplete[Random.Range(0, incomplete.Count)].itemPrefab;
        }

        return null; 
    }
}
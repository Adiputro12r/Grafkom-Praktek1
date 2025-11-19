using UnityEngine;

public class TaskSystem : MonoBehaviour {
    [Header("Game Parameters")]
    [SerializeField] private int plasticGoal = 10;
    [SerializeField] private int ironGoal = 5;
    [SerializeField] private int stickGoal = 8;

    private int plasticCollected = 0;
    private int ironCollected = 0;
    private int stickCollected = 0;

    [Header("References")]
    [SerializeField] private GameManager1 gameManager1;
    [SerializeField] private UIManager1 uiManager;

    // Struktur data untuk mengembalikan progres
    public struct TaskProgress {
        public int plastic;
        public int iron;
        public int stick;
        public int plasticG;
        public int ironG;
        public int stickG;
    }

    public void ResetTask() {
        plasticCollected = 0;
        ironCollected = 0;
        stickCollected = 0;
        uiManager.UpdateTaskUI(GetTaskProgress());
    }

    // Dipanggil dari Character saat mengambil item
    public void CollectItem(CollectibleType type) {
        if (gameManager1.gameState == GameManager1.GameState.Dead || gameManager1.gameState == GameManager1.GameState.Won) return;

        if (type == CollectibleType.Plastic) plasticCollected++;
        else if (type == CollectibleType.Iron) ironCollected++;
        else if (type == CollectibleType.Stick) stickCollected++;

        uiManager.UpdateTaskUI(GetTaskProgress());
        CheckTaskCompletion();
    }

    private void CheckTaskCompletion() {
        if (gameManager1.gameState == GameManager1.GameState.Won) return;
        
        if (plasticCollected >= plasticGoal && ironCollected >= ironGoal && stickCollected >= stickGoal) {
            gameManager1.WinGame(); // Beritahu GameManager bahwa tugas selesai
        }
    }

    public TaskProgress GetTaskProgress() {
        return new TaskProgress {
            plastic = plasticCollected,
            iron = ironCollected,
            stick = stickCollected,
            plasticG = plasticGoal,
            ironG = ironGoal,
            stickG = stickGoal
        };
    }
}
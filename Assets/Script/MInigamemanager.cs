using UnityEngine;
using System.Collections; // Untuk Coroutine (Timer peringatan)
using System.Collections.Generic;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("Settings")]
    public List<GameObject> trashPrefabs;
    public Transform spawnPoint;
    public GameObject nextStageButton;

    [Header("UI References")]
    public TextMeshProUGUI textBenar;      // Teks: "Dipilah: 0"
    public TextMeshProUGUI textTotal;      // Teks: "Target: 5"
    public TextMeshProUGUI textFeedback;   // Teks Peringatan: "BENAR!" / "SALAH!"

    [Header("Level Config")]
    public int jumlahSampahTotal = 5;

    private int jumlahBenar = 0;
    private GameObject currentTrashObject;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nextStageButton.SetActive(false);
        jumlahBenar = 0;

        // Sembunyikan teks peringatan di awal
        if(textFeedback != null) textFeedback.gameObject.SetActive(false);

        UpdateUI(); 
        SpawnNewTrash();
    }

    public void SpawnNewTrash()
    {
        int randomIndex = Random.Range(0, trashPrefabs.Count);
        GameObject selectedPrefab = trashPrefabs[randomIndex];

        currentTrashObject = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        currentTrashObject.transform.SetParent(spawnPoint);
        currentTrashObject.transform.localScale = Vector3.one;
        currentTrashObject.transform.localPosition = Vector3.zero;
    }

    // Dipanggil saat sampah masuk ke tong yang BENAR
    public void OnTrashCorrect()
    {
        jumlahBenar++;
        UpdateUI();
        
        // Tampilkan peringatan positif
        ShowFeedback(true);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.soundLibrary.GetClipFromName("collect"));

        if (jumlahBenar >= jumlahSampahTotal)
        {
            GameFinished();
        }
        else
        {
            SpawnNewTrash();
        }
    }

    // Dipanggil saat sampah masuk ke tong yang SALAH
    public void OnTrashWrong()
    {
        // Tampilkan peringatan negatif
        ShowFeedback(false);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.soundLibrary.GetClipFromName("Lose"));
    }

    void UpdateUI()
    {
        if (textBenar != null) textBenar.text = $"Dipilah: {jumlahBenar}";
        if (textTotal != null) textTotal.text = $"Target: {jumlahSampahTotal}";
    }

    // Fungsi untuk memunculkan teks peringatan sesaat
    void ShowFeedback(bool isSuccess)
    {
        if (textFeedback == null) return;

        textFeedback.gameObject.SetActive(true);

        if (isSuccess)
        {
            textFeedback.text = "BENAR!";
            textFeedback.color = Color.green; // Warna Hijau
        }
        else
        {
            textFeedback.text = "SALAH TEMPAT!";
            textFeedback.color = Color.red;   // Warna Merah
        }

        // Mulai timer untuk menyembunyikan teks
        StopAllCoroutines();
        StartCoroutine(HideFeedbackDelay());
    }

    IEnumerator HideFeedbackDelay()
    {
        yield return new WaitForSeconds(1.5f); // Teks muncul selama 1.5 detik
        textFeedback.gameObject.SetActive(false);
    }

    void GameFinished()
    {
        if(textFeedback != null) 
        {
            textFeedback.gameObject.SetActive(true);
            SoundManager.Instance.PlaySFX(SoundManager.Instance.soundLibrary.GetClipFromName("Win"));
            textFeedback.text = "SELESAI!";
            textFeedback.color = Color.yellow;
        }
        nextStageButton.SetActive(true);
    }
}
using UnityEngine;

// Enum untuk mendefinisikan tipe sampah
public enum CollectibleType {
    Plastic,
    Iron,
    Stick
}

public class Collectible : MonoBehaviour
{

    public CollectibleType type;

    [Header("Visual Effects")]
    [Tooltip("Kecepatan objek berputar (derajat per detik)")]
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Lighting Effect")]
    [Tooltip("Hubungkan komponen 'Point Light' yang ada di prefab ini")]
    [SerializeField] private Light collectibleLight;

    [Tooltip("Kecepatan cahaya berdenyut (pulsing)")]
    [SerializeField] private float pulseSpeed = 1f;

    [Tooltip("Intensitas cahaya minimum")]
    [SerializeField] private float minIntensity = 0.8f;

    [Tooltip("Intensitas cahaya maksimum")]
    [SerializeField] private float maxIntensity = 1.5f;

    // Digunakan untuk menyimpan rotasi asli dari komponen cahaya
    private Quaternion lightOriginalLocalRotation;

    void Start()
    {
        // Simpan rotasi awal dari light, jika ada
        if (collectibleLight != null)
        {
            lightOriginalLocalRotation = collectibleLight.transform.localRotation;
        }
    }

    void Update()
    {
        // --- 1. Logika Berputar Pelan ---
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // --- 2. Logika "Stroke Lighting" (Cahaya Berdenyut) ---
        if (collectibleLight != null)
        {

            // Nilai sin berubah dari -1 sampai 1
            float wave = Mathf.Sin(Time.time * pulseSpeed);

            // Threshold: kalau di atas 0.2 → nyala, kalau di bawah → mati
            if (wave > 0.1f)
            {
                collectibleLight.intensity = maxIntensity;
            }
            else
            {
                collectibleLight.intensity = minIntensity;
            }

            collectibleLight.transform.localRotation = lightOriginalLocalRotation;
        }
    }
}
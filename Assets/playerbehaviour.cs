using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    // ===== TRANSLASI =====
    // T = vektor arah * speed * t
    // [1, 0, 0] = kanan | [-1, 0, 0] = kiri

    Vector3 kekanan;
    Vector3 kekiri;

    [SerializeField]
    private float speed = 1f;

    // ===== ROTASI =====
    private bool isRotate = false;
    private Vector3 rotasiEuler; // roll (x), pitch (y), yaw (z)

    void Start()
    {
        // Inisialisasi arah kanan dan kiri
        kekanan = new Vector3(1f, 0f, 0f);   // kalau ini di rotasi, dia pitch
        kekiri = new Vector3(-1f, 0f, 0f);   // pitch beda arah
        Debug.Log("Sekali! Inisialisasi selesai");

    }
     // ===== KONVERSI MATRICS ROTASI → QUATERNION =====
    Quaternion ToQuaternion(Vector3 rot)
    {
        // rot.x = roll, rot.y = pitch, rot.z = yaw
        // ubah derajat ke radian
        rot *= Mathf.Deg2Rad;

        // Abbreviation (cos dan sin setengah sudut)
        float cr = Mathf.Cos(rot.x * 0.5f);
        float sr = Mathf.Sin(rot.x * 0.5f);
        float cp = Mathf.Cos(rot.y * 0.5f);
        float sp = Mathf.Sin(rot.y * 0.5f);
        float cy = Mathf.Cos(rot.z * 0.5f);
        float sy = Mathf.Sin(rot.z * 0.5f);

        Quaternion q;
        q.w = cr * cp * cy + sr * sp * sy;
        q.x = sr * cp * cy - cr * sp * sy;
        q.y = cr * sp * cy + sr * cp * sy;
        q.z = cr * cp * sy - sr * sp * cy;

        return q;
    }


    void Update()
    {
        // ===== KODINGNYA MANUAL =====
        if (Input.GetKey(KeyCode.L))
        {
            transform.position = transform.position + kekanan * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.J))
        {
            transform.position = transform.position + kekiri * speed * Time.deltaTime;
        }

        // ===== KODINGAN SEMI MANUAL =====
        float kanankiri = Input.GetAxisRaw("Horizontal");
        // transform.position = transform.position + new Vector3(kanankiri, 0f, 0f) * speed * Time.deltaTime;

        // ===== KODINGAN OTOMATIS =====
        transform.Translate(new Vector3(kanankiri, 0f, 0f) * speed * Time.deltaTime);
        
        //cara manual
        //transform.rotation = transform.rotation * ToQuaternion(kekanan * speed * Time.deltaTime);

        //cara semi manual
        //transform.rotation = transform.rotation * Quaternion.Euler(kekanan * speed * Time.deltaTime);

        //cara otomatis
        if (Input.GetKeyDown(KeyCode.R))
        {
        isRotate = !isRotate;
        }
        if (isRotate)
        {
        transform.Rotate(kekanan * speed * Time.deltaTime);
        Debug.Log("Berkali2!");
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    public TrashType2d acceptedType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        TrashItem trash = droppedObj.GetComponent<TrashItem>();

        if (trash != null)
        {
            if (trash.type == acceptedType)
            {
                // --- KONDISI BENAR ---
                // Panggil fungsi Benar di Manager
                MinigameManager.Instance.OnTrashCorrect();
                
                // Hancurkan sampah
                Destroy(droppedObj);
            }
            else
            {
                // --- KONDISI SALAH ---
                // Panggil fungsi Salah di Manager (untuk menampilkan teks peringatan)
                MinigameManager.Instance.OnTrashWrong();
                
                // Kembalikan sampah ke tempat asal
                trash.ResetPosition();
            }
        }
    }
}
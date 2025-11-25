using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    public TrashType2d acceptedType; // Tentukan tipe yang diterima bin ini

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        TrashItem trash = droppedObj.GetComponent<TrashItem>();

        if (trash != null)
        {
            if (trash.type == acceptedType)
            {
                // BENAR: Hancurkan sampah dan lapor ke Manager
                Debug.Log("Sampah Benar!");
                MinigameManager.Instance.CheckProgress();
                Destroy(droppedObj);
            }
            else
            {
                // SALAH: Kembalikan ke tempat spawn
                Debug.Log("Salah Tempat!");
                trash.ResetPosition();
            }
        }
    }
}
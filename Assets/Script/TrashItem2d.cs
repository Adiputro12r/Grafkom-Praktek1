using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Baris ini memaksa Unity menambahkan CanvasGroup jika belum ada
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))] 
public class TrashItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TrashType2d type;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Vector3 startPosition;
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // MENCARI CANVAS:
        // Jika spawn point ada di dalam canvas, ini aman.
        canvas = GetComponentInParent<Canvas>();
        
        // JAGA-JAGA: Jika canvas tidak ketemu (misal spawn point diluar canvas), cari manual
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>(); // Mencari sembarang canvas di scene
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // PENCEGAHAN ERROR: Cek apakah canvas ketemu
        if (canvas == null) 
        {
            Debug.LogError("ERROR: Sampah ini tidak berada di dalam Canvas!");
            return;
        }

        startPosition = transform.position;
        parentAfterDrag = transform.parent;
        
        transform.SetParent(canvas.transform); 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas.transform)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        transform.SetParent(parentAfterDrag);
        transform.position = startPosition;
    }
}
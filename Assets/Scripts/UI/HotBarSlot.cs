using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;
    Item? _item;
    public Item? Item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
            if (value != null)
            {
                imageComp.sprite = value.Icon;
            }
            else
            {
                imageComp.sprite = null;
            }
        }
    }
    bool _selected = false;
    public bool Selected
    {
        get
        {
            return _selected;
        }
            set
        {
                _selected = value;
            if (value)
                {
                imageComp.color = new Color(0.8f, 1, 0.8f, 0.9f);
            }
            else
                {
                imageComp.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public Hotbar Hotbar;

    Image imageComp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imageComp = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool newVal = !Selected;
        Hotbar.ResetSlots();
       this.Selected = newVal;
    }
}

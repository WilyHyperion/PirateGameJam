using UnityEngine;

public class Hotbar : MonoBehaviour
{
    
    public HotbarSlot[] hotbarSlots;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].slotNumber = i;
            hotbarSlots[i].Hotbar = this;
        }
    }
    public void ResetSlots()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].GetComponent<HotbarSlot>().Selected = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

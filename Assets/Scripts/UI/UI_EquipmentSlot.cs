using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment Slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(item==null||item.stackSize==0)
            return;
        Inventory.Instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.Instance.AddItem(item.data as ItemData_Equipment);
        GetComponentInParent<UI>().itemToolTip.HideToolTip();
        
        CleanUpSlot();
    }
}

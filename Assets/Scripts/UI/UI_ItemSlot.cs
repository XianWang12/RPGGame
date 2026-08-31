using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField]protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item= _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        item= null;
        itemImage.color = Color.clear;
        itemImage.sprite= null;
        itemText.text= "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0)
            return;

        if(Input.GetKey(KeyCode.LeftControl) )
        {
            Inventory.Instance.RemoveItem(item.data);
            ui.itemToolTip.HideToolTip();
            return;
        }

        if (item.data.itemtype == Itemtype.Equipment)
        { 
            Inventory.Instance.EquipItem(item.data);
            ui.itemToolTip.HideToolTip();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0)
            return;

        ui.itemToolTip.HideToolTip();

    }
}

using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;

    private void SetUpVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    public void SetUpItem(ItemData _itemData,Vector2 _velocity)
    {
        itemData=_itemData;
        rb.velocity = _velocity;

        SetUpVisuals();        
    }

    public void PickUpItem()
    {
        if (!Inventory.Instance.CanAddItem() && itemData.itemtype == Itemtype.Equipment)
        { 
            rb.velocity=new Vector2(0,7);
            return;
        }    

        AudioManager.instance.PlaySFX(18);

        Inventory.Instance.AddItem(itemData);
        Destroy(gameObject);
    }
}

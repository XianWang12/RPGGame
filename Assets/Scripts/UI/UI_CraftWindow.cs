using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;

    [SerializeField] private Image[] materialImage;
    [SerializeField] private Button craftButton;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            if (_data.craftingMaterials.Count > materialImage.Length)
                Debug.LogWarning("You have more materials amount than you have material slots in craft window");

            materialImage[i].color = Color.white;
            materialImage[i].sprite = _data.craftingMaterials[i].data.icon;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.color = Color.white;
            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
        }

        itemName.text = _data.itemName;
        itemIcon.sprite = _data.icon;
        itemDescription.text = _data.GetDestription();

        craftButton.onClick.AddListener(() => Inventory.Instance.CanCraft(_data, _data.craftingMaterials));
    }

}

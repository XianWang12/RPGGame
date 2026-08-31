using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;


    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;


    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - "+skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        ui=GetComponentInParent<UI>();
        skillImage = GetComponent<Image>();

        ui.skillToolTip.HideToolTip();
        skillImage.color= lockedSkillColor;
    
        if (unlocked) 
            skillImage.color=Color.white;
    }

    private void UnlockSkillSlot()
    {
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("Can't unlock skill.");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("Can't unlock skill.");
                return ;
            }
        }
        if (PlayerManager.instance.HaveEnoughMoney(skillPrice) == false)
            return;

        unlocked = true;
        skillImage.color = Color.white;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillName,skillDescription);

        Vector2 mousePosition =Input.mousePosition;
        Vector2 localMousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(ui.rectTransformUtility, mousePosition, null,out localMousePosition);

        float xOffset = localMousePosition.x > 0 ? -150f : 150f;
        float yOffset = localMousePosition.y > 0 ? -75f : 75f;

        ui.skillToolTip.transform.position=new Vector2 (mousePosition.x+xOffset,mousePosition.y+yOffset);
    }   

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if(_data==null||_data.skillTree==null)
            return;
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data==null||_data.skillTree==null) return;
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
            _data.skillTree.Add(skillName,unlocked);
    }
}

using UnityEngine;

public class UI : MonoBehaviour,ISaveManager
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public RectTransform rectTransformUtility;

    public UI_ItemToolTip itemToolTip;
    public UI_CraftWindow craftWindow;
    public UI_SkillToolTip skillToolTip;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
    }

    private void Start()
    {
        SwitchTo(skillTreeUI);//防止进入player模式时skilltree UI没有打开导致按钮分配事件失败
        rectTransformUtility = GetComponent<RectTransform>();

        SwitchTo(inGameUI);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchWithKeyTo(characterUI);
        if(Input.GetKeyDown(KeyCode.I))
            SwitchWithKeyTo(skillTreeUI);
        if( Input.GetKeyDown(KeyCode.U))
            SwitchWithKeyTo(craftUI);
        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(7);
            _menu.SetActive(true);
        }

        if(GameManager.instance != null)
        {
            if(_menu == inGameUI)
                GameManager.instance.pauseGame(false);
            else
                GameManager.instance.pauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu!= null&&_menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf)
                return;
        }

        SwitchTo(inGameUI);
    }

    public void LoadData(GameData _data)
    {
        foreach (var pair in _data.volumeSettings)
        {
            foreach (var item in volumeSettings)
            {
                if (item.parametr == pair.Key)
                {
                    item.LoadSlider(pair.Value);
                    break;
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();
        foreach (var item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}

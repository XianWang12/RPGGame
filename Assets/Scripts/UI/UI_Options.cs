using UnityEngine;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private UI ui;

    private void Awake()
    {
        if (ui == null)
            ui = GetComponentInParent<UI>();
    }

    public void ResumeGame()
    {
        if (ui != null)
            ui.SwitchWithKeyTo(gameObject);
    }

    public void SaveAndExit()
    {
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();

        if (GameManager.instance != null)
            GameManager.instance.pauseGame(false);

        Application.Quit();
    }
}

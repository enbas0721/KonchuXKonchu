using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown p1Insect;
    [SerializeField] private TMP_Dropdown p2Insect;
    [SerializeField] private Button startButton;

    [Header("Scene")]
    [SerializeField] private string battleSceneName = "Battle";

    private void Awake()
    {
        BattleConfig.SetDefaults();

        SetupDropdown(p1Insect, new List<string> { "Kabuto", "Kuwagata" });
        p1Insect.value = (int)BattleConfig.Player1.insectType;
        p2Insect.value = (int)BattleConfig.Player2.insectType;

        p1Insect.onValueChanged.AddListener(OnP1InsectChanged);
        p2Insect.onValueChanged.AddListener(OnP2InsectChanged);

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    private void SetupDropdown(TMP_Dropdown dd, List<string> items)
    {
        if (dd == null) return;
        dd.ClearOptions();
        dd.AddOptions(items);
    }

    private void OnP1InsectChanged(int idx)
    {
        BattleConfig.SetPlayer1Insect((InsectType)idx);
    }

    private void OnP2InsectChanged(int idx)
    {
        BattleConfig.SetPlayer2Insect((InsectType)idx);
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}

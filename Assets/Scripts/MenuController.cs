using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown p1Insect;
    [SerializeField] private TMP_Dropdown p2Insect;
    [SerializeField] private Button startButton;

    [Header("Preview Images (shown under dropdowns)")]
    [SerializeField] private Image p1PreviewImage;
    [SerializeField] private Image p2PreviewImage;

    [Header("Character Sprites (must match dropdown option order)")]
    [SerializeField] private Sprite[] insectSprites;

    [Header("Scene")]
    [SerializeField] private string battleSceneName = "Battle";

    private void Awake()
    {
        BattleConfig.SetDefaults();

        p1Insect.value = (int)BattleConfig.Player1.insectType;
        p2Insect.value = (int)BattleConfig.Player2.insectType;

        p1Insect.onValueChanged.AddListener(OnP1InsectChanged);
        p2Insect.onValueChanged.AddListener(OnP2InsectChanged);

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        UpdatePreview(p1PreviewImage, p1Insect.value, false);
        UpdatePreview(p2PreviewImage, p2Insect.value, true);
    }

    private void OnDestroy()
    {
        if (p1Insect != null) p1Insect.onValueChanged.RemoveListener(OnP1InsectChanged);
        if (p2Insect != null) p2Insect.onValueChanged.RemoveListener(OnP2InsectChanged);
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
    }

    private void OnP1InsectChanged(int idx)
    {
        BattleConfig.SetPlayer1Insect((InsectType)idx);
        UpdatePreview(p1PreviewImage, idx, false);
    }

    private void OnP2InsectChanged(int idx)
    {
        BattleConfig.SetPlayer2Insect((InsectType)idx);
        UpdatePreview(p2PreviewImage, idx, true);
    }

    private void UpdatePreview(Image target, int idx, bool flipX)
    {
        if (target == null) return;

        target.preserveAspect = true;

        if (insectSprites == null || idx < 0 || idx >= insectSprites.Length || insectSprites[idx] == null)
        {
            target.sprite = null;
            target.enabled = false;
            return;
        }

        target.sprite = insectSprites[idx];
        target.enabled = true;

        var rt = target.rectTransform;
        var s = rt.localScale;
        s.x = Mathf.Abs(s.x) * (flipX ? -1f : 1f);
        rt.localScale = s;
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}

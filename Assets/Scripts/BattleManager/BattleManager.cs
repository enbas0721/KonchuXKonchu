using UnityEngine;
using System.Collections;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    [Header("Prefabs")]
    [SerializeField] private GameObject kabutoPrefab;
    [SerializeField] private GameObject kuwagataPrefab;

    [Header("Start Countdown UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownStepSec = 1.0f;
    [SerializeField] private string goText = "GO!";
    [SerializeField] private float goTextSec = 0.6f;

    [Header("End UI")]
    [SerializeField] private TMP_Text resultText;
    private bool battleStarted = false;
    private bool battleEnded = false;


    [Header("UI")]
    [SerializeField] private RectTransform p1Gage;
    [SerializeField] private RectTransform p2Gage;

    private FighterConfig player1;
    private FighterConfig player2;

    private InsectControllerBase p1;
    private InsectControllerBase p2;

    private void Start()
    {
        player1 = BattleConfig.Player1;
        player2 = BattleConfig.Player2;

        p1 = SpawnFighter(player1, p1Spawn, "P1");
        p2 = SpawnFighter(player2, p2Spawn, "P2");

        if (p1 == null || p2 == null)
        {
            Debug.LogError("Failed to spawn fighters.");
            return;
        }

        if (p1Gage != null) p1.SetGage(p1Gage);
        if (p2Gage != null) p2.SetGage(p2Gage);

        p1.SetOpponent(p2);
        p2.SetOpponent(p1);

        (GetProvider(p1.gameObject) as IOpponentAware)?.SetOpponent(p2);
        (GetProvider(p2.gameObject) as IOpponentAware)?.SetOpponent(p1);

        StartCoroutine(BeginBattleSequence());
    }

    private void Update()
    {
        if (!battleStarted || battleEnded) return;
        if (p1 == null || p2 == null) return;

        if (p1.IsDead || p2.IsDead)
        {
            battleEnded = true;
            SetProvidersEnabled(false);

            string winner = p1.IsDead ? "CPU Win" : "Player Win";
            if (resultText != null)
            {
                resultText.text = winner;
                resultText.gameObject.SetActive(true);
            }

            Debug.Log($"Battle End: {winner}");
        }
    }

    private InsectControllerBase SpawnFighter(FighterConfig cfg, Transform spawn, string label)
    {
        if (spawn == null)
        {
            Debug.LogError($"{label} spawn point is not assigned.");
            return null;
        }

        var prefab = GetPrefab(cfg.insectType);
        if (prefab == null)
        {
            Debug.LogError($"{label} prefab is not assigned for {cfg.insectType}.");
            return null;
        }

        var go = Instantiate(prefab, spawn.position, spawn.rotation);
        go.name = label;

        var provider = SelectAndEnableProvider(go, cfg);
        InjectProviderToController(go, provider);

        var insect = go.GetComponent<InsectControllerBase>();
        if (insect == null)
        {
            Debug.LogError($"{label} prefab does not have InsectControllerBase-derived component.");
            return null;
        }

        return insect;
    }

    private GameObject GetPrefab(InsectType type)
    {
        switch (type)
        {
            case InsectType.Kabuto: return kabutoPrefab;
            case InsectType.Kuwagata: return kuwagataPrefab;
            default: return null;
        }
    }

    private IInsectInputProvider GetProvider(GameObject go)
    {
        foreach (var mb in go.GetComponents<MonoBehaviour>())
        {
            if (!mb.enabled) continue;
            if (mb is IInsectInputProvider p) return p;
        }
        return null;
    }

    private IInsectInputProvider SelectAndEnableProvider(GameObject go, FighterConfig cfg)
    {
        var providers = go.GetComponents<MonoBehaviour>();

        IInsectInputProvider selected = null;

        foreach (var mb in providers)
        {
            if (mb is IInsectInputProvider p)
            {
                // いったん全て無効化
                p.SetEnabled(false);
                mb.enabled = false;
            }
        }

        foreach (var mb in providers)
        {
            if (cfg.controlType == ControlType.CPU)
            {
                if (mb is CpuInputProvider cpu)
                {
                    cpu.SetEnabled(true);
                    cpu.enabled = true;
                    selected = cpu;
                    break;
                }
            }
            else
            {
                // Human Joycon
                if (cfg.insectType == InsectType.Kabuto && mb is KabutoJoyconInputProvider kj)
                {
                    kj.SetEnabled(true);
                    kj.enabled = true;
                    selected = kj;
                    break;
                }
                if (cfg.insectType == InsectType.Kuwagata && mb is KuwagataJoyconInputProvider qj)
                {
                    qj.SetEnabled(true);
                    qj.enabled = true;
                    selected = qj;
                    break;
                }
            }
        }

        if (selected == null)
            Debug.LogError($"{go.name}: Selected IInsectInputProvider not found on prefab. Add providers to prefab.");

        return selected;
    }

    private void InjectProviderToController(GameObject go, IInsectInputProvider provider)
    {
        if (provider == null) return;

        if (go.TryGetComponent<InsectController>(out var ic)) ic.SetInputProvider(provider);
    }

    private IEnumerator BeginBattleSequence()
    {
        // 入力停止
        SetProvidersEnabled(false);
        battleStarted = false;
        battleEnded = false;

        if (resultText != null) resultText.gameObject.SetActive(false);

        // カウントダウン表示
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
            yield return new WaitForSeconds(countdownStepSec);

            countdownText.text = "2";
            yield return new WaitForSeconds(countdownStepSec);

            countdownText.text = "1";
            yield return new WaitForSeconds(countdownStepSec);

            countdownText.text = goText;
            yield return new WaitForSeconds(goTextSec);

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        // 入力開始
        SetProvidersEnabled(true);
        battleStarted = true;
    }

    private void SetProvidersEnabled(bool enabled)
    {
        var p1Prov = GetProvider(p1.gameObject);
        var p2Prov = GetProvider(p2.gameObject);

        p1Prov?.SetEnabled(enabled);
        p2Prov?.SetEnabled(enabled);
    }

}

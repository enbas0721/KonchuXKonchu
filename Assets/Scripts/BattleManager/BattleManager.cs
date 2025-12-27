using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    [Header("Prefabs")]
    [SerializeField] private GameObject kabutoPrefab;
    [SerializeField] private GameObject kuwagataPrefab;

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

        // 4) CPU Provider Ç…ÇÕ opponent Çíçì¸ÅiäYìñé“ÇæÇØÅj
        (GetProvider(p1.gameObject) as IOpponentAware)?.SetOpponent(p2);
        (GetProvider(p2.gameObject) as IOpponentAware)?.SetOpponent(p1);
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

        // Providerç∑Çµë÷Ç¶ + ControllerÇ÷íçì¸
        var provider = ReplaceInputProviderAndReturn(go, cfg);
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
            if (mb is IInsectInputProvider p) return p;
        return null;
    }

    private IInsectInputProvider ReplaceInputProviderAndReturn(GameObject go, FighterConfig cfg)
    {
        // ä˘ë∂ProviderÇñ≥å¯âªÇµÇƒçÌèú
        foreach (var mb in go.GetComponents<MonoBehaviour>())
        {
            if (mb is IInsectInputProvider)
            {
                mb.enabled = false;
                Destroy(mb);
            }
        }

        MonoBehaviour added;
        if (cfg.insectType == InsectType.Kabuto)
        {
            added = (MonoBehaviour)go.AddComponent(
                cfg.controlType == ControlType.CPU ? typeof(KabutoCpuInputProvider) : typeof(KabutoJoyconInputProvider));
        }
        else
        {
            added = (MonoBehaviour)go.AddComponent(
                cfg.controlType == ControlType.CPU ? typeof(KabutoCpuInputProvider) : typeof(KuwagataJoyconInputProvider));
        }

        var provider = added as IInsectInputProvider;
        if (provider == null)
        {
            Debug.LogError($"{go.name}: Added provider does not implement IInsectInputProvider.");
        }
        return provider;
    }

    private void InjectProviderToController(GameObject go, IInsectInputProvider provider)
    {
        if (provider == null) return;

        if (go.TryGetComponent<KabutoController>(out var kab))
            kab.SetInputProvider(provider);

        if (go.TryGetComponent<KuwagataController>(out var kuw))
            kuw.SetInputProvider(provider);
    }
}

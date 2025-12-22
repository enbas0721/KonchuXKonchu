using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    [Header("Prefabs")]
    [SerializeField] private GameObject kabutoPrefab;
    [SerializeField] private GameObject kuwagataPrefab;

    [Header("Selection (temporary: set in Inspector)")]
    [SerializeField] private FighterConfig player1 = new FighterConfig();
    [SerializeField] private FighterConfig player2 = new FighterConfig();

    private InsectControllerBase p1;
    private InsectControllerBase p2;

    private void Start()
    {
        p1 = SpawnFighter(player1, p1Spawn, "1P");
        p2 = SpawnFighter(player2, p2Spawn, "2P");

        if (p1 == null || p2 == null)
        {
            Debug.LogError("Failed to spawn fighters.");
            return;
        }

        p1.SetOpponent(p2);
        p2.SetOpponent(p1);

        (p1.GetComponent<IInsectInputProvider>() as IOpponentAware)?.SetOpponent(p2);
        (p2.GetComponent<IInsectInputProvider>() as IOpponentAware)?.SetOpponent(p1);
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

        ReplaceInputProvider(go, cfg);

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

    private void ReplaceInputProvider(GameObject go, FighterConfig cfg)
    {
        // ä˘ë∂ProviderÇéEÇ∑
        foreach (var c in go.GetComponents<MonoBehaviour>())
        {
            if (c is IInsectInputProvider)
            {
                c.enabled = false;
                Destroy(c);
            }
        }

        // êVãKí«â¡ÇµÇƒ ÅgÇªÇÃéQè∆Åh Çï€éùÇ∑ÇÈ
        MonoBehaviour addedMb;
        if (cfg.insectType == InsectType.Kabuto)
            addedMb = (MonoBehaviour)go.AddComponent(cfg.controlType == ControlType.CPU ? typeof(KabutoCpuInputProvider) : typeof(KabutoJoyconInputProvider));
        else
            addedMb = (MonoBehaviour)go.AddComponent(cfg.controlType == ControlType.CPU ? typeof(KuwagataCpuInputProvider) : typeof(KuwagataJoyconInputProvider));

        var provider = addedMb as IInsectInputProvider;
        if (provider == null)
        {
            Debug.LogError($"{go.name}: added provider does not implement IInsectInputProvider");
            return;
        }

        // ControllerÇ…íçì¸ÅiKabutoÇ≈Ç‡KuwagataÇ≈Ç‡ämé¿Åj
        var insect = go.GetComponent<InsectControllerBase>();
        if (insect == null) return;

        if (insect is KabutoController kab) kab.SetInputProvider(provider);
        if (insect is KuwagataController kuw) kuw.SetInputProvider(provider);
    }

}

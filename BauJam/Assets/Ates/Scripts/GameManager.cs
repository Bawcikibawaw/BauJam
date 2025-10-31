using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Rastgele Hedefler")]
    [Tooltip("NPC'nin rastgele seçilerek gidebileceği tüm hedef noktalarının listesi.")]
    public List<PathTarget> availableTargets = new List<PathTarget>();

    public event Action<Vector3> OnNPCWalkToLocation; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void TriggerNPCWalk(Vector3 targetPosition)
    {
        if (OnNPCWalkToLocation != null)
        {
            OnNPCWalkToLocation.Invoke(targetPosition);
            Debug.Log($"NPC yürüme olayı tetiklendi. Hedef: {targetPosition}"); 
        }
    }

    public void SelectAndTriggerRandomTarget()
    {
        if (availableTargets.Count == 0)
        {
            Debug.LogWarning("Hedef listesi boş. Rastgele tetikleme yapılamıyor.");
            return;
        }

        int randomIndex = Random.Range(0, availableTargets.Count);
        PathTarget selectedTarget = availableTargets[randomIndex];

        if (selectedTarget != null)
        {
            TriggerNPCWalk(selectedTarget.transform.position);
            Debug.Log($"Rastgele seçilen Nihai Hedef: {selectedTarget.gameObject.name}");
        }
    }
}
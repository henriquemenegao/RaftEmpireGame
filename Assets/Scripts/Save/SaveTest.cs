using UnityEngine;

namespace Save
{
    public class SaveTest : MonoBehaviour
    {
        [SerializeField] private SaveManager saveManager;

        private void Start()
        {
            if (!saveManager.HasSave)
            {
                GameSaveData data = new GameSaveData
                {
                    raftLevel = 2,
                    currentObjectiveId = "CH01_BUILD_FISHING"
                };

                data.resources.Add(new ResourceSaveData
                {
                    resourceId = "wood",
                    amount = 15
                });

                data.resources.Add(new ResourceSaveData
                {
                    resourceId = "plastic",
                    amount = 8
                });

                saveManager.Save(data);
            }

            GameSaveData loaded = saveManager.Load();

            if (loaded == null)
            {
                Debug.LogError("[SaveTest] Failed to load save.");
                return;
            }

            Debug.Log(
                $"[SaveTest] Raft Level: {loaded.raftLevel} | " +
                $"Objective: {loaded.currentObjectiveId}"
            );

            foreach (ResourceSaveData resource in loaded.resources)
            {
                Debug.Log(
                    $"[SaveTest] {resource.resourceId}: {resource.amount}"
                );
            }
        }
    }
}
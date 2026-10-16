using Resources;
using UnityEngine;

namespace Save
{
    public class GamePersistence : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private SaveManager saveManager;

        [SerializeField] private ResourceInventory resourceInventory;

        private GameSaveData currentSave;

        private void Start()
        {
            currentSave = saveManager.LoadOrCreate();

            // Restaura os recursos da partida.
            resourceInventory.ImportSaveData(currentSave.resources);

            Debug.Log("[GamePersistence] Resources restored.");
        }

        public void SaveGame()
        {
            if (currentSave == null)
                currentSave = new GameSaveData();

            // Atualiza apenas os recursos.
            // Os demais dados serão integrados nas próximas etapas.
            currentSave.resources = resourceInventory.ExportSaveData();
            saveManager.Save(currentSave);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                SaveGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
using System;
using System.IO;
using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        private const string SaveFileName = "savegame.json";

        private string SavePath => Path.Combine(
            Application.persistentDataPath,
            SaveFileName
        );

        private string TempPath => SavePath + ".tmp";
        private string BackupPath => SavePath + ".bak";

        public bool HasSave =>
            File.Exists(SavePath) || File.Exists(BackupPath);

        public void Save(GameSaveData data)
        {
            if (data == null)
            {
                Debug.LogError("[SaveManager] Save data is null.");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, true);

                // Grava primeiro em um arquivo temporário.
                File.WriteAllText(TempPath, json);

                if (File.Exists(SavePath))
                {
                    // Substitui o principal e preserva o anterior.
                    File.Replace(TempPath, SavePath, BackupPath);
                }
                else
                {
                    // Primeira gravação.
                    File.Move(TempPath, SavePath);
                }

                Debug.Log("[SaveManager] Game saved successfully.");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[SaveManager] Failed to save: {exception}"
                );
            }
        }

        public GameSaveData Load()
        {
            GameSaveData data = TryLoad(SavePath);

            if (data != null)
                return data;

            // Tenta recuperar o backup.
            data = TryLoad(BackupPath);

            if (data != null)
            {
                Debug.LogWarning(
                    "[SaveManager] Main save unavailable. Backup recovered."
                );
            }

            return data;
        }

        public GameSaveData LoadOrCreate()
        {
            GameSaveData data = Load();

            if (data != null)
                return data;

            Debug.Log("[SaveManager] Creating new game.");

            return new GameSaveData();
        }

        private GameSaveData TryLoad(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                string json = File.ReadAllText(path);

                GameSaveData data =
                    JsonUtility.FromJson<GameSaveData>(json);

                if (data == null || data.saveVersion <= 0)
                    throw new InvalidDataException("Invalid save data.");

                Debug.Log($"[SaveManager] Game loaded: {path}");

                return data;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[SaveManager] Failed to load '{path}': {exception.Message}"
                );

                return null;
            }
        }
    }
}
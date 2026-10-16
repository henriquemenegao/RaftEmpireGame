using System;
using System.Collections.Generic;

namespace Save
{
    [Serializable]
    public class GameSaveData
    {
        public int saveVersion = 1;

        public int raftLevel = 1;

        public List<ResourceSaveData> resources = new();

        public List<EquipmentSaveData> equipments = new();

        public List<CrewSaveData> crew = new();

        public string currentObjectiveId = "";

        public List<ObjectiveProgressSaveData> objectiveProgress = new();

        public List<string> completedDialogueIds = new();
    }

    [Serializable]
    public class ResourceSaveData
    {
        public string resourceId;
        public int amount;
    }

    [Serializable]
    public class EquipmentSaveData
    {
        public string equipmentId;
        public bool isBuilt;
        public string assignedCrewId;
    }

    [Serializable]
    public class CrewSaveData
    {
        public string crewId;
        public bool isUnlocked;
    }

    [Serializable]
    public class ObjectiveProgressSaveData
    {
        public string objectiveId;
        public int progress;
    }
}
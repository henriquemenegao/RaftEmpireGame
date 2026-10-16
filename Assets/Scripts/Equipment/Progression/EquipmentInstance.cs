using System;

namespace Equipment.Progression
{
    [Serializable]
    public class EquipmentInstance
    {
        public string InstanceId;
        public string EquipmentId;
        public int Level;
        public string SlotId;
        public string AssignedCrewId;

        public bool IsInstalled => !string.IsNullOrEmpty(SlotId);

        public EquipmentInstance(
            string equipmentId,
            int level = 1)
        {
            InstanceId = Guid.NewGuid().ToString("N");
            EquipmentId = equipmentId;
            Level = level;
            SlotId = null;
            AssignedCrewId = null;
        }
    }
}
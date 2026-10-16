namespace Equipment.Progression
{
    public enum EquipmentBuildStatus
    {
        Available,
        Locked,
        LimitReached,
        InvalidConfiguration
    }

    public static class EquipmentBuildValidator
    {
        public static EquipmentBuildStatus Validate(
            EquipmentDefinition equipment,
            RaftLevelDefinition raftLevel,
            EquipmentInventory inventory)
        {
            if (equipment == null ||
                raftLevel == null ||
                inventory == null)
            {
                return EquipmentBuildStatus.InvalidConfiguration;
            }

            int maxInstances = raftLevel.GetMaxEquipmentInstances(equipment);

            if (maxInstances <= 0)
                return EquipmentBuildStatus.Locked;

            int ownedInstances = inventory.Count(equipment.EquipmentId);

            if (ownedInstances >= maxInstances)
                return EquipmentBuildStatus.LimitReached;

            return EquipmentBuildStatus.Available;
        }
    }
}
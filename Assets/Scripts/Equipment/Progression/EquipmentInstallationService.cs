using Equipment.Core;
using Equipment.Slots;
using UnityEngine;

namespace Equipment.Progression
{
    public class EquipmentInstallationService : MonoBehaviour
    {
        [SerializeField] private EquipmentInventory equipmentInventory;

        public bool CanInstall(
            EquipmentInstance instance,
            EquipmentDefinition definition,
            EquipmentSlot slot)
        {
            if (instance == null || definition == null ||
                slot == null || equipmentInventory == null)
                return false;

            if (instance.EquipmentId != definition.EquipmentId ||
                instance.IsInstalled || !slot.Allows(definition))
                return false;

            foreach (var equipment in equipmentInventory.Equipments)
            {
                if (equipment != instance &&
                    equipment.SlotId == slot.SlotId)
                    return false;
            }

            var level = definition.GetLevel(instance.Level);

            return level != null &&
                   level.Prefab != null &&
                   slot.TryGetPlacement(
                       definition, out _, out _);
        }

        public bool TryInstall(
            EquipmentInstance instance,
            EquipmentDefinition definition,
            EquipmentSlot slot,
            out GameObject installedObject)
        {
            installedObject = null;

            if (!CanInstall(instance, definition, slot))
                return false;

            var level = definition.GetLevel(instance.Level);

            installedObject = Instantiate(
                level.Prefab, slot.transform);

            installedObject.SetActive(false);

            slot.TryGetPlacement(
                definition,
                out Vector3 localPosition,
                out Quaternion localRotation);

            installedObject.transform.localPosition = localPosition;
            installedObject.transform.localRotation = localRotation;

            var workstation =
                installedObject.GetComponent<Workstation>();

            if (workstation != null)
                workstation.Configure(definition, instance.Level);

            instance.SlotId = slot.SlotId;
            slot.SetEmptyVisual(false);

            equipmentInventory.NotifyChanged();

            return true;
        }
    }
}
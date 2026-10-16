using Equipment.Core;
using Equipment.Slots;
using UnityEngine;

namespace Equipment.Progression
{
    public class EquipmentInstallationService : MonoBehaviour
    {
        public bool TryInstall(
            EquipmentInstance instance,
            EquipmentDefinition definition,
            EquipmentSlot slot,
            out GameObject installedObject)
        {
            installedObject = null;

            if (instance == null || definition == null || slot == null)
            {
                Debug.LogWarning(
                    "EquipmentInstallationService: parâmetros inválidos.");
                return false;
            }

            if (instance.EquipmentId != definition.EquipmentId)
            {
                Debug.LogWarning(
                    "EquipmentInstallationService: equipamento incompatível com a definição.");
                return false;
            }

            if (!slot.Allows(definition))
            {
                Debug.LogWarning(
                    $"Equipamento {definition.DisplayName} não permitido no slot {slot.SlotId}.");
                return false;
            }

            if (!string.IsNullOrEmpty(instance.SlotId))
            {
                Debug.LogWarning(
                    "EquipmentInstallationService: equipamento já está instalado.");
                return false;
            }

            var levelDefinition = definition.GetLevel(instance.Level);

            if (levelDefinition == null || levelDefinition.Prefab == null)
            {
                Debug.LogWarning(
                    "EquipmentInstallationService: prefab do nível não configurado.");
                return false;
            }

            if (!slot.TryGetPlacement(
                    definition,
                    out Vector3 localPosition,
                    out Quaternion localRotation))
            {
                return false;
            }

            installedObject = Instantiate(
                levelDefinition.Prefab,
                slot.transform);

            installedObject.transform.localPosition = localPosition;
            installedObject.transform.localRotation = localRotation;

            var workstation = installedObject.GetComponent<Workstation>();

            if (workstation != null)
                workstation.Configure(definition, instance.Level);

            instance.SlotId = slot.SlotId;
            slot.SetEmptyVisual(false);

            Debug.Log(
                $"Equipamento instalado: {definition.DisplayName} " +
                $"(nível {instance.Level}) no slot {slot.SlotId}");

            return true;
        }
    }
}
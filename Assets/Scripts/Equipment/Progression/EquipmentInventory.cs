using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equipment.Progression
{
    public class EquipmentInventory : MonoBehaviour
    {
        private readonly List<EquipmentInstance> equipments = new();
        public IReadOnlyList<EquipmentInstance> Equipments => equipments;
        public event Action OnChanged;

        public EquipmentInstance Add(EquipmentDefinition definition)
        {
            if (definition == null ||
                string.IsNullOrWhiteSpace(definition.EquipmentId))
            {
                Debug.LogError(
                    "EquipmentInventory: definição de equipamento inválida.");
                return null;
            }

            var instance = new EquipmentInstance(
                definition.EquipmentId);

            equipments.Add(instance);
            OnChanged?.Invoke();

            return instance;
        }

        public EquipmentInstance GetById(string instanceId)
        {
            return equipments.Find(equipment => equipment.InstanceId == instanceId);
        }

        public int Count(string equipmentId)
        {
            return equipments.FindAll(equipment => equipment.EquipmentId == equipmentId).Count;
        }

        public List<EquipmentInstance> GetStored()
        {
            return equipments.FindAll(equipment => !equipment.IsInstalled);
        }

        public List<EquipmentInstance> GetStored(string equipmentId)
        {
            return equipments.FindAll(equipment =>
                equipment.EquipmentId == equipmentId &&
                !equipment.IsInstalled);
        }

        public void NotifyChanged()
        {
            OnChanged?.Invoke();
        }
    }
}
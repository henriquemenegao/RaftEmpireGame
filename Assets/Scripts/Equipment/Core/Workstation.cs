using Crew;
using Equipment.Progression;
using UnityEngine;

namespace Equipment.Core
{
    public abstract class Workstation : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField]
        private EquipmentDefinition definition;

        [Header("Operation")]
        [SerializeField]
        private Transform interactionPoint;

        private int currentLevel = 1;

        public EquipmentDefinition Definition => definition;

        public string DisplayName =>
            definition != null ? definition.DisplayName : string.Empty;

        public int Level => currentLevel;

        public string ProductionDescription =>
            definition != null ? definition.Description : string.Empty;

        public Sprite ProductionIcon =>
            definition != null ? definition.Icon : null;

        public CrewMember Operator { get; private set; }

        public Transform InteractionPoint => interactionPoint;

        public bool HasOperator => Operator != null;

        public abstract CrewActivity Activity { get; }

        public abstract float Progress { get; }

        public abstract bool IsProducing { get; }

        protected EquipmentLevelDefinition CurrentLevelDefinition =>
            definition != null
                ? definition.GetLevel(currentLevel)
                : null;

        public virtual int BaseProductionAmount =>
            CurrentLevelDefinition?.BaseProductionAmount ?? 0;

        public virtual float ProductionDuration =>
            CurrentLevelDefinition?.ProductionDuration ?? 0f;

        public virtual void Configure(
            EquipmentDefinition equipmentDefinition,
            int level)
        {
            if (equipmentDefinition == null)
            {
                Debug.LogError(
                    $"Workstation '{name}': EquipmentDefinition inválido."
                );
                return;
            }

            var levelDefinition = equipmentDefinition.GetLevel(level);

            if (levelDefinition == null)
            {
                Debug.LogError(
                    $"Workstation '{name}': nível {level} não definido."
                );
                return;
            }

            definition = equipmentDefinition;
            currentLevel = level;
        }

        public virtual void AssignOperator(CrewMember member)
        {
            Operator = member;
        }

        public virtual void RemoveOperator()
        {
            Operator = null;
        }
    }
}
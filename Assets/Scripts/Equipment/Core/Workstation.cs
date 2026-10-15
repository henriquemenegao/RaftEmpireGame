using Crew;
using UnityEngine;

namespace Equipment.Core
{
    public abstract class Workstation : MonoBehaviour
    {
        [Header("Identidade")]
        [SerializeField] private string displayName;
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField] private string productionDescription;

        [Header("Apresentação")]
        [SerializeField] private Sprite productionIcon;

        [Header("Operação")]
        [SerializeField] private Transform interactionPoint;

        public string DisplayName => displayName;
        public int Level => level;
        public string ProductionDescription => productionDescription;
        public Sprite ProductionIcon => productionIcon;

        public CrewMember Operator { get; private set; }
        public Transform InteractionPoint => interactionPoint;
        public bool HasOperator => Operator != null;

        public abstract CrewActivity Activity { get; }
        
        public abstract float Progress { get; }
        public abstract bool IsProducing { get; }


        public abstract int BaseProductionAmount { get; }
        public abstract float ProductionDuration { get; }

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
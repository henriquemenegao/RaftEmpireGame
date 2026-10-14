using Crew;
using UnityEngine;

namespace Equipment.Core
{
    public abstract class Workstation : MonoBehaviour
    {
        [Header("Operação")]
        [SerializeField] private Transform interactionPoint;

        public CrewMember Operator { get; private set; }
        public Transform InteractionPoint => interactionPoint;
        public bool HasOperator => Operator != null;
        
        public abstract CrewActivity Activity { get; }

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
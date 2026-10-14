using Equipment.Core;
using UnityEngine;

namespace Crew
{
    public class CrewMember : MonoBehaviour
    {
        [Header("Identidade")]
        [SerializeField] private string crewName;
        [SerializeField] private bool isCaptain;

        [Header("Produtividade")]
        [SerializeField, Min(1)] private int efficiency = 1;
        
        public Workstation AssignedEquipment { get; private set; }

        private CrewManager crewManager;

        public string CrewName => crewName;
        public bool IsCaptain => isCaptain;
        public int Efficiency => efficiency;

        private void Start()
        {
            crewManager = FindAnyObjectByType<CrewManager>();

            if (crewManager == null)
            {
                Debug.LogWarning(
                    $"CrewManager não encontrado para {crewName}."
                );
                return;
            }

            crewManager.Register(this);
        }

        private void OnDestroy()
        {
            if (crewManager != null)
                crewManager.Unregister(this);
        }
        
        public void AssignEquipment(Workstation equipment)
        {
            AssignedEquipment = equipment;
        }

        public void ClearAssignment()
        {
            AssignedEquipment = null;
        }
    }
}
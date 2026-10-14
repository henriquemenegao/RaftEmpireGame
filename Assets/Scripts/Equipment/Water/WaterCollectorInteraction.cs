using Crew;
using UnityEngine;

namespace Equipment.Water
{
    public class WaterCollectorInteraction : MonoBehaviour
    {
        [SerializeField] private CrewMember selectedCrewMember;

        private WaterCollector waterCollector;
        private CrewManager crewManager;

        private void Awake()
        {
            waterCollector = GetComponent<WaterCollector>();
        }

        private void OnMouseDown()
        {
            if (crewManager == null)
                crewManager = FindAnyObjectByType<CrewManager>();

            if (crewManager == null || selectedCrewMember == null)
            {
                Debug.LogWarning(
                    "CrewManager ou tripulante não configurado."
                );
                return;
            }

            if (waterCollector.Operator == selectedCrewMember)
            {
                crewManager.Unassign(selectedCrewMember);
                return;
            }

            if (!crewManager.Assign(selectedCrewMember, waterCollector))
            {
                Debug.LogWarning(
                    "Não foi possível designar o tripulante ao coletor."
                );
            }
        }
    }
}
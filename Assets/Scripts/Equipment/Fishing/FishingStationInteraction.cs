
using Crew;
using Equipment.Core;
using Resources;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Equipment.Fishing
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(FishingStation))]
    public class FishingStationInteraction : MonoBehaviour
    {
        private FishingStation fishingStation;
        private CrewManager crewManager;
        private ResourceInventory inventory;
        
        [SerializeField] private CrewMember selectedCrewMember;

        private void Awake()
        {
            fishingStation = GetComponent<FishingStation>();
        }

        private void OnMouseDown()
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (crewManager == null)
                crewManager = FindAnyObjectByType<CrewManager>();

            if (inventory == null)
                inventory = FindAnyObjectByType<ResourceInventory>();
            

            if (crewManager == null || inventory == null || selectedCrewMember == null)
            {
                Debug.LogWarning(
                    "CrewManager, inventário ou tripulante não encontrado."
                );
                return;
            }

            if (fishingStation.Operator == selectedCrewMember)
            {
                crewManager.Unassign(selectedCrewMember);
                return;
            }

            fishingStation.Initialize(inventory);

            if (!crewManager.Assign(selectedCrewMember, fishingStation))
            {
                Debug.LogWarning(
                    $"Não foi possível designar {selectedCrewMember.CrewName}. " +
                    "A estação pode estar ocupada."
                );
            }
        }
    }
}

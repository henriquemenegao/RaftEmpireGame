using Crew;
using Equipment.Core;
using UnityEngine;

namespace UI
{
    public class CrewListUI : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private CrewManager crewManager;
        [SerializeField] private CrewMemberCardUI cardPrefab;
        [SerializeField] private Transform content;

        public void Refresh(Workstation workstation)
        {
            if (crewManager == null ||
                cardPrefab == null ||
                content == null ||
                workstation == null)
            {
                Debug.LogWarning(
                    "CrewListUI: referências não configuradas."
                );
                return;
            }

            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            foreach (CrewMember member in crewManager.Members)
            {
                CrewMemberCardUI card = Instantiate(cardPrefab, content);
                card.Initialize(member, workstation);
            }
        }
    }
}
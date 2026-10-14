using Crew;
using Equipment.Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CrewMemberCardUI : MonoBehaviour
    {
        [Header("Informações do tripulante")]
        [SerializeField] private TMP_Text crewName;
        [SerializeField] private TMP_Text crewEfficiency;
        [SerializeField] private TMP_Text crewStatus;
        
        [Header("Ação")]
        [SerializeField] private UnityEngine.UI.Button crewActionButton;

        private CrewManager crewManager;
        private CrewMember crewMember;
        private Workstation selectedWorkstation;

        public void Initialize(
            CrewMember member,
            Workstation workstation)
        {
            if (member == null || workstation == null)
            {
                Debug.LogWarning(
                    "CrewMemberCardUI: tripulante ou equipamento inválido."
                );
                return;
            }

            crewMember = member;
            selectedWorkstation = workstation;
            
            crewManager = FindAnyObjectByType<CrewManager>();

            crewActionButton.onClick.RemoveAllListeners();
            crewActionButton.onClick.AddListener(OnActionClicked);

            crewName.text = member.CrewName;
            crewEfficiency.text =
                $"Eficiência: {member.Efficiency}x";

            RefreshStatus();
        }
        
        
        private void OnActionClicked()
        {
            if (crewManager == null ||
                crewMember == null ||
                selectedWorkstation == null)
                return;

            if (crewMember.AssignedEquipment == selectedWorkstation)
            {
                crewManager.Unassign(crewMember);
            }
            else
            {
                bool success = crewManager.Assign(
                    crewMember,
                    selectedWorkstation
                );

                if (!success)
                {
                    Debug.LogWarning(
                        $"Não foi possível alocar {crewMember.CrewName}."
                    );
                    return;
                }
            }

            RefreshStatus();
        }


        public void RefreshStatus()
        {
            if (crewMember == null || selectedWorkstation == null)
                return;

            if (crewMember.AssignedEquipment == null)
            {
                crewStatus.text = "Disponível";
            }
            else if (crewMember.AssignedEquipment == selectedWorkstation)
            {
                crewStatus.text = "Operando aqui";
            }
            else
            {
                crewStatus.text = "Ocupado em outra estação";
            }
        }
    }
}
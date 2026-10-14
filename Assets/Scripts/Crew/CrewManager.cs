using System.Collections.Generic;
using Equipment.Core;
using UnityEngine;

namespace Crew
{
    public class CrewManager : MonoBehaviour
    {
        private readonly List<CrewMember> members = new();

        public IReadOnlyList<CrewMember> Members => members;

        public void Register(CrewMember member)
        {
            if (member == null || members.Contains(member))
                return;

            members.Add(member);

            Debug.Log($"Tripulante registrado: {member.CrewName}");
        }

        public void Unregister(CrewMember member)
        {
            if (member == null)
                return;

            Unassign(member);
            members.Remove(member);
        }

        public bool Assign(CrewMember member, Workstation workstation)
        {
            if (member == null || workstation == null)
                return false;

            if (!members.Contains(member))
                return false;
            
            CrewController controller = member.GetComponent<CrewController>();

            if (controller == null)
            {
                Debug.LogWarning(
                    $"Tripulante {member.CrewName} sem CrewController."
                );
                return false;
            }


            if (workstation.InteractionPoint == null)
            {
                Debug.LogWarning("Estação sem InteractionPoint.");
                return false;
            }

            if (workstation.Operator == member)
                return true;

            // Por enquanto, não substituímos operadores automaticamente.
            if (workstation.HasOperator)
                return false;

            // Libera o equipamento anterior do tripulante.
            Unassign(member);

           
            workstation.AssignOperator(member);
            member.AssignEquipment(workstation);

            
            controller.StartActivity(
                workstation.Activity,
                workstation.InteractionPoint
            );


            Debug.Log(
                $"{member.CrewName} designado para {workstation.name}"
            );

            return true;
        }

        
        
        public void Unassign(CrewMember member)
        {
            if (member == null)
                return;

            Workstation workstation = member.AssignedEquipment;

            if (workstation != null)
                workstation.RemoveOperator();

            CrewController controller = member.GetComponent<CrewController>();

            if (controller != null && workstation != null)
                controller.StopActivity();

            member.ClearAssignment();
        }


    }
}
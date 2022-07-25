using UnityEngine;
using UnityEditor;

namespace InariUdon.Interaction
{
    public class TouchSwitchGizmos
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void OnDrawGizmosSelected(TouchSwitch touchSwitch, GizmoType gizmoType)
        {
            try
            {
                Gizmos.color = Color.white;
                Gizmos.matrix = touchSwitch.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1.0f, 1.0f, touchSwitch.thickness));

                Gizmos.DrawWireSphere(Vector3.zero, touchSwitch.radius);

                if (touchSwitch.directionalMode)
                {
                    Gizmos.color = Color.red;
                    Vector3 right = Vector3.right;
                    Gizmos.DrawRay(Vector3.zero, right * touchSwitch.radius);

                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(Vector3.zero, Vector3.up * touchSwitch.radius);
                }

                if (touchSwitch.knobMode || touchSwitch.wheelMode)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(Vector3.zero, Vector3.forward * touchSwitch.radius);
                }
            }
            finally
            {
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}

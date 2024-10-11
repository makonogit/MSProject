using Unity.VisualScripting;
using UnityEngine;

public class CS_PredicitonPosition : MonoBehaviour
{
    [SerializeField]
    private CS_Burst_of_object burst;

    private void OnDrawGizmosSelected()
    {
        if (burst != null) burst.Info();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.125f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

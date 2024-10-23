using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
public class CS_PredictionPosition : MonoBehaviour
{
    [SerializeField]
    private CS_Burst_of_object burst;
   
    private void OnDrawGizmosSelected()
    {
        
        if (ShouldReturn()) return;
        if (burst != null) burst.Info();
        if (burst.DestinationShow) 
        {
            Gizmos.color = burst.DestinationColor;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
    private void OnDrawGizmos()
    {
        if (ShouldReturn()) return;
        if (burst.DestinationShow) 
        {
            Gizmos.color = burst.DestinationColor;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }

    private bool ShouldReturn() 
    {
        if(burst == null) burst = GetComponentInParent<CS_Burst_of_object>();
        return burst == null;
    }
}
#endif // UNITY_EDITOR
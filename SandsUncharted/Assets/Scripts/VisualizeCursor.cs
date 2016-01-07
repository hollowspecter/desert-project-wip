using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VisualizeCursor : MonoBehaviour
{
    [SerializeField]
    private LayerMask raycastmask;

    private Ray ray;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null) {
                    Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                }
            }
        }
        else {
            Debug.Log("Missed");
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(ray);
    }
}

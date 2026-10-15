using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(0f, 12f, -6f);

    private void LateUpdate()
    {
        if (Target != null)
        {
            transform.position = Target.position + Offset;

          
            transform.LookAt(Target.position);
        }
    }
}

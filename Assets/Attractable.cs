using UnityEngine;

public class Attractable : MonoBehaviour
{
    [SerializeField] private bool rotateToCenter = true;

    private Transform m_transform;
    private Rigidbody2D m_rigidbody;

    private Vector2 gravityDirection;
    private Attractor activeAttractor;

    void Start()
    {
        m_transform = transform;
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (activeAttractor != null)
        {
            Vector2 direction = gravityDirection.normalized;
            float strength = activeAttractor.gravity;

            m_rigidbody.AddForce(direction * strength * Time.fixedDeltaTime);

            if (rotateToCenter)
                RotateToCenter();
        }

        activeAttractor = null;
        gravityDirection = Vector2.zero;
    }

    public void Attract(Attractor attractorObj)
    {
        Vector2 dir = attractorObj.attractorTransform.position - m_transform.position;
        gravityDirection += dir;
        activeAttractor = attractorObj;
    }

    void RotateToCenter()
    {
        if (activeAttractor == null) return;

        Vector2 toCenter = (Vector2)activeAttractor.attractorTransform.position - (Vector2)m_transform.position;
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
        m_transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }
}

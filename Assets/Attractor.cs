using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public LayerMask AttractionLayer;
    public float gravity = 10;
    [SerializeField] private float Radius = 10;
    public List<Collider2D> AttractedObjects = new List<Collider2D>();
    [HideInInspector] public Transform attractorTransform;

    void Awake()
    {
        attractorTransform = GetComponent<Transform>();
    }

    void Update()
    {
        SetAttractedObjects();
    }

    void FixedUpdate()
    {
        AttractObjects();
    }

    void SetAttractedObjects()
    {
        AttractedObjects = Physics2D.OverlapCircleAll(attractorTransform.position, Radius, AttractionLayer).ToList();
    }

    void AttractObjects()
    {
        foreach (Collider2D col in AttractedObjects)
        {
            if (col == null) continue;

            Attractable attractable = col.GetComponent<Attractable>();
            if (attractable == null) continue;

            Vector2 directionToCenter = (attractorTransform.position - col.transform.position).normalized;

            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(directionToCenter * gravity * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

}
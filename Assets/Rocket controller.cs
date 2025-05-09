using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class RocketController : MonoBehaviour
{

    [SerializeField] private GameObject explosionPrefab;
    [Header("Parametri de miscare")] 
    public float thrustForce;
    public float rotationSpeed;

    [Header("Text Scor")] 
    public TextMeshProUGUI scorText;
    int scor = 0;

    public TextMeshProUGUI vitezaText;
    float vitezaFloat = 0.0f;

    [Header("Combustibil")] 
    public float maxFuel;
    public Slider fuelSlider;

    [Header("Aterizare")]
    [Tooltip("Viteza maxima la impact (m/s) pentru a ateriza in siguranta")] 
    public float landingSpeedThreshold;

    private float currentFuel;
    private Rigidbody2D rb2d;
    private Collider2D col2d;
    private Keyboard kb;

    void Awake()
    {
        currentFuel = maxFuel;
        if (fuelSlider)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = currentFuel;
        }

        rb2d = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        rb2d.gravityScale = 0f;
        rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb2d.constraints = RigidbodyConstraints2D.None;
    }

    void Start()
    {
        kb = Keyboard.current;
    }

    void Update()
    {
        if (rb2d != null && vitezaText != null)
        {
            vitezaFloat = rb2d.linearVelocity.magnitude;
            vitezaText.text = $"{vitezaFloat:F3} m/s";
        }

        HandleThrust();
        HandleRotation();
        UpdateFuelUI();
    }


    private void HandleThrust()
    {
        if (kb == null || currentFuel <= 0f) return;
        if (kb.spaceKey.isPressed)
        {
            currentFuel = Mathf.Max(currentFuel - (Time.deltaTime + 0.01f), 0f);
            if(currentFuel == 0f){
                Explode();
            }
            rb2d.AddForce(transform.up * thrustForce * Time.deltaTime, ForceMode2D.Force);
        }
    }

    private void HandleRotation()
    {
        if (kb == null) return;
        float rot = rotationSpeed * Time.deltaTime;
        if (kb.aKey.isPressed) rb2d.rotation += rot;
        else if (kb.dKey.isPressed) rb2d.rotation -= rot;
    }

    private void UpdateFuelUI()
    {
        if (fuelSlider) fuelSlider.value = currentFuel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Planet")){
            float speed = collision.relativeVelocity.magnitude;
            bool bottomContact = false;
            foreach (var cp in collision.contacts)
            {
                if (Vector2.Dot(cp.normal, -transform.up) < -0.9f)
                {
                    bottomContact = true;
                    break;
                }
            }

            if (bottomContact && speed <= landingSpeedThreshold)
            {
                Debug.Log($"Aterizare blanda ({speed:F3} m/s) – SAFE");
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
            }
            else
            {
                Debug.Log($"Impact fatal ({speed:F3} m/s) – EXPLODE");
                Explode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("fuel"))
        {    
            collision.gameObject.SetActive(false);
            currentFuel = maxFuel;
            scor++;
            scorText.text = "Scor: "+scor;
        }
    }

private void Explode()
{
    Debug.Log("a"+explosionPrefab);
    if (explosionPrefab != null)
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Debug.Log("b"+explosion);

        Animator anim = explosion.GetComponent<Animator>();
        Debug.Log("c"+anim);
        Debug.Log("d"+anim != null);
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }

    }
    gameObject.SetActive(false);
}


}

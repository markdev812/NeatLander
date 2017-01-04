using UnityEngine;
using UnityEngine.UI;

public class LanderController : MonoBehaviour
{
    public Text TxtAltitude;
    public Text TxtVelocity;
    public Text TxtFuel;

    private float _thrust;
    private float _fuel;
    private float lastTime = -1;
    private Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {
        _fuel = 500f;

        rb = GetComponent<Rigidbody2D>();
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha4))
            _thrust = 4f;
        else
        {
            _thrust = 0f;
        }
        _fuel -= _thrust * Time.deltaTime;

        TxtVelocity.text = rb.velocity.y.ToString("N1");
        TxtAltitude.text = transform.position.y.ToString("N0");
        TxtFuel.text = _fuel.ToString("N1");

        if (Mathf.Floor(Time.time) > lastTime)
        {
            Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + rb.velocity.y.ToString());
            lastTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(0, _thrust), ForceMode2D.Force);

    }
}

using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LanderControllerGame : MonoBehaviour
{
    public float StartingAltitude = 2400;
    public const float RangeX = 400;
    public const float StartingFuel = 500;
    public readonly Vector2 Gravity = new Vector2(0, -3.711f);
    public const float TerminalVel = -200;

    public int NumInputs;
    public int NumOutputs;

    public bool AIControlled = true;

    public Text TxtAltitude;
    public Text TxtVelocity;
    public Text TxtFuel;
    public Text TxtThrust;
    public ParticleSystem _flame;

    private bool _landed;
    private float _thrust;
    private float _fuel;
    private Vector2 _velocity;
    private float lastTime = -1;
    private float _torque;
    private float _rotation;

    private Rigidbody2D rb;


    private NeatExperiment _experiment;
    private IBlackBox _box;


    // Use this for initialization
    void Start()
    {


        Physics.gravity = Gravity;// Vector3(0, Gravity, 0);
        rb = GetComponent<Rigidbody2D>();

        _experiment = new NeatExperiment();
        _experiment.CreateSimpleEA(this.name, NumInputs, NumOutputs, null);
        _box = _experiment.LoadChamp();

        _fuel = StartingFuel;

        _flame.Stop();

        //lastTime = Time.time;
    }

    private void Update()
    {
        UpdateUI();
        UpdateFlame();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody2D>();

        if (_landed)
            return;

        if (AIControlled)
            AICompute();
        else
            UserCompute();



        _fuel -= _thrust * Time.fixedDeltaTime;


        if (_rotation > 135)
            _rotation = 135;
        else if (_rotation < 45)
            _rotation = 45;

        //_velocity = rb.velocity.y;

        rb.rotation = _rotation - 90;

        float a = _rotation * Mathf.Deg2Rad;
        Vector2 rot = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

        _velocity += rot * _thrust * Time.fixedDeltaTime;
        _velocity += Gravity * Time.fixedDeltaTime;

        rb.AddForce(_velocity, ForceMode2D.Force);

        // rb.AddTorque(_torque, ForceMode2D.Impulse);
        //transform.Translate(new Vector3(0, _velocity * Time.fixedDeltaTime));




        //if (Mathf.Floor(Time.time) > lastTime)
        //{
        //    Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + _velocity.ToString());
        //    lastTime = Time.time;
        //}
    }

    private void UpdateFlame()
    {
        if (_thrust > 0)
        {
            Vector3 desiredScale = new Vector3(1f, 1f, (1.5f * _thrust / 4f));

            _flame.transform.localScale = Vector3.Lerp(_flame.transform.localScale, desiredScale, Time.deltaTime);

            _flame.Play();
        }

        else
        {
            _flame.Stop();
        }
    }

    private void UserCompute()
    {
        _rotation += -Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.Alpha1))
            _thrust = 1f;
        else if (Input.GetKey(KeyCode.Alpha2))
            _thrust = 2f;
        else if (Input.GetKey(KeyCode.Alpha3))
            _thrust = 3f;
        else if (Input.GetKey(KeyCode.Alpha4))
            _thrust = 4f;
        else
        {
            _thrust = 0f;
        }
    }

    private void UpdateUI()
    {
        TxtVelocity.text = _velocity.ToString("N1");
        if (_velocity.y < -40)
            TxtVelocity.color = Color.red;
        else
        {
            TxtVelocity.color = Color.white;
        }
        TxtAltitude.text = transform.position.y.ToString("N0");
        TxtFuel.text = _fuel.ToString("N1");
        TxtThrust.text = _thrust.ToString("N0");
    }



    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision with: " + other.gameObject.name);
        if (other.gameObject.tag == "Ground")
        {
            _landed = true;
            _thrust = 0;
            _velocity = Vector2.zero;
            _flame.Stop();

        }
    }



    public void Reset()
    {
        //if (_learningMode == false)
        //    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        _velocity = Vector2.zero;
        transform.position = new Vector3(0, StartingAltitude, 0);
        transform.rotation = Quaternion.identity;
        _fuel = StartingFuel;


        _thrust = 0;

        //_box.ResetState();
        _landed = false;

    }


    void AICompute()
    {
        var rb = GetComponent<Rigidbody2D>();
        ISignalArray inputArr = _box.InputSignalArray;
        ISignalArray outputArr = _box.OutputSignalArray;


        //set inputs
        inputArr[0] = transform.position.y / StartingAltitude;
        inputArr[1] = transform.position.y / StartingAltitude;
        inputArr[2] = rb.velocity.x / TerminalVel;
        inputArr[3] = rb.velocity.y / TerminalVel;
        inputArr[4] = _fuel / StartingFuel;

        _box.Activate();

        _thrust = (float)Math.Round(outputArr[0] * 4); //Math.Floor(outputArr[0] * 4.0);
                                                       //Ship.Thrust = 3.42;
        _rotation = (float)outputArr[1] * 360f;
    }


}

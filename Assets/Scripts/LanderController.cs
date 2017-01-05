using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LanderController : MonoBehaviour
{
    public const float StartingAltitude = 2400;
    public const float StartingFuel = 500;
    //public const double Gravity = -3.711;
    public const double TerminalVel = -200;

    public bool AIControlled = true;

    public Text TxtAltitude;
    public Text TxtVelocity;
    public Text TxtFuel;
    public Text TxtThrust;
    public ParticleSystem _flame;

    private bool _landed;
    private float _thrust;
    private float _fuel;
    private float lastTime = -1;
    private Rigidbody2D rb;


    private MyExperiment _experiment;
    private NeatEvolutionAlgorithm<NeatGenome> _ea;
    private IBlackBox _box;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _flame = GetComponent<ParticleSystem>();

        _experiment = new MyExperiment();
        _ea = _experiment.CreateSimpleEA("lander", 3, 1, GetFitness);
        _box = _experiment.LoadChamp();

        _fuel = StartingFuel;




        lastTime = Time.time;
    }

    void DoAI()
    {
        ISignalArray inputArr = _box.InputSignalArray;
        ISignalArray outputArr = _box.OutputSignalArray;

        //set inputs
        inputArr[0] = transform.position.y / StartingAltitude;
        inputArr[1] = rb.velocity.y / TerminalVel;
        inputArr[2] = _fuel / StartingFuel;
        //inputArr[3] = (double)i / 100.0;

        _box.Activate();

        _thrust = (float)Math.Round(outputArr[0] * 4); //Math.Floor(outputArr[0] * 4.0);
                                                       //Ship.Thrust = 3.42;
    }
    // Update is called once per frame
    void Update()
    {
        if (_landed)
            return;

        if (AIControlled)
            DoAI();
        else
        {
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
        if (_thrust > 0)
            _flame.Play();
        else
        {
            _flame.Stop();
        }

        _fuel -= _thrust * Time.deltaTime;

        TxtVelocity.text = rb.velocity.y.ToString("N1");
        TxtAltitude.text = transform.position.y.ToString("N0");
        TxtFuel.text = _fuel.ToString("N1");
        TxtThrust.text = _thrust.ToString("N0");

        if (Mathf.Floor(Time.time) > lastTime)
        {
            // Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + rb.velocity.y.ToString());
            lastTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (_landed)
            return;

        rb.AddForce(new Vector2(0, _thrust), ForceMode2D.Force);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _landed = true;
            _thrust = 0;
            Debug.Log("Collision with: " + other.gameObject.name);
        }
    }

    double GetFitness(IBlackBox box)
    {
        return 0;
    }

    public void Reset()
    {
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(0, StartingAltitude, 0);

        _fuel = StartingFuel;


        _thrust = 0;

        //_box.ResetState();
        _landed = false;

    }
}

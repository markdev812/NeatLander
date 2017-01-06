using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LanderController : UnitController
{
    public const float StartingAltitude = 2400;
    public const float StartingFuel = 500;
    //public const double Gravity = -3.711;
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
    //private float lastTime = -1;

    private Rigidbody2D rb;


    private NeatExperiment _experiment;
    private IBlackBox _box;
    private bool _learningMode;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (_learningMode == false)
        {
            _experiment = new NeatExperiment();
            _experiment.CreateSimpleEA(this.name, NumInputs, NumOutputs, null);
            _box = _experiment.LoadChamp();
        }
        _fuel = StartingFuel;


        _flame.Stop();

        //lastTime = Time.time;
    }


    // Update is called once per frame
    private void Update()
    {
        if (_learningMode == false)
        {
            TxtVelocity.text = rb.velocity.y.ToString("N1");

            TxtAltitude.text = transform.position.y.ToString("N0");
            TxtFuel.text = _fuel.ToString("N1");
            TxtThrust.text = _thrust.ToString("N0");

            if (AIControlled)
                Compute();
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


            if (_landed)
                return;



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






        //if (Mathf.Floor(Time.time) > lastTime)
        //{
        //	// Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + rb.velocity.y.ToString());
        //	lastTime = Time.time;
        //}
    }

    void FixedUpdate()
    {
        if (_landed)
            return;

        _fuel -= _thrust * Time.deltaTime;

        rb.AddForce(new Vector2(0, _thrust), ForceMode2D.Force);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _landed = true;
            _thrust = 0;
            _flame.Stop();
            //Debug.Log("Collision with: " + other.gameObject.name);
        }
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

    public override void Stop()
    {
        _learningMode = false;
    }


    public override void Activate(IBlackBox box)
    {

        _box = box;
        _learningMode = true;
    }

    void Compute()
    {
        ISignalArray inputArr = _box.InputSignalArray;
        ISignalArray outputArr = _box.OutputSignalArray;

        //set inputs
        inputArr[0] = transform.position.y / StartingAltitude;
        inputArr[1] = GetComponent<Rigidbody2D>().velocity.y / TerminalVel;
        inputArr[2] = _fuel / StartingFuel;
        //inputArr[3] = (double)i / 100.0;

        _box.Activate();

        _thrust = (float)Math.Round(outputArr[0] * 4); //Math.Floor(outputArr[0] * 4.0);
                                                       //Ship.Thrust = 3.42;
    }

    public override float GetFitness()
    {


        float nFuel = _fuel / StartingFuel;
        float nVelocity = rb.velocity.y / TerminalVel;
        float nAltitude = transform.position.y / StartingAltitude;

        float fitness = 0;
        //small reward for lower velocity
        fitness = (1 - nVelocity) * 5;
        //fitness += (1 - nAltitude)* 10;
        if (transform.position.y <= 2) //landed
        {
            if (rb.velocity.y > -40) //no crash
            {

                //big reward for safe landing
                fitness += 100;
                //fuel savings bonus
                fitness += nFuel * 10;

            }

        }

        return fitness > 0 ? fitness : 0;
    }
}

using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using UnityEngine;

public class LanderControllerEA : UnitController
{
    public float StartingAltitude = 2400;
    public float StartingFuel = 200;
    public float Gravity = -3.711f;
    public const float TerminalVel = -200;

    public int NumInputs;
    public int NumOutputs;


    private bool _landed;
    private float _thrust;
    private float _fuel;
    private float _velocity;
    private float lastTime = -1;



    private NeatExperiment _experiment;
    private IBlackBox _box;
    private bool _eaRunning;

    // Use this for initialization
    void Start()
    {


        _fuel = StartingFuel;




        //lastTime = Time.time;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {

        Compute();



        _fuel -= _thrust * Time.fixedDeltaTime;
        _velocity += ((_thrust + Gravity) * Time.fixedDeltaTime);
        if (_velocity < TerminalVel)
            _velocity = TerminalVel;
        transform.Translate(new Vector3(0, _velocity * Time.fixedDeltaTime));










        //if (Mathf.Floor(Time.time) > lastTime)
        //{
        //    Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + _velocity.ToString());
        //    lastTime = Time.time;
        //}
    }






    public void Reset()
    {
        //if (_learningMode == false)
        //    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        _velocity = 0f;
        transform.position = new Vector3(0, StartingAltitude, 0);

        _fuel = StartingFuel;


        _thrust = 0;

        //_box.ResetState();


    }

    public override void Stop()
    {
        _eaRunning = false;
    }


    public override void Activate(IBlackBox box)
    {

        _box = box;
        _eaRunning = true;
    }

    void Compute()
    {
        ISignalArray inputArr = _box.InputSignalArray;
        ISignalArray outputArr = _box.OutputSignalArray;


        //set inputs
        inputArr[0] = transform.position.y / StartingAltitude;
        inputArr[1] = _velocity / TerminalVel;
        inputArr[2] = _fuel / StartingFuel;
        //inputArr[3] = (double)i / 100.0;

        _box.Activate();

        _thrust = (float)Math.Round(outputArr[0] * 4); //Math.Floor(outputArr[0] * 4.0);
                                                       //Ship.Thrust = 3.42;
    }

    public override float GetFitness()
    {


        float nFuel = _fuel / StartingFuel;
        float nVelocity = _velocity / TerminalVel;
        float nAltitude = transform.position.y / StartingAltitude;

        float fitness = 0;
        //small reward for lower velocity
        fitness = (1 - nVelocity) * 5;
        //fitness += (1 - nAltitude)* 10;
        if (transform.position.y <= 0) //landed
        {
            if (_velocity > -40) //no crash
            {

                //big reward for safe landing
                fitness += 100;
                //fuel savings bonus
                fitness += nFuel * 10;

            }

        }
        fitness = fitness > 0 ? fitness : 0;


        return fitness;
    }
}

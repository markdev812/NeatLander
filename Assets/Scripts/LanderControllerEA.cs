using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using UnityEngine;

public class LanderControllerEA : UnitController
{
    public const float StartingAltitude = 2400;
    public const float ViewWidth = 1000;
    public const float StartingFuel = 500;
    public readonly Vector2 Gravity = new Vector2(0, -3.711f);

    public const float TerminalVel = -200;
    public const float CrashSpeed = -40;

    // public Vector2 Position = new Vector2(0, 0);
    public float Rotation { get; set; }
    //public double Altitude { get; private set; }
    public Vector2 Velocity = new Vector2(0, 0);
    public float Fuel { get; private set; }
    public float Thrust { get; private set; }
    public int NumInputs;
    public int NumOutputs;


    private bool _landed;
    //private float _thrust;
    //private float _fuel;
    //private float _velocity;
    private float lastTime = -1;



    private NeatExperiment _experiment;
    private IBlackBox _box;
    private bool _eaRunning;

    // Use this for initialization
    void Start()
    {


        //transform.position.x = 300f; //right of center
        // transform.position.y = StartingAltitude;
        Fuel = StartingFuel;



        //lastTime = Time.time;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {

        Compute();

        if (Fuel <= 0)
            Thrust = 0;

        Fuel -= Thrust * Time.fixedDeltaTime;

        float a = (Rotation + 90f) * Mathf.Deg2Rad;
        Vector2 rot = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

        Velocity += rot * Thrust;
        Velocity += Gravity;

        //if (Velocity.Y < TerminalVel)
        //    Velocity.Y = TerminalVel;

        // Position += Velocity * Time.fixedDeltaTime;

        transform.Translate(Velocity * Time.fixedDeltaTime);

        // if (Position.y < 0)
        //   Position.y = 0;


        //_fuel -= _thrust * Time.fixedDeltaTime;
        //_velocity += ((_thrust + Gravity) * Time.fixedDeltaTime);
        //if (_velocity < TerminalVel)
        //    _velocity = TerminalVel;
        //transform.Translate(new Vector3(0, _velocity * Time.fixedDeltaTime));










        //if (Mathf.Floor(Time.time) > lastTime)
        //{
        //    Debug.Log("" + (Mathf.Floor(Time.time)).ToString() + " " + _velocity.ToString());
        //    lastTime = Time.time;
        //}
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
        inputArr[1] = transform.position.y / StartingAltitude;
        inputArr[2] = Velocity.x / TerminalVel;
        inputArr[3] = Velocity.y / TerminalVel;
        inputArr[4] = Fuel / StartingFuel;
        //inputArr[3] = (double)i / 100.0;

        _box.Activate();

        Thrust = (float)Math.Round(outputArr[0] * 4); //Math.Floor(outputArr[0] * 4.0);
        Rotation = (float)outputArr[1] * 360f;                                              //Ship.Thrust = 3.42;
    }
    /// <summary>
    /// Calculate a fitness by normalizing and scaling the value by a "weight", centered around a desired "best" value
    /// </summary>
    /// <param name="x">incoming value</param>
    /// <param name="min">value range mininum</param>
    /// <param name="max">value range maximum</param>
    /// <param name="best">best value for maximum fitness</param>
    /// <param name="weight">importance of this value compared to others</param>
    /// <returns></returns>
    public float NormalizeFitness(float x, float min, float max, float best, float weight)
    {
        if (x < min || x > max)
            return 0;

        var offset = Math.Abs(best - x);
        var range = max - min;
        //var v = (1 + offset) * (1 / range);
        var v = 1 - (offset / range);
        var fit = v * weight;
        return fit;
    }
    public override float GetFitness()
    {


        float f = NormalizeFitness(Fuel, 0, StartingFuel, StartingFuel, 10);
        float v = NormalizeFitness(Velocity.y, TerminalVel, 0, CrashSpeed + 1, 100);
        float x = NormalizeFitness(transform.position.x, 0, ViewWidth, 500, 200);
        //double a = NormalizedFitness(Altitude, 1, StartingAltitude, 10, 1);

        float fitness = f + v + x;
        if (transform.position.y < 2 && Velocity.y > CrashSpeed && Math.Abs(Velocity.x) < 5) //safe landing?
        {
            fitness += 100;

        }


        //float nFuel = _fuel / StartingFuel;
        //float nVelocity = _velocity / TerminalVel;
        //float nAltitude = transform.position.y / StartingAltitude;

        //float fitness = 0;
        ////small reward for lower velocity
        //fitness = (1 - nVelocity) * 5;
        ////fitness += (1 - nAltitude)* 10;
        //if (transform.position.y <= 0) //landed
        //{
        //    if (_velocity > -40) //no crash
        //    {

        //        //big reward for safe landing
        //        fitness += 100;
        //        //fuel savings bonus
        //        fitness += nFuel * 10;

        //    }

        //}
        fitness = fitness > 0 ? fitness : 0;


        return fitness;
    }
}

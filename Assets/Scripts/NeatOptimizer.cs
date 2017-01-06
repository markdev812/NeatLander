using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeatLander;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NeatOptimizer : MonoBehaviour
{
    public int NumInputs;
    public int NumOutputs;
    public float LearningSpeed = 25;
    public float LearningDuration = 30;
    public GameObject UnitPrefab;

    private NeatEvolutionAlgorithm<NeatGenome> _ea;
    private NeatExperiment _experiment;
    private IBlackBox _box;
    Dictionary<IBlackBox, NeatController> _controllerMap = new Dictionary<IBlackBox, NeatController>();

    private DateTime startTime;
    private float timeLeft;
    private float accum;
    private int frames;
    private float updateInterval = 12;


    void Start()
    {
        _experiment = new NeatExperiment();
        _ea = _experiment.CreateSimpleEA(this.name, NumInputs, NumOutputs, this);
        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

        Time.timeScale = LearningSpeed;
        startTime = DateTime.Now;
        _ea.StartContinue();


    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0)
        {
            var fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
            //   print("FPS: " + fps);
            if (fps < 10)
            {
                Time.timeScale = Time.timeScale - 1;
                Debug.Log("Lowering time scale to " + Time.timeScale);
            }
        }
    }

    void ea_UpdateEvent(object sender, EventArgs e)
    {
        Debug.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));

        //Fitness = _ea.Statistics._maxFitness;
        //Generation = _ea.CurrentGeneration;


        //    Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));


    }

    void Stop()
    {
        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
        {
            _ea.Stop();
        }
    }

    public void Evaluate(IBlackBox box)
    {
        GameObject obj = Instantiate(UnitPrefab, UnitPrefab.transform.position, UnitPrefab.transform.rotation) as GameObject;
        NeatController controller = obj.GetComponent<NeatController>();
        controller.LearningMode = true;

        _controllerMap.Add(box, controller);

        controller.Activate(box);
    }
    public void StopEvaluation(IBlackBox box)
    {
        if (_controllerMap.ContainsKey(box))
        {
            NeatController ct = _controllerMap[box];

            Destroy(ct.gameObject);
        }
    }
    public float GetFitness(IBlackBox box)
    {
        if (_controllerMap.ContainsKey(box))
        {
            return _controllerMap[box].GetFitness();
        }
        return 0;
    }
}

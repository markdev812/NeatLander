using SharpNeat.Phenomes;
using UnityEngine;

public abstract class NeatController : MonoBehaviour
{
    public bool LearningMode { get; set; }

    public abstract void Activate(IBlackBox box);

    //public abstract void Stop();

    public abstract float GetFitness();
}

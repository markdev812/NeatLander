using UnityEngine;
using System.Collections;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;

namespace SharpNeatLander
{
    public delegate double FitnessFunction(IBlackBox box);

    /// <summary>

    /// </summary>
    public class MyEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private readonly bool _hasStopFitness;
        private readonly double _stopFitness;
        ulong _evalCount;
        bool _stopConditionSatisfied;

        private readonly FitnessFunction _fitnessFunction;

        Dictionary<IBlackBox, FitnessInfo> _dict = new Dictionary<IBlackBox, FitnessInfo>();

        public MyEvaluator(FitnessFunction f)
        {
            _fitnessFunction = f;
            _hasStopFitness = false;
        }
        public MyEvaluator(FitnessFunction f, double stopFitness)
        {
            _fitnessFunction = f;
            _stopFitness = stopFitness;
            _hasStopFitness = true;
        }

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the problem domain and return its fitness score.
        /// </summary>
        public IEnumerator Evaluate(IBlackBox box)
        {
            double fitness = 0;

            _evalCount++;

            box.ResetState();


            if (_fitnessFunction != null)
            {
                fitness = _fitnessFunction(box);
                yield return new WaitForSeconds(30);
            }

            if (_hasStopFitness && fitness >= _stopFitness)
            {
                _stopConditionSatisfied = true;
            }

            FitnessInfo fitInfo = new FitnessInfo(fitness, fitness);
            _dict.Add(box, fitInfo);
            
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _dict = new Dictionary<IBlackBox, FitnessInfo>();
        }

        public FitnessInfo GetLastFitness(IBlackBox phenome)
        {
            if (_dict.ContainsKey(phenome))
            {
                FitnessInfo fit = _dict[phenome];
                _dict.Remove(phenome);

                return fit;
            }

            return FitnessInfo.Zero;
        }
    }
}
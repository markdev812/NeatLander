using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharpNeatLander
{
    // public delegate float FitnessFunction(IBlackBox box);

    /// <summary>

    /// </summary>
    public class NeatEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private readonly bool _hasStopFitness;
        private readonly float _stopFitness;
        ulong _evalCount;
        bool _stopConditionSatisfied;

        private readonly NeatOptimizer _optimizer;

        Dictionary<IBlackBox, FitnessInfo> _dict = new Dictionary<IBlackBox, FitnessInfo>();

        public NeatEvaluator(NeatOptimizer optimizer)
        {
            _optimizer = optimizer;
            _hasStopFitness = false;
        }
        public NeatEvaluator(NeatOptimizer optimizer, float stopFitness)
        {
            _optimizer = optimizer;
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
            if (_optimizer != null)
            {

                _optimizer.Evaluate(box);
                yield return new WaitForSeconds(_optimizer.LearningDuration);
                _optimizer.StopEvaluation(box);
                float fit = _optimizer.GetFitness(box);

                FitnessInfo fitness = new FitnessInfo(fit, fit);
                _dict.Add(box, fitness);

            }



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
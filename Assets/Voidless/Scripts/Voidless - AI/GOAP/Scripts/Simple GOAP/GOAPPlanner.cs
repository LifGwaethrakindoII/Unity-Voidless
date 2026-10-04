using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI
{
    [Serializable]
    public class GOAPPlanner
    {
        [SerializeField] private List<GOAPAction> _actions;
    
        /// <summary>Gets and Sets actions property.</summary>
        public List<GOAPAction> actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        /// <summary>Gets the immediate best Action to take.</summary>
        /// <param name="_goals">Current Goals.</param>
        /// <returns>Best Action to take given the current Goals.</returns>
        public GOAPAction Plan(List<GOAPGoal> _goals)
        {
            foreach(GOAPGoal goal in _goals)
            {
                if(!goal.Evaluate()) continue;

                GOAPAction bestAction = null;
                float lowestCost = Mathf.Infinity;

                foreach(GOAPAction action in actions)
                {
                    if(action.Evaluate() && action.cost < lowestCost)
                    {
                        bestAction = action;
                        lowestCost = action.cost;
                    }
                }

                return bestAction;
            }

            return null;
        }
    }
}
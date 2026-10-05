using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI
{
    [Serializable]
    public class GOAPAction
    {
        [SerializeField] private string _name;
        [SerializeField] private float _cost;
        [SerializeField] private Func<bool> _precondition;
        [SerializeField] private Action _effect;

        /// <summary>Gets and Sets name property.</summary>
        public string name
        {
           get { return _name; }
           set { _name = value; }
        }

        /// <summary>Gets and Sets cost property.</summary>
        public float cost
        {
          get { return _cost; }
          set { _cost = value; }
        }

        /// <summary>Gets and Sets precondition property.</summary>
        public Func<bool> precondition
        {
           get { return _precondition; }
           set { _precondition = value; }
        }

        /// <summary>Gets and Sets effect property.</summary>
        public Action effect
        {
           get { return _effect; }
           set { _effect = value; }
        }

        /// <summary>GOAPAction's constructor.</summary>
        /// <param name="_name">Action's name.</param>
        /// <param name="_cost">Action's cost.</param>
        /// <param name="_precondition">Action's precondition.</param>
        /// <param name="_effect">Action's effect.</param>
        public GOAPAction(string _name, float _cost, Func<bool> _precondition, Action _effect)
        {
            name = _name;
            cost = _cost;
            precondition = _precondition;
            effect = _effect;
        }

        /// <summary>Evaluates whether the Action can be performed.</summary>
        public bool Evaluate()
        {
            return precondition != null ? precondition() : false;
        }

        /// <summary>Performs Action.</summary>
        public void Perform()
        {
            if(effect != null) effect();
        }
    }
}
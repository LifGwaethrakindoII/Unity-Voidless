using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI
{
    public class GOAPGoal
    {
        [SerializeField] private string _name;
        [SerializeField] private Func<bool> _condition;

        /// <summary>Gets and Sets name property.</summary>
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>Gets and Sets condition property.</summary>
        public Func<bool> condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        /// <summary>GOAPGoal's constructor.</summary>
        /// <param name="_name">Goal's name.</param>
        /// <param name="_condition">Goal's condition.</param>
        public GOAPGoal(string _name, Func<bool> _condition)
        {
            name = _name;
            condition = _condition;
        }

        /// <summary>Evaluates whether the condition has been approved.</summary>
        public bool Evaluate()
        {
            return condition != null ? condition() : false;
        }
    }
}
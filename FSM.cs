using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Coursework
{
    class FSM
    {
        private object m_Owner;
        private List<State> m_States;
        private State m_CurrentState;
        public FSM()
        : this(null)
        {
        }
        public FSM(object owner)
        {
            m_Owner = owner;
            m_States = new List<State>();
            m_CurrentState = null;
        }
        public void Initialise(string stateName)
        {
            m_CurrentState = m_States.Find(state => state.Name.Equals(stateName));
            if (m_CurrentState != null)
            {
                m_CurrentState.Enter(m_Owner);
            }
        }
        public void AddState(State state)
        {
            m_States.Add(state);
        }
        public void Update(GameTime gameTime)
        {
            // Null check the current state of the FSM
            if (m_CurrentState == null) return;
            // Check the conditions for each transition of the current state
            foreach (Transition t in m_CurrentState.Transitions)
            {
                // If the condition has evaluated to true
                // then transition to the next state
                if (t.Condition())
                {
                    m_CurrentState.Exit(m_Owner);
                    m_CurrentState = t.NextState;
                    m_CurrentState.Enter(m_Owner);
                    break;
                }
            }
            // Execute the current state
            m_CurrentState.Execute(m_Owner, gameTime);
        }
    }
}

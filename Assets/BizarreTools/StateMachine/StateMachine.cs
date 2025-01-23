using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizarreTools.StateMachine
{
    public abstract class StateMachine: MonoBehaviour
    {
        private IState currentState;

        public List<StateTransition> stateTransitions = new List<StateTransition>();

        public string debugState = "";

        public virtual void ChangeState(IState _state)
        {
            if(currentState != null)
            {
                currentState.OnStateExit();
            }


            currentState = _state;
            currentState.OnStateEnter();
            debugState = currentState.GetType().ToString();
        }

        public void UpdateState()
        {
            currentState.OnStateUpdate();

        }

        public void FixedUpdateState()
        {
            currentState.OnFixedUpdate();
        }

        public void LateUpdateState()
        {
            currentState.OnLateUpdate();
        }

        public IState GetCurrentState()
        {
            return currentState;
        }

        public void DefineTransition(IState _from, IState _to)
        {
            StateTransition trans = new StateTransition() { from = _from, to = _to};

            trans.SetupID();
            stateTransitions.Add(trans);
        }

        public virtual void TryChangeState(IState _to)
        {
            if(ValidTransitionCheck(GetCurrentState(), _to))
            {
                ChangeState(_to);
            }
        }

        public bool ValidTransitionCheck(IState _from, IState _to)
        {
            if(_from != GetCurrentState())
            {
                return false;
            }

            bool value = false;

            string ID = _from.GetType().ToString() + _to.GetType().ToString();

            if (stateTransitions.Exists(x => x.id == ID))
            {
                value = true;
            }

            return value;
        }

    }

    public class StateTransition
    {
        public IState from, to;

        public string id;

        public void SetupID()
        {
            id = (from.GetType().ToString() + to.GetType().ToString());
        }
    }

}


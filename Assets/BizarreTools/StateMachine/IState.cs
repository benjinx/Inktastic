using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizarreTools.StateMachine
{
    public interface IState
    {
        void OnStateEnter();
        void OnStateUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
        void OnStateExit();

    }
}


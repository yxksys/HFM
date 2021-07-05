using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    delegate bool HandleType<T>(IState current, IState pervious, ref T value);

    interface IContext<T> : IEnumerator<T>, IEnumerable<T>
    {
        StateArgs Value { get; set; }
        IDictionary<Tuple<IState, IState>, HandleType<StateArgs>> Handles { get; set; }
        IState CurrentState { get; set; }
        bool Transition(IState next);        
    }
}

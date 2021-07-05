using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    class Context<T>:IContext<T>
    {
        StateArgs _data;

        StateArgs IContext<T>.Value { get => _data; set => _data = value; }
        IDictionary<Tuple<IState, IState>, HandleType<StateArgs>> IContext<T>.Handles { get; set; } = new Dictionary<Tuple<IState, IState>, HandleType<StateArgs>>();        
        public IState CurrentState { get; set; }

        T IEnumerator<T>.Current =>(T)(object)(this as IContext<T>).Value;

        object IEnumerator.Current =>(this as IContext<T>).Value;        

        bool IContext<T>.Transition(IState next)
        {
            IContext<T> context = this as IContext<T>;
            if(context.CurrentState==null || context.CurrentState.Nexts.Contains(next))
            {
                //前件处理
                var key = Tuple.Create(next, context.CurrentState);
                if(context.Handles.ContainsKey(key) && context.Handles[key]!=null)
                {
                    if(!context.Handles[key](next,context.CurrentState,ref this._data))
                    {
                        return false;
                    }
                }
                context.CurrentState = next;
                return true;
            }
            return false;
        }
        bool IEnumerator.MoveNext()
        {
            //后件处理
            IContext<T> context = this as IContext<T>;
            IState current = context.CurrentState;
            if(current==null)
            {
                throw new Exception("必须设置初始状态");
            }
            if(context.CurrentState.Selector!=null)
            {
                IState next = context.CurrentState.Selector(context.CurrentState);
                return context.Transition(next);
            }
            return false;
        }
        public void Reset()
        {
            throw new NotImplementedException();
        }
        #region IDisposable Support
        bool disposedValue = false;//要检测冗余调用
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //TODO:释放托管状态（托管对象）
                }
                //TODO：释放未托管资源（未托管对象）并在以下内容中替代终结器
                //TODO：将大型字段设置为null
                disposedValue = true;
            }
        }
        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Context() {
        // // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        // Dispose(false);
        // }
        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            //GC.SuppressFinalize(this);
        }        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion        
    }
}

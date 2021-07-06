using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HFM.Components
{
    /// <summary>
    /// 继承自系统Dictionary，可以监听对Dictionary对象的Add、Clear、Remove方法及键值赋值操作
    /// 当操作发生时回调绑定的处理方法
    /// </summary>
    public delegate void VoidValueCallback();   
    public delegate void ValueCallback<TKey, TValue>(TKey key, TValue value);
    public delegate void ValueCallback<TKey>(TKey key);      
    public class ObserverDictionary<TKey,TValue>:System.Collections.Generic.Dictionary<TKey,TValue>
    {        
        public VoidValueCallback DoOnClear;        
        public ValueCallback<TKey, TValue> DoOnAdd,DoOnSetKeyValue;
        public ValueCallback<TKey> DoOnRemove;        
        #region 构造方法的重写
        public ObserverDictionary():base()
        {

        }
        public ObserverDictionary(int capacity):base(capacity)
        {

        }
        public ObserverDictionary(IEqualityComparer<TKey> comparer):base(comparer)
        {

        }
        public ObserverDictionary(IDictionary<TKey, TValue> dictionary):base(dictionary)
        {

        }
        public ObserverDictionary(int capacity, IEqualityComparer<TKey> comparer):base(capacity,comparer)
        {

        }
        public ObserverDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer):base(dictionary,comparer)
        {

        }
        protected ObserverDictionary(SerializationInfo info, StreamingContext context):base(info,context)
        {

        }
        #endregion
        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
                DoOnSetKeyValue?.Invoke(key, value);
            }
        }
        public new void Add(TKey key, TValue value)
        {            
            base.Add(key, value);
            //通过委托回调方法           
            DoOnAdd?.Invoke(key, value);
        }
        public new void Clear()
        {
            base.Clear();
            //通过委托回调方法
            DoOnClear?.Invoke();
        }
        public new void Remove(TKey key)
        {
            base.Remove(key);
            //通过委托回调方法
            DoOnRemove?.Invoke(key);
        }
    }
}

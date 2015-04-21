using System;
using System.Collections;
using System.Collections.Generic;

namespace NarenT.Common
{
    public abstract class Option<T> : IOption<T>
    {
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual bool HasValue()
        {
            return false;
        }

        public abstract IOption<TResult> Into<TResult>(Func<T, IOption<TResult>> fn);

        public abstract IOption<TResult> Into<TResult>(Func<T, TResult> fn);

        public abstract void IntoAction(Action<T> action);

        public abstract IOption<T> When(Predicate<T> predicate);
    }
}
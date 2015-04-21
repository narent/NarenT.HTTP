using System;
using System.Collections.Generic;

namespace NarenT.Common
{
    public interface IOption<T> : IEnumerable<T>
    {
        bool HasValue();

        IOption<TResult> Into<TResult>(Func<T, IOption<TResult>> fn);

        IOption<TResult> Into<TResult>(Func<T, TResult> fn);

        void IntoAction(Action<T> action);

        IOption<T> When(Predicate<T> predicate);
    }
}
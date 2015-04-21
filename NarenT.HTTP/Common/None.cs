using System;
using System.Collections.Generic;

namespace NarenT.Common
{
    internal class None<T> : Option<T>, IEquatable<None<T>>
    {
        public override IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        public override string ToString()
        {
            return "None";
        }

        public bool Equals(None<T> other)
        {
            return !object.ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var type = typeof(None<T>);

            if (obj.GetType() != type)
            {
                return obj.GetType().GetGenericTypeDefinition() == type.GetGenericTypeDefinition();
            }

            var other = (None<T>)obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override IOption<TResult> Into<TResult>(Func<T, IOption<TResult>> fn)
        {
            return Maybe.None<TResult>();
        }

        public override IOption<TResult> Into<TResult>(Func<T, TResult> fn)
        {
            return Maybe.None<TResult>();
        }

        public override void IntoAction(Action<T> action)
        {
        }

        public override IOption<T> When(Predicate<T> predicate)
        {
            return this;
        }
    }
}
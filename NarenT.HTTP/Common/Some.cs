using System;
using System.Collections.Generic;

namespace NarenT.Common
{
    internal class Some<T> : Option<T>, IEquatable<Some<T>>
    {
        private Some(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public static Option<TSome> SomeOrNone<TSome>(TSome someValue)
        {
            if (EqualityComparer<TSome>.Default.Equals(someValue, default(TSome)))
            {
                return new None<TSome>();
            }

            return new Some<TSome>(someValue);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            yield return this.Value;
        }

        public override bool HasValue()
        {
            return true;
        }

        public override IOption<TResult> Into<TResult>(Func<T, IOption<TResult>> fn)
        {
            return fn(this.Value);
        }

        public override IOption<TResult> Into<TResult>(Func<T, TResult> fn)
        {
            return Some<T>.SomeOrNone(fn(this.Value));
        }

        public override void IntoAction(Action<T> action)
        {
            action(this.Value);
        }

        public override IOption<T> When(Predicate<T> predicate)
        {
            return predicate(this.Value) ? this : Maybe.None<T>();
        }

        public override string ToString()
        {
            return string.Format("Some{{ Value: {0}}}", this.Value);
        }

        public bool Equals(Some<T> other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            return object.ReferenceEquals(this, other) || object.Equals(other.Value, this.Value);
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

            var some = obj as Some<T>;

            return some != null && this.Equals(some);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
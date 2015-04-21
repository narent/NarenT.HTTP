using System;
using System.Collections.Generic;
using System.Linq;

namespace NarenT.Common
{
    public static class MaybeExtensions
    {
        public static IOption<T> SomeOrNone<T>(this T? nullableValue) where T : struct
        {
            return nullableValue.HasValue ? nullableValue.Value.SomeOrNone() : Maybe.None<T>();
        }

        public static IOption<T> SomeOrNone<T>(this T someValue)
        {
            return Maybe.SomeOrNone(someValue);
        }

        public static IOption<string> NoneIfBlankOrNull(this string someString)
        {
            return !string.IsNullOrWhiteSpace(someString) ? Maybe.SomeOrNone(someString) : Maybe.None<string>();
        }

        public static IOption<TValue> KeyValueOrNone<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            return dict.TryGetValue(key, out val) ? val.SomeOrNone() : Maybe.None<TValue>();
        }

        public static IOption<TResult> Or<TResult, T>(this IOption<T> some, TResult other) where T : TResult
        {
            return some.HasValue() ? some as IOption<TResult> : other.SomeOrNone();
        }

        public static IOption<T> FirstOrNone<T>(this IEnumerable<T> someEnum)
        {
            return someEnum.FirstOrDefault().SomeOrNone();
        }

        public static IOption<string> SomeOrBlank(this string value)
        {
            return Maybe.SomeOrBlank(value);
        }

        public static string OrBlank(this IOption<string> value)
        {
            return value.FirstOrLazy(() => string.Empty);
        }

        public static IOption<TInput> TapInto<TInput>(this IOption<TInput> thing, Action<TInput> sideEffect)
        {
            thing.IntoAction(sideEffect);
            return thing;
        }

        public static TInput Tap<TInput>(this TInput thing, Action<TInput> sideEffect)
        {
            if (sideEffect == null)
            {
                throw new ArgumentNullException("sideEffect");
            }

            sideEffect(thing);
            return thing;
        }

        public static TInput DoNot<TInput>(this TInput thing, Action<TInput> sideEffect)
        {
            if (sideEffect == null)
            {
                throw new ArgumentNullException("sideEffect");
            }

            return thing;
        }

        public static Predicate<T> OrTimeHasPassed<T>(this Predicate<T> condition, TimeSpan duration, Action<T> onTimeout = null)
        {
            return condition.OrTimeHasPassed(DateTime.Now.Add(duration), onTimeout);
        }

        public static IOption<Uri> UriOrNone(this string href)
        {
            Uri uri;
            return Uri.TryCreate(href, UriKind.Absolute, out uri) ? uri.SomeOrNone() : Maybe.None<Uri>();
        }

        private static Predicate<T> OrTimeHasPassed<T>(this Predicate<T> condition, DateTime later, Action<T> onTimeout = null)
        {
            Predicate<T> wrapperPredicate = t =>
                {
                    if (condition(t))
                    {
                        return true;
                    }

                    var timeout = DateTime.Now > later;
                    if (timeout && onTimeout != null)
                    {
                        onTimeout(t);
                    }

                    return timeout;
                };
            return wrapperPredicate;
        }
    }
}
namespace NarenT.Common
{
    public static class Maybe
    {
        public static IOption<T> SomeOrNone<T>(T someValue)
        {
            return Some<T>.SomeOrNone(someValue);
        }

        public static IOption<object> NullObject()
        {
            return SomeOrNone<object>(null);
        }

        public static IOption<string> SomeOrBlank(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Maybe.None<string>() : value.SomeOrNone();
        }

        public static IOption<T> None<T>()
        {
            return new None<T>();
        }
    }
}
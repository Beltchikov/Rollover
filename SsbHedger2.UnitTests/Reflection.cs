namespace SsbHedger2.UnitTests
{
    public class Reflection
    {
        public static void CallMethod(object sut, string methodName, object[] parameters)
        {
            var methodInfo = sut.GetType().GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo?.Invoke(sut, parameters);
        }

        public static void SetFiledValue(object sut, string fieldName, object value)
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(sut, value);
        }

        public static T? GetFiledValue<T>(object sut, string fieldName)
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T?)fieldInfo?.GetValue(sut);
        }

        public static void SetPropertyValue(object sut, string fieldName, object value)
        {
            var fieldInfo = sut.GetType().GetField(
                $"<{fieldName}>k__BackingField",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(sut, value);
        }
    }
}

using System.Reflection;

namespace BNRNew_API.utils
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination, bool skipIfDestNotNull = false, bool skipIfSourceNull = false)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");

            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();
            // Collect all the valid properties to map
            var results = from srcProp in typeSrc.GetProperties()
                          let targetProperty = typeDest.GetProperty(srcProp.Name)
                          where srcProp.CanRead
                          && targetProperty != null
                          && (targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate)
                          && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0
                          && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
                          select new { sourceProperty = srcProp, targetProperty = targetProperty };

            //map the properties
            foreach (var props in results)
            {
                /*if (!skipIfDestNotNull) {
                    var destVal = props.targetProperty.GetValue(destination, null);
                    if (destVal != null)
                        continue;
                }*/
                var sourceVal = props.sourceProperty.GetValue(source, null);

                if (skipIfSourceNull && sourceVal == null)
                    continue;

                props.targetProperty.SetValue(destination, sourceVal, null);
            }
        }
    }
}

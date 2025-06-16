using System.Reflection;
using OptimizationEngine.Models;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Extracts cBot parameter metadata via reflection.
    /// </summary>
    public static class ParameterExtractor
    {
        public static IEnumerable<ParameterDefinition> Extract(Type cBotType)
        {
            var props = cBotType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes()
                    .Any(attr => attr.GetType().Name == "ParameterAttribute"));

            foreach (var pi in props)
            {
                // get the ParameterAttribute instance
                var attr = pi.GetCustomAttributes()
                    .First(a => a.GetType().Name == "ParameterAttribute");
                var param = new ParameterDefinition
                {
                    Name = attr.GetType().GetProperty("Name")?.GetValue(attr) as string ?? pi.Name,
                    PropertyName = pi.Name,
                    ParameterType = pi.PropertyType,
                    DefaultValue = pi.GetValue(Activator.CreateInstance(cBotType)),
                };

                // handle numeric constraints
                if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(double) || pi.PropertyType == typeof(decimal))
                {
                    param.MinValue = attr.GetType().GetProperty("MinValue")?.GetValue(attr);
                    param.MaxValue = attr.GetType().GetProperty("MaxValue")?.GetValue(attr);
                    param.StepValue = attr.GetType().GetProperty("Step")?.GetValue(attr);
                }

                // handle enum types
                if (pi.PropertyType.IsEnum)
                {
                    param.EnumValues = Enum.GetNames(pi.PropertyType);
                }

                yield return param;
            }
        }
    }
}

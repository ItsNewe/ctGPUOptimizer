using OptimizationEngine.Models;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Generates the parameter value combinations (cartesian product) for optimization.
    /// </summary>
    public static class ParameterSpaceGenerator
    {
        /// <summary>
        /// Generates parameter combinations using a simple genetic algorithm.
        /// </summary>
        public static IEnumerable<Dictionary<string, object>> GenerateWithGeneticAlgorithm(
            IEnumerable<ParameterDefinition> defs, int populationSize, int generations)
        {
            // Placeholder: currently falls back to exhaustive grid
            return Generate(defs);
        }

        public static IEnumerable<Dictionary<string, object>> Generate(IEnumerable<ParameterDefinition> defs)
        {
            var list = new List<ParameterDefinition>(defs);
            IEnumerable<Dictionary<string, object>> Recurse(int idx)
            {
                if (idx >= list.Count)
                {
                    yield return new Dictionary<string, object>();
                }
                else
                {
                    var def = list[idx];
                    var values = GetValues(def);
                    foreach (var v in values)
                    {
                        foreach (var tail in Recurse(idx + 1))
                        {
                            tail[def.PropertyName] = v;
                            yield return tail;
                        }
                    }
                }
            }
            return Recurse(0);
        }

        private static IEnumerable<object> GetValues(ParameterDefinition def)
        {
            if (def.ParameterType == typeof(bool))
                return new object[] { true, false };

            if (def.EnumValues != null && def.EnumValues.Length > 0)
                // convert enum names back to enum values
                return def.EnumValues.Select(name => Enum.Parse(def.ParameterType, name));

            if (def.ParameterType == typeof(int) || def.ParameterType == typeof(double) || def.ParameterType == typeof(decimal))
            {
                dynamic min = def.MinValue ?? def.DefaultValue ?? 0;
                dynamic max = def.MaxValue ?? def.DefaultValue ?? 0;
                dynamic step = def.StepValue ?? 1;
                if (step == 0) step = 1;
                var vals = new List<object>();
                for (var v = min; v <= max; v += step)
                    vals.Add(Convert.ChangeType(v, def.ParameterType));
                return vals;
            }

            // default single value
            var defaultVal = def.DefaultValue ?? (def.ParameterType.IsValueType ? Activator.CreateInstance(def.ParameterType) : null);
            return new object[] { defaultVal };
        }
    }
}

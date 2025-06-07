using System;

namespace OptimizationEngine.Models
{
    /// <summary>
    /// Defines a single cBot parameter with its metadata.
    /// </summary>
    public class ParameterDefinition
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public Type ParameterType { get; set; }
        public object? DefaultValue { get; set; }

        // Numeric constraints
        public object? MinValue { get; set; }
        public object? MaxValue { get; set; }
        public object? StepValue { get; set; }

        // For enum types
        public string[]? EnumValues { get; set; }

        // Any custom validation rules
        public string? ValidationRule { get; set; }
    }
}

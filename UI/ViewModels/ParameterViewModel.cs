using System;
using OptimizationEngine.Models;
using ReactiveUI;

namespace OptimizationEngine.UI.ViewModels
{
    /// <summary>
    /// ViewModel wrapper for a single parameter definition and its chosen value.
    /// </summary>
    public class ParameterViewModel : ReactiveObject
    {
        private object _value;

        public ParameterViewModel(ParameterDefinition def)
        {
            Name = def.Name;
            PropertyName = def.PropertyName;
            ParameterType = def.ParameterType;
            MinValue = def.MinValue;
            MaxValue = def.MaxValue;
            StepValue = def.StepValue;
            EnumValues = def.EnumValues;
            DefaultValue = def.DefaultValue;
            // Initialize value
            Value = DefaultValue;
        }

        public string Name { get; }
        public string PropertyName { get; }
        public Type ParameterType { get; }
        public object DefaultValue { get; }
        public object MinValue { get; }
        public object MaxValue { get; }
        public object StepValue { get; }
        public string[] EnumValues { get; }

        public object Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}

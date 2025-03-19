using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rcon.Commands;

namespace RconCopilot
{
    public class LocalizedCommandButton : Button
    {
        public static readonly BindableProperty CommandValueProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandValue),
                returnType: typeof(string),
                declaringType: typeof(LocalizedCommandButton),
                defaultValue: string.Empty);

        public string CommandValue
        {
            get => (string)GetValue(CommandValueProperty);
            set => SetValue(CommandValueProperty, value);
        }

        // This property stores the complete command data.
        public static readonly BindableProperty CommandDataProperty =
            BindableProperty.Create(
                propertyName: nameof(CommandData),
                returnType: typeof(PredefinedCommand),
                declaringType: typeof(LocalizedCommandButton),
                defaultValue: default(PredefinedCommand));

        public PredefinedCommand CommandData
        {
            get => (PredefinedCommand)GetValue(CommandDataProperty);
            set => SetValue(CommandDataProperty, value);
        }

    }
}

using System.Windows.Controls;
using System.Windows.Media;

using static LUC.Console.CConsole;

namespace LUC.Console
{
    public class CustomTextBox : TextBox
    {
        internal readonly SolidColorBrush m_infoForeground = new(Colors.White);
        internal readonly SolidColorBrush m_errorForeground = new(Colors.LightYellow);
        internal readonly SolidColorBrush m_criticalForeground = new(Colors.Orange);
        internal readonly SolidColorBrush m_fatalForeground = new(Colors.Red);

        public CustomTextBox () { }

        public CustomTextBox (string logString, TypeOfLog typeOfLog, string typeOfLogger)
        {
            Background = new SolidColorBrush(Colors.DimGray);
            BorderBrush = new SolidColorBrush(Colors.DimGray);
            TextWrapping = System.Windows.TextWrapping.Wrap;
            IsReadOnly = true;
            Text = logString;
            ToolTip = typeOfLogger;

            Foreground = typeOfLog switch
            {
                TypeOfLog.Error => m_errorForeground,
                TypeOfLog.CriticalError => m_criticalForeground,
                TypeOfLog.FatalError => m_fatalForeground,
                TypeOfLog.Info => m_infoForeground,
                _ => m_infoForeground,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using static LUC.Console.CConsole;

namespace LUC.Console
{
    public partial class MainWindow : Window
    {
        private const int LINES_BEFORE_ENDLOG = 5;
        private const int NUMBER_OF_LINES_IN_LOG = 500;
        private readonly object m_lock = new();

        private bool m_isCriticalError;
        private bool m_isError;
        private bool m_isFatalError;
        private bool m_isInfo;

        public MainWindow()
        {
            FullLog = new ObservableCollection<TextBox>();
            Log = new ObservableCollection<TextBox>();

            InitializeComponent();

            Logs.ItemsSource = Log;
        }

        public ObservableCollection<TextBox> Log { get; }
        private ObservableCollection<TextBox> FullLog { get; set; }
        private Dictionary<string, ObservableCollection<TextBox>> LoggerCollections { get; set; } = new Dictionary<string, ObservableCollection<TextBox>>();

        public void WriteLine(string logString, TypeOfLog typeOfLog, string typeOfLogger)
        {
            lock (m_lock)
            {
                var newLog = new CustomTextBox(logString, typeOfLog, typeOfLogger);

                FullLog.Add(newLog);

                while (FullLog.Count > NUMBER_OF_LINES_IN_LOG)
                {
                    FullLog.RemoveAt(0);
                }

                AddLogToList(newLog, typeOfLog);

                //This is autoscroll.
                if (loggers.SelectedItem == null)
                {
                    return;
                }

                if (loggers.SelectedItem is not TabItem ti || ti.Content is not ListBox currentLog)
                {
                    return;
                }

                if (currentLog.Items.Count < 5)
                {
                    currentLog.SelectedIndex = currentLog.Items.Count - 1;
                }

                if (currentLog.SelectedIndex > currentLog.Items.Count - LINES_BEFORE_ENDLOG && currentLog.Items.Count > 5)
                {
                    currentLog.SelectedIndex = currentLog.Items.Count - 1;
                    currentLog.ScrollIntoView(currentLog.Items[currentLog.SelectedIndex]);
                }

                if (currentLog.Items.Count == 1)
                {
                    currentLog.SelectedIndex = 0;
                }
            }
        }

        private void AddLogToList(CustomTextBox log, TypeOfLog typeOfLog)
        {
            lock (m_lock)
            {
                if (!ShouldAddLogString(typeOfLog))
                {
                    return;
                }

                Log.Add(log);

                string? typeOfLogger = log.ToolTip.ToString();

                // Checking if a collection for this logger already exists
                if (!LoggerCollections.TryGetValue(typeOfLogger, out var loggerCollection))
                {
                    loggerCollection = new ObservableCollection<TextBox>();
                    LoggerCollections[typeOfLogger] = loggerCollection;

                    // Creating a new tab for new logger
                    AddNewTab(typeOfLogger, loggerCollection);
                }

                loggerCollection.Add(log);

                while (loggerCollection.Count > NUMBER_OF_LINES_IN_LOG)
                {
                    loggerCollection.RemoveAt(0);
                }
            }
        }

        private void AddNewTab(string loggerName, ObservableCollection<TextBox> collection)
        {
            var listBox = new ListBox
            {
                Name = $"{loggerName}_Logs",
                ItemsSource = collection,
                Background = Brushes.DimGray
            };

            ScrollViewer.SetCanContentScroll(listBox, true);

            TabItem newTab = new TabItem
            {
                Header = loggerName,
                Content = listBox
            };

            loggers.Items.Add(newTab);
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            lock (m_lock)
            {
                switch (((ToggleButton)sender).Name)
                {
                    case "buttonInfo":
                        m_isInfo = true;
                        break;
                    case "buttonError":
                        m_isError = true;
                        break;
                    case "buttonCriticalError":
                        m_isCriticalError = true;
                        break;
                    case "buttonFatalError":
                        m_isFatalError = true;
                        break;
                    default:
                        m_isInfo = true;
                        break;
                }

                MakeLogList();
            }
        }

        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            lock (m_lock)
            {
                switch (((ToggleButton)sender).Name)
                {
                    case "buttonInfo":
                        m_isInfo = false;
                        break;
                    case "buttonError":
                        m_isError = false;
                        break;
                    case "buttonCriticalError":
                        m_isCriticalError = false;
                        break;
                    case "buttonFatalError":
                        m_isFatalError = false;
                        break;
                    default:
                        m_isInfo = false;
                        break;
                }

                MakeLogList();
            }
        }

        private void MakeLogList()
        {
            lock (m_lock)
            {
                //if Logs don`t initialized when we don`t need this method
                if (Logs == null)
                {
                    return;
                }

                Log.Clear();

                foreach (var loggerCollection in LoggerCollections.Values)
                {
                    loggerCollection.Clear();
                }

                var customTextBox = new CustomTextBox();

                foreach (TextBox logString in FullLog)
                {
                    if (!ShouldAddLogString(logString, customTextBox))
                    {
                        continue;
                    }

                    Log.Add(logString);

                    string typeOfLogger = (string)logString.ToolTip;

                    if (LoggerCollections.TryGetValue(typeOfLogger, out var loggerCollection))
                    {
                        loggerCollection.Add(logString);
                    }
                }
            }
        }

        private void OnTabSelected(object sender, RoutedEventArgs e) => MakeLogList();

        private bool ShouldAddLogString(TypeOfLog typeOfLog)
        {

            return (typeOfLog != TypeOfLog.Info || m_isInfo)
                && (typeOfLog != TypeOfLog.Error || m_isError)
                && (typeOfLog != TypeOfLog.CriticalError || m_isCriticalError)
                && (typeOfLog != TypeOfLog.FatalError || m_isFatalError);
        }

        private bool ShouldAddLogString(TextBox logString, CustomTextBox customTextBox)
        {
            Brush color = logString.Foreground;
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;

            return (color.ToString(cultureInfo).Equals(customTextBox.m_infoForeground.ToString(cultureInfo), StringComparison.Ordinal) && m_isInfo)
                || (color.ToString(cultureInfo).Equals(customTextBox.m_errorForeground.ToString(cultureInfo), StringComparison.Ordinal) && m_isError)
                || (color.ToString(cultureInfo).Equals(customTextBox.m_criticalForeground.ToString(cultureInfo), StringComparison.Ordinal) && m_isCriticalError)
                || (color.ToString(cultureInfo).Equals(customTextBox.m_fatalForeground.ToString(cultureInfo), StringComparison.Ordinal) && m_isFatalError);
        }
    }
}

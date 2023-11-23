# Dynamic Logging Console for WPF Applications

This project provides a dynamic logging console for WPF applications. It is designed to display logs from various sources, categorize them according to severity, and allow for real-time monitoring and debugging of applications.

## Features

- **Real-time Log Updates:** The console updates in real time, showing the latest log entries as they are generated.
- **Dynamic Tab Creation:** Automatically creates new tabs for different loggers when they start sending data, keeping the UI clean and organized.
- **Severity Level Filtering:** Allows users to filter logs based on severity levels such as Info, Error, Critical, and Fatal, making it easier to pinpoint issues.
- **Customizable Log Appearance:** Custom `TextBox` controls to differentiate between log types visually.

## How to Use

This logging console is available as a NuGet package for easy integration into your WPF applications. Follow these steps to include it in your project:

1. **Install the NuGet Package**: Add the package to your project via NuGet Package Manager.

2. **Initialize the Logging Console**: In your WPF application, create an instance of the logging console. This is typically done in the main window or the appropriate place in your application.

    ```csharp
    var loggingConsole = new DynamicConsole();
    ```

3. **Log Messages**: Use the `WriteLine` method to log messages. 

    ```csharp
    loggingConsole.WriteLine("Your log message", TypeOfLog.Info, "LoggerName");
    ```

    Replace `"Your log message"` with your log message, `TypeOfLog.Info` with the appropriate log level, and `"LoggerName"` with the name of the logger you are using. A good practice would be to add this line to the corresponding logger code.

4. **Customize as Needed**: You can customize the appearance and behavior of the logging console as per your application's requirements.

## Example

After installing and setting up the NuGet package, you can log messages like this:

```csharp
loggingConsole.WriteLine("Application started", TypeOfLog.Info, "MainApp");
loggingConsole.WriteLine("An error occurred", TypeOfLog.Error, "ErrorHandler");
```
## Contributing
Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are greatly appreciated.

## License
Distributed under the MIT License. See LICENSE for more information.

## Contact
julikbondar@gmail.com

## Project Link: https://github.com/TrickyShit/Dynamic.Console

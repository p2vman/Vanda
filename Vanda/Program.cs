using NLog;
using NLog.Config;
using NLog.Targets;

namespace Vanda;

public class Program
{

    static void Main(string[] args)
    {
        var config = new LoggingConfiguration();
        
        var consoleTarget = new ColoredConsoleTarget("console")
        {
            Layout = "[${date:format=HH\\:mm\\:ss}]:[${logger}]:[${level:uppercase=true}]: ${message} ${exception:format=toString}"
        };
        
        var fileTarget = new FileTarget("file")
        {
            FileName = "logs/app-${shortdate}.log",
            Layout = "[${longdate}]:[${logger}]:[${level:uppercase=true}]:${message}${onexception:${newline}${exception:format=tostring}}",
            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveFiles = 7,
            KeepFileOpen = false
        };
        
        config.AddTarget(consoleTarget);
        config.AddTarget(fileTarget);
        
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
        config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
        
        LogManager.Configuration = config;
        
        GameWindow.Instance.Run();
    }
}
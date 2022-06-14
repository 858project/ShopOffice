using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ShopOffice.Database;
using ShopOffice.Windows;
using System;
using System.IO;
using System.Windows;
using ShopOffice.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows.Threading;
using System.Threading.Tasks;
using Serilog.Core;

namespace ShopOffice
{
    //class Program
    //{
    //    [STAThread]
    //    static void Main()
    //    {
    //        App application = new App();
    //        application.InitializeComponent();
    //        application.Run();
    //    }
    //}
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public App()
        {
            // base directory
            Environment.SetEnvironmentVariable("BASEDIR", AppContext.BaseDirectory);

            // unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            // host
            var builder = this.InternalInitializeHostBuilder();
            this.mHost = builder.Build();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Host for services
        /// </summary>
        private IHost mHost = default!;
        #endregion

        #region - Private Methods -
        private HostBuilder InternalInitializeHostBuilder()
        {
            var builder = new HostBuilder();

            // create configuration
            builder.ConfigureAppConfiguration((hostingContext, configHost) =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configHost.AddEnvironmentVariables();
            });

            // configure logger
            builder.ConfigureLogging((hostingContext, logging) =>
            {
                // clear all providers
                logging.ClearProviders();

                // add configuration
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                // initialize configuration
                var loggerConfiguration = new LoggerConfiguration();

                // create logger
                var logger = loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                                                         .CreateLogger();

                // add seriolog
                logging.AddSerilog(logger, dispose: true);
            });

            // configure services
            builder.ConfigureServices((hostContext, services) =>
            {
                // database configuration
                services.AddDbContextFactory<DatabaseContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("Connection"));
                }, ServiceLifetime.Scoped);

                // custom configuration
                services.Configure<ConfigurationModel>(hostContext.Configuration.GetSection("Configuration"));

                // services
                services.AddSingleton<MainWindow>();
            });

            // return builde
            return builder;
        }
        /// <summary>
        /// Occurs when the Run() method of the Application object is called.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // start host and services
            await this.mHost.StartAsync();

            // get main window
            var mainWindow = mHost.Services.GetService<MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }
        /// <summary>
        /// Occurs just before an application shuts down and cannot be canceled.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            // check host
            using (this.mHost)
            {
                // stop host and services
                await mHost.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
        #endregion

        #region - Private Static Methods -
        private static Logger? InternalGetLogger()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json")
                .Build();

            if (configuration != null)
            {
                // initialize configuration
                var loggerConfiguration = new LoggerConfiguration();

                // create logger
                var logger = loggerConfiguration.ReadFrom.Configuration(configuration)
                                                         .CreateLogger();

                return logger;
            }
            return null;
        }
        #endregion

        #region - Unobserved Exception Methods -
        /// <summary>
        /// Occurs when a faulted task's unobserved exception is about to trigger exception escalation policy, which, by default, would terminate the process.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            // get logger
            using (var logger = App.InternalGetLogger())
            {
                if (logger != null)
                {
                    Exception exception = (Exception)e.Exception;
                    logger.Error(exception.ToString());
                }
            }
        }
        /// <summary>
        /// Occurs when a thread exception is thrown and uncaught during execution of a delegate by way of Overload:System.Windows.Threading.Dispatcher.Invoke or Overload:System.Windows.Threading.Dispatcher.BeginInvoke.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // get logger
            using (var logger = App.InternalGetLogger())
            {
                if (logger != null)
                {
                    Exception exception = (Exception)e.Exception;
                    logger.Error(exception.ToString());
                }
            }

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
        /// <summary>
        /// Occurs when an exception is not caught.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // get logger
            using (var logger = App.InternalGetLogger())
            {
                if (logger != null)
                {
                    Exception exception = (Exception)e.ExceptionObject;
                    logger.Error(exception.ToString());
                }
            }
        }
        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // get logger
            using (var logger = App.InternalGetLogger())
            {
                if (logger != null)
                {
                    Exception exception = (Exception)e.Exception;
                    logger.Error(exception.ToString());
                }
            }

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
        #endregion
    }
}

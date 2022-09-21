﻿using MarkItDesktop.Data;
using MarkItDesktop.Services;
using MarkItDesktop.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace MarkItDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((context,config) =>
                {
                    
                    config.SetBasePath(Environment.CurrentDirectory);
                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddEnvironmentVariables();

                })
                .ConfigureServices((context,services) =>
                {
                    
                    var connectionString = context.Configuration.GetConnectionString("ClientStore");
                    services.AddDbContext<ClientDbContext>(options => options.UseSqlite(connectionString));

                    // Register services
                    services.AddHttpClient<IAuthService, AuthService>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:5000/api/auth/");
                    });

                    services.AddHttpClient<ITodoService, TodoService>( (services, client) =>
                    {
                        // TODO : Use Data Store service instead
                        ClientDbContext dbContext = services.GetRequiredService<ClientDbContext>();
                        ClientData? data = dbContext.Data.FirstOrDefault();
                        if(data is not null)
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);

                        client.BaseAddress = new Uri("https://localhost:5000/api/todos/");
                    });

                    services.AddSingleton<MainWindow>();
                    // TODO : Move these to a seperate extension method
                    services.AddSingleton<ApplicationViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<MainViewModel>();
                })
                .Build();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Start the generic Host
            AppHost.Start();
            // Request the main window as a singleton
            MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            MainWindow.Visibility = Visibility.Visible;
            // Make sure the database is there and updated.
            ClientDbContext context = AppHost.Services.GetRequiredService<ClientDbContext>();
            context.Database.Migrate();

            // TODO : Move this to AuthService ( LoginPage ) and verify credentials via API
            ClientData? data = context.Data.FirstOrDefault();
            if(data is not null)
            {
                AppHost.Services.GetRequiredService<ApplicationViewModel>().CurrentPage = Models.ApplicationPage.MainPage;
            }

        }
    }
}

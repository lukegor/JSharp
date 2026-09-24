using JSharp.Domain.Operations;
using JSharp.Domain.Services;
using JSharp.Services;
using JSharp.UI.Views;
using JSharp.ViewModels;
using JSharp.ViewModels.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JSharp.Configuration
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// Discovers every IImageOperation implementation in the Domain assembly, registers
        /// them as singletons and wires the operation catalog + executor.
        /// </summary>
        public static IServiceCollection AddImageOperations(this IServiceCollection services)
        {
            Assembly domainAssembly = typeof(IImageOperation<>).Assembly;

            List<Type> operationTypes = domainAssembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces().Any(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IImageOperation<>)))
                .ToList();

            foreach (Type type in operationTypes)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton<IOperationCatalog>(sp => OperationCatalog.Build(sp.GetRequiredService, operationTypes));
            services.AddSingleton<IOperationExecutor, OperationExecutor>();
            services.AddSingleton<JSharp.Operations.IOperationDialogRouter>(sp =>
                new JSharp.Operations.OperationDialogRouter(
                    () => sp.GetRequiredService<ViewModels.MainWindowViewModel>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IMessageService>()));

            return services;
        }

        public static IServiceCollection ConfigureMainWindowWithVM(this IServiceCollection services)
        {
            services.AddSingleton<RleImageCodec>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>(sp =>
            {
                return new MainWindow
                {
                    DataContext = sp.GetRequiredService<MainWindowViewModel>()
                };
            });

            return services;
        }
    }
}

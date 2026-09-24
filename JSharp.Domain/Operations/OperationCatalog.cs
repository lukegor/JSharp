using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed class OperationCatalog : IOperationCatalog
    {
        private sealed record Entry(
            OperationDescriptor Descriptor,
            Func<Mat, OperationParams?, CancellationToken, Task<Mat>> Runner);

        private readonly Dictionary<string, Entry> _entries;

        private OperationCatalog(Dictionary<string, Entry> entries) => _entries = entries;

        public IReadOnlyCollection<OperationDescriptor> Descriptors =>
            _entries.Values.Select(e => e.Descriptor).ToArray();

        public bool TryGetDescriptor(string operationId, [NotNullWhen(true)] out OperationDescriptor? descriptor)
        {
            if (_entries.TryGetValue(operationId, out Entry? entry))
            {
                descriptor = entry.Descriptor;
                return true;
            }

            descriptor = null;
            return false;
        }

        public OperationDescriptor GetDescriptor(string operationId)
        {
            if (!TryGetDescriptor(operationId, out OperationDescriptor? descriptor))
            {
                throw new InvalidOperationException($"Unknown operation '{operationId}'.");
            }

            return descriptor;
        }

        public Task<Mat> InvokeAsync(string operationId, Mat source, OperationParams? parameters, CancellationToken cancellationToken)
        {
            if (!_entries.TryGetValue(operationId, out Entry? entry))
            {
                throw new InvalidOperationException($"Unknown operation '{operationId}'.");
            }

            return entry.Runner(source, parameters, cancellationToken);
        }

        /// <summary>
        /// Builds a catalog from the given operation types. The factory resolves instances,
        /// keeping the Domain layer free of any container dependency.
        /// </summary>
        public static OperationCatalog Build(Func<Type, object> instanceFactory, IEnumerable<Type> operationTypes)
        {
            Dictionary<string, Entry> entries = new();

            foreach (Type type in operationTypes)
            {
                Type paramsType = type.GetInterfaces()
                    .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IImageOperation<>))
                    .GetGenericArguments()[0];

                object instance = instanceFactory(type);
                OperationDescriptor descriptor = (OperationDescriptor)type
                    .GetProperty(nameof(IImageOperation<OperationParams>.Descriptor))!
                    .GetValue(instance)!;
                MethodInfo apply = type.GetMethod(nameof(IImageOperation<OperationParams>.Apply))!;

                Func<Mat, OperationParams?, CancellationToken, Task<Mat>> runner =
                    (source, parameters, cancellationToken) =>
                    {
                        if (!paramsType.IsInstanceOfType(parameters))
                        {
                            throw new OperationValidationException(
                                ErrorCodes.ParamsMismatch,
                                $"Operation '{descriptor.Id}' expects parameters of type '{paramsType.Name}'.");
                        }

                        object? invoked;
                        try
                        {
                            invoked = apply.Invoke(instance, new[] { source, parameters, (object)cancellationToken });
                        }
                        catch (TargetInvocationException tie)
                        {
                            // Surface the operation's own exception (validation, cancellation, bugs)
                            // instead of the reflection wrapper, so the executor maps codes correctly.
                            throw tie.InnerException ?? tie;
                        }

                        return Task.FromResult((Mat)invoked!);
                    };

                if (!entries.TryAdd(descriptor.Id, new Entry(descriptor, runner)))
                {
                    throw new InvalidOperationException($"Duplicate operation id '{descriptor.Id}' (type '{type.Name}').");
                }
            }

            return new OperationCatalog(entries);
        }
    }
}

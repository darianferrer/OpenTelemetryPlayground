using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.Testing;

namespace Customer.Api.FunctionalTests.Server;

public sealed class AspireHostFactory : DistributedApplicationFactory, IAsyncLifetime
{
    private bool _started = false;

    public AspireHostFactory() : base(typeof(Projects.AppHost))
    {
    }

    protected override void OnBuilding(DistributedApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.TryAddLifecycleHook<SessionResourceOverwriteHook>();
    }

    public async Task InitializeAsync()
    {
        if (!_started)
        {
            await StartAsync();
            _started = true;
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_started)
        {
            await DisposeAsync();
            _started = false;
        }
    }

    private sealed class SessionResourceOverwriteHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            var containerResources = appModel.Resources.OfType<ContainerResource>();

            foreach (var item in containerResources)
            {
                item.TryGetAnnotationsOfType<ContainerLifetimeAnnotation>(out var lifetimeAnnotations);
                var last = lifetimeAnnotations?.Last(); // there's always one or zero
                if (last is null) continue;

                item.Annotations.Remove(last);
                item.Annotations.Add(new ContainerLifetimeAnnotation { Lifetime = ContainerLifetime.Session });
            }

            return Task.CompletedTask;
        }
    }
}

[CollectionDefinition(nameof(AspireHostFactoryCollection), DisableParallelization = true)]
public class AspireHostFactoryCollection : ICollectionFixture<AspireHostFactory>;

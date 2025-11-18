#nullable enable
using VContainer;
using VContainer.Unity;

namespace Orbit.Title
{
    public class TitleLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Input
            builder.Register<InputSystemActions>(Lifetime.Scoped);
            
            // Title
            builder.RegisterComponentInHierarchy<TitleManager>();
            builder.RegisterComponentInHierarchy<TitleMotionController>();
        }
    }
}

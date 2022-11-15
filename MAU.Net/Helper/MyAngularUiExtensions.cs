using System;
using Microsoft.Extensions.DependencyInjection;

namespace MAU.Helper;

public static class MyAngularUiExtensions
{
    public static IServiceCollection AddMyAngularUi(this IServiceCollection services, Type assemblyMarker)
    {
        MyAngularUi.Init(assemblyMarker.Assembly, true);

        foreach ((Type type, object nullContainerInstance) in MyAngularUi.GetAllContainers())
            services.AddSingleton(type);

        return services;
    }
}
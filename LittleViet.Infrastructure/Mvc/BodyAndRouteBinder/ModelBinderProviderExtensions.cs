using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;

public static class ModelBinderProviderExtensions
{
    public static void InsertBodyAndRouteBinding(this IList<IModelBinderProvider> providers)
    {
        var bodyProvider = providers.Single(provider => provider is BodyModelBinderProvider) as BodyModelBinderProvider;
        var complexProvider = providers.Single(provider => provider is ComplexObjectModelBinderProvider) as ComplexObjectModelBinderProvider;

        var bodyAndRouteProvider = new BodyAndRouteModelBinderProvider(bodyProvider, complexProvider);

        providers.Insert(0, bodyAndRouteProvider);
    }
}
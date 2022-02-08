﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LittleViet.Infrastructure.Mvc.BodyAndRouteBinder;

public class BodyAndRouteModelBinder : IModelBinder
{
    private readonly IModelBinder _bodyBinder;
    private readonly IModelBinder _complexBinder;

    public BodyAndRouteModelBinder(IModelBinder bodyBinder, IModelBinder complexBinder)
    {
        _bodyBinder = bodyBinder;
        _complexBinder = complexBinder;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        await _bodyBinder.BindModelAsync(bindingContext);

        if (bindingContext.Result.IsModelSet)
        {
            bindingContext.Model = bindingContext.Result.Model;
        }

        await _complexBinder.BindModelAsync(bindingContext);
    }
}
﻿using Limbo.Umbraco.Spa.Configuration;
using Limbo.Umbraco.Spa.Factories;
using Limbo.Umbraco.Spa.Repositories;
using Limbo.Umbraco.Spa.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.Spa.Composers; 

public class SpaComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        // Register options
        builder.Services.AddOptions<SpaConfiguration>()
            .Bind(builder.Config.GetSection("Limbo:Spa"), o => o.BindNonPublicProperties = true)
            .ValidateDataAnnotations();

        // Register services
        builder.Services.AddSingleton<SpaRequestHelperDependencies>();
        builder.Services.AddUnique<ISpaCacheService, SpaCacheService>();
        builder.Services.AddUnique<ISpaContentFactory, SpaContentFactory>();
        builder.Services.AddUnique<ISpaItemFactory, SpaItemFactory>();
        builder.Services.AddUnique<ISpaMetaDataFactory, SpaMetaDataFactory>();
        builder.Services.AddUnique<ISpaRequestAccessor, SpaRequestAccessor>();
        builder.Services.AddSingleton<SpaDomainRepository>();

        // Add notification handlers
        builder.AddNotificationHandler<ContentSavedNotification, SpaCacheNotificationHandler>();
        builder.AddNotificationHandler<ContentMovingToRecycleBinNotification, SpaCacheNotificationHandler>();
        builder.AddNotificationHandler<ContentDeletedNotification, SpaCacheNotificationHandler>();

    }

}

public class SpaCacheNotificationHandler : INotificationHandler<ContentSavedNotification>, INotificationHandler<ContentMovingToRecycleBinNotification>, INotificationHandler<ContentDeletedNotification>  {

    private readonly ISpaCacheService _spaCacheService;

    public SpaCacheNotificationHandler(ISpaCacheService spaCacheService) {
        _spaCacheService = spaCacheService;
    }

    public void Handle(ContentSavedNotification notification) {
        _spaCacheService.ClearAll();
    }

    public void Handle(ContentMovingToRecycleBinNotification notification) {
        _spaCacheService.ClearAll();
    }

    public void Handle(ContentDeletedNotification notification) {
        _spaCacheService.ClearAll();
    }

}
using System;
using System.Reflection;
using MediatR;
using MediaValet.Api.Configs;
using MediaValet.Application.Handlers.OrdersHandler;
using MediaValet.Application.Interceptors;
using MediaValet.Application.Repositories;
using MediaValet.Application.Repositories.TableEntities;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Infrastructure.Cloud.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace MediaValet.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddOptions();
      services.Configure<MessagingConfig>(Configuration.GetSection(nameof(MessagingConfig)));
      services.Configure<StorageConfig>(Configuration.GetSection(nameof(StorageConfig)));

      services.AddSingleton(typeof(IOrdersRepository), typeof(OrdersRepository));
      services.AddSingleton<IMessagingService>(provider =>
      {
        var messagingConfig = provider.GetService<IOptions<MessagingConfig>>()?.Value;
        if (messagingConfig != null)
        {
          return new MessagingService(messagingConfig.ConnectionString,messagingConfig.QueueName);
        }
        throw new Exception("Missing messaging Config");
      });

      services.AddSingleton<IStorageService<ConfirmationTableEntity>>(provider =>
      {
        var storageConfig = provider.GetService<IOptions<StorageConfig>>()?.Value;
        if (storageConfig != null)
        {
          return new StorageService<ConfirmationTableEntity>(storageConfig.ConnectionString, storageConfig.TableName);
        }

        throw new Exception("Missing storage Config");
      });
      services.AddMediatR(typeof(OrderHandler).GetTypeInfo().Assembly);
      services.AddControllers();

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingInterceptor<,>));

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MediaValet", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediaValet v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

namespace Mockaco
{
    public partial class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache()
                .AddHttpClient()
                .AddCors()
                .AddOptions()

                .Configure<MockacoOptions>(_configuration.GetSection("Mockaco"))
                .Configure<TemplateFileProviderOptions>(_configuration.GetSection("Mockaco:TemplateFileProvider"))

                .AddScoped<IMockacoContext, MockacoContext>()
                .AddScoped<IScriptContext, ScriptContext>()
                .AddTransient<IGlobalVariableStorage, ScriptContextGlobalVariableStorage>()

                .AddSingleton<IScriptRunnerFactory, ScriptRunnerFactory>()

                .AddSingleton<IFakerFactory, LocalizedFakerFactory>()
                .AddSingleton<IMockProvider, MockProvider>()
                .AddSingleton<ITemplateProvider, TemplateFileProvider>()

                .AddScoped<IRequestMatcher, RequestMethodMatcher>()
                .AddScoped<IRequestMatcher, RequestRouteMatcher>()
                .AddScoped<IRequestMatcher, RequestConditionMatcher>()

                .AddTransient<IRequestBodyFactory, RequestBodyFactory>()

                .AddTransient<IRequestBodyStrategy, JsonRequestBodyStrategy>()
                .AddTransient<IRequestBodyStrategy, XmlRequestBodyStrategy>()
                .AddTransient<IRequestBodyStrategy, FormRequestBodyStrategy>()

                .AddTransient<IResponseBodyFactory, ResponseBodyFactory>()

                .AddTransient<IResponseBodyStrategy, BinaryResponseBodyStrategy>()
                .AddTransient<IResponseBodyStrategy, JsonResponseBodyStrategy>()
                .AddTransient<IResponseBodyStrategy, XmlResponseBodyStrategy>()
                .AddTransient<IResponseBodyStrategy, DefaultResponseBodyStrategy>()

                .AddTransient<ITemplateTransformer, TemplateTransformer>();
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation("{assemblyName} v{assemblyVersion} by Renato Lima [github.com/natenho]\n\n{logo}", assemblyName.Name, assemblyName.Version, _logo);
            
            app.UseCors(configurePolicy => configurePolicy.AllowAnyOrigin())
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseMiddleware<ResponseDelayMiddleware>()
                .UseMiddleware<RequestMatchingMiddleware>()
                .UseMiddleware<ResponseMockingMiddleware>()
                .UseMiddleware<CallbackMiddleware>();
        }
    }
}
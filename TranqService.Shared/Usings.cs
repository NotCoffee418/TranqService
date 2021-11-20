// .NET
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Reflection;


// Nuget packages
global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using CoffeeToolkit.Database;
global using Dapper;
global using Npgsql;
global using Google.Apis.Services;
global using Google.Apis.Util;
global using Google.Apis.YouTube.v3;
global using Microsoft.Extensions.Configuration;
global using Serilog;


// Internal
global using TranqService.Shared.Models.ApplicationModels.YoutubeDownloaderService;
global using TranqService.Shared.ApiHandlers;
global using TranqService.Shared.Data;
global using TranqServices.Shared.ApiHandlers;
global using TranqService.Shared.DataAccess;
global using TranqService.Shared.Logic;
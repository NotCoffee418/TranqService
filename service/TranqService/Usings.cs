// .NET
global using System.Reflection;
global using System.Diagnostics;
global using System.Runtime.InteropServices;

// Packages
global using Serilog;
global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using Serilog.Events;
global using Serilog.Sinks.Discord;
global using Polly;
global using Polly.Contrib.WaitAndRetry;

// Internal
global using TranqService;
global using TranqService.StaticLogic;
global using TranqService.Services;
global using TranqService.Shared.Logic;
global using TranqService.Common;
global using TranqService.Database;
global using TranqService.Shared;
global using TranqService.Database.Queries;
global using TranqService.Common.Data;
global using TranqService.Database.Models;
global using TranqService.Common.DataAccess;
global using TranqService.Common.Models.Configs;
global using TranqService.Common.Models;
global using TranqService.Shared.DataAccess.Ytdlp;
global using static TranqService.Common.Data.Enums;
// .NET
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Reflection;
global using System.Text.Json.Nodes;
global using System.Net.Http;
global using System.Text.RegularExpressions;


// Nuget packages
global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using Google.Apis.Services;
global using Google.Apis.Util;
global using Google.Apis.YouTube.v3;
global using Microsoft.Extensions.Configuration;
global using Serilog;
global using CliWrap;
global using CliWrap.Buffered;
global using Polly.Contrib.WaitAndRetry;
global using Polly.Retry;
global using Polly;



// Internal
global using TranqService.Common.Data;
global using TranqService.Shared.DataAccess.ApiHandlers;
global using TranqService.Shared.Logic;
global using System.ComponentModel.DataAnnotations;
global using TranqService.Common.Extensions;
global using Google.Apis.YouTube.v3.Data;
global using TranqService.Database.Models;
global using TranqService.Database.Queries;
global using TranqService.Shared.DataAccess;
global using TranqService.Common.Attributes;
global using static TranqService.Common.Attributes.DependencyScopeAttribute;
global using TranqService.Common.Models.Configs;
global using TranqService.Shared.Factories;


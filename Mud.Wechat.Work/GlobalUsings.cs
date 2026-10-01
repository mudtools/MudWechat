global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Work.Abstractions;
global using Mud.Wechat.Work.Abstractions.Configuration;
global using Mud.Wechat.Work.Abstractions.Authentication;
global using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
global using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
global using Mud.Wechat.Work.DataModels;
global using Mud.Wechat.Work.DataModels.ProviderAuthentication;
// Mud.HttpUtils.Generator 将 [HttpClientApi] 接口的实现类生成到「接口命名空间 + .Internal」，
// 子接口源码的 InheritedFrom = nameof(父实现类) 需要该命名空间可解析。
global using Mud.Wechat.Work.Internal;

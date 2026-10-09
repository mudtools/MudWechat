global using Xunit;
global using FluentAssertions;
global using Moq;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Abstractions;
// 说明：Mud.Wechat.Pay{,.Abstractions,.DataModels} 三包在 P0-a 期为**空工程**（尚无任何命名空间声明），
// 故此处不 global using —— 否则 CS0234。P0-c 落回调三闸时随首批源文件补入。

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Abstractions;
global using Mud.Wechat.Abstractions.TokenManager;
global using Mud.Wechat.OpenPlatform.Abstractions;
global using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
global using Mud.Wechat.OpenPlatform.Abstractions.Transport;
global using Mud.Wechat.OpenPlatform.DataModels;
global using Mud.Wechat.OpenPlatform.DataModels.Account;
global using Mud.Wechat.OpenPlatform.DataModels.Component;
global using Mud.Wechat.OpenPlatform.DataModels.OpenAccount;
global using Mud.Wechat.OpenPlatform.DataModels.Sns;

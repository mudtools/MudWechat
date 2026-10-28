// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 注：根 Directory.Build.props 已开 ImplicitUsings，此处仅补**非隐式**的组件面与本产品线命名空间。
// 本行为广告线唯一源工程（2026-10 包合并：原 Abstractions / DataModels 两包并入本包，
// 故其全局导入并集落此）；命名空间仍按原三包分面，未随合并改变。
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Abstractions;
global using Mud.Wechat.Abstractions.Configuration;
global using Mud.Wechat.Abstractions.Contracts;
global using Mud.Wechat.Abstractions.Exceptions;
global using Mud.Wechat.Abstractions.TokenManager;
global using Mud.Wechat.Ads.Abstractions.Transport;
global using Mud.Wechat.Ads.DataModels.Common;
global using System.Text.Json.Serialization;

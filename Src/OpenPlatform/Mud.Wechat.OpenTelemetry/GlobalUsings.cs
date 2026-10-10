// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

global using System;
global using System.Collections.Generic;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using OpenTelemetry;
global using OpenTelemetry.Exporter;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using Mud.Wechat.Abstractions.Observability;
// 上游可观测性共享装配内核（Mud.HttpUtils.OpenTelemetry 3.0.3+）。
// 本包不再直接引用 Mud.HttpUtils：MudHttpActivitySource / MudHttpMeter 由内核经
// MudObservabilityContribution.IncludeMudHttpSources 内部注册。
global using Mud.HttpUtils.OpenTelemetry;
// 别名消歧：Mud.HttpUtils.OpenTelemetry 也导出 OtlpExportProtocol，与 OpenTelemetry.Exporter 的同名类型
// 在本工程（上一条 global using OpenTelemetry.Exporter）中形成二义（CS0104）。
global using MudOtlpExportProtocol = Mud.HttpUtils.OpenTelemetry.OtlpExportProtocol;

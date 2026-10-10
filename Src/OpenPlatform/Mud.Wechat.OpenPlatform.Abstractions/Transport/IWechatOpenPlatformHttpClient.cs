// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.OpenPlatform.Abstractions.Transport;

/// <summary>
/// 开放平台线专用 HTTP 客户端契约。
/// </summary>
/// <remarks>
/// <b>标记式接口</b>（成员全部来自 <see cref="IEnhancedHttpClient"/>）：它存在的意义是
/// <b>占住一个独立的 DI 类型键</b>，使本线的命名客户端与企微线/支付线的默认实例互不干扰
/// （理由见 <see cref="OpenPlatformHttpClientNames"/>）。
/// </remarks>
public interface IWechatOpenPlatformHttpClient : IEnhancedHttpClient;

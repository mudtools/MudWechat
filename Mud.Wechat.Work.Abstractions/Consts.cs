// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 企业微信 SDK 全局常量（对齐 Feishu Consts）。
/// </summary>
/// <remarks>
/// 域名与 SSRF 白名单已下沉公用层：<see cref="WechatApiHosts.WorkBaseUrl"/> /
/// <see cref="WechatApiHosts.AllowedBaseUrlDomains"/>（跨产品线并集单一来源，白名单由
/// <c>WechatTokenRecoveryRegistration.AddWechatTokenRecovery</c> 统一登记）。
/// </remarks>
internal static class Consts
{
    /// <summary>命名 HttpClient 客户端名前缀（per-app 客户端名 = 前缀 + "-" + AppKey）。</summary>
    public const string HttpClientNamePrefix = "wechat-work";

    /// <summary>应用上下文退役队列默认宽限期（秒），对齐 Feishu TMA-07/TMA-24。</summary>
    public const int DefaultContextRetireDelaySeconds = 300;
}

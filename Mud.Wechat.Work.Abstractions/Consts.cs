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
internal static class Consts
{
    /// <summary>企业微信 API 默认域名（企业主体/服务商接口统一）。</summary>
    public const string DefaultBaseUrl = "https://qyapi.weixin.qq.com";

    /// <summary>
    /// <see cref="Configuration.WechatAppConfig.AllowCustomBaseUrl"/> 为 false 时的 BaseUrl 域名白名单
    /// （SSRF 防线；host 等于域或以其子域结尾即放行）。
    /// </summary>
    public static readonly string[] AllowedBaseUrlDomains =
    {
        "weixin.qq.com",
        "work.weixin.qq.com",
    };

    /// <summary>命名 HttpClient 客户端名前缀（per-app 客户端名 = 前缀 + "-" + AppKey）。</summary>
    public const string HttpClientNamePrefix = "wechat-work";

    /// <summary>应用上下文退役队列默认宽限期（秒），对齐 Feishu TMA-07/TMA-24。</summary>
    public const int DefaultContextRetireDelaySeconds = 300;

    /// <summary>持久化令牌键前缀（对齐 Feishu DefaultTokenKeyPrefix = "feishu"）。</summary>
    public const string DefaultTokenKeyPrefix = "wechat";

    /// <summary>企业微信官方文档根地址（用于 XML 注释引用）。</summary>
    public const string DocsBaseUrl = "https://developer.work.weixin.qq.com/document/";
}

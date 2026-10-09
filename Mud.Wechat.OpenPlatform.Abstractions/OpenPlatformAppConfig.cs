// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 第三方平台应用配置（<b>component 模式的平台自身身份</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>这不是</b>被授权方（authorizer）的身份，而是<b>平台自己</b>的身份：
/// <c>component_appid</c> / <c>component_appsecret</c> 在开放平台后台创建第三方平台时获得。
/// 被授权方的令牌（<c>authorizer_access_token</c>）在本线的后续步骤中才有。
/// </para>
/// <para>
/// <b>密钥不入日志</b>：<see cref="ComponentAppSecret"/> 与其它线的 <c>AppSecret</c> 同级敏感，
/// 本类<b>不重写 <c>ToString</c></b>（避免被结构化日志整体打印）。
/// </para>
/// </remarks>
public sealed class OpenPlatformAppConfig
{
    /// <summary>第三方平台 appid（官方 <c>component_appid</c>）。</summary>
    public string? ComponentAppId { get; set; }

    /// <summary>第三方平台 appsecret（官方 <c>component_appsecret</c>）。<b>不得入日志</b>。</summary>
    public string? ComponentAppSecret { get; set; }

    /// <summary>
    /// 消息校验 Token（开放平台后台「授权事件接收 URL」处配置的 Token）。
    /// </summary>
    /// <remarks>用于校验推送签名（<c>msg_signature</c>）；<b>不得入日志</b>。</remarks>
    public string? Token { get; set; }

    /// <summary>
    /// 消息加解密密钥（开放平台后台配置的 43 位 EncodingAESKey）。
    /// </summary>
    /// <remarks>
    /// <b>必须 43 位</b>：官方以 <c>Base64Decode(EncodingAESKey + "=")</c> 得到 32 字节 AES 密钥，
    /// 43 位字符是「32 字节 → Base64（无填充）」的固定长度。长度不对会在解密时抛出难以定位的
    /// 格式异常，故在<b>注册期</b>就点名。<b>不得入日志</b>。
    /// </remarks>
    public string? EncodingAesKey { get; set; }

    /// <summary>
    /// 注册期校验（fail-fast）。
    /// </summary>
    /// <exception cref="InvalidOperationException">任一必填项为空白。</exception>
    /// <remarks>
    /// <b>为何在注册期就校验</b>：缺 appid/secret 的后果是「首次取令牌才失败」——
    /// 而那一刻通常已经在线上了；且官方对错误参数只回一个笼统的 <c>errcode</c>，
    /// 排查成本远高于启动期直接点名。
    /// </remarks>
    internal void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(ComponentAppId))
        {
            throw new InvalidOperationException(
                "第三方平台配置缺少 ComponentAppId（官方 component_appid）。");
        }

        if (string.IsNullOrWhiteSpace(ComponentAppSecret))
        {
            throw new InvalidOperationException(
                "第三方平台配置缺少 ComponentAppSecret（官方 component_appsecret）。");
        }

        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new InvalidOperationException(
                "第三方平台配置缺少 Token（授权事件接收 URL 的消息校验 Token）—— 无它无法校验推送签名。");
        }

        // 推送凭据**同样是硬前提**：component_verify_ticket 只能由微信后台推送到达，
        // 没有 Token/EncodingAESKey 就收不了票据 ⇒ 也就永远取不到令牌（整条链断在起点）。
        if (string.IsNullOrWhiteSpace(EncodingAesKey) || EncodingAesKey!.Length != 43)
        {
            throw new InvalidOperationException(
                "第三方平台配置的 EncodingAesKey 必须为 43 位字符"
                + $"（当前 {(EncodingAesKey?.Length ?? 0)} 位）—— 官方以 Base64Decode(key + \"=\") 得到 32 字节 AES 密钥。");
        }
    }
}

/// <summary>
/// 开放平台线的领域异常（凭证链失败）。
/// </summary>
/// <remarks>
/// <b>保留官方错误码</b>：官方以 <c>errcode</c> / <c>errmsg</c> 报错，
/// 它是判错与自愈的唯一依据；把码丢掉只留一段人话消息会让调用方无法分支处理。
/// </remarks>
public sealed class WechatOpenPlatformException : Exception
{
    /// <summary>创建异常。</summary>
    /// <param name="message">错误消息（<b>不得</b>包含 appsecret / ticket 原文）。</param>
    /// <param name="errorCode">官方错误码（<c>errcode</c>）；无则为 <c>null</c>。</param>
    public WechatOpenPlatformException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>官方错误码（<c>errcode</c>，字符串形式）；无则为 <c>null</c>。</summary>
    public string? ErrorCode { get; }
}

/// <summary>
/// 第三方平台令牌提供者（component 凭证链的对外入口）。
/// </summary>
/// <remarks>
/// <para>
/// <b>契约语义</b>：返回一把<b>当前可用</b>的 <c>component_access_token</c> ——
/// 实现负责「按需刷新 + 缓存 + 并发单飞」，调用方<b>不</b>需要关心时机。
/// </para>
/// <para>
/// <b>失败语义（fail-closed）</b>：无法给出可用令牌时<b>必须抛出</b>
/// （<see cref="WechatOpenPlatformException"/>），<b>绝不</b>返回 <c>null</c>/空串 ——
/// 让调用方拿到空串去拼 URL，会得到一个 40001 的普通报错，反而掩盖了「凭证链断了」这一真相。
/// </para>
/// </remarks>
public interface IComponentTokenProvider
{
    /// <summary>获取当前可用的 <c>component_access_token</c>（必要时自动刷新）。</summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>可用的令牌。</returns>
    /// <exception cref="WechatOpenPlatformException">票据缺失、官方报错、或应答不含令牌。</exception>
    Task<string> GetComponentAccessTokenAsync(CancellationToken cancellationToken = default);
}

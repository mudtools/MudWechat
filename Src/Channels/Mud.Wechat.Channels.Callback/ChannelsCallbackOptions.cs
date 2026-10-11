// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Callback;

/// <summary>
/// 微信小店 / 视频号回调配置（节名 <c>ChannelsCallback</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>凭据唯一来源</b>：回调凭据（Token / EncodingAESKey / AppId）以 <see cref="Apps"/> 字典为唯一来源，
/// 键为经形状校验的应用键（路由 <c>/{GlobalRoutePrefix}/{AppKey}</c>）；通配键
/// <see cref="Mud.Wechat.Abstractions.Callback.WechatCallbackRouteKeys.Wildcard"/> 承接「全局兜底凭据 + 全局处理器桶」。
/// </para>
/// <para>
/// <b>与公众号侧的差异</b>：小店回调官方<b>强制 JSON + 安全模式</b>（不可选明文 / 兼容），故无安全模式维度；
/// 解密明文尾部的 <c>appid</c>（小店 AppID）必须与配置一致（fail-closed）。
/// </para>
/// <para>
/// <b>AOT</b>：不使用 <c>required</c>（ConfigurationBinder 以 <c>new T()</c> 构造 ⇒ <c>CS9035</c>），
/// 必填校验统一走 <see cref="Validate"/>。
/// </para>
/// </remarks>
public sealed class ChannelsCallbackOptions
{
    /// <summary>配置节名。</summary>
    public const string SectionName = "ChannelsCallback";

    /// <summary>通配应用键（叶层常量单一来源）。</summary>
    public const string WildcardAppKey = Mud.Wechat.Abstractions.Callback.WechatCallbackRouteKeys.Wildcard;

    /// <summary>回调路由前缀（中间件路径形如 <c>/{GlobalRoutePrefix}/{AppKey}</c>；默认 <c>channels</c>，与企微 <c>wechat</c> / 公众号 <c>mp</c> 不冲突）。</summary>
    public string GlobalRoutePrefix { get; set; } = "channels";

    /// <summary>请求体字节上限（默认 1MB；按字节计数，含多字节字符）。</summary>
    public int MaxRequestBodySize { get; set; } = 1_048_576;

    /// <summary>回调来源 IP 白名单（空 = 不限；支持 IPv4 与 IPv4 CIDR）。</summary>
    public List<string> AllowedSourceIPs { get; set; } = new();

    /// <summary>是否信任反向代理头（<c>X-Forwarded-For</c>）。</summary>
    /// <remarks>默认 <c>false</c>（信任可伪造的头会直接击穿 IP 白名单，fail-closed）。</remarks>
    public bool TrustedProxies { get; set; }

    /// <summary>事件处理软超时（毫秒；默认 4000，必须 &lt; 5000 —— 官方 5 秒未响应即判推送失败并重试）。</summary>
    public int EventHandlingTimeoutMs { get; set; } = 4_000;

    /// <summary>并发处理上限（构造期快照；热更不改容量）。</summary>
    public int MaxConcurrentEvents { get; set; } = 10;

    /// <summary>时间戳时效窗容差（秒；默认 300）。</summary>
    public int AllowClockSkewSeconds { get; set; } = 300;

    /// <summary>多应用回调凭据（键 = 应用键或通配键）。</summary>
    public Dictionary<string, ChannelsAppCallbackOptions> Apps { get; set; } = new();

    /// <summary>解析指定应用键的回调凭据（精确键优先，回退通配键）。</summary>
    /// <param name="appKey">应用键（来自回调路由路径段）。</param>
    /// <returns>命中的回调凭据；未命中返回 <c>null</c>。</returns>
    public ChannelsAppCallbackOptions? ResolveApp(string appKey)
    {
        if (appKey != null && Apps.TryGetValue(appKey, out var app))
        {
            return app;
        }

        return Apps.TryGetValue(WildcardAppKey, out var wildcard) ? wildcard : null;
    }

    /// <summary>校验回调配置完整性（缺失必填项或非法取值时抛出）。</summary>
    /// <exception cref="InvalidOperationException">配置非法时抛出（启动阶段即失败）。</exception>
    public void Validate()
    {
        if (Apps.Count == 0)
        {
            throw new InvalidOperationException(
                $"回调配置缺少 Apps（至少配置一个小店的回调凭据；全局兜底使用通配 \"{WildcardAppKey}\" 键）。");
        }

        if (string.IsNullOrWhiteSpace(GlobalRoutePrefix) || GlobalRoutePrefix.Contains('/'))
        {
            throw new InvalidOperationException("回调路由前缀不能为空白且不得包含 '/'。");
        }

        // 官方 5 秒未响应即判推送失败（重试 0s/0s/10s/1min/5min/30min/60min… 至多 10 次）：
        // 软超时必须严格小于 5000ms。
        if (EventHandlingTimeoutMs <= 0 || EventHandlingTimeoutMs >= 5000)
        {
            throw new InvalidOperationException(
                $"EventHandlingTimeoutMs 必须落在 (0, 5000) 区间（当前 {EventHandlingTimeoutMs}）："
                + "微信小店服务器 5 秒内收不到响应即判推送失败并重试，软超时必须严格小于该时限。");
        }

        if (MaxRequestBodySize <= 0)
        {
            throw new InvalidOperationException("MaxRequestBodySize 必须为正数。");
        }

        if (AllowClockSkewSeconds <= 0)
        {
            throw new InvalidOperationException("AllowClockSkewSeconds 必须为正数。");
        }

        foreach (var pair in Apps)
        {
            if (!string.Equals(pair.Key, WildcardAppKey, StringComparison.Ordinal))
            {
                Mud.Wechat.Abstractions.WechatAppKeyValidator.Validate(pair.Key);
            }

            pair.Value.Validate(pair.Key);
        }
    }
}

/// <summary>
/// 单小店回调凭据（Token / EncodingAESKey / AppId）。
/// </summary>
/// <remarks>
/// <para>
/// <b>AppId 为何在回调配置中</b>：官方要求解密后校验明文尾部的 <c>appid</c> 与自身小店一致，
/// 而回调处理时 URL 只携带 AppKey；回调包需对「仅接回调、不调 API」的宿主自足
/// （与公众号侧 <c>MpAppCallbackOptions.AppId</c> 同构）。
/// </para>
/// </remarks>
public sealed class ChannelsAppCallbackOptions
{
    /// <summary>服务器配置 Token（验签基准）。</summary>
    public string PushToken { get; set; } = string.Empty;

    /// <summary>服务器配置 EncodingAESKey（43 位；小店强制安全模式，必填）。</summary>
    public string PushEncodingAESKey { get; set; } = string.Empty;

    /// <summary>微信小店 AppID（解密明文尾部 <c>appid</c> 校验基准；解密结果 receiveid）。</summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>是否要求一次性指纹闸（默认开启；仅在排障时可关，生产不得关闭）。</summary>
    public bool RequireReplayGuard { get; set; } = true;

    /// <summary>校验单应用回调凭据完整性。</summary>
    /// <param name="appKey">归属应用键（仅用于异常消息定位）。</param>
    /// <exception cref="InvalidOperationException">配置非法时抛出。</exception>
    public void Validate(string appKey)
    {
        if (string.IsNullOrWhiteSpace(PushToken))
        {
            throw new InvalidOperationException($"回调配置 Apps[\"{appKey}\"] 缺少 PushToken。");
        }

        if (string.IsNullOrWhiteSpace(AppId))
        {
            throw new InvalidOperationException(
                $"回调配置 Apps[\"{appKey}\"] 缺少 AppId（解密明文尾部 appid 校验基准，官方要求与自身小店一致）。");
        }

        if (string.IsNullOrWhiteSpace(PushEncodingAESKey) || PushEncodingAESKey.Length != 43)
        {
            throw new InvalidOperationException(
                $"回调配置 Apps[\"{appKey}\"] 的 PushEncodingAESKey 必须为 43 位字符（小店强制安全模式）。");
        }
    }
}

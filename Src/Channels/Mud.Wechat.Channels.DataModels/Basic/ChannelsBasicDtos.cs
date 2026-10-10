// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>获取微信 API 服务器 IP（<c>get_api_domain_ip</c>）响应。</summary>
/// <remarks>
/// 官方契约：<b>GET</b> + Query <c>access_token</c>（[Token] 管线注入）；
/// 出口 IP 及入口 IP 可能变动，官方建议每天请求 1 次以更新 IP 列表。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsGetApiDomainIpResponse : ChannelsResponse
{
    /// <summary>获取或设置微信 API 服务器 IP 列表（官方 <c>ip_list</c>）。</summary>
    [JsonPropertyName("ip_list")]
    public List<string>? IpList { get; set; }
}

/// <summary>获取微信推送服务器 IP（<c>getcallbackip</c>）响应。</summary>
/// <remarks>响应形态与 <see cref="ChannelsGetApiDomainIpResponse"/> 一致（官方两页正文逐项一致）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsGetCallbackIpResponse : ChannelsResponse
{
    /// <summary>获取或设置微信推送服务器 IP 列表（官方 <c>ip_list</c>）。</summary>
    [JsonPropertyName("ip_list")]
    public List<string>? IpList { get; set; }
}

/// <summary>网络通信检测（<c>callback/check</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体（<c>action</c> 与 <c>check_operator</c> 均为必填）。
/// <c>action</c> 取值见 <see cref="ChannelsCallbackCheckActions"/>；<c>check_operator</c> 见 <see cref="ChannelsCallbackCheckOperators"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsCallbackCheckRequest
{
    /// <summary>获取或设置检测动作（官方 <c>action</c>，取值：<c>all</c>/<c>dns</c>/<c>ping</c>）。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>获取或设置检测运营商（官方 <c>check_operator</c>，取值：<c>CHINANET</c>/<c>UNICOM</c>/<c>CAP</c>/<c>BACKBONE</c>）。</summary>
    [JsonPropertyName("check_operator")]
    public string CheckOperator { get; set; } = string.Empty;
}

/// <summary>网络通信检测（<c>callback/check</c>）响应。</summary>
/// <remarks>
/// <c>package_loss</c> 因仅发送一个 ping 包，取值只有 <c>0%</c> / <c>100%</c> 两种，
/// 不宜作为精细化网络质量指标；官方「注意事项」章节原文为「本接口无特殊注意事项」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsCallbackCheckResponse : ChannelsResponse
{
    /// <summary>获取或设置 DNS 解析结果（官方 <c>dns</c>）。</summary>
    [JsonPropertyName("dns")]
    public ChannelsDnsCheckResult? Dns { get; set; }

    /// <summary>获取或设置 PING 检测结果（官方 <c>ping</c>）。</summary>
    [JsonPropertyName("ping")]
    public ChannelsPingCheckResult? Ping { get; set; }
}

/// <summary>DNS 解析结果（官方 <c>dns</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsDnsCheckResult
{
    /// <summary>获取或设置 DNS 解析结果 IP（官方 <c>ip</c>）。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>获取或设置解析所用时长（官方 <c>real_operator</c>，单位：毫秒）。</summary>
    [JsonPropertyName("real_operator")]
    public string? RealOperator { get; set; }
}

/// <summary>PING 检测结果（官方 <c>ping</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsPingCheckResult
{
    /// <summary>获取或设置 ping 的 IP（官方 <c>ip</c>）。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>获取或设置丢包率（官方 <c>package_loss</c>，取值 <c>0%</c> / <c>100%</c>）。</summary>
    [JsonPropertyName("package_loss")]
    public string? PackageLoss { get; set; }

    /// <summary>获取或设置耗时（官方 <c>time</c>，单位：毫秒）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }
}

/// <summary>网络通信检测动作常量（官方 <c>action</c> 参数取值）。</summary>
public static class ChannelsCallbackCheckActions
{
    /// <summary>全部检测（DNS 解析 + PING）。</summary>
    public const string All = "all";

    /// <summary>仅 DNS 解析。</summary>
    public const string Dns = "dns";

    /// <summary>仅 PING 检测。</summary>
    public const string Ping = "ping";
}

/// <summary>网络通信检测运营商常量（官方 <c>check_operator</c> 参数取值）。</summary>
public static class ChannelsCallbackCheckOperators
{
    /// <summary>中国电信。</summary>
    public const string ChinaNet = "CHINANET";

    /// <summary>中国联通。</summary>
    public const string Unicom = "UNICOM";

    /// <summary>中国移动。</summary>
    public const string Cap = "CAP";

    /// <summary>骨干网（官方 BACKBONE）。</summary>
    public const string Backbone = "BACKBONE";
}

/// <summary>重置 API 调用次数（<c>clear_quota</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体 <c>{"appid":…}</c>（<c>access_token</c> 在 Query——官方明确「不在 body 中」）。
/// <b>每月 10 次</b>（与 <c>clear_quota/v2</c> 合计；官方原文「每个账号每月共 10 次清零操作机会，
/// 清零生效一次即用掉一次机会」）。清空哪类账号的 quota 就需用该账号对应的 token。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsClearQuotaRequest
{
    /// <summary>获取或设置要被清空的账号的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}

/// <summary>使用 AppSecret 重置 API 调用次数（<c>clear_quota/v2</c>）请求体（<b>免令牌端点</b>）。</summary>
/// <remarks>
/// 官方契约：该接口<b>不消费 access_token</b>——官方注意事项原文「该接口通过 appsecret 调用，解决了
/// access_token 耗尽无法调用『重置 API 调用次数』的问题」。
/// <b>参数位置裁决（I4 同款）</b>：官方参数表把 <c>appid</c>/<c>appsecret</c> 列于「请求体 Request Payload」，
/// 但官方代码示例把两参数放在 Query String——两者矛盾。SDK 按<b>参数表（请求体）</b>建模：
/// <c>appsecret</c> 是唯一凭证密钥，进 URL 会扩大泄露面。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsClearQuotaV2Request
{
    /// <summary>获取或设置要被清空的账号的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置唯一凭证密钥（官方 <c>appsecret</c>，必填；SDK 走请求体，不进 URL）。</summary>
    [JsonPropertyName("appsecret")]
    public string AppSecret { get; set; } = string.Empty;
}

/// <summary>查询 / 重置指定 API 调用次数的请求体（<c>openapi/quota/get</c> 与 <c>openapi/quota/clear</c> 请求体字段集一致 ⇒ 共用）。</summary>
/// <remarks>
/// 官方契约：<c>cgi_path</c> 为 api 的请求地址——官方原文「不要前缀 https://api.weixin.qq.com，
/// 也不要漏了 /，否则都会 76003 的报错」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsCgiPathRequest
{
    /// <summary>获取或设置 api 的请求地址（官方 <c>cgi_path</c>，必填；以 / 开头、不带 https://api.weixin.qq.com 前缀）。</summary>
    [JsonPropertyName("cgi_path")]
    public string CgiPath { get; set; } = string.Empty;
}

/// <summary>查询 API 调用额度（<c>openapi/quota/get</c>）响应。</summary>
/// <remarks>官方契约：<c>"/xxx/sns/xxx"</c> 类接口不支持本查询（76022）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsGetApiQuotaResponse : ChannelsResponse
{
    /// <summary>获取或设置当日调用额度详情（官方 <c>quota</c>）。</summary>
    [JsonPropertyName("quota")]
    public ChannelsApiQuota? Quota { get; set; }

    /// <summary>获取或设置普通调用频率限制（官方 <c>rate_limit</c>）。</summary>
    [JsonPropertyName("rate_limit")]
    public ChannelsApiRateLimit? RateLimit { get; set; }

    /// <summary>获取或设置代调用频率限制（官方 <c>component_rate_limit</c>，字段同 rate_limit；非代调用形态可能缺省）。</summary>
    [JsonPropertyName("component_rate_limit")]
    public ChannelsApiRateLimit? ComponentRateLimit { get; set; }
}

/// <summary>当日调用额度详情（官方 <c>quota</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsApiQuota
{
    /// <summary>获取或设置当天可调用次数（官方 <c>daily_limit</c>）。</summary>
    [JsonPropertyName("daily_limit")]
    public int DailyLimit { get; set; }

    /// <summary>获取或设置当天已调用次数（官方 <c>used</c>）。</summary>
    [JsonPropertyName("used")]
    public int Used { get; set; }

    /// <summary>获取或设置当天剩余调用次数（官方 <c>remain</c>）。</summary>
    [JsonPropertyName("remain")]
    public int Remain { get; set; }
}

/// <summary>调用频率限制（官方 <c>rate_limit</c> / <c>component_rate_limit</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsApiRateLimit
{
    /// <summary>获取或设置周期内可调用数量（官方 <c>call_count</c>，单位次）。</summary>
    [JsonPropertyName("call_count")]
    public int CallCount { get; set; }

    /// <summary>获取或设置更新周期（官方 <c>refresh_second</c>，单位秒）。</summary>
    [JsonPropertyName("refresh_second")]
    public int RefreshSecond { get; set; }
}

/// <summary>查询 rid 信息（<c>openapi/rid/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsGetRidInfoRequest
{
    /// <summary>获取或设置调用接口报错返回的 rid（官方 <c>rid</c>，必填；有效期仅 7 天）。</summary>
    [JsonPropertyName("rid")]
    public string Rid { get; set; } = string.Empty;
}

/// <summary>查询 rid 信息（<c>openapi/rid/get</c>）响应。</summary>
/// <remarks>
/// 官方契约：rid 查询属开发者私密行为，<b>仅支持同账号查询</b>
/// （账号 A 产生的 rid 须用账号 A 的 access_token 查询）；rid 有效期仅 <b>7 天</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsGetRidInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置该 rid 对应的请求详情（官方 <c>request</c>）。</summary>
    [JsonPropertyName("request")]
    public ChannelsRidRequestInfo? Request { get; set; }
}

/// <summary>rid 对应的请求详情（官方 <c>request</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsRidRequestInfo
{
    /// <summary>获取或设置发起请求的时间戳（官方 <c>invoke_time</c>）。</summary>
    [JsonPropertyName("invoke_time")]
    public long? InvokeTime { get; set; }

    /// <summary>获取或设置请求毫秒级耗时（官方 <c>cost_in_ms</c>）。</summary>
    [JsonPropertyName("cost_in_ms")]
    public int? CostInMs { get; set; }

    /// <summary>获取或设置请求的 URL 参数（官方 <c>request_url</c>）。</summary>
    [JsonPropertyName("request_url")]
    public string? RequestUrl { get; set; }

    /// <summary>获取或设置 post 请求的请求参数（官方 <c>request_body</c>）。</summary>
    [JsonPropertyName("request_body")]
    public string? RequestBody { get; set; }

    /// <summary>获取或设置接口请求返回参数（官方 <c>response_body</c>）。</summary>
    [JsonPropertyName("response_body")]
    public string? ResponseBody { get; set; }

    /// <summary>获取或设置接口请求的客户端 ip（官方 <c>client_ip</c>）。</summary>
    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }
}

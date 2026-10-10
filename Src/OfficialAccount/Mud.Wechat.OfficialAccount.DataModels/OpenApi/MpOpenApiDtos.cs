// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OpenApi;

/// <summary>重置 API 调用次数（<c>clear_quota</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体 <c>{"appid":…}</c>
/// （<c>access_token</c> 在 Query——官方明确「不在 body 中」；access_token 在 Query 由 [Token] 管线注入）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpClearQuotaRequest
{
    /// <summary>获取或设置要被清空的账号的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}

/// <summary>
/// 使用 AppSecret 重置 API 调用次数（<c>clear_quota/v2</c>）请求体（<b>免令牌端点</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：该接口<b>不消费 access_token</b>——官方注意事项原文
/// 「该接口通过 appsecret 调用，解决了 access_token 耗尽无法调用『重置 API 调用次数』的问题」。
/// </para>
/// <para>
/// <b>参数位置裁决（I4，官方文档矛盾照录）</b>：官方参数表把 <c>appid</c>/<c>appsecret</c> 列于
/// 「请求体 Request Payload」，但官方代码示例把两参数放在 Query String——两者矛盾。
/// SDK 按<b>参数表（请求体）</b>建模：<c>appsecret</c> 是唯一凭证密钥，进 URL 会扩大泄露面
/// （组件脱敏词表按精确匹配 Query 参数名设计，请求体形态天然不入 URL 日志）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpClearQuotaV2Request
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
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>cgi_path</c> 为 api 的请求地址——官方原文
/// 「不要前缀 https://api.weixin.qq.com，也不要漏了 /，否则都会 76003 的报错」
/// （quota/get 页原文；quota/clear 页示例以 /channels/ec/ 开头——照录两页差异）。
/// </para>
/// <para>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：两页请求体均为单个
/// <c>cgi_path</c>（必填）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpCgiPathRequest
{
    /// <summary>获取或设置 api 的请求地址（官方 <c>cgi_path</c>，必填；以 / 开头、不带 https://api.weixin.qq.com 前缀）。</summary>
    [JsonPropertyName("cgi_path")]
    public string CgiPath { get; set; } = string.Empty;
}

/// <summary>查询 API 调用额度（<c>openapi/quota/get</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>"/xxx/sns/xxx"</c> 类接口不支持本查询（76022）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpGetApiQuotaResponse : MpResponse
{
    /// <summary>获取或设置当日调用额度详情（官方 <c>quota</c>）。</summary>
    [JsonPropertyName("quota")]
    public MpApiQuota? Quota { get; set; }

    /// <summary>获取或设置普通调用频率限制（官方 <c>rate_limit</c>）。</summary>
    [JsonPropertyName("rate_limit")]
    public MpApiRateLimit? RateLimit { get; set; }

    /// <summary>获取或设置代调用频率限制（官方 <c>component_rate_limit</c>，字段同 rate_limit；非代调用形态可能缺省）。</summary>
    [JsonPropertyName("component_rate_limit")]
    public MpApiRateLimit? ComponentRateLimit { get; set; }
}

/// <summary>当日调用额度详情（官方 <c>quota</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpApiQuota
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
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpApiRateLimit
{
    /// <summary>获取或设置周期内可调用数量（官方 <c>call_count</c>，单位次）。</summary>
    [JsonPropertyName("call_count")]
    public int CallCount { get; set; }

    /// <summary>获取或设置更新周期（官方 <c>refresh_second</c>，单位秒）。</summary>
    [JsonPropertyName("refresh_second")]
    public int RefreshSecond { get; set; }
}

/// <summary>查询 rid 信息（<c>openapi/rid/get</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpGetRidInfoRequest
{
    /// <summary>获取或设置调用接口报错返回的 rid（官方 <c>rid</c>，必填；有效期仅 7 天）。</summary>
    [JsonPropertyName("rid")]
    public string Rid { get; set; } = string.Empty;
}

/// <summary>查询 rid 信息（<c>openapi/rid/get</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：rid 查询属开发者私密行为，<b>仅支持同账号查询</b>
/// （账号 A 产生的 rid 须用账号 A 的 access_token 查询）；rid 有效期仅 <b>7 天</b>；
/// <c>"/xxx/sns/xxx"</c> 类接口不支持本查询。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpGetRidInfoResponse : MpResponse
{
    /// <summary>获取或设置该 rid 对应的请求详情（官方 <c>request</c>）。</summary>
    [JsonPropertyName("request")]
    public MpRidRequestInfo? Request { get; set; }
}

/// <summary>rid 对应的请求详情（官方 <c>request</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpRidRequestInfo
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

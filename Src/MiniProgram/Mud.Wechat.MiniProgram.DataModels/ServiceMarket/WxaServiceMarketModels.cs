// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.ServiceMarket;

/// <summary>调用服务市场接口请求体（<c>POST /wxa/servicemarket</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>wx-service-market/api_invokeservice.html</c>。</para>
/// <para>
/// 用于调用服务平台上架的 API（适用于公众号、小程序与第三方平台，区别仅在 access_token 生成方式）；
/// <c>service</c>（服务 ID）与 <c>api</c>（接口名）为必填，<c>data</c> 为服务方接口定义的数据对象
/// （原始键值透传，SDK 不做本地校验）。
/// </para>
/// <para><b>敏感信息警示</b>：<see cref="Data"/> 承载服务方业务参数，可能含个人/业务敏感数据，
/// 禁止写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class WxaServiceMarketRequest
{
    /// <summary>服务 ID（<c>service</c>，必填；服务市场上架时分配）。</summary>
    [JsonPropertyName("service")]
    public string? Service { get; set; }

    /// <summary>接口名（<c>api</c>，必填；如 <c>image_analysis.image_analysis</c>）。</summary>
    [JsonPropertyName("api")]
    public string? Api { get; set; }

    /// <summary>是否异步 API（<c>async</c>，选填；<c>true</c> 时随响应返回 <c>request_id</c> 供 <c>servicemarketretrieve</c> 拉取）。</summary>
    [JsonPropertyName("async")]
    public bool? IsAsync { get; set; }

    /// <summary>服务提供方接口定义的数据（<c>data</c>，原始键值透传）。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, JsonElement>? Data { get; set; }

    /// <summary>调用方请求的唯一标识（<c>client_msg_id</c>，选填；建议用 UUID）。</summary>
    [JsonPropertyName("client_msg_id")]
    public string? ClientMessageId { get; set; }
}

/// <summary>调用服务市场接口应答（<c>request_id</c> / <c>data</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>wx-service-market/api_invokeservice.html</c>。</para>
/// <para><c>data</c> 为服务方返回的 <b>JSON 字符串</b>（官方类型即 string，非对象）；异步场景（请求 <c>async=true</c>）时填 <c>request_id</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class WxaServiceMarketResponse : WxaResponse
{
    /// <summary>异步调用 ID（<c>request_id</c>，仅异步 API 返回；供 <see cref="IWxaServiceMarketService"/> 拉取结果）。</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>服务方回包 JSON 字符串（<c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

/// <summary>异步获取服务市场处理数据请求体（<c>POST /wxa/servicemarketretrieve</c>）。</summary>
/// <remarks>官方文档：<c>wx-service-market/api_servicemarketretrieve.html</c>。<c>request_id</c> 取自 <c>serveicemarket</c> 的异步应答。</remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class WxaServiceMarketRetrieveRequest
{
    /// <summary>异步调用 ID（<c>request_id</c>，必填；<c>调用服务市场接口</c> 异步应答返回）。</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}

/// <summary>异步获取服务市场处理数据应答（<c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class WxaServiceMarketRetrieveResponse : WxaResponse
{
    /// <summary>服务方回包 JSON 字符串（<c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}
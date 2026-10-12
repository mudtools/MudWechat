// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.ServiceMarket;

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

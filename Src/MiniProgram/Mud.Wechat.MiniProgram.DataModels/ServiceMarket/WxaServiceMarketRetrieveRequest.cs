// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.ServiceMarket;

/// <summary>异步获取服务市场处理数据请求体（<c>POST /wxa/servicemarketretrieve</c>）。</summary>
/// <remarks>官方文档：<c>wx-service-market/api_servicemarketretrieve.html</c>。<c>request_id</c> 取自 <c>serveicemarket</c> 的异步应答。</remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class WxaServiceMarketRetrieveRequest
{
    /// <summary>异步调用 ID（<c>request_id</c>，必填；<c>调用服务市场接口</c> 异步应答返回）。</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
}

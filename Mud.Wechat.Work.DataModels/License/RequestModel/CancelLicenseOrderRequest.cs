// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 取消订单请求体（<c>/cgi-bin/license/cancel_order</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class CancelLicenseOrderRequest
{
    /// <summary>获取或设置订单 id（官方必填）。官方约束：只可取消未支付且未失效的订单。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置企业 id（官方可选）。
    /// <para>官方约束：如果是多企业新购订单时不填，否则必填。</para>
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }
}

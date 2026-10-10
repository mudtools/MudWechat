// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 获取订单详情请求体（<c>/cgi-bin/service/get_order</c>，应用版本付费族）。
/// </summary>
/// <remarks>
/// <b>官方契约陷阱</b>：官方参数名为全小写 <c>orderid</c>（非 <c>order_id</c>），本 SDK 照抄官方原文。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class GetPayToolVersionOrderDetailRequest
{
    /// <summary>获取或设置订单号（官方必填，官方字段名为全小写 orderid）。</summary>
    [JsonPropertyName("orderid")]
    public string OrderId { get; set; } = string.Empty;
}
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送取消配送单请求体（<c>POST /cgi-bin/express/intracity/cancelorder</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_cancelorder"/>。</para>
/// <para><c>cancel_reason_id</c> 取消原因枚举：1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它；骑手已接单后取消可能产生扣款（<c>deductfee</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCancelIntracityOrderRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，可选）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>，可选；与 <c>wx_order_id</c> 二选一）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>，可选；与 <c>store_order_id</c> 二选一）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>取消原因 ID（<c>cancel_reason_id</c>，必填；1001 用户取消，1002 门店取消，1003 骑手取消，1004 其它）。</summary>
    [JsonPropertyName("cancel_reason_id")]
    public int CancelReasonId { get; set; }

    /// <summary>取消原因描述（<c>cancel_reason</c>，可选）。</summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 分享员直播间订单汇总应答（<c>POST /shop/sharer/get_sharer_live_order_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表键名 <c>orders</c></b>（官方原文），与 <c>sharers</c> / <c>lives</c> 并列本域三种列表键名之一。</para>
/// <para><b>敏感面</b>：订单条目含 <c>transaction_id</c> / <c>prepay_id</c> / 快递单号 / openid 与全部金额字段，
/// 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerLiveOrderListResponse : WxaResponse
{
    /// <summary>订单列表（<c>orders</c>，object array）。</summary>
    [JsonPropertyName("orders")]
    public List<WxaShopSharerLiveOrder>? Orders { get; set; }

    /// <summary>订单总数（<c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}

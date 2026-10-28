// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询订单发货状态」响应（<c>POST /wxa/sec/order/get_order</c>）。
/// </summary>
/// <remarks>
/// <para>官方返回体表除通用两字段外只有 <c>order</c> 一个对象。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderGetResponse : WxaResponse
{
    /// <summary>order（官方类型 <c>object</c>）：支付单信息。</summary>
    [JsonPropertyName("order")]
    public WxaOrderInfo? Order { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「发货信息合单录入」请求体（<c>POST /wxa/sec/order/upload_combined_shipping_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>顶层没有 <c>logistics_type</c> / <c>delivery_mode</c> / <c>shipping_list</c></b>：
/// 合单模式下物流信息只出现在 <c>sub_orders</c> 每一项内 ⇒ 不得把单条录入的请求体直接搬过来。</para>
/// <para><b><c>sub_orders</c> 官方标「否」</b>（请求体表原文），但业务上无子单即无内容可录
/// ⇒ 必填性以官方表格为准、SDK 侧不加校验（见守卫 OS4 的必填面断言）。</para>
/// <para><b>与单条录入同为「一次重新发货机会」语义</b>（官方注意事项逐字相同）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadcombinedshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderCombinedShippingUploadRequest
{
    /// <summary>order_key（官方类型 <c>object</c>，必填）：合单订单，需要上传物流详情的合单订单，根据订单类型二选一</summary>
    [JsonPropertyName("order_key")]
    public WxaOrderKey? OrderKey { get; set; }

    /// <summary>sub_orders（官方类型 <c>objarray</c>）：子单物流详情</summary>
    [JsonPropertyName("sub_orders")]
    public List<WxaOrderSubOrder>? SubOrders { get; set; }

    /// <summary>upload_time（官方类型 <c>string</c>，必填）：上传时间，用于标识请求的先后顺序，RFC 3339 格式</summary>
    [JsonPropertyName("upload_time")]
    public string? UploadTime { get; set; }

    /// <summary>payer（官方类型 <c>object</c>，必填）：支付者，支付者信息</summary>
    [JsonPropertyName("payer")]
    public WxaOrderPayer? Payer { get; set; }
}

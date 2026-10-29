// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 单笔购物详情（官方 <c>order_list</c> 元素；普通订单上传）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShoppingInfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderInfo
{
    /// <summary>商户交易订单编号（官方必填 <c>merchant_order_no</c>）。</summary>
    [JsonPropertyName("merchant_order_no")]
    public string? MerchantOrderNumber { get; set; }

    /// <summary>订单详情页跳转链接（官方选填 <c>order_detail_jump_link</c>）。</summary>
    [JsonPropertyName("order_detail_jump_link")]
    public OpenPlatformUserOrderJumpLink? OrderDetailJumpLink { get; set; }

    /// <summary>订单购买的商品列表（官方必填 <c>item_list</c>）。</summary>
    [JsonPropertyName("item_list")]
    public List<OpenPlatformUserOrderItem>? ItemList { get; set; }
}

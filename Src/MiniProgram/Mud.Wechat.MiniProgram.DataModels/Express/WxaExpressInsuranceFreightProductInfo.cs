// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>无忧退货投保订单信息（<c>product_info</c> 内嵌对象）。</summary>
/// <remarks>官方原文：用于微信下发投保和理赔通知给用户，用户点击可查看投保订单、点击订单可跳回商家小程序。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsuranceFreightProductInfo
{
    /// <summary>投保订单在商家小程序的 path（<c>order_path</c>，必填）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }

    /// <summary>投保商品列表（<c>goods_list</c>，必填；元素为对象），见 <see cref="WxaExpressInsuranceFreightGoods"/>。</summary>
    [JsonPropertyName("goods_list")]
    public WxaExpressInsuranceFreightGoods[]? GoodsList { get; set; }
}

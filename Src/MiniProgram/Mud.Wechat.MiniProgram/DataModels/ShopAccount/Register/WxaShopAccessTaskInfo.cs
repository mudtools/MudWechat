// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 接入任务完成度（官方 <c>/shop/register/check</c> 回包 <c>data.access_info</c>，11 支 0/1 位）。
/// </summary>
/// <remarks>
/// <para><b>每支都是「未成功/未完成 = 0，成功/完成 = 1」</b>（官方逐支给出 0/1 语义），
/// 且官方把「做过（success）」与「申报完成（finished）」分成两位 ⇒ 只有后者能被
/// <c>finish_access_info</c> 置起，前者由真实调用产生。</para>
/// <para>官方回包示例里 <c>deploy_wxa_finished</c> 一行写作 <c>"deploy_wxa_finished" :0</c>（键后带空格），
/// 属正文排版差异、键名不受影响。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccessTaskInfo
{
    /// <summary>上传商品并审核成功（<c>spu_audit_success</c>，0 未成功 / 1 成功）。</summary>
    [JsonPropertyName("spu_audit_success")]
    public long? SpuAuditSuccess { get; set; }

    /// <summary>物流接口调用成功（<c>send_delivery_success</c>，0 未成功 / 1 成功）。</summary>
    [JsonPropertyName("send_delivery_success")]
    public long? SendDeliverySuccess { get; set; }

    /// <summary>发起一笔订单并支付成功（<c>ec_order_success</c>，0 未成功 / 1 成功）。</summary>
    [JsonPropertyName("ec_order_success")]
    public long? EcOrderSuccess { get; set; }

    /// <summary>售后接口调用成功（<c>ec_after_sale_success</c>，0 未成功 / 1 成功）。</summary>
    [JsonPropertyName("ec_after_sale_success")]
    public long? EcAfterSaleSuccess { get; set; }

    /// <summary>商品接口调试完成（<c>spu_audit_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("spu_audit_finished")]
    public long? SpuAuditFinished { get; set; }

    /// <summary>物流接口调试完成（<c>send_delivery_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("send_delivery_finished")]
    public long? SendDeliveryFinished { get; set; }

    /// <summary>订单接口调试完成（<c>ec_order_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("ec_order_finished")]
    public long? EcOrderFinished { get; set; }

    /// <summary>售后接口调试完成（<c>ec_after_sale_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("ec_after_sale_finished")]
    public long? EcAfterSaleFinished { get; set; }

    /// <summary>测试完成（<c>test_api_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("test_api_finished")]
    public long? TestApiFinished { get; set; }

    /// <summary>发版完成（<c>deploy_wxa_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("deploy_wxa_finished")]
    public long? DeployWxaFinished { get; set; }

    /// <summary>完成自定义组件全部任务（<c>open_product_task_finished</c>，0 未完成 / 1 已完成）。</summary>
    [JsonPropertyName("open_product_task_finished")]
    public long? OpenProductTaskFinished { get; set; }
}

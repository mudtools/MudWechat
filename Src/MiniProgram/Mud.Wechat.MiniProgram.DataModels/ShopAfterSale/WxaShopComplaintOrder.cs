// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 纠纷单摘要（<c>complaint/get</c> 与 <c>complaint/get_list</c> 应答共用）。
/// </summary>
/// <remarks>
/// <para><b>字段取自官方回包示例</b>：本域官方<b>未发布</b> <c>ComplaintOrder</c> / <c>ComplaintOrderState</c> /</para>
/// <para><c>ComplaintOrderType</c> 的字段表与枚举值 ⇒ <c>state</c> / <c>type</c> / <c>judge_result</c> 只建模为</para>
/// <para><c>int</c>，<b>不臆造枚举常量</b>（官方页只写「见 ComplaintOrderState / ComplaintOrderType」而无表）。</para>
/// <para><b>敏感值</b>：<c>openid</c> 与 <c>phone_number</c> 均属个人信息 ⇒ 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintOrder
{
    /// <summary>关联售后单号。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public long? AfterSaleOrderId { get; set; }

    /// <summary>纠纷单号。</summary>
    [JsonPropertyName("complaint_order_id")]
    public long? ComplaintOrderId { get; set; }

    /// <summary>创建时间（官方示例为秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>当前状态到期时间（官方示例为秒级时间戳；0 表示无期限）。</summary>
    [JsonPropertyName("expired_time")]
    public long? ExpiredTime { get; set; }

    /// <summary>平台判定结果（官方未发布枚举值）。</summary>
    [JsonPropertyName("judge_result")]
    public int? JudgeResult { get; set; }

    /// <summary>用户 <c>openid</c>（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>关联订单号（官方示例为字符串）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>投诉人电话（敏感个人信息）。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>纠纷单状态（官方未发布枚举值）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>纠纷单类型（官方未发布枚举值）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>更新时间（官方示例为秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

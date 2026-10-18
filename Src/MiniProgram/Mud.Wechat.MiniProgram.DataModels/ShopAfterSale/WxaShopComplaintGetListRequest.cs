// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 获取纠纷单列表请求体（<c>POST /shop/complaint/get_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方表与示例字段名不一致（照录）</b>：参数表写 <c>status</c>，请求示例写 <c>state</c> ⇒</para>
/// <para>本类按<b>参数表</b>建模 <c>status</c>；若网关按示例只认 <c>state</c>，须由宿主与官方确认后处置，SDK 不双写。</para>
/// <para><b>翻页</b>：<c>offset</c> 从 0 开始、<c>limit</c> 最大 50，二者均必填；时间窗口为秒级时间戳。</para>
/// <para><b>敏感值</b>：<c>order_id</c> 与响应中的 <c>openid</c> / <c>phone_number</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaShopComplaintGetListRequest
{
    /// <summary>纠纷单状态（选填；官方未发布枚举值）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>纠纷单类型（选填；官方未发布枚举值）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>申请时间起始，Unix 秒级时间戳（选填）。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>申请时间结束，Unix 秒级时间戳（选填）。</summary>
    [JsonPropertyName("end_create_time")]
    public long? EndCreateTime { get; set; }

    /// <summary>本次拉取起始位置，从 0 开始（必填）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>本次拉取大小，最大 50（必填）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>关联售后单号（选填）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public long? AfterSaleOrderId { get; set; }

    /// <summary>关联订单号（选填，敏感）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

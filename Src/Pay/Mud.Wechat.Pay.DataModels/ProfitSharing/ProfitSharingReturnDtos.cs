// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.ProfitSharing;

/// <summary>
/// 请求分账回退（<c>POST /v3/profitsharing/return-orders</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_3.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.29；支持商户：普通商户）。
/// </para>
/// <para>
/// <b>⚠️ 路由与直觉不同</b>：回退<b>不是</b> <c>/orders/{out_order_no}/return</c>，
/// 而是独立资源的 <c>/v3/profitsharing/return-orders</c>（无 path / query 参数）。
/// 故本域同时存在「分账单」与「回退单」两个资源族，勿混。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingReturnOrderRequest
{
    /// <summary>
    /// 微信分账单号（<c>order_id</c>，<b>选填</b>）：与 <see cref="OutOrderNo"/> <b>二选一</b>填写（官方原文）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 商户分账单号（<c>out_order_no</c>，<b>选填</b>）：与 <see cref="OrderId"/> <b>二选一</b>填写。
    /// </summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>商户回退单号（<c>out_return_no</c>，必填 string(64)）：商户系统内唯一。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>回退商户号（<c>return_mchid</c>，必填 string(32)）：<b>只能</b>为原分账单中某个接收方的商户号。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchId { get; set; }

    /// <summary>回退金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不得超过原分账金额。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>回退描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 分账回退结果（<b>请求分账回退</b>与<b>查询分账回退结果</b>两接口<b>同一应答形态</b> ⇒ 共用本 DTO）。
/// </summary>
/// <remarks>
/// 官方两页应答字段表逐项一致（<c>order_id</c> / <c>out_order_no</c> / <c>out_return_no</c> /
/// <c>return_id</c> / <c>return_mchid</c> / <c>amount</c> / <c>description</c> / <c>result</c> /
/// <c>fail_reason</c> / <c>create_time</c> / <c>finish_time</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingReturnOrderResponse : WechatPayResponse
{
    /// <summary>微信分账单号（<c>order_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>商户回退单号（<c>out_return_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>微信回退单号（<c>return_id</c>，必填 string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }

    /// <summary>回退商户号（<c>return_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchId { get; set; }

    /// <summary>回退金额（<c>amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>回退描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>回退结果（<c>result</c>，必填）：见 <see cref="ProfitSharingReturnResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>，选填）：官方注明<b>仅</b> <c>result=FAILED</c> 时返回，见 <see cref="ProfitSharingReturnFailReasons"/>。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>创建时间（<c>create_time</c>，必填，RFC3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>完成时间（<c>finish_time</c>，必填，RFC3339）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }
}

/// <summary>
/// 解冻剩余资金（<c>POST /v3/profitsharing/orders/unfreeze</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_5.shtml"/>
/// （2026-10-09 逐字段核验）。<b>应答形态与「请求分账」完全相同</b>（<c>transaction_id</c> /
/// <c>out_order_no</c> / <c>order_id</c> / <c>state</c> / <c>receivers[]</c>）⇒ 复用
/// <see cref="ProfitSharingOrderResponse"/>，不另设类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingUnfreezeRequest
{
    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 商户分账单号（<c>out_order_no</c>，必填 string(64)）：商户系统内部唯一，<b>多次请求等同一次</b>（官方原文）。
    /// </summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>解冻剩余资金描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 查询剩余待分金额应答（<c>GET /v3/profitsharing/transactions/{transaction_id}/amounts</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_6.shtml"/>
/// （2026-10-09 核验；页面标题「查询剩余待分金额」）。应答<b>仅两字段</b>，官方字段表即此。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingAmountsResponse : WechatPayResponse
{
    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>剩余待分金额（<c>unsplit_amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("unsplit_amount")]
    public int? UnsplitAmount { get; set; }
}

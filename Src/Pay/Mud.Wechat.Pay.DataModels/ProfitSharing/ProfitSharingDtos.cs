// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.ProfitSharing;

/// <summary>
/// 添加分账接收方请求（<c>POST /v3/profitsharing/receivers/add</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528995"/>
/// （2026-10-09 逐字段核验；支持商户：<b>普通商户</b>；页面更新时间 2025.09.29）。
/// 官方「注意事项」：商户的分账接收方数量上限为 <b>2 万</b>，达上限须先删除未使用的接收方。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingAddReceiverRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：APP / 小程序 / 公众号三种任填其一，须与 <c>mchid</c> 有绑定关系。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方帐号（<c>account</c>，必填 string(64)）：<c>MERCHANT_ID</c> 时为商户号；<c>PERSONAL_OPENID</c> 时为个人 OpenID。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>
    /// 接收方姓名（<c>name</c>，选填 string(1024)）：需使用<b>微信支付公钥</b>（推荐）或平台证书公钥加密，
    /// 算法 <b>RSAES-OAEP</b>，并把 <c>Wechatpay-Serial</c> 设为对应公钥 ID / 证书序列号。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>接收方与商户的关系（<c>relation_type</c>，必填）：见 <see cref="ProfitSharingRelationTypes"/>。</summary>
    [JsonPropertyName("relation_type")]
    public string? RelationType { get; set; }

    /// <summary>自定义关系（<c>custom_relation</c>，选填 string(10)）：<c>relation_type</c> 取 <c>CUSTOM</c> 时填写。</summary>
    [JsonPropertyName("custom_relation")]
    public string? CustomRelation { get; set; }
}

/// <summary>分账接收方（<c>receivers/add</c> 应答体，官方与请求同字段）。</summary>
/// <remarks>
/// 继承 <see cref="WechatPayResponse"/> 以承载官方失败体的 <c>code</c>/<c>message</c>（判错面）——
/// 本域接口带 <c>[AllowAnyStatusCode]</c>，4xx 错误体不被组件拦成异常，错误体必须有落点。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingReceiver : WechatPayResponse
{
    /// <summary>接收方类型（<c>type</c>）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方帐号（<c>account</c>）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>接收方姓名（<c>name</c>，选填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>接收方与商户的关系（<c>relation_type</c>）：见 <see cref="ProfitSharingRelationTypes"/>。</summary>
    [JsonPropertyName("relation_type")]
    public string? RelationType { get; set; }

    /// <summary>自定义关系（<c>custom_relation</c>，选填）。</summary>
    [JsonPropertyName("custom_relation")]
    public string? CustomRelation { get; set; }
}

/// <summary>
/// 请求分账（<c>POST /v3/profitsharing/orders</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>
/// （2026-10-09 逐字段核验）。官方错误码含 <c>RULE_LIMIT</c>（超出最大分账比例）、
/// <c>NOT_ENOUGH</c>（分账金额不足）、<c>FREQUENCY_LIMITED</c>（同笔订单分账频率过高）。
/// </para>
/// <para>
/// <b>前置条件</b>：① 下单时须在请求体带 <c>settle_info.profit_sharing = true</c>（否则订单不可分账）；
/// ② 每个接收方须先经 <c>receivers/add</c> 建立关系（官方 FAQ：「分账接收方关系不存在」即此因）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingOrderRequest
{
    /// <summary>
    /// 公众账号 ID（<c>appid</c>，<b>选填</b>）：<b>仅当</b>接收方含 <c>PERSONAL_OPENID</c> 时必填
    /// （官方原文，不得统一按必填处理）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 商户分账单号（<c>out_order_no</c>，必填 string(64)）：商户系统内唯一；
    /// <b>同一分账单号多次请求等同一次</b>（幂等键）。仅可含数字、大小写字母及 <c>_-|*@</c>。
    /// </summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>分账接收方列表（<c>receivers</c>，必填，<b>最多 50 个</b>）：可把出资商户自身设为接收方。</summary>
    [JsonPropertyName("receivers")]
    public List<ProfitSharingOrderReceiver>? Receivers { get; set; }

    /// <summary>
    /// 是否解冻剩余未分资金（<c>unfreeze_unsplit</c>，必填 bool）：
    /// <c>true</c> 剩余未分金额解冻回发起方（<b>解冻后不可再次分账</b>）；<c>false</c> 不解冻，可再次分账。
    /// </summary>
    [JsonPropertyName("unfreeze_unsplit")]
    public bool? UnfreezeUnsplit { get; set; }
}

/// <summary>请求分账的接收方条目（<c>receivers[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingOrderReceiver
{
    /// <summary>接收方类型（<c>type</c>，必填）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方帐号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>
    /// 接收方姓名（<c>name</c>，选填 string(1024)）：<c>MERCHANT_ID</c> 时为商户全称（必传；小微/个体户为开户人姓名）；
    /// <c>PERSONAL_OPENID</c> 时为个人姓名（选传，**传则校验实名匹配，不匹配会拒绝分账请求**）。
    /// 需 RSAES-OAEP 加密（同添加接收方）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>分账金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不得超过原订单支付金额及最大分账比例金额。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>分账描述（<c>description</c>，必填 string(80)）：会在查询分账结果与分账账单中原样返回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 分账结果（<b>请求分账</b>与<b>查询分账结果</b>两接口<b>同一应答形态</b> ⇒ 共用本 DTO）。
/// </summary>
/// <remarks>
/// 官方两页的应答字段表逐项一致（<c>transaction_id</c> / <c>out_order_no</c> / <c>order_id</c> /
/// <c>state</c> / <c>receivers[]</c>），故刻意不拆成两个类型 —— 拆开只会让「同一份事实」有两处可能漂移。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingOrderResponse : WechatPayResponse
{
    /// <summary>微信支付订单号（<c>transaction_id</c>，必填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，必填 string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>分账单状态（<c>state</c>，必填）：见 <see cref="ProfitSharingOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>分账接收方结果列表（<c>receivers</c>，必填）。</summary>
    [JsonPropertyName("receivers")]
    public List<ProfitSharingReceiverResult>? Receivers { get; set; }
}

/// <summary>分账接收方结果（应答 <c>receivers[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingReceiverResult
{
    /// <summary>分账金额（<c>amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    /// <summary>分账描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填）：见 <see cref="ProfitSharingReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>
    /// 分账结果（<c>result</c>，必填）：见 <see cref="ProfitSharingReceiverResults"/>。
    /// <b>注意</b>：<c>state=FINISHED</c> 仅代表分账动账执行完毕，<b>各接收方是否成功看本字段</b>。
    /// </summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>分账失败原因（<c>fail_reason</c>，选填）：<c>result=CLOSED</c> 时返回，见 <see cref="ProfitSharingFailReasons"/>。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>分账创建时间（<c>create_time</c>，必填，RFC3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>分账完成时间（<c>finish_time</c>，必填，RFC3339）。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>分账明细单号（<c>detail_id</c>，必填 string(64)）：可与资金账单对账（对应「业务凭证号」）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }
}

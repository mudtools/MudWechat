// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 请求分账（<c>POST /v3/ecommerce/profitsharing/orders</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>本接口是 <c>Wechatpay-Serial</c> 必填的端点之一</b>：<c>receivers[].receiver_name</c> 属敏感字段，
/// 必须以微信支付公钥（推荐）或平台证书公钥加密后传入，并在 Header 带上对应公钥 ID / 证书序列号 ⇒
/// 明文姓名直接塞进该字段会得到密文解析失败类错误，而不是可读的校验错误。
/// </para>
/// <para>
/// <b><c>appid</c> 官方标选填，但「分账接收方类型包含 <c>PERSONAL_OPENID</c> 时必填」</b> ⇒
/// 条件必填，本类型不做静态强制，由宿主按 <see cref="EcommerceReceiverTypes.PersonalOpenId"/> 自查。
/// </para>
/// <para>
/// <b><c>finish</c> 是必填的「是否分账完成」开关</b>，与 4012477745「解冻剩余资金」是两条独立路径：
/// <c>finish=true</c> 时官方限定<b>分账接收商户只支持平台商户</b>，且剩余未分金额当场解冻回二级商户、
/// 该订单<b>不支持再次分账</b>。传错方向不可回滚。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingOrderRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，<b>选填</b> string(32)）：须与 <c>mchid</c> 有绑定关系；接收方含 <c>PERSONAL_OPENID</c> 时<b>必填</b>。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）：即分账的出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 商户分账单号（<c>out_order_no</c>，必填 string(64)）：只能是数字、大小写字母 <c>-_|*@</c>；
    /// <b>单次分账、多次分账、完结分账应使用不同的商户分账单号</b>，同一分账单号多次请求等同一次。
    /// </summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>分账接收方列表（<c>receivers</c>，必填数组）：可以设置出资商户作为分账接收方。</summary>
    [JsonPropertyName("receivers")]
    public EcommerceProfitSharingReceiverInput[]? Receivers { get; set; }

    /// <summary>是否分账完成（<c>finish</c>，必填 boolean）：<c>true</c> 时接收方只支持平台商户且剩余资金当场解冻、不可再分账。</summary>
    [JsonPropertyName("finish")]
    public bool? Finish { get; set; }
}

/// <summary>
/// 请求分账的接收方入参（<c>receivers[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与应答侧接收方是两个类型</b>：入参含 <c>amount</c>/<c>description</c>/<c>receiver_name</c> 但无
/// <c>result</c>/<c>detail_id</c>/<c>fail_reason</c>/<c>finish_time</c>，官方两张表不重合 ⇒ 不做「一个类两套语义」。
/// </para>
/// <para>
/// <b><c>receiver_name</c> 的必填性是「分类型必填」</b>：官方原文「分账接收方类型是 <c>MERCHANT_ID</c> 时，
/// 是商户全称（必传），当商户是小微商户或个体户时，是开户人姓名」；<c>PERSONAL_OPENID</c> 时选传、
/// 传了则校验实名匹配，<b>不匹配会拒绝分账请求</b>。类型上统一标为可空 <c>string</c>，密文/明文由宿主决定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingReceiverInput
{
    /// <summary>分账接收方类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommerceReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>分账接收方账号（<c>receiver_account</c>，必填 string(64)）：<c>MERCHANT_ID</c> 时为商户号，<c>PERSONAL_OPENID</c> 时为个人 OpenID。</summary>
    [JsonPropertyName("receiver_account")]
    public string? ReceiverAccount { get; set; }

    /// <summary>分账金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不能超过原订单支付金额及最大分账比例金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>分账描述（<c>description</c>，必填 string(80)）：查询分账结果与分账账单中原样返回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 分账个人接收方姓名（<c>receiver_name</c>，<b>选填</b> string(10240)）：
    /// <b>需公钥加密</b>（长度上限 10240 即密文长度）；<c>MERCHANT_ID</c> 时为商户全称、官方标「必传」。
    /// 该字段属敏感信息，<b>不得进日志、遥测或异常消息</b>。
    /// </summary>
    [JsonPropertyName("receiver_name")]
    public string? ReceiverName { get; set; }
}

/// <summary>
/// 分账单（<c>POST</c> 请求分账 与 <c>GET</c> 查询分账结果 <b>两支的共用应答</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/> 应答 14 行与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/> 应答 16 行，
/// 差异<b>只有三处</b>：<c>status</c> 在 POST 页标选填、在 GET 页标必填；GET 页另有选填
/// <c>finish_amount</c> / <c>finish_description</c>（「仅当查询分账完结的执行结果时存在」）。
/// 字段名与类型逐行相同 ⇒ 以超集共用；本仓 DTO 属性一律可空，必填性差异只写进注释。
/// </para>
/// <para>
/// <b><c>status</c> 是单状态、<c>receivers[].result</c> 是每个接收方的结果</b>：官方原文「每个接收方的分账结果
/// 请查看 receivers 中的 result 字段」，且 <c>FINISHED</c>「仅代表分账动账执行完毕」<b>不等于全部分账成功</b>
/// —— 只判 <c>status</c> 会把「有接收方 CLOSED」的账单误记为成功。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingOrderResponse : WechatPayResponse
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）：分账的出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，必填 string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 分账单状态（<c>status</c>）：<b>POST 应答标选填、GET 应答标必填</b>；见 <see cref="EcommerceSplitOrderStatuses"/>。
    /// <c>FINISHED</c> 只代表动账执行完毕，不代表每个接收方成功。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>分账接收方列表（<c>receivers</c>，必填数组）。</summary>
    [JsonPropertyName("receivers")]
    public EcommerceProfitSharingReceiver[]? Receivers { get; set; }

    /// <summary>分账完结金额（<c>finish_amount</c>，<b>选填</b> integer，单位分）：仅查询分账完结的执行结果时存在（故 <c>long</c> 承载）。</summary>
    [JsonPropertyName("finish_amount")]
    public long? FinishAmount { get; set; }

    /// <summary>分账完结描述（<c>finish_description</c>，<b>选填</b> string(80)）：仅查询分账完结的执行结果时存在。</summary>
    [JsonPropertyName("finish_description")]
    public string? FinishDescription { get; set; }
}

/// <summary>
/// 分账单应答里的接收方（查询侧形态）。
/// </summary>
/// <remarks>
/// <b>两支的接收方应答表逐行相同</b>（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>）⇒ 共用一个类型。
/// <b><c>fail_reason</c> 仅当 <c>result</c> 为 <c>CLOSED</c> 时返回</b>，其余终态下缺省。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingReceiver
{
    /// <summary>分账金额（<c>amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>分账描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>分账结果（<c>result</c>，必填 string(32)）：见 <see cref="EcommerceProfitSharingResults"/>；<c>PENDING</c> 为非终态。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>分账完成时间（<c>finish_time</c>，必填）：rfc3339 文本。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>分账失败原因（<c>fail_reason</c>，<b>选填</b> string(64)）：<c>result</c> 为 <c>CLOSED</c> 时返回；见 <see cref="EcommerceProfitSharingFailReasons"/>。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommerceReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>receiver_account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("receiver_account")]
    public string? ReceiverAccount { get; set; }

    /// <summary>分账明细单号（<c>detail_id</c>，必填 string(64)）：可与资金账单对账，对应资金账单中的「业务凭证号」。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }
}

/// <summary>
/// 请求分账回退（<c>POST /v3/ecommerce/profitsharing/returnorders</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b><c>order_id</c> 与 <c>out_order_no</c> 二选一</b>（两支均标选填）：全不传无法定位原分账单，
/// 官方未把它做成必填而是靠「二选一」约束 ⇒ 属宿主责任。
/// </para>
/// <para>
/// <b><c>transaction_id</c> 选填，但「大于 6 个月的订单必填」</b>（官方原文）：条件必填，超期订单不传会被拒。
/// </para>
/// <para>
/// <b><c>return_mchid</c> 对应请求分账时传入的 <c>receiver_account</c></b>：即回退接收商户是原分账接收方，
/// 而 <c>sub_mchid</c> 是原出资商户 —— 两者方向相反，是最易写反的一对字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingReturnRequest
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）：分账回退的接收商户，对应原分账<b>出资</b>的分账方商户。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，<b>选填</b> string(64)）：与 <c>out_order_no</c> 二选一。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，<b>选填</b> string(64)）：与 <c>order_id</c> 二选一。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>商户回退单号（<c>out_return_no</c>，必填 string(64)）：后台新生成的回退单号，唯一；同一回退单号多次请求等同一次。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>回退商户号（<c>return_mchid</c>，必填 string(32)）：对应请求分账接口时传入的 <c>receiver_account</c>。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchId { get; set; }

    /// <summary>回退金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不能超过原始分账单分出给该接收方的金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>回退描述（<c>description</c>，必填 string(80)）：查询回退结果与分账账单中原样返回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，<b>选填</b> string(32)）：订单超过 6 个月时<b>必填</b>。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>
/// 分账回退单（<c>POST</c> 请求回退 与 <c>GET</c> 查询回退结果 <b>两支的共用应答</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477740"/> 的应答表<b>11 行字段名、类型、必填性逐行相同</b>
/// （仅行序不同）⇒ 共用一个类型。
/// </para>
/// <para>
/// <b>但 <c>fail_reason</c> 的取值集两支不同</b>：POST 页只列三值，GET 页多列 <c>INVALID_REQUEST</c>
/// （见 <see cref="EcommerceProfitSharingReturnFailReasons"/>）⇒ 共用类型、按页分列取值口径。
/// </para>
/// <para>
/// <b>官方的资金安全警示（原文）</b>：结果为 <c>PROCESSING</c> 时「请勿变更商户回退单号，使用相同的参数再次发起
/// 分账回退，否则会出现资金风险」；处理中的回退单 5 天未成功会被超时置为 <c>FAILED</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingReturnResponse : WechatPayResponse
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）：分账回退的接收商户。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，必填 string(64)）：原发起分账请求时微信返回的单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填 string(64)）：原发起分账请求时使用的商户分账单号。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>商户回退单号（<c>out_return_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>微信回退单号（<c>return_no</c>，必填 string(64)）：微信分账回退系统返回的唯一标识。</summary>
    [JsonPropertyName("return_no")]
    public string? ReturnNo { get; set; }

    /// <summary>回退商户号（<c>return_mchid</c>，必填 string(32)）：对应请求分账时的 <c>receiver_account</c>。</summary>
    [JsonPropertyName("return_mchid")]
    public string? ReturnMchId { get; set; }

    /// <summary>回退金额（<c>amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>回退结果（<c>result</c>，必填 string(32)）：见 <see cref="EcommerceProfitSharingReturnResults"/>；<c>PROCESSING</c> 非终态。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>，<b>选填</b> string(64)）：<c>result</c> 为 <c>FAILED</c> 时返回；见 <see cref="EcommerceProfitSharingReturnFailReasons"/>。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>回退完成时间（<c>finish_time</c>，必填）：rfc3339 文本。</summary>
    [JsonPropertyName("finish_time")]
    public string? FinishTime { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>
/// 分账完结·解冻剩余资金（<c>POST /v3/ecommerce/profitsharing/finish-order</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477745"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。路由段是 <c>finish-order</c>（连字符），
/// <b>不是</b> <c>finishorder</c> —— 与本域其余 <c>returnorders</c>（无连字符）拼法不一致，照抄官方。
/// </para>
/// <para>
/// <b><c>out_order_no</c> 须与请求分账时不同</b>：官方在描述里写明「单次分账、多次分账、完结分账应使用不同的
/// 商户分账单号」—— 复用原分账单号会被当同一笔请求处理。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingFinishRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：分账出资的二级商户。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填 string(64)）：完结分账须使用<b>新的</b>分账单号。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>解冻原因描述（<c>description</c>，必填 string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 分账完结应答（<c>POST /v3/ecommerce/profitsharing/finish-order</c>）：官方只回四行，<b>无结果状态</b>。
/// </summary>
/// <remarks>
/// 应答表只有 <c>sub_mchid</c> / <c>transaction_id</c> / <c>out_order_no</c> / <c>order_id</c> ⇒
/// <b>解冻是否生效须回头用 4012477734 查单或 4012477751 查待分金额确认</b>，不能以本应答的返回即记为完结成功。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProfitSharingFinishResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 查询订单剩余待分金额（<c>GET /v3/ecommerce/profitsharing/orders/{transaction_id}/amounts</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477751"/>（2026-10-15 核验；支持商户：【平台商户】）。
/// 路由把 <c>transaction_id</c> 夹在中间、<c>amounts</c> 收尾 ⇒ 不能按「<c>/orders</c> + query」的直觉拼。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceUnsplitAmountResponse : WechatPayResponse
{
    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单剩余待分金额（<c>unsplit_amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("unsplit_amount")]
    public long? UnsplitAmount { get; set; }
}

/// <summary>
/// 添加分账接收方（<c>POST /v3/ecommerce/profitsharing/receivers/add</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477758"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b><c>name</c> 的必填性分类型</b>：<c>MERCHANT_ID</c> 时官方标「必传」（小微/个体户时为开户人姓名），
/// <c>PERSONAL_OPENID</c> 时选传、传了会校验实名匹配。表上标选填 ⇒ 类型保持可空。
/// </para>
/// <para>
/// <b>本页 <c>name</c> 未标注需加密</b>（与 4012691594 的 <c>receiver_name</c> 不同，且无 <c>Wechatpay-Serial</c> 头）
/// ⇒ 明文传输，照抄官方，不做「顺手对齐」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceAddReceiverRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，<b>选填</b> string(32)）：接收方类型为 <c>PERSONAL_OPENID</c> 时<b>必填</b>。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommerceReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）：<b>本接口字段名是 <c>account</c>，不是 <c>receiver_account</c></b>。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>接收方名称（<c>name</c>，<b>选填</b> string(256)）：<c>MERCHANT_ID</c> 时为商户全称、官方标必传。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>与分账方的关系类型（<c>relation_type</c>，必填 string(32)）：见 <see cref="EcommerceRelationTypes"/>。</summary>
    [JsonPropertyName("relation_type")]
    public string? RelationType { get; set; }
}

/// <summary>
/// 删除分账接收方（<c>POST /v3/ecommerce/profitsharing/receivers/delete</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477759"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。入参比添加支少 <c>name</c> 与 <c>relation_type</c> 两行。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceDeleteReceiverRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，<b>选填</b> string(32)）：接收方类型为 <c>PERSONAL_OPENID</c> 时<b>必填</b>。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>接收方类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommerceReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// 分账接收方维护应答（<c>receivers/add</c> 与 <c>receivers/delete</c> <b>两支的共用应答</b>）。
/// </summary>
/// <remarks>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：两页应答表均只有 <c>type</c> + <c>account</c> 两行且必填性一致 ⇒ 共用。
/// 官方无「关系类型」「名称」回显 ⇒ 添加支也不回 <c>relation_type</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceReceiverResponse : WechatPayResponse
{
    /// <summary>接收方类型（<c>type</c>，必填 string(32)）：见 <see cref="EcommerceReceiverTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，必填 string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// 发起补差（<c>POST /v3/ecommerce/subsidies/create</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477631"/>
/// （2026-10-15 逐字段核验；<b>支持商户：【普通服务商】</b> —— 补差三支均为普通服务商，
/// 与本域其余端点的【平台商户】不同，见接口注释）。
/// </para>
/// <para>
/// <b><c>amount</c> 「不能超过下单时候的最大补差金额」</b>（官方原文）：上限来自主订单的
/// <c>subsidy.total</c>，事后不可提额 ⇒ 补差金额需与下单时约定一致。
/// </para>
/// <para>
/// <b><c>out_subsidy_no</c> 官方标选填</b>：不传则无商户侧幂等键，重复请求不等同一次 ⇒
/// 需要幂等的宿主必须显式传（数字、大小写字母 <c>-_|*@</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCreateSubsidyRequest
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）：补差的二级商户，微信支付分配的商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(64)）：注意本页长度标注是 64，与分账页的 32 不同，照抄。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>补差金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不能超过下单时候的最大补差金额。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>补差描述（<c>description</c>，必填 string(80)）：查询时原样带回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商户补差单号（<c>out_subsidy_no</c>，<b>选填</b>）：商户系统内部唯一，同一单号多次请求等同一次。</summary>
    [JsonPropertyName("out_subsidy_no")]
    public string? OutSubsidyNo { get; set; }
}

/// <summary>
/// 补差单（发起补差应答）。
/// </summary>
/// <remarks>
/// <b><c>result</c> 有 <c>REFUND</c>（全额回退补差）三值</b>，而回退补差支的 <c>result</c> 只有两值 ⇒
/// 两支的结果枚举不可互相套用（见 <see cref="EcommerceSubsidyCreateResults"/> 与 <see cref="EcommerceSubsidyReturnResults"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCreateSubsidyResponse : WechatPayResponse
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>微信补差单号（<c>subsidy_id</c>，必填 string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("subsidy_id")]
    public string? SubsidyId { get; set; }

    /// <summary>补差描述（<c>description</c>，必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>补差金额（<c>amount</c>，<b>选填</b> integer，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>补差单结果（<c>result</c>，必填）：见 <see cref="EcommerceSubsidyCreateResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>补差完成时间（<c>success_time</c>，必填）：rfc3339 文本。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>商户补差单号（<c>out_subsidy_no</c>，<b>选填</b>）：请求未传则应答亦缺省。</summary>
    [JsonPropertyName("out_subsidy_no")]
    public string? OutSubsidyNo { get; set; }
}

/// <summary>
/// 补差回退（<c>POST /v3/ecommerce/subsidies/return</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477636"/>
/// （2026-10-15 逐字段核验；支持商户：【普通服务商】）。
/// </para>
/// <para>
/// <b><c>refund_id</c> 与 <c>subsidy_id</c> 均选填</b>：官方对 <c>refund_id</c> 注明
/// 「因用户账户异常而无法退款时可不传，其他情况必传」⇒ 条件必填；<c>subsidy_id</c> 缺省时按订单定位补差单。
/// </para>
/// <para>
/// <b>本次回退金额 + 已回退金额 ≤ 补差金额</b>，且指定出资账户时「本次回退金额不超过该账户实际可用资金」；
/// <c>amount</c> 在指定出资时「必须为所有出资账户金额之和」（当前仅支持一个账户出资）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceReturnSubsidyRequest
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户补差回退单号（<c>out_order_no</c>，必填 string(64)）：商户系统内部唯一，同一单号多次请求等同一次。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>微信退款单号（<c>refund_id</c>，<b>选填</b> string(64)）：用户账户异常无法退款时可不传，其他情况必传。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>补差回退金额（<c>amount</c>，必填 integer，<b>单位分</b>）：不能超过补差单的补差金额；指定出资账户时须为出资账户金额之和。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>补差回退描述（<c>description</c>，必填 string(80)）：查询时原样带回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>微信补差单号（<c>subsidy_id</c>，<b>选填</b> string(64)）。</summary>
    [JsonPropertyName("subsidy_id")]
    public string? SubsidyId { get; set; }

    /// <summary>回退出资账户及金额（<c>from</c>，<b>选填</b>数组）：需从指定账户出资时传；当前仅支持从一个账户出资。</summary>
    [JsonPropertyName("from")]
    public EcommerceSubsidyFunding[]? From { get; set; }
}

/// <summary>
/// 补差回退的出资账户及金额（<c>from[]</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceSubsidyFunding
{
    /// <summary>出资账户类型（<c>account</c>，<b>选填</b>）：<c>AVAILABLE</c> 可用余额 / <c>UNAVAILABLE</c> 不可用余额，见 <see cref="EcommerceFundingAccountTypes"/>。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>出资金额（<c>amount</c>，<b>选填</b> integer，<b>单位分</b>，只能为整数）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>
/// 补差回退单（<c>POST /v3/ecommerce/subsidies/return</c> 应答）。
/// </summary>
/// <remarks>
/// <b>回退单号字段名是 <c>subsidy_refund_id</c></b>（微信补差回退单号），与请求侧的 <c>subsidy_id</c>（原补差单号）
/// 同表并存，是本页最易认错的一对。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceReturnSubsidyResponse : WechatPayResponse
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>微信补差回退单号（<c>subsidy_refund_id</c>，必填）：补差回退系统返回的唯一标识。</summary>
    [JsonPropertyName("subsidy_refund_id")]
    public string? SubsidyRefundId { get; set; }

    /// <summary>微信退款单号（<c>refund_id</c>，<b>选填</b>）：因用户账户异常而无法退款时缺省。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户补差回退单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>补差回退金额（<c>amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>补差回退描述（<c>description</c>，必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>补差回退结果（<c>result</c>，必填）：见 <see cref="EcommerceSubsidyReturnResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>补差回退完成时间（<c>success_time</c>，必填）：rfc3339 文本。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>微信补差单号（<c>subsidy_id</c>，<b>选填</b> string(64)）。</summary>
    [JsonPropertyName("subsidy_id")]
    public string? SubsidyId { get; set; }

    /// <summary>回退出资账户及金额（<c>from</c>，<b>选填</b>数组）：请求未指定出资账户时缺省。</summary>
    [JsonPropertyName("from")]
    public EcommerceSubsidyFunding[]? From { get; set; }
}

/// <summary>
/// 取消补差（<c>POST /v3/ecommerce/subsidies/cancel</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012477639"/>
/// （2026-10-15 逐字段核验；支持商户：【普通服务商】）。
/// </para>
/// <para>
/// <b>本页无金额、无补差单号入参</b>：只按 <c>sub_mchid</c> + <c>transaction_id</c> 整单取消 ⇒
/// 与「补差回退（部分金额）」是两种语义，不可互相替代。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelSubsidyRequest
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>取消补差描述（<c>description</c>，必填 string(80)）：查询时原样带回。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 取消补差应答：官方只回四行（无单号、无时间）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelSubsidyResponse : WechatPayResponse
{
    /// <summary>电商平台二级商户号（<c>sub_mchid</c>，必填）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信订单号（<c>transaction_id</c>，必填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>取消补差结果（<c>result</c>，必填）：<c>SUCCESS</c> / <c>FAIL</c>，见 <see cref="EcommerceSubsidyCancelResults"/>。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>取消补差描述（<c>description</c>，必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

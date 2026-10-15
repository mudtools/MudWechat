// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 电商服务商委托代扣预签约（<c>POST /v3/ecommerce/combine-papay/contracts/pre-entrust-sign</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012881608"/>
/// （2026-10-15 逐字段核验，全表 7 行；旧版 wiki 面 <c>chapter5_5_1.shtml</c> 现直接重定向至该页，
/// 官方目录现行页面即此 ID）。
/// </para>
/// <para>
/// <b>本支无 <c>Wechatpay-Serial</c>、无加密字段</b>（官方 Header 表只有 Authorization/Accept/Content-Type）
/// ⇒ 与商家转账、异常退款不同，预签约的 <c>contract_display_account</c> 是明文展示名。
/// </para>
/// <para>
/// <b>本接口只完成「预签约」</b>：签约动作仍需在客户端调起微信（App 拉端或小程序），
/// 应答里的 <see cref="EcommercePapayPreEntrustSignResponse.PreEntrustwebId"/> 才是拉端凭据，
/// 且官方限定<b>两小时内有效</b> ⇒ 取到即发起，不要缓存复用。
/// </para>
/// <para>
/// <see cref="NotifyUrl"/> 官方要求「直接可访问的 URL、不能携带参数、必须使用 https 协议」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayPreEntrustSignRequest
{
    /// <summary>商户 AppID（<c>appid</c>，必填 string(32)）：电商服务商在公众平台或开放平台申请<b>移动应用</b>后分配的账号 ID。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>委托代扣协议模板 ID（<c>plan_id</c>，必填 integer）：按接入指引申请得到。</summary>
    [JsonPropertyName("plan_id")]
    public int? PlanId { get; set; }

    /// <summary>商户签约协议号（<c>out_contract_code</c>，必填 string(32)）：商户侧签约协议号，商户侧须保证唯一。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>用户账户展示名称（<c>contract_display_account</c>，必填 string(32)）：签约用户的名称，签约页面展示，由商户传入。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>回调通知地址（<c>notify_url</c>，必填 string(256)）：接收微信支付异步通知；须直接可访问、不能携带参数、<b>必须 https</b>。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>返回 APP 标识（<c>return_app</c>，选填 boolean）：控制签约页结束后的返回路径；不传则签约完成后停留在微信内。<c>true</c> 表示返回 APP，且要求签约 AppID 为发起签约的 APP 所有并在开放平台注册过。</summary>
    [JsonPropertyName("return_app")]
    public bool? ReturnApp { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(255)）：创建签约时可传入的自定义数据，对用户不可见，用于存储签约相关的商户自定义信息。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }
}

/// <summary>
/// 预签约应答：回传拉端凭据与小程序签约参数。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012881608"/>。
/// <para>
/// <b><see cref="PreEntrustwebId"/> 两小时内有效</b>（官方原文）⇒ 拿到就用来调起微信客户端签约。
/// </para>
/// <para>
/// <b><see cref="MiniprogramUsername"/> / <see cref="MiniprogramPath"/> 只按模板条件返回</b>：
/// 官方限定两种情形才出现 —— 模板 ID 为 2025-09-23 及之后申请的委托代扣模板，或 2025-09-23 之前申请但已开通
/// <c>WXLaunchMiniProgram</c> 权限的模板；返回时用 LaunchMiniProgram SDK 签约且<b>字段名与值须原样填入</b>，
/// 未返回则说明该模板只能走 APP 调起签约 ⇒ 二者必须成对判断，不要单独假设其一存在。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayPreEntrustSignResponse : WechatPayResponse
{
    /// <summary>预签约 ID（<c>pre_entrustweb_id</c>，必填 string(128)）：委托代扣签约会话 ID，<b>两小时内有效</b>。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：微信支付分配的商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>委托代扣协议模板 ID（<c>plan_id</c>，必填 integer）：回显请求值，与 <see cref="OutContractCode"/> 共同构成后续查询/解约的定位键。</summary>
    [JsonPropertyName("plan_id")]
    public int? PlanId { get; set; }

    /// <summary>商户签约协议号（<c>out_contract_code</c>，必填 string(32)）：回显请求值。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>跳转签约小程序的 username（<c>miniprogram_username</c>，选填）：仅特定模板条件返回，须与 <see cref="MiniprogramPath"/> 一起原样用于 LaunchMiniProgram SDK。</summary>
    [JsonPropertyName("miniprogram_username")]
    public string? MiniprogramUsername { get; set; }

    /// <summary>跳转签约小程序的 path（<c>miniprogram_path</c>，选填）：返回条件同 <see cref="MiniprogramUsername"/>。</summary>
    [JsonPropertyName("miniprogram_path")]
    public string? MiniprogramPath { get; set; }
}

/// <summary>
/// 委托代扣签约协议查询应答（<c>GET /v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>
/// （2026-10-15 逐字段核验，全表 14 行；旧 wiki 面 <c>chapter5_5_2.shtml</c> 重定向至该页）。
/// </para>
/// <para>
/// <b>定位键是 Path 上的 <c>plan_id</c> + <c>out_contract_code</c>，没有 query 参数</b>；
/// 应答回显二者，同时给出协议主键 <see cref="ContractId"/> —— 免密扣款下单要用的是 <see cref="ContractId"/>，
/// 不是本接口的定位键 ⇒ 两者须分别持久化。
/// </para>
/// <para>
/// <see cref="ContractTerminateInfo"/> 官方注明<b>仅当 <c>contract_state=TERMINATED</c> 时该值有效</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayContractQueryResponse : WechatPayResponse
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：微信支付分配的商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>委托代扣协议 ID（<c>contract_id</c>，必填 string(32)）：协议主键，唯一定义该资源；免密扣款下单时传入本值。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>商户 AppID（<c>appid</c>，必填 string(32)）：电商服务商的移动应用账号 ID。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>委托代扣协议模板 ID（<c>plan_id</c>，必填 integer）。</summary>
    [JsonPropertyName("plan_id")]
    public int? PlanId { get; set; }

    /// <summary>商户签约协议号（<c>out_contract_code</c>，必填 string(32)）：商户侧唯一。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>用户账户展示名称（<c>contract_display_account</c>，必填 string(32)）：签约时由商户传入的名称，用于页面展示。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>委托代扣协议状态（<c>contract_state</c>，必填）：见 <see cref="EcommercePapayContractStates"/>。</summary>
    [JsonPropertyName("contract_state")]
    public string? ContractState { get; set; }

    /// <summary>协议签署时间（<c>contract_signed_time</c>，必填 string(32)）：rfc3339 格式 <c>YYYY-MM-DDTHH:mm:ss+TIMEZONE</c>。</summary>
    [JsonPropertyName("contract_signed_time")]
    public string? ContractSignedTime { get; set; }

    /// <summary>协议到期时间（<c>contract_expired_time</c>，必填 string(32)）：rfc3339 格式；扣款前须比对，过期协议不能免密扣款。</summary>
    [JsonPropertyName("contract_expired_time")]
    public string? ContractExpiredTime { get; set; }

    /// <summary>协议解约信息（<c>contract_terminate_info</c>，选填）：官方注明仅当 <c>contract_state=TERMINATED</c> 时该值有效。</summary>
    [JsonPropertyName("contract_terminate_info")]
    public EcommercePapayContractTerminateInfo? ContractTerminateInfo { get; set; }

    /// <summary>用户 OpenID（<c>openid</c>，必填 string(64)）：商户 AppID 下的用户唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(255)）：预签约时传入的商户数据包，在本 API 原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }
}

/// <summary>
/// 协议解约信息（<c>contract_terminate_info</c>）：三字段官方均标必填，但外层对象只在已解约时出现。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>。
/// <see cref="ContractTerminationRemark"/> 与解约接口请求里的同名字段对应
/// （API 发起解约时回显商户填写的备注，见
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884072"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayContractTerminateInfo
{
    /// <summary>协议解约方式（<c>contract_termination_mode</c>，必填 string(32)）：见 <see cref="EcommercePapayTerminationModes"/>，用于判别解约由用户、商户还是微信侧发起。</summary>
    [JsonPropertyName("contract_termination_mode")]
    public string? ContractTerminationMode { get; set; }

    /// <summary>协议解约时间（<c>contract_terminated_time</c>，必填 string(32)）：rfc3339 格式。</summary>
    [JsonPropertyName("contract_terminated_time")]
    public string? ContractTerminatedTime { get; set; }

    /// <summary>解约备注（<c>contract_termination_remark</c>，必填 string(128)）：解约原因的备注说明，如「签约信息有误，须重新签约」。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }
}

/// <summary>
/// 委托代扣解约（<c>POST /v3/ecommerce/combine-papay/contracts/plan-id/{plan_id}/out-contract-code/{out_contract_code}/terminate</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884072"/>
/// （2026-10-15 逐字段核验，Body 仅 1 行；旧 wiki 面 <c>chapter5_5_3.shtml</c> 重定向至该页）。
/// </para>
/// <para>
/// <b>协议定位全靠 Path</b>（<c>plan_id</c> integer + <c>out_contract_code</c>），
/// Body 只有解约备注 ⇒ 与预签约「协议号进 Body」的形态相反，勿把参数放错位置。
/// </para>
/// <para>
/// 官方把 <see cref="ContractTerminationRemark"/> 标<b>必填</b>（string(256)）⇒
/// SDK 不代为补默认文案，宿主须自行给出可审计的解约原因（该值会回显在查询应答的解约信息里）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayContractTerminateRequest
{
    /// <summary>解约备注（<c>contract_termination_remark</c>，必填 string(256)）：解约原因的备注说明，如「签约信息有误，须重新签约」。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }
}

/// <summary>
/// 委托代扣解约应答：只回定位键，<b>不回协议状态</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884072"/>（3 行）。
/// 官方本应答没有 <c>contract_state</c> ⇒ 需要确认解约生效须再调查询接口看是否 <c>TERMINATED</c>，
/// 不能把本应答成功当作已解约的终局结论。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayContractTerminateResponse : WechatPayResponse
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：微信支付分配的商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>委托代扣协议模板 ID（<c>plan_id</c>，必填 integer）：回显 Path 值。</summary>
    [JsonPropertyName("plan_id")]
    public int? PlanId { get; set; }

    /// <summary>商户签约协议号（<c>out_contract_code</c>，必填 string(32)）：回显 Path 值。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }
}

/// <summary>
/// 电商免密合单支付（<c>POST /v3/ecommerce/combine-papay/transactions</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>
/// （2026-10-15 逐字段核验，全表 20 行；旧 wiki 面 <c>chapter5_5_4.shtml</c> 重定向至该页）。
/// </para>
/// <para>
/// <b>本支不需要 <c>Wechatpay-Serial</c></b>（免密扣款走协议，无敏感字段加密面）。
/// </para>
/// <para>
/// <b>合单要素与协议要素分两层</b>：<c>combine_appid</c>/<c>combine_mchid</c>/<c>combine_out_trade_no</c>
/// 描述合单发起方，<see cref="ContractId"/> 描述免密扣款用的协议，子单里再各自带 <c>sub_mchid</c> ⇒
/// 三层商户号（合单发起方、子单发起方、二级商户）不得混填，官方要求子单 <c>mchid</c> 必须与发起方 AppID 有绑定关系。
/// </para>
/// <para>
/// <b>子单上限 50</b>（官方在 <see cref="SubOrders"/> 上明确「最多支持子单条数：50」）。
/// 单号字符集为「32 个字符内，只能是数字、大小写字母 <c>-_|*@</c>」，且在同一商户号下唯一 ——
/// 注意这是<b>合单</b>口径（允许符号），与商家转账的「仅数字与大小写字母」不同。
/// </para>
/// <para>
/// <see cref="RiskInfo"/> 为<b>合单层</b>风控信息，子单另有 <see cref="EcommercePapaySubOrderInfo.SubRiskInfo"/>；
/// 两者格式统一见官方「风控信息字段规范」（<see href="https://pay.weixin.qq.com/doc/v3/partner/4014886364"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayCombineTransactionRequest
{
    /// <summary>合单商户 AppID（<c>combine_appid</c>，必填 string(32)）：合单发起方的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppid { get; set; }

    /// <summary>合单商户号（<c>combine_mchid</c>，必填 string(32)）：合单发起方商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchid { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）：合单支付总订单号，32 字符内、只能是数字与大小写字母及 <c>-_|*@</c>，同一商户号下唯一；后续查询与撤销都以它定位。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>委托代扣协议 ID（<c>contract_id</c>，必填 string(32)）：签约成功后微信返回的协议 ID（查询协议应答中的 <c>contract_id</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>子单信息（<c>sub_orders</c>，必填数组）：<b>最多支持 50 条子单</b>。</summary>
    [JsonPropertyName("sub_orders")]
    public IReadOnlyList<EcommercePapaySubOrderInfo>? SubOrders { get; set; }

    /// <summary>风控信息（<c>risk_info</c>，选填 string(255)）：合单发起方传入的实时风控数据，格式见官方风控信息字段规范。</summary>
    [JsonPropertyName("risk_info")]
    public string? RiskInfo { get; set; }
}

/// <summary>
/// 电商免密合单的子单（<c>sub_orders[]</c> 的请求侧形态）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>。
/// <see cref="Description"/> 官方给出命名口径「需传入应用市场上的 APP 名字-实际商品名称，例如：天天爱消除-游戏充值」
/// ⇒ 该值会出现在用户账单与商户平台交易记录中，不是自由文本。
/// <see cref="SettleInfo"/> 里的 <c>profit_sharing</c> 决定是否待分账，
/// <c>subsidy_amount</c> 官方注明「<c>profit_sharing</c> 为 true 时该金额才生效」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySubOrderInfo
{
    /// <summary>子单商户号（<c>mchid</c>，必填 string(32)）：子单发起方商户号，必须与发起方 AppID 有绑定关系。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）：查询 API 中原样返回，可作自定义参数。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填）：子单标价金额与币种。</summary>
    [JsonPropertyName("amount")]
    public EcommercePapaySubOrderRequestAmount? Amount { get; set; }

    /// <summary>子单商户订单号（<c>out_trade_no</c>，必填 string(32)）：32 字符内、只能是数字与大小写字母及 <c>-_|*@</c>，同一商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：由微信支付生成并下发的二级商户商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>二级商户 AppID（<c>sub_appid</c>，选填 string(32)）：二级商户的 AppID。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(128)）：官方要求「APP 名字-实际商品名称」形态，如「天天爱消除-游戏充值」。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填）：是否指定分账与补差金额。</summary>
    [JsonPropertyName("settle_info")]
    public EcommercePapaySettleInfo? SettleInfo { get; set; }

    /// <summary>子单风控信息（<c>sub_risk_info</c>，选填 string(255)）：子单维度的实时风控数据，格式见官方风控信息字段规范。</summary>
    [JsonPropertyName("sub_risk_info")]
    public string? SubRiskInfo { get; set; }
}

/// <summary>合单子单的请求侧金额（<c>sub_orders[].amount</c>）：只有标价金额与标价币种两项。</summary>
/// <remarks>
/// <b>与应答侧金额形态不同</b>（那边另有 <c>payer_amount</c>/<c>payer_currency</c>，
/// 见 <see cref="EcommercePapaySubOrderResponseAmount"/>）⇒ 请求/应答分建两类，避免把可选字段当必填提交。
/// 官方在本支把 <c>currency</c> 收窄为「仅支持人民币：CNY」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySubOrderRequestAmount
{
    /// <summary>标价金额（<c>total_amount</c>，必填 integer）：子单金额，单位为<b>分</b>。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>标价币种（<c>currency</c>，必填 string(8)）：官方注明<b>仅支持人民币 <c>CNY</c></b>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>合单子单的应答侧金额（<c>sub_orders[].amount</c>）：含现金支付金额与币种。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>：外层 <c>amount</c> 本身标<b>选填</b>，
/// 而内层 <c>total_amount</c>/<c>currency</c> 标必填 ⇒ 「父选填、子必填」，
/// 反序列化时先判 <c>amount</c> 是否为 null，再取内层值。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySubOrderResponseAmount
{
    /// <summary>标价金额（<c>total_amount</c>，必填 integer）：子单金额，单位为<b>分</b>。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>标价币种（<c>currency</c>，必填 string(8)）：符合 ISO 4217 的三位字母代码，人民币为 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>现金支付金额（<c>payer_amount</c>，选填 integer）：订单现金支付金额，单位为<b>分</b>。</summary>
    [JsonPropertyName("payer_amount")]
    public long? PayerAmount { get; set; }

    /// <summary>现金支付币种（<c>payer_currency</c>，选填 string(8)）：货币类型，默认人民币 <c>CNY</c>。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}

/// <summary>
/// 合单子单的结算信息（<c>sub_orders[].settle_info</c>）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>。
/// 官方：<c>subsidy_amount</c> 只在 <c>profit_sharing=true</c> 时生效 ⇒
/// 未指定分账却传补差金额不会起作用（也不会报错），须自行保证语义一致。
/// 补差本身另有独立端点族（<c>/v3/ecommerce/subsidies/*</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySettleInfo
{
    /// <summary>是否指定分账（<c>profit_sharing</c>，选填 boolean）：<c>true</c> 是、<c>false</c> 否。</summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

    /// <summary>补差金额（<c>subsidy_amount</c>，选填 integer）：单位为<b>分</b>；官方注明仅当 <see cref="ProfitSharing"/> 为 <c>true</c> 时该金额才生效。</summary>
    [JsonPropertyName("subsidy_amount")]
    public long? SubsidyAmount { get; set; }
}

/// <summary>
/// 免密合单支付/查询应答（21 行）—— <b>两支共用同一形态</b>：
/// 支付（<c>POST /v3/ecommerce/combine-papay/transactions</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>）与
/// 查询（<c>GET /v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>），
/// 2026-10-15 逐字段比对结果为<b>完全一致</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>没有合单级的状态字段</b>：官方本应答只有子单 <c>trade_state</c> ⇒
/// 整单是否完成须自行聚合各子单状态，且支付同步应答与查询应答同形，
/// 拿到 <c>NOTPAY</c>/<c>USERPAYING</c> 时应按查单轮询而不是判失败。
/// </para>
/// <para>
/// <b>没有合单号回写字段之外的 <c>contract_id</c></b>：协议 ID 只在请求侧出现 ⇒ 需要留存协议与订单的对应关系。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayCombineTransactionResponse : WechatPayResponse
{
    /// <summary>合单商户 AppID（<c>combine_appid</c>，必填 string(32)）：合单发起方的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppid { get; set; }

    /// <summary>合单商户号（<c>combine_mchid</c>，必填 string(32)）：合单发起方商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchid { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）：合单支付总订单号，查询与撤销的定位键。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>子单信息（<c>sub_orders</c>，必填数组）：逐子单的交易结果，最多 50 条。</summary>
    [JsonPropertyName("sub_orders")]
    public IReadOnlyList<EcommercePapaySubOrderResult>? SubOrders { get; set; }

    /// <summary>支付者（<c>combine_payer_info</c>，选填）：使用合单 AppID 获取的用户标识。</summary>
    [JsonPropertyName("combine_payer_info")]
    public EcommercePapayPayerInfo? CombinePayerInfo { get; set; }
}

/// <summary>
/// 免密合单子单的应答（<c>sub_orders[]</c>，11 项）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>（两页逐字段相同）。
/// <see cref="TradeType"/> 官方在本域只给 <c>PAP</c>（委托代扣，见 <see cref="EcommercePapayTradeTypes"/>）；
/// <see cref="TradeState"/> 见 <see cref="EcommercePapayTradeStates"/>。
/// <see cref="SuccessTime"/> 与 <see cref="BankType"/> 为选填 ⇒ 未支付成功时不返回。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySubOrderResult
{
    /// <summary>子单商户号（<c>mchid</c>，必填 string(32)）：子单发起方商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，必填 string(16)）：本域官方唯一取值 <c>PAP</c>（委托代扣）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>，必填 string(32)）：见 <see cref="EcommercePapayTradeStates"/>。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，选填 string(16)）：字符串类型的银行标识。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）：请求时传入的自定义数据原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，选填 string(32)）：rfc3339 格式。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>子单微信订单号（<c>transaction_id</c>，必填 string(32)）：微信支付订单号，后续分账/退款以它定位。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>子单商户订单号（<c>out_trade_no</c>，必填 string(32)）：商户系统内部订单号。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>二级商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>订单金额（<c>amount</c>，<b>选填</b>）：内含字段官方标必填，但父对象本身可缺省 ⇒ 取值前先判空。</summary>
    [JsonPropertyName("amount")]
    public EcommercePapaySubOrderResponseAmount? Amount { get; set; }
}

/// <summary>合单支付者信息（<c>combine_payer_info</c>）。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>：对象本身<b>选填</b>，
/// 内层 <c>openid</c> 标必填。此处 <c>openid</c> 长度是 <b>string(128)</b>，
/// 与协议查询应答里的 <c>openid</c>（string(64)）长度口径不同 ⇒ 官方原文如此，照抄不做归一化。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayPayerInfo
{
    /// <summary>用户标识（<c>openid</c>，必填 string(128)）：使用合单 AppID 获取的对应用户 OpenID。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }
}

/// <summary>
/// 免密合单撤销（<c>POST /v3/ecommerce/combine-papay/transactions/{combine_out_trade_no}/reverse</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012884126"/>
/// （2026-10-15 逐字段核验，全表 5 行；旧 wiki 面 <c>chapter5_5_5.shtml</c> 重定向至该页）。
/// </para>
/// <para>
/// <b><c>combine_out_trade_no</c> 走 Path、不进 Body</b>：Body 只有合单发起方 AppID 与待撤销子单列表；
/// 子单定位键是 <c>mchid</c> + <c>out_trade_no</c>（官方还带 <c>sub_mchid</c> 必填、<c>sub_appid</c> 选填）。
/// </para>
/// <para>
/// <b>本接口官方无应答参数表</b>（「无应答包体」形态）⇒ 接口声明为纯 <c>Task</c>，
/// 撤销结果必须以查询接口的子单状态为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapayReverseRequest
{
    /// <summary>合单商户 AppID（<c>combine_appid</c>，必填 string(32)）：合单发起方的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppid { get; set; }

    /// <summary>待撤销的子单信息（<c>sub_orders</c>，必填数组）：最多支持 50 条子单。</summary>
    [JsonPropertyName("sub_orders")]
    public IReadOnlyList<EcommercePapaySubOrderReverseInfo>? SubOrders { get; set; }
}

/// <summary>
/// 免密合单撤销的子单条目（<c>sub_orders[]</c>）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884126"/>。
/// 与下单支的 <see cref="EcommercePapaySubOrderInfo"/> <b>不共用</b>：撤销不需要金额、描述、结算信息，
/// 但 <c>sub_mchid</c> 在本页官方未标长度（原文仅 string）⇒ 仍按 string 建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePapaySubOrderReverseInfo
{
    /// <summary>子单商户号（<c>mchid</c>，必填 string(32)）：子单发起方商户号，必须与发起方 AppID 有绑定关系。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>子单商户订单号（<c>out_trade_no</c>，必填 string(32)）：32 字符内、只能是数字与大小写字母及 <c>-_|*@</c>，同一商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string）：由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>二级商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 二级商户账户余额（<b>实时余额与日终余额两支的共用应答</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：实时余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476690"/> 与日终余额
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476693"/> 的<b>应答表四行的字段名、类型、必填性
/// 逐行相同</b>（<c>sub_mchid</c> / <c>available_amount</c> / <c>pending_amount</c> / <c>account_type</c>），
/// 差异只在请求侧（日终支多一个必填 query <c>date</c>）。
/// <b>唯一例外是枚举文本</b>：<c>account_type</c> 一行的「可选取值」在实时页是四值、在日终页只有
/// <c>BASIC</c> / <c>DEPOSIT</c> 两值 ⇒ 共用类型成立，但取值口径按页分列（见 <see cref="EcommerceAccountTypes"/>）。
/// </para>
/// <para>
/// <b>金额单位是分、以 <c>long</c> 承载</b>：官方类型写 <c>integer</c>，而二级商户余额量级可越出 int32
/// （约 21.47 亿分 = 2147 万元），用 <c>int</c> 会在头部平台静默溢出。
/// </para>
/// <para>
/// <b><c>pending_amount</c> 与 <c>account_type</c> 官方标选填</b> ⇒ 字段缺省不等于 0 / 空串，
/// 不得据此判定「无不可用余额」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceBalanceResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>可用余额（<c>available_amount</c>，必填 integer，<b>单位分</b>）：官方注明「此余额可做提现操作」。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>不可用余额（<c>pending_amount</c>，<b>选填</b> integer，单位分）：字段缺省不等于 0。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }

    /// <summary>账户类型（<c>account_type</c>，<b>选填</b>）：见 <see cref="EcommerceAccountTypes"/> —— 实时页应答列四值、日终页应答只列 <c>BASIC</c>/<c>DEPOSIT</c> 两值。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }
}

/// <summary>
/// 申请二级商户预约提现（<c>POST /v3/ecommerce/fund/withdraw</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476652"/>
/// （2026-10-15 逐字段核验；更新时间 2024.12.19；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b><c>amount</c> 官方上限是「不能超过 8 亿元」</b>（= 8×10⁹ 分）⇒ 该值<b>必然越出 int32</b>，
/// 故本字段用 <c>long</c>；宿主若把「8 亿」按元理解再传 <c>800000000</c> 分只是 800 万元，是另一种误读。
/// </para>
/// <para>
/// <b><c>out_request_no</c> 限 32 且「必须是字母数字」</b>（官方原文）：与支付线多数域「商户单号 ≤64」的直觉不同，
/// 带 <c>-</c> / <c>_</c> 的编号会被拒。
/// </para>
/// <para>
/// <b><c>account_type</c> 三值集</b>：<c>BASIC</c> / <c>FEES</c> / <c>OPERATION</c>，<b>不含</b> <c>DEPOSIT</c>
/// （保证金账户不可由本接口提现）。
/// </para>
/// <para>
/// <b><c>notify_url</c> 是 HTTPS、不得携带 URL 参数</b>，且提现结果含「退票」这一非终态直觉的分支
/// （见 <see cref="EcommerceWithdrawStatuses.Refund"/>）⇒ 未配置回调的宿主必须自行轮询查询支闭环。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceWithdrawRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：由微信支付生成并下发。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>
    /// 商户预约提现单号（<c>out_request_no</c>，必填 string(32)）：官方要求<b>必须是字母数字</b>，
    /// 商户系统内唯一，重复提交等同一笔。
    /// </summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>提现金额（<c>amount</c>，必填 integer，<b>单位分</b>）：官方上限「不能超过 8 亿元」⇒ 以 <c>long</c> 承载。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>提现备注（<c>remark</c>，选填 string(56)）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>，选填 string(32)）：数字、字母、汉字；能否展示依赖银行系统支持。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>出款账户类型（<c>account_type</c>，选填）：<b>三值</b> <c>BASIC</c>/<c>FEES</c>/<c>OPERATION</c>，见 <see cref="EcommerceAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>
    /// 提现结果通知地址（<c>notify_url</c>，选填 string(256)）：必须公网可访问、<b>必须 HTTPS、不能携带 URL 参数</b>。
    /// </summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// 预约提现受理应答（<c>POST /v3/ecommerce/fund/withdraw</c>，<b>仅三字段</b>）。
/// </summary>
/// <remarks>
/// <b>受理 ≠ 到账</b>：官方应答表只有 <c>sub_mchid</c> / <c>withdraw_id</c> / <c>out_request_no</c>，
/// <b>没有</b> <c>status</c> ⇒ 拿到本应答不能记账，必须落库 <c>withdraw_id</c>（或 <c>out_request_no</c>）
/// 后经查询支读 <c>status</c> 闭环。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceWithdrawApplyResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>微信支付预约提现单号（<c>withdraw_id</c>，必填 string(128)）：查询申请状态的唯一标识，<b>必须落库</b>。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>商户预约提现单号（<c>out_request_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}

/// <summary>
/// 预约提现单详情（<b>两支查询的共用应答</b>：按 <c>out_request_no</c> 查 / 按 <c>withdraw_id</c> 查）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476656"/>
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476665"/> 的应答表<b>十五行逐字段相同</b>，
/// 差异只在定位键落在 path 还是 query。
/// </para>
/// <para>
/// <b>「必填」不等于「有值」</b>：<c>reason</c> 官方标 <c>true</c> 但正文写明「仅在提现失败、退票、关单时有值」，
/// <c>remark</c> / <c>bank_memo</c> 亦注明「未传入或输入不合法则该值为空」⇒ 这些字段的成功场景是空串，
/// 不要按「必填即非空」断言。
/// </para>
/// <para>
/// <b><c>account_number</c> 只有后四位</b>（string(4)）⇒ 只能用于展示与人工核对，不是账号指纹。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceWithdrawQueryResponse : WechatPayResponse
{
    /// <summary>收付通平台商户号（<c>sp_mchid</c>，必填 string）：即发起方电商平台自身的商户号。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>预约提现单状态（<c>status</c>，必填）：六值见 <see cref="EcommerceWithdrawStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>微信支付预约提现单号（<c>withdraw_id</c>，必填 string(128)）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>商户预约提现单号（<c>out_request_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>提现金额（<c>amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>提交预约时间（<c>create_time</c>，必填，rfc3339 字符串）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>提现状态更新时间（<c>update_time</c>，必填，rfc3339 字符串）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>失败原因（<c>reason</c>，string(255)）：<b>仅</b>失败 / 退票 / 关单时有值。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>提现备注（<c>remark</c>，string(255)）：发起时未传或不合法则为空。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>银行附言（<c>bank_memo</c>，string(32)）：发起时未传或不合法则为空。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>出款账户类型（<c>account_type</c>，必填）：应答侧为 <c>BASIC</c>/<c>FEES</c>/<c>OPERATION</c> 三值。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>入账银行账号<b>后四位</b>（<c>account_number</c>，string(4)）。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>入账银行（<c>account_bank</c>，string(10)）：银行简称。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>入账银行全称含支行（<c>bank_name</c>，<b>选填</b> string(128)）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }
}

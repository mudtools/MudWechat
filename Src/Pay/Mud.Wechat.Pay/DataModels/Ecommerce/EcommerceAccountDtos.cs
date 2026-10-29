// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 商户号注销并提现申请（<c>POST /v3/ecommerce/account/apply-cancel-withdraw</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>
/// （2026-10-15 逐字段核验；更新时间 2025.03.13；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>本支要求 <c>Wechatpay-Serial</c> 请求头</b>（官方 Header 参数表把它列为必填）：
/// <see cref="EcommerceBankAccountInfo.AccountName"/>、<see cref="EcommerceBankAccountInfo.AccountNumber"/>、
/// <see cref="EcommerceCancelIdentityInfo.IdentificationName"/>、
/// <see cref="EcommerceCancelIdentityInfo.IdentificationNo"/> 四个字段<b>必须传密文</b>
/// （官方类型一律 string(2048)，即加密后 Base64 的长度），明文直传会被拒且属违规。
/// </para>
/// <para>
/// <b><c>withdraw</c> 是「是否提取资金」的开关，不是状态</b>：填 <c>APPLY_WITHDRAW</c> 才需要
/// <see cref="PayeeInfo"/>；只注销不提款时留空。官方原文提示该字段是给「被平台不收不付管控的二级商户」用的。
/// </para>
/// <para>
/// <b><c>out_request_no</c> 限 32 且「必须仅包含大小写字母与数字」</b>（官方原文）⇒ 与预约提现同一口径，
/// 带分隔符的单号会被拒。
/// </para>
/// <para>
/// <b>受理 ≠ 注销生效</b>：应答只有 <c>applyment_id</c> / <c>out_request_no</c> 两字段、
/// <b>没有状态</b> ⇒ 必须落库申请单号后经查询支读 <c>cancel_state</c>（九值）闭环，
/// 其中 <c>WAITING_MERCHANT_CONFIRM</c> 还要商户超级管理员确认（见
/// <see cref="EcommerceCancelWithdrawQueryResponse.ConfirmCancel"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelWithdrawApplyRequest
{
    /// <summary>申请注销的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户注销申请单号（<c>out_request_no</c>，必填 string(32)）：服务商维度唯一，<b>仅大小写字母与数字</b>。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>是否提取资金（<c>withdraw</c>，选填）：<c>NOT_APPLY_WITHDRAW</c> / <c>APPLY_WITHDRAW</c>，见 <see cref="EcommerceCancelWithdrawFlags"/>。</summary>
    [JsonPropertyName("withdraw")]
    public string? Withdraw { get; set; }

    /// <summary>收款账号信息（<c>payee_info</c>，选填）：仅当 <see cref="Withdraw"/> 为 <c>APPLY_WITHDRAW</c> 时填写。</summary>
    [JsonPropertyName("payee_info")]
    public EcommercePayeeInfo? PayeeInfo { get; set; }

    /// <summary>付款申请材料（<c>proof_medias</c>，选填数组）：企业证照注吊撤、事业单位/政府机关/社会组织等场景必填。</summary>
    [JsonPropertyName("proof_medias")]
    public IReadOnlyList<EcommerceProofMedia>? ProofMedias { get; set; }

    /// <summary>其他补充材料（<c>additional_materials</c>，选填 string 数组）：元素是图片上传通道返回的 <c>media_id</c>。</summary>
    [JsonPropertyName("additional_materials")]
    public IReadOnlyList<string>? AdditionalMaterials { get; set; }

    /// <summary>备注（<c>remark</c>，选填 string(32)）：展示在收款银行系统中，供平台与二级商户对账。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>注销提现的收款账号信息（<c>payee_info</c>）。</summary>
/// <remarks>
/// <b><c>account_type</c> 是「对公/对私」而不是余额账户类型</b>：本域同名概念有三套词汇
/// （<c>BASIC</c> 系列、<c>BASIC_ACCOUNT</c> 系列、本处 <c>ACCOUNT_TYPE_CORPORATE</c> 系列），
/// 逐页核对不可互换，见 <see cref="EcommercePayeeAccountTypes"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommercePayeeInfo
{
    /// <summary>账户类型（<c>account_type</c>，必填）：对公/对私，见 <see cref="EcommercePayeeAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>银行账户（<c>bank_account_info</c>，必填）。</summary>
    [JsonPropertyName("bank_account_info")]
    public EcommerceBankAccountInfo? BankAccountInfo { get; set; }

    /// <summary>证件信息（<c>identity_info</c>，选填）：<b>付款到对私银行卡时必填</b>。</summary>
    [JsonPropertyName("identity_info")]
    public EcommerceCancelIdentityInfo? IdentityInfo { get; set; }
}

/// <summary>注销提现的银行账户信息（<c>payee_info.bank_account_info</c>）。</summary>
/// <remarks>
/// <b><c>bank_branch_id</c> 与 <c>bank_branch_name</c> 二选一</b>（官方原文：由「是否需要填写支行」判定），
/// 两者都填或都不填都属非法状态。<b>对私账户的开户名称必须与法人身份证姓名一致；对公账户的开户名称必须与
/// 营业执照「商户名称」一致</b>，不一致须走补充材料。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceBankAccountInfo
{
    /// <summary>开户名称（<c>account_name</c>，必填 string(2048)）：<b>须传密文</b>（<c>Wechatpay-Serial</c> 必填）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，必填 string(128)）：取值须来自「查询支持个人业务的银行列表」对照表。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(128)）：与 <see cref="BankBranchName"/> 二选一。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称含支行（<c>bank_branch_name</c>，选填 string(128)）：与 <see cref="BankBranchId"/> 二选一。</summary>
    [JsonPropertyName("bank_branch_name")]
    public string? BankBranchName { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(2048)）：<b>须传密文</b>。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}

/// <summary>对私收款账户的证件信息（<c>payee_info.identity_info</c>）。</summary>
/// <remarks>
/// <b>本族的 <c>id_doc_type</c> 取值集合与进件族不同形</b>：注销页用
/// <c>IDENTIFICATION_TYPE_ID_CARD</c>（身份证）、<c>…_HONGKONG_PASSPORT</c>，
/// 而进件页用 <c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c>、<c>…_HONGKONG</c> ⇒
/// 照抄进件枚举值会被拒，见 <see cref="EcommerceCancelIdDocTypes"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelIdentityInfo
{
    /// <summary>证件类型（<c>id_doc_type</c>，选填；对私场景必填）：见 <see cref="EcommerceCancelIdDocTypes"/>。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>证件姓名（<c>identification_name</c>，string(2048)）：<b>须传密文</b>。</summary>
    [JsonPropertyName("identification_name")]
    public string? IdentificationName { get; set; }

    /// <summary>证件号码（<c>identification_no</c>，string(2048)）：<b>须传密文</b>；身份证为 15 位数字或 17 位数字 + 1 位数字|X。</summary>
    [JsonPropertyName("identification_no")]
    public string? IdentificationNo { get; set; }
}

/// <summary>付款申请材料（<c>proof_medias[]</c> 的元素形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceProofMedia
{
    /// <summary>材料类型（<c>proof_media_type</c>，必填）：本页给出 <c>WITHDRAWAL_APPLICATION</c>（注销后可用余额提取申请书）。</summary>
    [JsonPropertyName("proof_media_type")]
    public string? ProofMediaType { get; set; }

    /// <summary>证明材料照片（<c>proof_media</c>，必填 string(1024)）：图片上传通道返回的 <c>media_id</c>，<b>不是</b>图片 URL 或 base64。</summary>
    [JsonPropertyName("proof_media")]
    public string? ProofMedia { get; set; }
}

/// <summary>
/// 注销提现申请受理应答（<c>POST /v3/ecommerce/account/apply-cancel-withdraw</c>）。
/// </summary>
/// <remarks>官方把两个字段都标<b>选填</b> ⇒ 受理成功但字段缺省时无法定位单据，须以请求侧 <c>out_request_no</c> 兜底。</remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelWithdrawApplyResponse : WechatPayResponse
{
    /// <summary>微信支付注销提现申请单号（<c>applyment_id</c>，<b>选填</b> string(32)）：查询申请状态的唯一标识。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>商户注销申请单号（<c>out_request_no</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}

/// <summary>
/// 注销提现申请单详情（<b>两支查询的共用应答</b>：按 <c>out_request_no</c> 查 / 按 <c>applyment_id</c> 查）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892765"/> 的应答表<b>十八行字段名、类型、必填性逐行相同</b>
/// （含 <c>cancel_state</c> 九值、<c>withdraw_state</c> 三值、两个账户枚举列全部一致）⇒ 共用方法与 DTO。
/// </para>
/// <para>
/// <b><c>cancel_state</c> 在本域有两个互不相干的值集</b>：注销提现族是这里的<b>九值</b>，
/// 而电商子商户注销申请族（<c>/v3/ecommerce/account/cancel-applications</c>）是<b>三值</b>
/// （<c>REVIEWING</c>/<c>REJECTED</c>/<c>CANCEL_SUCCESS</c>）⇒
/// 别把两族写进同一个判定器，见 <see cref="EcommerceCancelStates"/> 与 <see cref="EcommerceCancelApplicationStates"/>。
/// </para>
/// <para>
/// <b>状态与资金状态是两条线</b>：<c>cancel_state=FUND_PROCESSING</c> 时才有 <c>withdraw_state</c> 与
/// <c>account_withdraw_result</c>；<b>单据完成（<c>FINISH</c>）不等于每个账户都付款成功</b>，
/// 逐账户看 <see cref="EcommerceAccountWithdrawResult.PayState"/>（含 <c>BANK_REFUNDED</c> 退票）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelWithdrawQueryResponse : WechatPayResponse
{
    /// <summary>微信支付注销提现申请单号（<c>applyment_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>商户注销申请单号（<c>out_request_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>注销提现申请单状态（<c>cancel_state</c>，必填）：九值见 <see cref="EcommerceCancelStates"/>。</summary>
    [JsonPropertyName("cancel_state")]
    public string? CancelState { get; set; }

    /// <summary>状态文字描述（<c>cancel_state_description</c>，必填 string(256)）：给人看的，判状态只看 <see cref="CancelState"/>。</summary>
    [JsonPropertyName("cancel_state_description")]
    public string? CancelStateDescription { get; set; }

    /// <summary>是否提取资金（<c>withdraw</c>，选填）：回显提交时的标记，见 <see cref="EcommerceCancelWithdrawFlags"/>。</summary>
    [JsonPropertyName("withdraw")]
    public string? Withdraw { get; set; }

    /// <summary>提现状态（<c>withdraw_state</c>，选填）：仅当申请单进入「资金处理中」时返回，见 <see cref="EcommerceCancelWithdrawStates"/>。</summary>
    [JsonPropertyName("withdraw_state")]
    public string? WithdrawState { get; set; }

    /// <summary>提现状态描述（<c>withdraw_state_description</c>，选填 string(256)）。</summary>
    [JsonPropertyName("withdraw_state_description")]
    public string? WithdrawStateDescription { get; set; }

    /// <summary>账户提现结果（<c>account_withdraw_result</c>，选填数组）：<b>逐账户</b>的付款状态，注销提现会全额清退所有账户。</summary>
    [JsonPropertyName("account_withdraw_result")]
    public IReadOnlyList<EcommerceAccountWithdrawResult>? AccountWithdrawResult { get; set; }

    /// <summary>最后更新时间（<c>modify_time</c>，选填 string(32)，rfc3339 字符串）。</summary>
    [JsonPropertyName("modify_time")]
    public string? ModifyTime { get; set; }

    /// <summary>申请注销的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>账户信息（<c>account_info</c>，选填数组）：已开通账户及其余额，受理及后续状态均返回。</summary>
    [JsonPropertyName("account_info")]
    public IReadOnlyList<EcommerceAccountAmountInfo>? AccountInfo { get; set; }

    /// <summary>商户确认引导信息（<c>confirm_cancel</c>，选填）：<b>仅</b>状态为 <c>WAITING_MERCHANT_CONFIRM</c> 时返回。</summary>
    [JsonPropertyName("confirm_cancel")]
    public EcommerceConfirmCancelInfo? ConfirmCancel { get; set; }
}

/// <summary>单个账户的提现付款结果（<c>account_withdraw_result[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceAccountWithdrawResult
{
    /// <summary>出款子账户类型（<c>out_account_type</c>，必填）：四值见 <see cref="EcommerceOutAccountTypes"/>。</summary>
    [JsonPropertyName("out_account_type")]
    public string? OutAccountType { get; set; }

    /// <summary>付款状态（<c>pay_state</c>，必填）：四值见 <see cref="EcommercePayStates"/>（含 <c>BANK_REFUNDED</c> 退票）。</summary>
    [JsonPropertyName("pay_state")]
    public string? PayState { get; set; }

    /// <summary>付款状态描述（<c>state_description</c>，必填 string(256)）。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }
}

/// <summary>单个已开通账户的类型与余额（<c>account_info[]</c>，注销族与可注销校验族共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceAccountAmountInfo
{
    /// <summary>出款子账户类型（<c>out_account_type</c>，必填）：四值见 <see cref="EcommerceOutAccountTypes"/>，<b>仅返回已开通的账户</b>。</summary>
    [JsonPropertyName("out_account_type")]
    public string? OutAccountType { get; set; }

    /// <summary>账户金额（<c>amount</c>，必填 integer，<b>单位分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>引导商户超级管理员确认注销的信息（<c>confirm_cancel</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceConfirmCancelInfo
{
    /// <summary>确认注销的页面 URL（<c>confirm_cancel_url</c>，选填 string(1024)）：官方建议转成二维码让商户员工用微信扫码确认；平台也可改用代商户确认接口。</summary>
    [JsonPropertyName("confirm_cancel_url")]
    public string? ConfirmCancelUrl { get; set; }
}

/// <summary>
/// 平台代商户确认注销（<c>POST /v3/ecommerce/account/apply-cancel-withdraw/confirm</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013892772"/>
/// （2026-10-15 逐字段核验；更新时间 2025.03.13；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>定位键二选一</b>：<c>applyment_id</c> 与 <c>out_request_no</c> 官方都标<b>选填</b>，
/// 含义是「两者至少给一个」⇒ 两个都留空不是「不限定」而是非法请求。
/// </para>
/// <para>
/// <b>三个必填的留痕字段</b>：<c>operator_identification</c>(string(100))、<c>confirm_time</c>(string(32))、
/// <c>cancel_contract_version</c>(string(32)) —— 本支是「平台代替商户做法律确认」，
/// 操作人身份与协议版本号必须真实可追溯，<b>不得用占位常量</b>糊过去。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelWithdrawConfirmRequest
{
    /// <summary>微信支付注销提现申请单号（<c>applyment_id</c>，<b>选填</b> string(32)）：与 <see cref="OutRequestNo"/> 二选一。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>商户注销申请单号（<c>out_request_no</c>，<b>选填</b> string(32)）：与 <see cref="ApplymentId"/> 二选一。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>申请注销的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>操作人身份标识（<c>operator_identification</c>，必填 string(100)）：代商户确认的操作人留痕。</summary>
    [JsonPropertyName("operator_identification")]
    public string? OperatorIdentification { get; set; }

    /// <summary>确认时间（<c>confirm_time</c>，必填 string(32)）。</summary>
    [JsonPropertyName("confirm_time")]
    public string? ConfirmTime { get; set; }

    /// <summary>注销协议版本号（<c>cancel_contract_version</c>，必填 string(32)）：商户所确认的协议版本，须与 <see cref="EcommerceConfirmCancelInfo.ConfirmCancelUrl"/> 页面版本一致。</summary>
    [JsonPropertyName("cancel_contract_version")]
    public string? CancelContractVersion { get; set; }
}

/// <summary>代商户确认注销应答（回显三个定位字段，<b>不含状态</b>）。</summary>
/// <remarks>确认成功不代表注销完成，仍须查询支读 <c>cancel_state</c> 推进到 <c>FINISH</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelWithdrawConfirmResponse : WechatPayResponse
{
    /// <summary>微信支付注销提现申请单号（<c>applyment_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>申请注销的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户注销申请单号（<c>out_request_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}

/// <summary>
/// 注销提现的前置校验结果（<c>GET /v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{sub_mchid}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>
/// （2026-10-15 逐字段核验；支持商户：【普通服务商】—— <b>本域唯一一支服务商面</b>）。
/// </para>
/// <para>
/// <b><c>validate_result</c> 为 <c>NOT_ALLOW_CANCEL_WITHDRAW</c> 不是错误</b>：HTTP 仍是 200，
/// 原因在 <see cref="BlockReasons"/> 里逐条给出 ⇒ 不要用「成功即已阻塞」的直觉，也不要把不可注销当成接口失败重试。
/// </para>
/// <para>
/// <b><c>merchant_state</c> 与 <c>validate_result</c> 是两个维度</b>：前者说「是不是已经注销过」，
/// 后者说「现在能不能发起」。<c>HAS_BEEN_CANCELLED</c> 时 <c>validate_result</c> 仍会给出值，须自行判读。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceValidateCancelResponse : WechatPayResponse
{
    /// <summary>被校验的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户状态（<c>merchant_state</c>，必填）：<c>NORMAL</c> / <c>HAS_BEEN_CANCELLED</c>，见 <see cref="EcommerceMerchantStates"/>。</summary>
    [JsonPropertyName("merchant_state")]
    public string? MerchantState { get; set; }

    /// <summary>校验结论（<c>validate_result</c>，必填）：见 <see cref="EcommerceValidateResults"/>。</summary>
    [JsonPropertyName("validate_result")]
    public string? ValidateResult { get; set; }

    /// <summary>账户信息（<c>account_info</c>，选填数组）：已开通账户与余额，与查询族同型。</summary>
    [JsonPropertyName("account_info")]
    public IReadOnlyList<EcommerceAccountAmountInfo>? AccountInfo { get; set; }

    /// <summary>阻塞原因（<c>block_reasons</c>，选填数组）：<b>可注销时整字段缺省</b>，不是空数组。</summary>
    [JsonPropertyName("block_reasons")]
    public IReadOnlyList<EcommerceBlockReason>? BlockReasons { get; set; }
}

/// <summary>不可注销的原因（<c>block_reasons[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceBlockReason
{
    /// <summary>原因类型（<c>type</c>，选填）：四值见 <see cref="EcommerceBlockReasonTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>原因描述（<c>description</c>，选填 string(512)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 电商子商户注销申请（<c>POST /v3/ecommerce/account/cancel-applications</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/>
/// （2026-10-15 逐字段核验；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>与注销提现族是两套契约</b>：本族走「上传材料 → 提交申请 → 审核」，定位键是 <c>out_apply_no</c>
/// （不是 <c>out_request_no</c>），状态只有三值（见 <see cref="EcommerceCancelApplicationStates"/>），
/// 且<b>不涉及提款</b>。两支共用 <c>cancel_state</c> 字段名而值集不同 —— 是本域最易写错的一处。
/// </para>
/// <para>
/// <b><c>application_media_id</c> 必须来自本族的媒体上传通道</b>（<c>POST /v3/ecommerce/account/cancel-applications/media</c>，
/// multipart ⇒ 手工通道 <c>IWechatPayEcommerceMediaService</c>），<b>不是</b>通用媒体上传 <c>/v3/merchant/media/upload</c> 的产物。
/// </para>
/// <para>
/// <b>两个枚举值官方已声明废弃</b>：<c>SP_MERCHANT_APPLICATION</c> / <c>SUB_MERCHANT_APPLICATION</c> /
/// <c>MISSING_OFFICIAL_SEAL_LETTER</c> ⇒ 只留 <c>SP_CANCEL_ACCOUNT_APPLICATION</c> 与
/// <c>SUB_CANCEL_ACCOUNT_APPLICATION</c>（官方原文「此枚举值已废弃，请使用新字段」），SDK 不为其提供常量。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelApplicationRequest
{
    /// <summary>申请注销的二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户注销申请单号（<c>out_apply_no</c>，必填 string(32)）：注意字段名与注销提现族的 <c>out_request_no</c> 不同。</summary>
    [JsonPropertyName("out_apply_no")]
    public string? OutApplyNo { get; set; }

    /// <summary>申请材料列表（<c>application_info</c>，必填数组）：每项是一份材料的类型 + <c>media_id</c>。</summary>
    [JsonPropertyName("application_info")]
    public IReadOnlyList<EcommerceCancelApplicationInfo>? ApplicationInfo { get; set; }
}

/// <summary>注销申请的一项材料（<c>application_info[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelApplicationInfo
{
    /// <summary>材料类型（<c>application_type</c>，必填）：见 <see cref="EcommerceCancelApplicationTypes"/>（官方另有三个已废弃值，SDK 不提供）。</summary>
    [JsonPropertyName("application_type")]
    public string? ApplicationType { get; set; }

    /// <summary>材料图片 ID（<c>application_media_id</c>，必填 string(512)）：本族媒体上传通道返回的 <c>media_id</c>。</summary>
    [JsonPropertyName("application_media_id")]
    public string? ApplicationMediaId { get; set; }
}

/// <summary>
/// 注销申请状态（<b>两支的共用应答</b>：提交返回 / 按 <c>out_apply_no</c> 查询）。
/// </summary>
/// <remarks>
/// <b>共用依据</b>（2026-10-15 逐字段比对）：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/>
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476223"/> 的应答表<b>五行逐字段相同</b>
/// 且<b>五个字段全部标选填</b> ⇒ 提交支的应答可能只回 <c>out_apply_no</c>，判进展必须走查询支。
/// <b><c>reject_reason</c> 只在 <c>cancel_state=REJECTED</c> 时有值</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceCancelApplicationResponse : WechatPayResponse
{
    /// <summary>商户注销申请单号（<c>out_apply_no</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("out_apply_no")]
    public string? OutApplyNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，<b>选填</b> string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，<b>选填</b> string(512)）：仅 <c>REJECTED</c> 时返回。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>注销申请状态（<c>cancel_state</c>，<b>选填</b>）：三值见 <see cref="EcommerceCancelApplicationStates"/>。</summary>
    [JsonPropertyName("cancel_state")]
    public string? CancelState { get; set; }

    /// <summary>更新时间（<c>update_time</c>，<b>选填</b> string(32)，rfc3339 字符串）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 个人收款方授权结果（<b>两支的共用应答形态之基础型</b>：按 <c>openid</c> 查 / 按 <c>out_contract_code</c> 查）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两页应答表不同形</b>（2026-10-15 逐字段比对）：按 <c>openid</c> 查
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012739528"/>）只回三字段
/// （<c>individual_auth_id</c> / <c>operation_time</c> / <c>auth_state</c>），
/// 按 <c>out_contract_code</c> 查（<see href="https://pay.weixin.qq.com/doc/v3/partner/4015851492"/>）
/// 回八字段、另有 <c>real_name_auth_result</c> 子结构与 <c>signed_time</c> / <c>individual_openid</c>
/// ⇒ <b>分建两型</b>（<see cref="EcommerceIndividualContractByOpenIdResponse"/> 与
/// <see cref="EcommerceIndividualContractResponse"/>），不做「超集共用」。
/// </para>
/// <para>
/// <b><c>access_token</c> 是凭据</b>：见 <see cref="EcommerceRealNameAuthResult.AccessToken"/>，
/// 不得写日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualContractByOpenIdResponse : WechatPayResponse
{
    /// <summary>个人收款方授权编号（<c>individual_auth_id</c>，必填 string）。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>授权操作时间（<c>operation_time</c>，必填，rfc3339 字符串）。</summary>
    [JsonPropertyName("operation_time")]
    public string? OperationTime { get; set; }

    /// <summary>授权状态（<c>auth_state</c>，必填）：<c>AUTHORIZED</c> / <c>UNAUTHORIZED</c>，见 <see cref="EcommerceAuthStates"/>。</summary>
    [JsonPropertyName("auth_state")]
    public string? AuthState { get; set; }
}

/// <summary>个人收款方签约详情（<c>GET /v3/ecommerce/individual-contracts/out-contract-code/{out_contract_code}</c>）。</summary>
/// <remarks>出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015851492"/>（2026-10-15 逐字段核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceIndividualContractResponse : WechatPayResponse
{
    /// <summary>个人收款方授权编号（<c>individual_auth_id</c>，<b>选填</b> string(128)）：未签约时缺省。</summary>
    [JsonPropertyName("individual_auth_id")]
    public string? IndividualAuthId { get; set; }

    /// <summary>签约时间（<c>signed_time</c>，必填，rfc3339 字符串）。</summary>
    [JsonPropertyName("signed_time")]
    public string? SignedTime { get; set; }

    /// <summary>授权状态（<c>auth_state</c>，必填）：见 <see cref="EcommerceAuthStates"/>。</summary>
    [JsonPropertyName("auth_state")]
    public string? AuthState { get; set; }

    /// <summary>个人收款方的 openid（<c>individual_openid</c>，<b>选填</b>）。</summary>
    [JsonPropertyName("individual_openid")]
    public string? IndividualOpenId { get; set; }

    /// <summary>实名认证结果（<c>real_name_auth_result</c>，<b>选填</b>）：仅完成实名认证时返回。</summary>
    [JsonPropertyName("real_name_auth_result")]
    public EcommerceRealNameAuthResult? RealNameAuthResult { get; set; }
}

/// <summary>实名认证结果（<c>real_name_auth_result</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceRealNameAuthResult
{
    /// <summary>授权范围（<c>scope</c>，必填）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// 访问令牌（<c>access_token</c>，必填）：<b>凭据</b> —— 不得进日志、遥测或异常消息，
    /// 有效期见 <see cref="AccessTokenExpireTime"/>。
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>令牌过期时间（<c>access_token_expire_time</c>，必填，rfc3339 字符串）。</summary>
    [JsonPropertyName("access_token_expire_time")]
    public string? AccessTokenExpireTime { get; set; }
}

/// <summary>
/// 主体名下子商户列表项（<c>GET /v3/ecommerce/subject-sub-merchants</c> 的 <c>data[]</c> 元素）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4033742105"/>（2026-10-15 逐字段核验）。
/// <b>本支是分页形态</b>：<c>offset</c> / <c>limit</c> / <c>total_count</c> 与 <c>data</c> 同层回显，
/// 且应答的 <c>offset</c>/<c>limit</c> 是<b>必填</b> ⇒ 不要只按 <c>data</c> 是否为空判结束，须比 <c>total_count</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceSubjectSubMerchantItem
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户简称（<c>merchant_shortname</c>，必填 string(64)）。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortName { get; set; }

    /// <summary>签约时间（<c>sign_time</c>，必填 string(32)，rfc3339 字符串）。</summary>
    [JsonPropertyName("sign_time")]
    public string? SignTime { get; set; }

    /// <summary>主体类型（<c>organization_type</c>，必填）：见 <see cref="EcommerceSubjectOrganizationTypes"/>。</summary>
    [JsonPropertyName("organization_type")]
    public string? OrganizationType { get; set; }
}

/// <summary>主体名下子商户分页应答（<c>GET /v3/ecommerce/subject-sub-merchants</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceSubjectSubMerchantsResponse : WechatPayResponse
{
    /// <summary>子商户列表（<c>data</c>，必填数组）。</summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<EcommerceSubjectSubMerchantItem>? Data { get; set; }

    /// <summary>本次请求的偏移量（<c>offset</c>，必填 integer）：官方回显请求参数。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>本次请求的每页数量（<c>limit</c>，必填 integer）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>总记录数（<c>total_count</c>，必填 integer）：翻页终止的权威依据。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

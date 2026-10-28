// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.BankTransfer;

/// <summary>
/// 从业机构（银行）「发起批量转账」请求（<b>POST /v3/bank-transfer/batches</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465889"/>，2026-10-14 逐字段核验。</para>
/// <para><b>五层商户标识体系</b>（守卫 BT-B3 锁定，与服务商面 <c>sp_/sub_</c> 两套、还款面
/// <c>combine_/sub_orders[]</c> 一套互不相通）：
/// <c>platform_mchid</c> 服务平台 + <c>specialized_payment_mchid</c> 付款企业 +
/// <c>sponsor_mchid</c> 实际出资方（须为前两者之一）+ <c>user_authorized_mchid</c> /
/// <c>user_authorized_appid</c> 用户信息授权方（须为前两者之一，且 AppID 与该商户号完成 MA 绑定）。
/// 五节<b>全部必填</b>且语义交叉 ⇒ 无法由配置推导，只能调用期给出。</para>
/// <para><b>与服务商面必填性相反的两处</b>：本面 <c>transfer_detail_list</c> 官方标<b>选填</b>
/// （服务商面必填）、明细 <c>user_name</c> 标<b>必填</b>（服务商面选填）⇒ 字段名相同不代表形态相同。</para>
/// <para><b>上限与服务商面不同</b>：<c>total_count</c>「一个转账批次单最多发起<b>三千</b>笔」（服务商面一千），
/// 且本面字段名为 <c>total_count</c> 而非 <c>total_num</c>。</para>
/// <para><b>敏感字段加密</b>：<c>user_name</c> / <c>user_id_card</c> 须加密（<c>Wechatpay-Serial</c> 请求头必填）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferBatchCreateRequest
{
    /// <summary>服务平台商户号（<c>platform_mchid</c>，必填 string(32)）：提供信息服务、财税服务等平台的商户号。</summary>
    [JsonPropertyName("platform_mchid")]
    public string? PlatformMchId { get; set; }

    /// <summary>付款企业商户号（<c>specialized_payment_mchid</c>，必填 string(32)）：承载付款场景与付款业务的商户号。</summary>
    [JsonPropertyName("specialized_payment_mchid")]
    public string? SpecializedPaymentMchId { get; set; }

    /// <summary>实际出资商户号（<c>sponsor_mchid</c>，必填 string(32)）：官方限定须在付款企业商户号、服务平台商户号中二选一。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchId { get; set; }

    /// <summary>用户授权商户号（<c>user_authorized_mchid</c>，必填 string(32)）：官方限定须在付款企业商户号、服务平台商户号中二选一。</summary>
    [JsonPropertyName("user_authorized_mchid")]
    public string? UserAuthorizedMchId { get; set; }

    /// <summary>用户授权 AppID（<c>user_authorized_appid</c>，必填 string(32)）：须与 <c>user_authorized_mchid</c> 完成 MA 绑定。</summary>
    [JsonPropertyName("user_authorized_appid")]
    public string? UserAuthorizedAppId { get; set; }

    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）：银行服务商系统内唯一，<b>发起后不可更换</b>。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>批次名称（<c>batch_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>，必填 string(32)）：转账说明，UTF8 编码。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>附加信息（<c>attach_remark</c>，选填 string(32)）：官方建议填技术服务商 / 付款专用商户的业务单号用于对账。</summary>
    [JsonPropertyName("attach_remark")]
    public string? AttachRemark { get; set; }

    /// <summary>转账总金额（<c>total_amount</c>，必填 integer）：单位为<b>分</b>，须等于明细金额之和。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>转账总笔数（<c>total_count</c>，必填 integer）：官方上限<b>三千</b>笔，本面字段名为 <c>total_count</c>。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>转账场景（<c>transfer_scene</c>，<b>本面必填</b>，2 值）：见 <see cref="BankTransferScenes"/>。</summary>
    [JsonPropertyName("transfer_scene")]
    public string? TransferScene { get; set; }

    /// <summary>批量转账用途（<c>transfer_purpose</c>，<b>本面必填</b>，6 值）：见 <see cref="BankTransferPurposes"/>。</summary>
    [JsonPropertyName("transfer_purpose")]
    public string? TransferPurpose { get; set; }

    /// <summary>转账明细列表（<c>transfer_detail_list</c>，<b>本面选填</b> array）：见 <see cref="BankTransferDetailInput"/>。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<BankTransferDetailInput>? TransferDetailList { get; set; }
}

/// <summary>
/// 转账明细入参（官方构件名 <c>TransferDetailInput</c>，从业机构（银行）面）。
/// </summary>
/// <remarks>
/// <para><b>与服务商面同名构件的必填性差异</b>：本面 <c>user_name</c> <b>必填</b>（务工卡须实名校验），
/// 服务商面选填 ⇒ 两域各建一套类型（字段名照抄官方原文，守卫 BT-B4 比对两域字段集）。</para>
/// <para><c>user_name</c> / <c>user_id_card</c> 为<b>密文</b>字段（string(1024)）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferDetailInput
{
    /// <summary>银行明细单号（<c>out_detail_no</c>，必填 string(32)）：批次单下唯一。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为<b>分</b>。</summary>
    [JsonPropertyName("transfer_amount")]
    public int? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：微信用户会收到该备注。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）：归属由用户信息授权方决定。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>，<b>本面必填</b> string(1024)）：<b>密文</b>，须加密后传入。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>收款用户证件号（<c>user_id_card</c>，选填 string(1024)）：<b>密文</b>。</summary>
    [JsonPropertyName("user_id_card")]
    public string? UserIdCard { get; set; }
}

/// <summary>
/// 从业机构（银行）「发起批量转账」应答（200 报文，仅三字段）。
/// </summary>
/// <remarks>
/// <b>受理 ≠ 转账成功</b>：批次状态须经查单轮询；官方明令批量转账一旦发起<b>不允许撤销</b>，
/// 报错时<b>不要更换单号重试</b>。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465889"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferBatchCreateResponse : WechatPayResponse
{
    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）：原样回显入参。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信支付批次单号（<c>batch_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>批次受理时间（<c>create_time</c>，必填 string(32)）：rfc3339，保持 <c>string</c>。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>
/// 从业机构（银行）「查询批次单」应答（两支按 <c>batch_id</c> / <c>out_batch_no</c> 定位的 GET <b>共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：微信支付批次单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465891"/>、
/// 银行批次单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465903"/>，
/// 2026-10-14 逐页比对字段表<b>完全相同</b> ⇒ 共用一型（守卫 BT-B6）。</para>
/// <para><b>应答多出 <c>bank_sp_mchid</c></b>（银行服务商号）—— 请求五层标识里没有该节，
/// 它是微信侧回给的银行服务商号 ⇒ 应答字段表是「请求五层 + bank_sp_mchid」共六层。</para>
/// <para><b>计数三名与服务商面全部不同名</b>：<c>total_count</c> / <c>success_count</c> / <c>fail_count</c>
/// 对服务商面的 <c>total_num</c> / <c>success_num</c> / <c>fail_num</c>；批次状态字段本面为
/// <c>batch_state</c>、服务商面为 <c>batch_status</c> ⇒ 两域类型不得共用。</para>
/// <para><b>本域明细概要多出 <c>fail_reason</c></b>（服务商面概要只有三节）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferBatchQueryResponse : WechatPayResponse
{
    /// <summary>银行服务商号（<c>bank_sp_mchid</c>，必填 string(32)）：微信支付分配的银行服务商号，仅应答侧出现。</summary>
    [JsonPropertyName("bank_sp_mchid")]
    public string? BankSpMchId { get; set; }

    /// <summary>服务平台商户号（<c>platform_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("platform_mchid")]
    public string? PlatformMchId { get; set; }

    /// <summary>付款企业商户号（<c>specialized_payment_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("specialized_payment_mchid")]
    public string? SpecializedPaymentMchId { get; set; }

    /// <summary>实际出资商户号（<c>sponsor_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sponsor_mchid")]
    public string? SponsorMchId { get; set; }

    /// <summary>用户授权商户号（<c>user_authorized_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("user_authorized_mchid")]
    public string? UserAuthorizedMchId { get; set; }

    /// <summary>用户授权 AppID（<c>user_authorized_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("user_authorized_appid")]
    public string? UserAuthorizedAppId { get; set; }

    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信支付批次单号（<c>batch_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>转账场景（<c>transfer_scene</c>，必填，2 值）：见 <see cref="BankTransferScenes"/>。</summary>
    [JsonPropertyName("transfer_scene")]
    public string? TransferScene { get; set; }

    /// <summary>批次名称（<c>batch_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>附加信息（<c>attach_remark</c>，选填 string(32)）。</summary>
    [JsonPropertyName("attach_remark")]
    public string? AttachRemark { get; set; }

    /// <summary>批量转账用途（<c>transfer_purpose</c>，必填，6 值）：见 <see cref="BankTransferPurposes"/>。</summary>
    [JsonPropertyName("transfer_purpose")]
    public string? TransferPurpose { get; set; }

    /// <summary>批次状态（<c>batch_state</c>，必填 string(32)，4 值）：见 <see cref="BankTransferBatchStates"/>，本面字段名为 <c>batch_state</c>。</summary>
    [JsonPropertyName("batch_state")]
    public string? BatchState { get; set; }

    /// <summary>批次关闭原因（<c>close_reason</c>，选填，2 值）：仅 <c>CLOSED</c> 时返回，见 <see cref="BankTransferCloseReasons"/>。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>转账总金额（<c>total_amount</c>，必填 integer）：单位为分。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>转账总笔数（<c>total_count</c>，必填 integer）：本面字段名为 <c>total_count</c>。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>批次创建时间（<c>create_time</c>，选填 string(32)）：批次受理成功时返回，rfc3339。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>批次更新时间（<c>update_time</c>，选填 string(32)）：最近一次状态变更时间，rfc3339。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>转账成功金额（<c>success_amount</c>，选填 integer）：<c>PROCESSING</c> 期间随时变化。</summary>
    [JsonPropertyName("success_amount")]
    public int? SuccessAmount { get; set; }

    /// <summary>转账成功笔数（<c>success_count</c>，选填 integer）：本面字段名为 <c>success_count</c>。</summary>
    [JsonPropertyName("success_count")]
    public int? SuccessCount { get; set; }

    /// <summary>转账失败金额（<c>fail_amount</c>，选填 integer）。</summary>
    [JsonPropertyName("fail_amount")]
    public int? FailAmount { get; set; }

    /// <summary>转账失败笔数（<c>fail_count</c>，选填 integer）：本面字段名为 <c>fail_count</c>。</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }

    /// <summary>转账明细列表（<c>transfer_detail_list</c>，选填 array）：仅批次 <c>FINISHED</c> 时返回。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<BankTransferBatchDetailItem>? TransferDetailList { get; set; }
}

/// <summary>
/// 批次单应答内嵌的转账明细概要（<c>transfer_detail_list[]</c>，从业机构（银行）面，官方 4 字段）。
/// </summary>
/// <remarks><b>比服务商面概要多 <c>fail_reason</c>，且状态字段名为 <c>detail_state</c></b>（服务商面 <c>detail_status</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferBatchDetailItem
{
    /// <summary>微信支付明细单号（<c>detail_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>银行明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>明细状态（<c>detail_state</c>，必填 string(32)）：本面字段名为 <c>detail_state</c>。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

    /// <summary>明细失败原因（<c>fail_reason</c>，选填 string）：概要内亦回传，取值原样透传。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}

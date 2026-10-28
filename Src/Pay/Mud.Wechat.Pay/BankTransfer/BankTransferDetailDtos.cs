// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.BankTransfer;

/// <summary>
/// 从业机构（银行）「查询明细单」应答（两支按 <c>detail_id</c> / <c>out_detail_no</c> 定位的 GET <b>共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：微信支付明细单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465897"/>、
/// 银行明细单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465915"/>，
/// 2026-10-14 逐页比对字段表<b>完全相同</b> ⇒ 共用一型（守卫 BT-B6）。</para>
/// <para><b>必填性与服务商面三处相反</b>：本面 <c>username</c> / <c>account_type</c> <b>必填</b>、
/// <c>initiate_time</c> <b>选填</b>；服务商面 <c>username</c> / <c>account_type</c> 选填、
/// <c>initiate_time</c> 必填 ⇒ 同名不同必填性，两域类型不可共用。</para>
/// <para><b>状态字段名 <c>detail_state</c>、时间字段名 <c>initiate_time</c></b>（本域无 <c>create_time</c>）。</para>
/// <para><b><c>username</c> 是密文</b>（string(1024)，官方注明「采用标准 RSA 算法，公钥由微信侧提供」）：
/// 本 SDK 只回传密文，解密由宿主以商户私钥完成。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferDetailQueryResponse : WechatPayResponse
{
    /// <summary>银行服务商号（<c>bank_sp_mchid</c>，必填 string(32)）：仅应答侧出现。</summary>
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

    /// <summary>银行明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>微信支付明细单号（<c>detail_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）：归属由用户信息授权方决定。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>username</c>，<b>本面必填</b> string(1024)）：<b>密文</b>。</summary>
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为分。</summary>
    [JsonPropertyName("transfer_amount")]
    public int? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>明细状态（<c>detail_state</c>，必填 string(32)，4 值）：见 <see cref="BankTransferDetailStates"/>。</summary>
    [JsonPropertyName("detail_state")]
    public string? DetailState { get; set; }

    /// <summary>明细失败原因（<c>fail_reason</c>，选填 string）：取值原样透传（本域不建常量类，见守卫 BT-B5 裁决）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>转账发起时间（<c>initiate_time</c>，<b>本面选填</b> string(32)）：rfc3339。</summary>
    [JsonPropertyName("initiate_time")]
    public string? InitiateTime { get; set; }

    /// <summary>明细更新时间（<c>update_time</c>，必填 string(32)）：最后一次状态变更时间，rfc3339。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>收款账户类型（<c>account_type</c>，<b>本面必填</b> string）：本面取值集合官方未附「可选取值」表 ⇒ 原样透传。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>银行名称（<c>bank_name</c>，选填 string(64)）：转账到银行卡时输出。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行卡尾号（<c>bank_card_number_tail</c>，选填 string(10)）：转账到银行卡时输出。</summary>
    [JsonPropertyName("bank_card_number_tail")]
    public string? BankCardNumberTail { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransfer;

/// <summary>
/// 服务商「查询明细单」应答（两支按 <c>detail_id</c> / <c>out_detail_no</c> 定位的 GET <b>共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：微信支付明细单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465781"/>、
/// 商家明细单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465795"/>，
/// 2026-10-14 逐页比对字段表<b>完全相同</b> ⇒ 共用一型（守卫 PTF-B6）。</para>
/// <para><b>本应答没有 <c>sub_mchid</c></b>：批次应答有，明细应答只有 <c>sp_mchid</c> +
/// <c>appid</c>（选填，其归属随 <c>authorization_type</c> 在特约商户 / 服务商之间切换）。</para>
/// <para><b>时间字段是 <c>initiate_time</c> 而非 <c>create_time</c></b>（两支均必填，rfc3339）
/// ⇒ 照抄批次应答的 <c>create_time</c> 会静默丢字段。本域时间一律 <c>string</c>。</para>
/// <para><b><c>username</c> 是密文</b>（string(1024)，官方注明「采用标准 RSA 算法，公钥由微信侧提供」）：
/// 仅当转账时传入了收款用户姓名才返回，宿主须自行用<b>商户私钥</b>解密，本 SDK 不做解密。</para>
/// <para><b>银行卡三节只在转账到银行卡时出现</b>：<c>account_type</c> / <c>bank_name</c> /
/// <c>bank_card_number_tail</c> 均选填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferDetailQueryResponse : WechatPayResponse
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，必填 string(32)）：本应答不含 <c>sub_mchid</c>。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>商家批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信支付批次单号（<c>batch_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>商户 AppID（<c>appid</c>，选填 string(32)）：随授权类型取特约商户或服务商 AppID。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商家明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>微信支付明细单号（<c>detail_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>明细状态（<c>detail_status</c>，必填 string(32)，6 值）：见 <see cref="PartnerTransferDetailStates"/>。</summary>
    [JsonPropertyName("detail_status")]
    public string? DetailStatus { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为分。</summary>
    [JsonPropertyName("transfer_amount")]
    public int? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>明细失败原因（<c>fail_reason</c>，选填 string）：转账失败时返回，取值原样透传（本域不建常量类，见守卫 PTF-B5 裁决）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>username</c>，选填 string(1024)）：<b>密文</b>，转账时传入姓名才返回。</summary>
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    /// <summary>转账发起时间（<c>initiate_time</c>，必填 string(32)）：rfc3339，字段名非 <c>create_time</c>。</summary>
    [JsonPropertyName("initiate_time")]
    public string? InitiateTime { get; set; }

    /// <summary>明细更新时间（<c>update_time</c>，必填 string(32)）：最后一次状态变更时间，rfc3339。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>收款账户类型（<c>account_type</c>，选填，2 值）：见 <see cref="PartnerTransferAccountTypes"/>。</summary>
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }

    /// <summary>银行名称（<c>bank_name</c>，选填 string）：转账到银行卡时输出。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行卡尾号（<c>bank_card_number_tail</c>，选填 string）：转账到银行卡时输出。</summary>
    [JsonPropertyName("bank_card_number_tail")]
    public string? BankCardNumberTail { get; set; }
}

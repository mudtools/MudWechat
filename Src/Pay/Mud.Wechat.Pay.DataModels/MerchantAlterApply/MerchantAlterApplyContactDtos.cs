// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantAlterApply;

/// <summary>
/// 超级管理员资料变更申请的包体（官方 <c>POST /v3/mchalterapply/mchcontactalterapplyment</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/>，2026-10-14 逐字段核验。</para>
/// <para><b>清单转述层的一处错配已在方案 §2.7 一-1 登记</b>：缺口清单用主体族四个 docId 承载本族四支，
/// 逐页核得本族现行页是 <c>4015617906</c> / <c>4015617911</c> / <c>4015617919</c> / <c>4015617915</c>，
/// 路由段是 <c>mchcontactalterapplyment</c>（照页，<b>不是</b> <c>contactalter</c> 简写）。</para>
/// <para><b><c>out_request_no</c> 官方长度是 string(124)</b>（主体族 64、简称族 124）：
/// 长度写在表行括号内，属字段表的一部分 ⇒ 各族请求分建（守卫 MAA-B3）。</para>
/// <para><b>本支 <c>Wechatpay-Serial</c> 是请求头表行必填</b>：<see cref="ContactInfo"/> 内的姓名、
/// 证件号、手机号须以微信支付公钥（推荐）或平台证书加密后传入，DTO 只承载密文 string。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyContactSubmitRequest
{
    /// <summary>商户号（<c>merchant_code</c>，必填 string(32)）：本服务商负责进件的特约商户/二级商户号。</summary>
    [JsonPropertyName("merchant_code")]
    public string? MerchantCode { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 <b>string(124)</b>）：官方长度与主体族的 64 不同，照页。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>超级管理员信息（<c>contact_info</c>，必填 object）：本族唯一的资料对象。</summary>
    [JsonPropertyName("contact_info")]
    public MerchantAlterApplyContactInfo? ContactInfo { get; set; }
}

/// <summary>
/// 超级管理员信息（官方 <c>contact_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617906"/>，2026-10-14 逐字段核验。</para>
/// <para><b>字段名体系是 <c>contact_id_*</c> 前缀</b>（<c>contact_id_doc_type</c> /
/// <c>contact_id_card_number</c> / <c>contact_id_doc_copy</c>…），与主体族的 <c>card_*</c> 体系分家
/// ⇒ 两族证件对象不得复用（守卫 MAA-B3 两侧断言字段名）。证件类型 <c>contact_id_doc_type</c>
/// 的值集与主体族 <c>id_doc_type</c> 相同（8 值），共用 <see cref="MerchantAlterApplyIdDocTypes"/>。</para>
/// <para><b>三字段须加密</b>：<c>contact_name</c>、<c>contact_id_card_number</c>、<c>mobile_phone</c>
/// 官方注明「需进行加密处理」；<c>contact_id_doc_copy_back</c> 是本族唯一选填字段（护照类证件无反面）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyContactInfo
{
    /// <summary>联系人姓名（<c>contact_name</c>，必填 string(512)）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>证件类型（<c>contact_id_doc_type</c>，必填 string）：见 <see cref="MerchantAlterApplyIdDocTypes"/>。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>证件号码（<c>contact_id_card_number</c>，必填 string）：<b>须加密</b>后传入。</summary>
    [JsonPropertyName("contact_id_card_number")]
    public string? ContactIdCardNumber { get; set; }

    /// <summary>证件正面照片（<c>contact_id_doc_copy</c>，必填 string）：填《图片上传 API》返回的 MediaID。</summary>
    [JsonPropertyName("contact_id_doc_copy")]
    public string? ContactIdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>contact_id_doc_copy_back</c>，选填 string）：本对象唯一选填字段。</summary>
    [JsonPropertyName("contact_id_doc_copy_back")]
    public string? ContactIdDocCopyBack { get; set; }

    /// <summary>证件有效期开始时间（<c>contact_id_doc_period_begin</c>，必填 string）：<c>yyyy-MM-dd</c>。</summary>
    [JsonPropertyName("contact_id_doc_period_begin")]
    public string? ContactIdDocPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>contact_id_doc_period_end</c>，必填 string）：<c>yyyy-MM-dd</c> 或「长期」。</summary>
    [JsonPropertyName("contact_id_doc_period_end")]
    public string? ContactIdDocPeriodEnd { get; set; }

    /// <summary>手机号（<c>mobile_phone</c>，必填 string）：<b>须加密</b>后传入，用于超级管理员身份确认。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }
}

/// <summary>
/// 超级管理员变更申请单的查询应答（<b>按申请单号与按「商户号 + 业务申请编号」两支共用</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015617911"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015617915"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与主体族查询应答分建</b>：字段名与长度两页相同，唯一差异是 <c>state</c> 值清单
/// （本族 5 值、<b>无</b> <c>APPLYMENT_STATE_EDITTING</c>）⇒ 按「表不同则分建」处理（守卫 MAA-B6）。</para>
/// <para><b>应答侧 <c>out_request_no</c> 官方标 string(64)</b>，而提交请求侧标 string(124)
/// —— 同域两页长度不一致，官方原文如此，<b>不得「统一」</b>（守卫 MAA-B3 同时锁两处）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantAlterApply")]
public class MerchantAlterApplyContactQueryResponse : WechatPayResponse
{
    /// <summary>申请单号（<c>apply_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("apply_id")]
    public string? ApplyId { get; set; }

    /// <summary>业务申请编号（<c>out_request_no</c>，必填 <b>string(64)</b>）：与提交侧的 string(124) 不同，照页。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>商户号（<c>merchant_code</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_code")]
    public string? MerchantCode { get; set; }

    /// <summary>申请单状态（<c>state</c>，必填 string）：本族 5 值，见 <see cref="MerchantAlterApplyContactStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总体驳回原因（<c>audit_reject_reason</c>，选填 string）：仅 <c>REJECTED</c> 状态返回。</summary>
    [JsonPropertyName("audit_reject_reason")]
    public string? AuditRejectReason { get; set; }

    /// <summary>逐项驳回明细（<c>audit_reject_detail</c>，选填 array[object]）：与主体族共用列表项类型。</summary>
    [JsonPropertyName("audit_reject_detail")]
    public List<MerchantAlterApplyAuditRejectDetail>? AuditRejectDetail { get; set; }
}

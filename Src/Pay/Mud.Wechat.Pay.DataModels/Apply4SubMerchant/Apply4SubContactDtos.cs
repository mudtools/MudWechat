// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 超级管理员信息（进件 <c>contact_info</c>）。
/// </summary>
/// <remarks>
/// 字段表照官方「提交申请单」页原文（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>，
/// 2026-10-14 逐字段核验）。<b>必填性随 <see cref="ContactType"/> 分流</b>：取 <c>LEGAL</c>（经营者 /
/// 法定代表人）时证件类字段无需上传；取 <c>SUPER</c>（经办人）时须上传经办人证件照片、号码与有效期。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubContactInfo
{
    /// <summary>超级管理员类型（<c>contact_type</c>，必填）：取值见 <see cref="Apply4SubIdHolderTypes"/>（<c>LEGAL</c> / <c>SUPER</c>）。</summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>超级管理员姓名（<c>contact_name</c>，必填 string(2048)，<b>需加密</b>）：2-100 字符，前后不能有空白，不能仅含数字或特殊字符。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>证件类型（<c>contact_id_doc_type</c>，选填）：仅超级管理员为经办人时上传，取值见 <see cref="Apply4SubIdDocTypes"/>。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>证件号码（<c>contact_id_number</c>，选填 string(2048)，<b>需加密</b>）：仅经办人时上传，号码规范按证件类型而异（官方原文）。</summary>
    [JsonPropertyName("contact_id_number")]
    public string? ContactIdNumber { get; set; }

    /// <summary>证件正面照片（<c>contact_id_doc_copy</c>，选填）：仅经办人时上传，填「图片上传 API」产出的 MediaID。</summary>
    [JsonPropertyName("contact_id_doc_copy")]
    public string? ContactIdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>contact_id_doc_copy_back</c>，选填）：仅经办人时上传；护照无需上传反面。</summary>
    [JsonPropertyName("contact_id_doc_copy_back")]
    public string? ContactIdDocCopyBack { get; set; }

    /// <summary>证件有效期开始时间（<c>contact_period_begin</c>，选填）：<c>yyyy-MM-dd</c>，不得早于 1900-01-01、不得晚于当前日期。</summary>
    [JsonPropertyName("contact_period_begin")]
    public string? ContactPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>contact_period_end</c>，选填）：<c>yyyy-MM-dd</c> 或 <c>长期</c>，须晚于开始时间。</summary>
    [JsonPropertyName("contact_period_end")]
    public string? ContactPeriodEnd { get; set; }

    /// <summary>业务办理授权函（<c>business_authorization_letter</c>，选填）：<b>官方明示即将下线、无需传入</b>，仅为兼容既有调用保留。</summary>
    [JsonPropertyName("business_authorization_letter")]
    public string? BusinessAuthorizationLetter { get; set; }

    /// <summary>超级管理员微信 OpenID（<c>openid</c>，选填）：填写后超级管理员签约时校验微信号是否一致。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>联系手机（<c>mobile_phone</c>，必填 string(2048)，<b>需加密</b>）：11 位手机号，或 5-20 位数字 / 连字符 / 加号；用于接收管理信息与操作验证码。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>联系邮箱（<c>contact_email</c>，必填 string(2048)，<b>需加密</b>）：须含 <c>@</c> 并通过邮箱格式校验，用于接收开户邮件与业务通知。</summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }
}

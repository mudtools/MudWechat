// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4Subject;

/// <summary>
/// 联系人信息（主体进件 <c>contact_info</c>，官方文档
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域语义与特约商户进件不同</b>：官方原文要求联系人是<b>商户的联系人</b>，且
/// 「请确定联系人为商户法定代表人或经营者再进行操作」；只有当联系人<b>非</b>法定代表人 / 经营者时，
/// 才需要提交经办人证件与业务办理授权函（<c>authorize_letter_copy</c> 在
/// <see cref="Apply4SubjectIdentificationInfo"/> 内）。
/// </para>
/// <para>
/// <b>字段名与加密口径</b>：<c>name</c> / <c>mobile</c> / <c>id_card_number</c> 均为
/// string(1024) 且官方标注「需加密」⇒ 传入的是密文而非明文（长度上限是为密文预留的）；
/// <c>contact_id_doc_copy</c> / <c>contact_id_doc_copy_back</c> 是图片上传 API 返回的 MediaID，不加密。
/// </para>
/// <para>
/// <b>经办人条件必填</b>：<c>contact_id_doc_type</c> 及正反照片、<c>contact_period_begin</c> /
/// <c>contact_period_end</c> 在字段表内均标<b>选填</b>，官方说明为「当联系人类型是经办人时」必传
/// ⇒ 类型上可空、条件由官方审核侧判定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectContactInfo
{
    /// <summary>联系人姓名（<c>name</c>，必填 string(1024)）：<b>需加密</b>；类型为法定代表人时须与营业执照一致。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>联系人手机号（<c>mobile</c>，必填 string(1024)）：<b>需加密</b>；11 位手机号或 5-20 位数字/连字符/加号，接收重要管理信息与操作验证码。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>联系人证件号码（<c>id_card_number</c>，选填 string(1024)）：<b>需加密</b>；可传身份证、来往内地/大陆通行证、护照等证件号码。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>联系人类型（<c>contact_type</c>，必填）：取值见 <see cref="Apply4SubjectHolderTypes"/>；小微 / 个人卖家只能 <c>LEGAL</c>，其余主体可 <c>LEGAL</c> / <c>SUPER</c>。</summary>
    [JsonPropertyName("contact_type")]
    public string? ContactType { get; set; }

    /// <summary>联系人证件类型（<c>contact_id_doc_type</c>，选填）：取值见 <see cref="Apply4SubjectIdDocTypes"/>；联系人类型为经办人时上传。</summary>
    [JsonPropertyName("contact_id_doc_type")]
    public string? ContactIdDocType { get; set; }

    /// <summary>联系人证件正面照片（<c>contact_id_doc_copy</c>，选填 string(256)）：图片上传 API 返回的 MediaID；证件类型为身份证时传人像面。</summary>
    [JsonPropertyName("contact_id_doc_copy")]
    public string? ContactIdDocCopy { get; set; }

    /// <summary>联系人证件反面照片（<c>contact_id_doc_copy_back</c>，选填 string(256)）：MediaID；证件类型为护照时无需上传反面。</summary>
    [JsonPropertyName("contact_id_doc_copy_back")]
    public string? ContactIdDocCopyBack { get; set; }

    /// <summary>联系人证件有效期开始时间（<c>contact_period_begin</c>，选填 string(128)）：<c>YYYY-MM-DD</c>，不得早于 1900-01-01、不得晚于或等于当前日期。</summary>
    [JsonPropertyName("contact_period_begin")]
    public string? ContactPeriodBegin { get; set; }

    /// <summary>联系人证件有效期结束时间（<c>contact_period_end</c>，选填 string(128)）：<c>YYYY-MM-DD</c>，长期请填写「长期」，须晚于开始时间。</summary>
    [JsonPropertyName("contact_period_end")]
    public string? ContactPeriodEnd { get; set; }
}

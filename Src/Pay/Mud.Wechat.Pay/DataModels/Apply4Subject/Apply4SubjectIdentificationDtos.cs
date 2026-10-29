// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4Subject;

/// <summary>
/// 法定代表人身份信息（主体进件<b>顶层</b> <c>identification_info</c>，官方文档
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域为顶层对象</b>（与 <c>subject_info</c> 平级），而特约商户进件的同一语义信息嵌在
/// <c>subject_info.identity_info</c> 且字段名不同（那边是 <c>id_doc_copy</c> / <c>id_doc_copy_back</c> /
/// <c>id_doc_number</c>，本域是 <c>identification_front_copy</c> / <c>identification_back_copy</c> /
/// <c>identification_number</c>）⇒ 两个域的类型<b>不共用、不互转</b>。
/// </para>
/// <para>
/// <b>「条件必填」在本块特别绕</b>：官方对块内几乎所有字段都写「当证件持有人类型为法定代表人时填写，
/// 其他情况无需上传」⇒ <c>id_holder_type = SUPER</c>（经办人）时这些字段<b>整体留空</b>，
/// 改由 <c>contact_info</c> 承载经办人证件，而 <c>authorize_letter_copy</c>（法定代表人说明函）
/// 反而<b>必须</b>上传。
/// </para>
/// <para>
/// <b>加密字段</b>：<c>identification_name</c> / <c>identification_number</c> /
/// <c>identification_address</c>（三者均 string(1024) 或 string(512) 且标注「需加密」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectIdentificationInfo
{
    /// <summary>证件持有人类型（<c>id_holder_type</c>，选填）：取值见 <see cref="Apply4SubjectHolderTypes"/>；仅主体为政府机关 / 事业单位且改用经办人证件时上传，小微 / 企业 / 个体户等默认经营者 / 法定代表人、不传。</summary>
    [JsonPropertyName("id_holder_type")]
    public string? IdHolderType { get; set; }

    /// <summary>证件类型（<c>identification_type</c>，选填）：取值见 <see cref="Apply4SubjectIdDocTypes"/>；政府机关与小微仅支持中国大陆居民-身份证。</summary>
    [JsonPropertyName("identification_type")]
    public string? IdentificationType { get; set; }

    /// <summary>证件姓名（<c>identification_name</c>，选填 string(1024)）：<b>需加密</b>；填法定代表人证件上的姓名。</summary>
    [JsonPropertyName("identification_name")]
    public string? IdentificationName { get; set; }

    /// <summary>证件号码（<c>identification_number</c>，选填 string(1024)）：<b>需加密</b>；可传身份证 / 通行证 / 护照等，格式随证件类型而异。</summary>
    [JsonPropertyName("identification_number")]
    public string? IdentificationNumber { get; set; }

    /// <summary>证件有效日期（<c>identification_valid_date</c>，选填 string(64)）：<b>字符串内嵌 JSON 数组文本</b>，官方示例 <c>["2017-10-28","长期"]</c>。</summary>
    [JsonPropertyName("identification_valid_date")]
    public string? IdentificationValidDate { get; set; }

    /// <summary>证件正面照片（<c>identification_front_copy</c>，选填 string(255)）：MediaID；身份证传人像面，非身份证传正面。</summary>
    [JsonPropertyName("identification_front_copy")]
    public string? IdentificationFrontCopy { get; set; }

    /// <summary>证件反面照片（<c>identification_back_copy</c>，选填 string(255)）：MediaID；身份证传国徽面，往来通行证 / 居留证传反面，护照无需反面。</summary>
    [JsonPropertyName("identification_back_copy")]
    public string? IdentificationBackCopy { get; set; }

    /// <summary>法定代表人说明函（<c>authorize_letter_copy</c>，选填 string(255)）：MediaID；<b>证件持有人类型为经办人时必须上传</b>，信息需打印、不支持手写且须加盖公章。</summary>
    [JsonPropertyName("authorize_letter_copy")]
    public string? AuthorizeLetterCopy { get; set; }

    /// <summary>
    /// 经营者 / 法定代表人是否为受益人（<c>owner</c>，选填 boolean）。
    /// </summary>
    /// <remarks>官方标注<b>「该字段即将下线处理，无需传入」</b>；照原文保留建模（主体类型为企业时原要求填写），新接入建议不填。</remarks>
    [JsonPropertyName("owner")]
    public bool? Owner { get; set; }

    /// <summary>证件居住地址（<c>identification_address</c>，选填 string(512)）：<b>需加密</b>；主体类型为企业时需要填写，按身份证住址填写。</summary>
    [JsonPropertyName("identification_address")]
    public string? IdentificationAddress { get; set; }
}

/// <summary>
/// 最终受益人信息（<c>ubo_info_list[]</c>，官方文档
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类型同时是「复数列表项」</b>：官方本页字段表只有顶层 <c>ubo_info_list</c>（array），
/// 示例报文里另有一份<b>不在字段表内</b>的单数 <c>ubo_info</c>
/// （<c>ubo_id_type</c> / <c>ubo_id_card_copy</c> / <c>ubo_id_card_national</c> / <c>ubo_name</c> /
/// <c>ubo_id_number</c> / <c>ubo_id_card_valid_date</c>）⇒ 以<b>字段表为权威</b>，本仓不建单数类型。
/// </para>
/// <para>
/// 官方标注该列表「即将下线处理，无需传入」；原语义为主体类型为企业、且经营者 / 法定代表人
/// 不是最终受益所有人时补提受益人（最多 4 人，是之一时最多 3 人）。
/// 加密字段：<c>ubo_id_doc_name</c> / <c>ubo_id_doc_number</c> / <c>ubo_id_doc_address</c>。
/// </para>
/// <para>
/// <b>有效期是两字段而不是区间字符串</b>：<c>ubo_period_begin</c> / <c>ubo_period_end</c>
/// 各自独立（同 <c>contact_info</c>），与本域三处「有效日期」的区间字符串形态<b>不同</b>，勿统一。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]
public class Apply4SubjectUboInfo
{
    /// <summary>证件类型（<c>ubo_id_doc_type</c>，选填）：取值见 <see cref="Apply4SubjectIdDocTypes"/>。</summary>
    [JsonPropertyName("ubo_id_doc_type")]
    public string? UboIdDocType { get; set; }

    /// <summary>证件正面照片（<c>ubo_id_doc_copy</c>，选填 string(256)）：MediaID；身份证传人像面。</summary>
    [JsonPropertyName("ubo_id_doc_copy")]
    public string? UboIdDocCopy { get; set; }

    /// <summary>证件反面照片（<c>ubo_id_doc_copy_back</c>，选填 string(256)）：MediaID；护照无需上传反面。</summary>
    [JsonPropertyName("ubo_id_doc_copy_back")]
    public string? UboIdDocCopyBack { get; set; }

    /// <summary>证件姓名（<c>ubo_id_doc_name</c>，选填 string(1024)）：<b>需加密</b>。</summary>
    [JsonPropertyName("ubo_id_doc_name")]
    public string? UboIdDocName { get; set; }

    /// <summary>证件号码（<c>ubo_id_doc_number</c>，选填 string(1024)）：<b>需加密</b>。</summary>
    [JsonPropertyName("ubo_id_doc_number")]
    public string? UboIdDocNumber { get; set; }

    /// <summary>证件居住地址（<c>ubo_id_doc_address</c>，选填 string）：<b>需加密</b>，长度 1-512。</summary>
    [JsonPropertyName("ubo_id_doc_address")]
    public string? UboIdDocAddress { get; set; }

    /// <summary>证件有效期开始时间（<c>ubo_period_begin</c>，选填 string(128)）：<c>YYYY-MM-DD</c>，不得早于 1900-01-01、不得晚于或等于当前日期。</summary>
    [JsonPropertyName("ubo_period_begin")]
    public string? UboPeriodBegin { get; set; }

    /// <summary>证件有效期结束时间（<c>ubo_period_end</c>，选填 string(128)）：<c>YYYY-MM-DD</c>，长期填「长期」，须晚于开始时间。</summary>
    [JsonPropertyName("ubo_period_end")]
    public string? UboPeriodEnd { get; set; }
}

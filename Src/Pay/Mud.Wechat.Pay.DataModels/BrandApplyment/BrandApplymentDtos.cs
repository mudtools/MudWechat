// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandApplyment;

/// <summary>品牌入驻申请提交请求·【管理员信息】 提交申请单时需填写品牌管理员的个人信息。品牌管理员是负责品牌经营。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestAdminInfo
{
    /// <summary>【管理员姓名】 1、长度为2-50个字符； 2、前后不能有空格、制表符、换行符； 3、支持填写数字、英文字母、汉字及部分特殊字符，但完整内容不能只包含数字、部分特殊字符； 4、允许使用的部分特殊字符有： 英文标点： ``. ( ) - _ — ~ ! @ # % ^ &amp; * + ` = { } &amp;#124; : ' " ? [ ] \ ; , /`` 中文标点： `。 ？ ！ ， 、 ； ： “ ” ‘ ' （ ） 《 》 〈 〉 【 】 …（官方字段路径 <c>admin_info.admin_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("admin_name")]
    public string? AdminName { get; set; }

    /// <summary>【管理员证件类型】 不支持护照和外国人居住证类型。可选取值：`IDENTIFICATION_TYPE_MAINLAND_ID_CARD` 中国大陆居民-身份证 / `IDENTIFICATION_TYPE_HONGKONG` 中国香港居民--来往内地通行证 / `IDENTIFICATION_TYPE_MACAO` 中国澳门居民--来往内地通行证 / `IDENTIFICATION_TYPE_TAIWAN` 中国台湾居民--来往大陆通行证 / `IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT` 港澳居民居住证 / `IDENTIFICATION_TYPE_TAIWAN_RESIDENT` 台湾居民居住证（官方字段路径 <c>admin_info.id_doc_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("id_doc_type")]
    public string? IdDocType { get; set; }

    /// <summary>【管理员证件号】 1、可传身份证、来往内地通行证、来往大陆通行证等证件号码，号码规范如下： 身份证（限中国大陆居民）：17位数字+1位数字&amp;#124;X； 中国香港居民--来往内地通行证：H/h开头+8或10位数字/字母； 中国澳门居民--来往内地通行证：M/m开头+8或10位数字/字母； 中国台湾居民--来往大陆通行证：8位数字或10位数字； 台湾居民居住证/港澳居民居住证：17位数字+1位数字&amp;#124;X； 2、经营管理员授权确认时，…（官方字段路径 <c>admin_info.id_card_number</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

}

/// <summary>品牌入驻申请提交请求·【主体信息】 请提交品牌所属企业的主体信息，包括主体名称、主体类型和统一社会信用。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestSubjectInfo
{
    /// <summary>【主体类型】 1、主体类型需与营业执照/登记证书上一致，可参考 选择主体指引； 2、只支持企业和个体户类型，其他暂不支持。可选取值：`SUBJECT_TYPE_ENTERPRISE` （企业）营业执照上的主体类型一般为有限公司、有限责任公司 / `SUBJECT_TYPE_INDIVIDUAL` （个体户）营业执照上的主体类型一般为个体户、个体工商户、个体经营（官方字段路径 <c>subject_info.subject_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>【主体名称】 1、入驻商家主体的名称； 2、长度限制不超过50个字符（官方字段路径 <c>subject_info.subject_name</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("subject_name")]
    public string? SubjectName { get; set; }

    /// <summary>【统一社会信用代码】 1、入驻商家主体的统一社会信用代码； 2、长度限制为18个字符，由数字和大写英文字母组成（官方字段路径 <c>subject_info.unified_social_credit_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("unified_social_credit_code")]
    public string? UnifiedSocialCreditCode { get; set; }

}

/// <summary>品牌入驻申请提交请求·【品牌基础信息】 品牌的基本信息，包括品牌名称和品牌logo，是品牌入驻的必要资。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestBrandBasicInfo
{
    /// <summary>【品牌名称】 商户申请的品牌名称，长度限制30字符以内（官方字段路径 <c>brand_basic_info.brand_name</c>，<b>必填</b>，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>【品牌Logo】 1、商家申请的品牌Logo； 2、支持上传jpg、png格式，最大支持2MB； 3、要求上传尺寸不小于200px \* 200px，不大于300px \* 300px，图片宽高比1:1； 4、需上传1张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID（官方字段路径 <c>brand_basic_info.brand_logo</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_logo")]
    public string? BrandLogo { get; set; }

}

/// <summary>品牌入驻申请提交请求·【品牌商标资料】 商标相关资料，包括是否有商标、商标注册证、授权书或无商标时的经·【商标注册证】 商标注册证相关信息，有商标时必填。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestTrademarkTrademarkRegistrationCertificate
{
    /// <summary>【商标注册证】 1、若选择有商标，请参考 示例 上传Logo商标注册证的图片； 2、支持上传png、jpg格式，最大2MB； 3、请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：该字段仅支持单张图片，不可与 certificate_list 同时填写。自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用。新接入服务商请使用 certificate_list 字段，支持多张图片传入（官方字段路径 <c>trademark.trademark_registration_certificate.certificate</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("certificate")]
    public string? Certificate { get; set; }

    /// <summary>【商标名称】 1、商标注册时登记的名称，可前往 商标备案网 查询； 2、支持输入中文、英文、数字特殊字符； 3、限制50个字符（官方字段路径 <c>trademark.trademark_registration_certificate.name</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【商标注册号】 选择有商标时必填： 1、支持输入英文、数字； 2、限制50个字符（官方字段路径 <c>trademark.trademark_registration_certificate.number</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>【商标有效期开始时间】 1、商标注册证上的有效期开始时间； 2、格式要求为： `YYYY-MM-DD`，不支持其他格式。 注意：自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用（官方字段路径 <c>trademark.trademark_registration_certificate.valid_begin_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【商标有效期结束时间】 1、商标注册证上的有效期截止时间； 2、有效期结束时间必须晚于当前提交申请时间，否则提交将被拒绝； 3、格式要求：请按照标准格式 `YYYY-MM-DD` 填写（如 `2025-12-31`），月份和日期为两位数字，必须为合法日期（官方字段路径 <c>trademark.trademark_registration_certificate.valid_end_time</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【商标国际分类】 1、请填写商标注册证上的商标国际分类，具体类别参考 商标分类表； 2、取值范围为1-45，只支持数字，不支持字母、符号、小数等（官方字段路径 <c>trademark.trademark_registration_certificate.international_class</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("international_class")]
    public string? InternationalClass { get; set; }

    /// <summary>【商标注册人】 1、需填写商标注册证上的注册人名称； 2、支持中文、英文、数字、括号，限制50个字符； 3、如果与主体名称不一致，需上传“商标许可使用授权书”（官方字段路径 <c>trademark.trademark_registration_certificate.holder</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("holder")]
    public string? Holder { get; set; }

    /// <summary>【商标许可使用授权书】 1、商标注册人为当前主体提供的授权书，商标注册人与主体名称不一致时需填写； 2、支持上传png、jpg格式，最大2MB； 3、请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：该字段仅支持单张图片，不可与 license_list 同时填写。自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用。新接入服务商请使用 license_list 字段，支持多张图片传入（官方字段路径 <c>trademark.trademark_registration_certificate.license</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("license")]
    public string? License { get; set; }

    /// <summary>【商标授权有效期开始时间】 1、授权书上的有效期起始时间，商标注册人与主体名称不一致时需填写； 2、格式要求为： `YYYY-MM-DD`，不支持其他格式。 注意：自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用（官方字段路径 <c>trademark.trademark_registration_certificate.authorization_begin_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("authorization_begin_time")]
    public string? AuthorizationBeginTime { get; set; }

    /// <summary>【商标授权有效期结束时间】 1、授权书上的有效期截止时间，商标注册人与主体名称不一致时需填写； 2、有效期结束时间必须晚于当前提交申请时间，否则提交将被拒绝； 3、格式要求： 日期填写规范：请按照标准格式 `YYYY-MM-DD` 填写（如 `2025-12-31`），月份和日期为两位数字，必须为合法日期； 有效期限制：授权有效期不得超过商标有效期（官方字段路径 <c>trademark.trademark_registration_certificate.authorization_end_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("authorization_end_time")]
    public string? AuthorizationEndTime { get; set; }

    /// <summary>【商标注册证列表】 1、若选择有商标，请参考 示例 上传商标注册证的图片； 2、支持上传png、jpg格式，最大2MB； 3、可上传1-5张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：不可与 certificate 同时填写（官方字段路径 <c>trademark.trademark_registration_certificate.certificate_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("certificate_list")]
    public List<string>? CertificateList { get; set; }

    /// <summary>【商标许可使用授权书列表】 1、商标注册人为当前主体提供的授权书列表，商标注册人与主体名称不一致时需填写； 2、支持上传png、jpg格式，最大2MB； 3、可上传1-5张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：不可与 license 同时填写（官方字段路径 <c>trademark.trademark_registration_certificate.license_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("license_list")]
    public List<string>? LicenseList { get; set; }

}

/// <summary>品牌入驻申请提交请求·【品牌商标资料】 商标相关资料，包括是否有商标、商标注册证、授权书或无商标时的经·【Logo商标注册证】 Logo商标注册证相关信息，只有有商标时可填写。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestTrademarkLogoTrademarkRegistrationCertificate
{
    /// <summary>【Logo商标注册证】 1、若品牌Logo已注册商标，请参考 示例 上传Logo商标注册证的图片； 2、支持上传png、jpg格式，最大2MB； 3、请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：该字段仅支持单张图片，不可与 certificate_list 同时填写。自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用。新接入服务商请使用 certificate_list 字段，支持多…（官方字段路径 <c>trademark.logo_trademark_registration_certificate.certificate</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("certificate")]
    public string? Certificate { get; set; }

    /// <summary>【Logo商标名称】 1、商标注册时登记的名称，可前往 商标备案网 查询； 2、支持输入中文、英文、数字特殊字符； 3、限制50个字符（官方字段路径 <c>trademark.logo_trademark_registration_certificate.name</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【Logo商标注册号】 1、支持输入英文、数字； 2、限制50个字符（官方字段路径 <c>trademark.logo_trademark_registration_certificate.number</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>【Logo商标有效期开始时间】 1、Logo商标注册证上的有效期起始时间； 2、格式要求为： `YYYY-MM-DD`，不支持其他格式。 注意：自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用（官方字段路径 <c>trademark.logo_trademark_registration_certificate.valid_begin_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("valid_begin_time")]
    public string? ValidBeginTime { get; set; }

    /// <summary>【Logo商标有效期结束时间】 1、Logo商标注册证上的有效期截止时间； 2、有效期结束时间必须晚于当前提交申请时间，否则提交将被拒绝； 3、格式要求：请按照标准格式 `YYYY-MM-DD` 填写（如 `2025-12-31`），月份和日期为两位数字，必须为合法日期（官方字段路径 <c>trademark.logo_trademark_registration_certificate.valid_end_time</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("valid_end_time")]
    public string? ValidEndTime { get; set; }

    /// <summary>【Logo商标国际分类】 1、若有上传Logo商标注册证，请填写商标注册证上的商标国际分类，具体类别参考 商标分类表； 2、取值范围为1-45，只支持数字，不支持字母、符号、小数等（官方字段路径 <c>trademark.logo_trademark_registration_certificate.international_class</c>，<b>必填</b>，类型 <c>string(5)</c>）。</summary>
    [JsonPropertyName("international_class")]
    public string? InternationalClass { get; set; }

    /// <summary>【Logo商标注册人】 1、需填写Logo商标注册证上的注册人名称； 2、支持中文、英文、数字、中英文括号，限制50个字符； 3、如果商标注册人与主体名称不一致，需上传“Logo商标许可使用授权书”（官方字段路径 <c>trademark.logo_trademark_registration_certificate.holder</c>，<b>必填</b>，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("holder")]
    public string? Holder { get; set; }

    /// <summary>【Logo商标许可使用授权书】 1、商标注册人为当前主体提供的授权书，商标注册人与主体名称不一致时需填写； 2、支持上传png、jpg格式，最大2MB； 3、请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：该字段仅支持单张图片，不可与 license_list 同时填写。自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用。新接入服务商请使用 license_list 字段，支持多张图片传…（官方字段路径 <c>trademark.logo_trademark_registration_certificate.license</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("license")]
    public string? License { get; set; }

    /// <summary>【Logo商标授权有效期开始时间】 1、Logo商标授权书上的有效期起始时间，商标注册人与主体名称不一致时需填写； 2、格式要求为： `YYYY-MM-DD`，不支持其他格式。 注意：自 2026 年 4 月 15 日起，该字段不再对新增服务商开放；存量已接入服务商可继续正常使用（官方字段路径 <c>trademark.logo_trademark_registration_certificate.authorization_begin_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("authorization_begin_time")]
    public string? AuthorizationBeginTime { get; set; }

    /// <summary>【Logo商标授权有效期结束时间】 1、Logo商标授权书上的有效期截止时间，商标注册人与主体名称不一致时需填写； 2、有效期结束时间必须晚于当前提交申请时间，否则提交将被拒绝； 3、格式要求： 日期填写规范：请按照标准格式 `YYYY-MM-DD` 填写（如 `2025-12-31`），月份和日期为两位数字，必须为合法日期； 有效期限制：授权有效期不得超过商标有效期（官方字段路径 <c>trademark.logo_trademark_registration_certificate.authorization_end_time</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("authorization_end_time")]
    public string? AuthorizationEndTime { get; set; }

    /// <summary>【Logo商标注册证列表】 1、若品牌Logo已注册商标，请参考 示例 上传Logo商标注册证的图片； 2、支持上传png、jpg格式，最大2MB； 3、可上传1-5张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：不可与 certificate 同时填写（官方字段路径 <c>trademark.logo_trademark_registration_certificate.certificate_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("certificate_list")]
    public List<string>? CertificateList { get; set; }

    /// <summary>【Logo商标许可使用授权书列表】 1、商标注册人为当前主体提供的授权书列表，商标注册人与主体名称不一致时需填写； 2、支持上传png、jpg格式，最大2MB； 3、可上传1-5张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：不可与 license 同时填写（官方字段路径 <c>trademark.logo_trademark_registration_certificate.license_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("license_list")]
    public List<string>? LicenseList { get; set; }

}

/// <summary>品牌入驻申请提交请求·【品牌商标资料】 商标相关资料，包括是否有商标、商标注册证、授权书或无商标时的经。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequestTrademark
{
    /// <summary>【有无商标】 1、若使用的品牌名称或logo，已被注册为商标，则需选择有商标并上传相应的商标注册证信息； 2、若选择无商标，则不可使用已注册的商标作为品牌名称或 logo，并需提供证明材料，证明品牌名称命名与企业名称、经营场景（门店、小程序、网页等）对应。可选取值：`TRADEMARK_EXISTS` 有商标 / `TRADEMARK_NONE` 无商标（官方字段路径 <c>trademark.trademark_exists</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("trademark_exists")]
    public string? TrademarkExists { get; set; }

    /// <summary>【商标注册证】 商标注册证相关信息，有商标时必填（官方字段路径 <c>trademark.trademark_registration_certificate</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("trademark_registration_certificate")]
    public BrandApplymentSubmitRequestTrademarkTrademarkRegistrationCertificate? TrademarkRegistrationCertificate { get; set; }

    /// <summary>【Logo商标注册证】 Logo商标注册证相关信息，只有有商标时可填写（官方字段路径 <c>trademark.logo_trademark_registration_certificate</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("logo_trademark_registration_certificate")]
    public BrandApplymentSubmitRequestTrademarkLogoTrademarkRegistrationCertificate? LogoTrademarkRegistrationCertificate { get; set; }

    /// <summary>【无商标额外证明材料】 选择无商标时，不可使用已注册商标作为品牌名称和Logo，需提供证明材料，证明品牌名称命名与企业名称、经营场景（门店、小程序、网页等）对应； 1、请上传可以体现品牌经营场景的图片材料，如店铺门头照、点餐/购物/下单小程序截图、公司官网截图、APP 页面截图等，平台将审核品牌名称与经营场景的对应性； 2、支持上传png、jpg格式，最大2MB； 3、可上传1张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：…（官方字段路径 <c>trademark.no_trademark_addition_prove</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("no_trademark_addition_prove")]
    public string? NoTrademarkAdditionProve { get; set; }

    /// <summary>【无商标额外证明材料列表】 选择无商标时，不可使用已注册商标作为品牌名称和Logo，需提供证明材料列表，证明品牌名称命名与企业名称、经营场景（门店、小程序、网页等）对应； 1、请上传可以体现品牌经营场景的图片材料，如店铺门头照、点餐/购物/下单小程序截图、公司官网截图、APP 页面截图等； 2、支持上传png、jpg格式，最大2MB； 3、可上传1-5张图片，请填写通过 图片上传API 预先上传图片生成好的MediaID。 注意：不可与 no_tradem…（官方字段路径 <c>trademark.no_trademark_addition_prove_list</c>，选填，类型 <c>array[string]</c>）。</summary>
    [JsonPropertyName("no_trademark_addition_prove_list")]
    public List<string>? NoTrademarkAdditionProveList { get; set; }

}

/// <summary>品牌入驻申请提交请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentSubmitRequest
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单； 4、若申请被驳回，可填写相同的“业务申请编号”，即可覆盖修改原申请信息（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【管理员信息】 提交申请单时需填写品牌管理员的个人信息。品牌管理员是负责品牌经营的管理人员，后续将承担权限分配、功能使用等关键管理责任（官方字段路径 <c>admin_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("admin_info")]
    public BrandApplymentSubmitRequestAdminInfo? AdminInfo { get; set; }

    /// <summary>【主体信息】 请提交品牌所属企业的主体信息，包括主体名称、主体类型和统一社会信用代码（官方字段路径 <c>subject_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("subject_info")]
    public BrandApplymentSubmitRequestSubjectInfo? SubjectInfo { get; set; }

    /// <summary>【品牌基础信息】 品牌的基本信息，包括品牌名称和品牌logo，是品牌入驻的必要资料（官方字段路径 <c>brand_basic_info</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("brand_basic_info")]
    public BrandApplymentSubmitRequestBrandBasicInfo? BrandBasicInfo { get; set; }

    /// <summary>【品牌商标资料】 商标相关资料，包括是否有商标、商标注册证、授权书或无商标时的经营证明，用于品牌合法性验证（官方字段路径 <c>trademark</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("trademark")]
    public BrandApplymentSubmitRequestTrademark? Trademark { get; set; }

}

/// <summary>品牌入驻申请提交响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016249989"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentAcceptResponse : WechatPayResponse
{
    /// <summary>【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成； 2、数字，不超过 16 个字符（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

}

/// <summary>品牌入驻申请撤销请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016257700"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentCancelRequest
{
    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单； 4、填写调用 提交入驻申请 时传递的业务申请编号（官方字段路径 <c>business_code</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【微信支付申请单号】 1、微信支付分配的申请单号，在调用 提交入驻申请 时返回； 2、和业务申请编号二选一作为请求参数； 3、由数字组成，长度限制16个字符以内（官方字段路径 <c>applyment_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

}

/// <summary>品牌入驻申请状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016257685"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandApplyment")]
public class BrandApplymentStateResponse : WechatPayResponse
{
    /// <summary>【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成； 2、数字，不超过 16 个字符（官方字段路径 <c>applyment_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("applyment_id")]
    public string? ApplymentId { get; set; }

    /// <summary>【业务申请编号】 1、只能由数字、字母或下划线组成； 2、服务商自定义的商户唯一编号； 3、每个编号对应一个申请单（官方字段路径 <c>business_code</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("business_code")]
    public string? BusinessCode { get; set; }

    /// <summary>【申请状态】 申请单当前所处的审核或处理状态，如审核中、审核通过、已驳回等。可选取值：`APPLYMENT_STATE_UNKNOWN` 「未知状态，非终态」提交申请发生错误导致，请尝试使用相同的业务申请编号重新提交 / `APPLYMENT_STATE_UNSUBMITTED` 「申请未提交，非终态」提交资料需修改，请修改后尝试使用相同的业务申请编号重新提交 / `APPLYMENT_STATE_WAITING_VALIDATE` 「待主体意愿确认，非终态」申请提交后主体意愿确认中 / `APPLYMENT_STATE_WAITING_AUDIT` 「资料审核中，非终态」主体意愿确认完成，资料审核中 / `APPLYMENT_STATE_WAITING_AUTH` 「待授权确认，非终态」请经营管理员使用微信打开返回的“授权链接”，根据页面指引完成授权确认 / `APPLYMENT_STATE_OPEN_ACCOUNT` 「开号中，非终态」系统开通相关权限中，请耐心等待 / `APPLYMENT_STATE_FINISH` 「入驻完成，终态」所有入驻流程均已完成，包括品牌开号、服务商授权等步骤 / `APPLYMENT_STATE_AUDIT_REJECTED` 「审核驳回，非终态」申请单已被驳回，请按照驳回原因修改申请资料，使用相同的业务申请编号重新提交 / `APPLYMENT_STATE_CANCELED` 「申请撤销，终态」入驻申请已被撤销（官方字段路径 <c>applyment_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>【申请状态说明】 由于申请单流程中存在并行处理的阶段，该字段将对“申请状态”做进一步说明：当流程进入并行阶段时，会同时展示并说明所有相关环节的状态（官方字段路径 <c>applyment_state_desc</c>，<b>必填</b>，类型 <c>string(30)</c>）。</summary>
    [JsonPropertyName("applyment_state_desc")]
    public string? ApplymentStateDesc { get; set; }

    /// <summary>【授权确认二维码链接】 注册品牌需管理员扫码检查申请信息，并授权确认。 1、当申请状态为 待主体意愿确认、资料审核中、待授权确认 时才返回； 2、建议将链接转为二维码展示，使用申请单填写的管理员微信进行扫码； 3、确认流程包含检查品牌注册信息、协议签署、服务商授权等，若管理员不是企业法人则需进行主体意愿确认（官方字段路径 <c>authorization_confirmation_qr_code</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("authorization_confirmation_qr_code")]
    public string? AuthorizationConfirmationQrCode { get; set; }

    /// <summary>【驳回原因】 当申请状态是「审核驳回」时返回，此处展示具体的驳回原因，便于申请人修改后重新提交（官方字段路径 <c>reject_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }

    /// <summary>【品牌ID】 1、当申请状态是「入驻完成」时返回，作为品牌唯一标识符，用于后续查询或管理品牌信息； 2、数字，长度不超过 20 个字符（官方字段路径 <c>brand_id</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

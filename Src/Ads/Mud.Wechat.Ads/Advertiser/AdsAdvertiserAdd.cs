// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Advertiser;

/// <summary>
/// <c>POST /v3.0/advertiser/add</c> 的请求体（2026-10-17 官方页逐表核验，13 键）。
/// </summary>
/// <remarks>
/// <para>
/// 表内硬必填只有 <c>system_industry_id</c> / <c>introduction_url</c> 两支；其余字段的「必填」全部是
/// <b>条件必填</b>（官方按 <c>registration_type</c> 分【企业账号】/【个人账号】两套口径，
/// 如 <c>corporation_name</c> 企业必填、个人默认填充姓名）⇒ SDK 不做本地拦截，错配由应答 <c>code</c> 表达。
/// </para>
/// <para>
/// <b>幂等由请求头承担、不在请求体里</b>：官方为本端点单列 <c>X-Request-Id</c> 幂等请求头表，
/// 入口在 <c>IWechatAdsAdvertiserService.AddAsync</c> 的签名上（形态同 <c>adgroups/add</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserAddRequest
{
    /// <summary>服务商账号 id（官方 <c>agency_id</c>，<c>integer</c>，最小值 1；官方原文
    /// 「服务商主体请求时必填、其他 token 忽略此参数」）。</summary>
    [JsonPropertyName("agency_id")]
    public long? AgencyId { get; set; }

    /// <summary>账户主体类型（官方 <c>registration_type</c>，<c>enum</c>；可选值
    /// <c>{ REGISTRATION_TYPE_CORPORATION, REGISTRATION_TYPE_INDIVIDUAL }</c>，默认
    /// <c>REGISTRATION_TYPE_CORPORATION</c>。本参数决定其余字段的必填套路口径）。</summary>
    [JsonPropertyName("registration_type")]
    public string? RegistrationType { get; set; }

    /// <summary>企业名称（官方 <c>corporation_name</c>；1–120 字节；企业账号必填、
    /// 个人账号默认填充个人姓名）。</summary>
    [JsonPropertyName("corporation_name")]
    public string? CorporationName { get; set; }

    /// <summary>企业营业执照注册号（官方 <c>corporation_licence</c>；1–18 字节；仅企业账号使用）。</summary>
    [JsonPropertyName("corporation_licence")]
    public string? CorporationLicence { get; set; }

    /// <summary>营业执照 / 企业资质证明图片 id（官方 <c>certification_image_id</c>；1–64 字节；
    /// 仅企业账号使用；经官方「资质模块」上传获得）。</summary>
    [JsonPropertyName("certification_image_id")]
    public string? CertificationImageId { get; set; }

    /// <summary>身份证明（官方 <c>individual_qualification</c>，<c>struct</c>；
    /// 企业账号=法人身份证明、个人账号=个人身份证明，两类账号均必填本结构）。</summary>
    [JsonPropertyName("individual_qualification")]
    public AdsIndividualQualification? IndividualQualification { get; set; }

    /// <summary>公司所在地（官方 <c>area_code</c>，<c>integer</c>；仅企业账号使用，
    /// 官方原文「需与营业执照注册地域一致」，具体值参照附录「地域信息」）。</summary>
    [JsonPropertyName("area_code")]
    public long? AreaCode { get; set; }

    /// <summary>开户行业 id（官方 <c>system_industry_id</c>，<b>必填</b>，<c>integer</c>；
    /// 须填<b>二级行业</b> id，取值见官方「行业分类」）。</summary>
    [JsonPropertyName("system_industry_id")]
    public long? SystemIndustryId { get; set; }

    /// <summary>业务介绍页地址（官方 <c>introduction_url</c>，<b>必填</b>；1–255 字节；
    /// 可填公司网站 / APP 下载页 / H5 链接，作为开户信息参考）。</summary>
    [JsonPropertyName("introduction_url")]
    public string? IntroductionUrl { get; set; }

    /// <summary>竞价投放账户日预算（官方 <c>daily_budget</c>，<c>integer</c>，单位为分；
    /// 区间 5,000–4,000,000,000 分、0 表示不限；默认值 1,000,000。仅对竞价投放生效）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>品牌名称（官方 <c>corporate_brand_name</c>；0–256 字节）。</summary>
    [JsonPropertyName("corporate_brand_name")]
    public string? CorporateBrandName { get; set; }

    /// <summary>是否为 ADX 程序化投放账号（官方 <c>is_adx</c>，<c>boolean</c>；可选值 <c>{ true, false }</c>）。</summary>
    [JsonPropertyName("is_adx")]
    public bool? IsAdx { get; set; }

    /// <summary>联系人座机（官方 <c>contact_person_telephone</c>；0–20 字节；
    /// 格式「区号-座机号」，如 <c>0755-8601338</c>）。</summary>
    [JsonPropertyName("contact_person_telephone")]
    public string? ContactPersonTelephone { get; set; }

    /// <summary>联系人手机（官方 <c>contact_person_mobile</c>；0–20 字节；
    /// 如 <c>+8613900000000</c> 或 <c>13900000000</c>）。</summary>
    [JsonPropertyName("contact_person_mobile")]
    public string? ContactPersonMobile { get; set; }
}

/// <summary><c>advertiser/add</c> 的 <c>data</c> 载荷（官方只回新建的账户 id）。</summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserAddData
{
    /// <summary>新建账户 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }
}

/// <summary><c>POST /v3.0/advertiser/add</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserAddResponse : AdsResponse<AdsAdvertiserAddData>
{
}

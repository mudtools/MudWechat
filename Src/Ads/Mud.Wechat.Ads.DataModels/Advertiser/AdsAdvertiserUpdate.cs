// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Advertiser;

/// <summary>
/// 更新客户信息（<c>POST /v3.0/advertiser/update</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段集与 <c>get</c> 应答不同</b>（2026-10-10 逐页核验）：本页字段表<b>不含</b>
/// <c>memo</c> / <c>contact_person</c> / <c>contact_person_email</c> / <c>registration_type</c> /
/// <c>mdm_*</c> / <c>customized_industry</c> / <c>is_adx</c> / <c>agency_account_id</c> / <c>operators</c>
/// ⇒ 本类只声明官方给出的可改字段，<b>不得</b>「照 get 的形态补全」（多传字段官方行为未定义，
/// 且会让 SDK 看起来支持实际不存在的修改能力）。
/// </para>
/// <para>
/// <b>只 <c>account_id</c> 标必填</b>，其余按「不传即不改」的官方局部更新语义留空 ⇒
/// 全部可空 + <c>WhenWritingNull</c> 省略，序列化结果天然是一份补丁报文。
/// </para>
/// <para>
/// <b>长度与取值上限一律不在 SDK 拦截</b>（官方逐字段给出字节上限，如
/// <c>corporation_name</c> ≤120 字节、<c>corporation_licence</c> ≤18 字节、
/// <c>certification_image_id</c> ≤64 字节、<c>introduction_url</c> ≤255 字节、
/// <c>corporate_brand_name</c> / <c>business_alias</c> ≤256 字节、
/// <c>contact_person_telephone</c> / <c>contact_person_mobile</c> ≤20 字节）——
/// 与企微线同一处置：越界由应答 <c>code</c> 表达，本地拦截会挡掉官方实际接受的边界值。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<b>必填</b>，<c>integer</c>；原文「有操作权限的帐号 id，<b>不支持代理商 id</b>」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>
    /// 竞价投放账户日预算（官方 <c>daily_budget</c>，<c>integer</c>，单位为分，<c>0</c> 表示不设预算）。
    /// 官方四条约束（区间、每次修改幅度、微信公众号平台小程序账户的提高幅度、不得低于今日已消耗）
    /// 写在 <c>advertiser/update</c> 端点的接口 XML remarks（守卫 ADS-B2 锁定），本处不重复。
    /// </summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>企业名称（官方 <c>corporation_name</c>；个人账号不使用）。</summary>
    [JsonPropertyName("corporation_name")]
    public string? CorporationName { get; set; }

    /// <summary>企业营业执照注册号（官方 <c>corporation_licence</c>；个人账号不使用）。</summary>
    [JsonPropertyName("corporation_licence")]
    public string? CorporationLicence { get; set; }

    /// <summary>营业执照 / 企业资质证明图片 id（官方 <c>certification_image_id</c>，需先经资质模块上传取得）。</summary>
    [JsonPropertyName("certification_image_id")]
    public string? CertificationImageId { get; set; }

    /// <summary>身份证明（官方 <c>individual_qualification</c>；本字段一旦出现，其 <c>name</c> 与 <c>identification_number</c> 官方标必填）。</summary>
    [JsonPropertyName("individual_qualification")]
    public AdsIndividualQualification? IndividualQualification { get; set; }

    /// <summary>公司所在地（官方 <c>area_code</c>，<c>integer</c>；原文要求「需与营业执照注册地域一致」）。</summary>
    [JsonPropertyName("area_code")]
    public long? AreaCode { get; set; }

    /// <summary>开户行业 id（官方 <c>system_industry_id</c>，<c>integer</c>；原文「请填写<b>二级</b>行业 id」）。</summary>
    [JsonPropertyName("system_industry_id")]
    public long? SystemIndustryId { get; set; }

    /// <summary>业务介绍页地址（官方 <c>introduction_url</c>，可填公司网站 / APP 下载页 / H5 链接）。</summary>
    [JsonPropertyName("introduction_url")]
    public string? IntroductionUrl { get; set; }

    /// <summary>品牌名称（官方 <c>corporate_brand_name</c>）。</summary>
    [JsonPropertyName("corporate_brand_name")]
    public string? CorporateBrandName { get; set; }

    /// <summary>联系人座机（官方 <c>contact_person_telephone</c>，格式「区号-座机号」）。</summary>
    [JsonPropertyName("contact_person_telephone")]
    public string? ContactPersonTelephone { get; set; }

    /// <summary>联系人手机（官方 <c>contact_person_mobile</c>）。</summary>
    [JsonPropertyName("contact_person_mobile")]
    public string? ContactPersonMobile { get; set; }

    /// <summary>客户工作台账号标签（官方 <c>business_alias</c>）。</summary>
    [JsonPropertyName("business_alias")]
    public string? BusinessAlias { get; set; }

    /// <summary>
    /// 推广链接列表（官方 <c>websites</c>，<c>struct[]</c>，数组长度 0–255）。
    /// <b>覆盖式更新</b>：官方把本字段作为整体提交（元素内 <c>website_domain</c> / <c>icp_image_id</c> 均标必填），
    /// 不是增量追加 ⇒ 传空数组与不传在官方语义上不同（前者清空、后者不改），SDK 不做本地推断。
    /// </summary>
    [JsonPropertyName("websites")]
    public List<AdsAdvertiserWebsite>? Websites { get; set; }
}

/// <summary>
/// <c>advertiser/update</c> 的 <c>data</c> 载荷。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserUpdateData
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>；官方示例值为占位符字符串，见 <see cref="AdsAdvertiserInfo"/> 的矛盾记录）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }
}

/// <summary>
/// <c>advertiser/update</c> 应答信封（闭合类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserUpdateResponse : AdsResponse<AdsAdvertiserUpdateData>
{
}

/// <summary>
/// 批量修改日预算（<c>POST /v3.0/advertiser/update_daily_budget</c>）的单条规格（官方 <c>update_daily_budget_spec</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方：<c>update_daily_budget_spec</c> 标<b>必填</b>且数组最大长度 <b>100</b>；
/// 元素内 <c>account_id</c> / <c>daily_budget</c> 均标必填，<c>use_min_daily_budget</c> 可选、默认 <c>false</c>。
/// </para>
/// <para>
/// <b><c>use_min_daily_budget</c> 的反直觉语义（官方原文要点，勿弱化）</b>：
/// 「因账户消耗 / 延迟扣费等原因下调失败时是否自动设置为系统允许最小值，
/// 当触发自动设置功能时，可能出现『期望下调、实际上调』的情况」⇒ 该开关会让结果与请求方向相反，
/// 返回值以应答元素的 <c>daily_budget</c> 为准，而非请求值。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsUpdateDailyBudgetSpec
{
    /// <summary>账户 id（官方 <c>account_id</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>日预算（官方 <c>daily_budget</c>，<b>必填</b>，单位为分，<c>0</c> 表示不限预算）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>下调失败时是否自动设为系统允许最小值（官方 <c>use_min_daily_budget</c>，<c>boolean</c>，不填默认 <c>false</c>）。</summary>
    [JsonPropertyName("use_min_daily_budget")]
    public bool? UseMinDailyBudget { get; set; }
}

/// <summary>
/// 批量修改日预算的<b>逐条</b>结果（官方 <c>data.list[]</c> 元素，继承 <see cref="AdsBatchResultItem"/> 的三字段）。
/// </summary>
/// <remarks>
/// <b>双层失败语义</b>：外层信封 <c>code == 0</c> 只表示「请求被受理」，<b>每条</b>成败看本元素 <c>code</c>，
/// 批量项部分失败时外层仍为 <c>0</c>（另有 <c>fail_id_list</c> 汇总）⇒ 判错必须两层都做（守卫 ADS-B2）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserDailyBudgetResultItem : AdsBatchResultItem
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>实际生效的日预算（官方 <c>daily_budget</c>，单位为分；与请求值可能不同，见 <see cref="AdsUpdateDailyBudgetSpec.UseMinDailyBudget"/>）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>是否使用了系统允许最小值（官方 <c>use_min_daily_budget</c>；原文「触发自动设置时返回 true」）。</summary>
    [JsonPropertyName("use_min_daily_budget")]
    public bool? UseMinDailyBudget { get; set; }
}

/// <summary>
/// <c>advertiser/update_daily_budget</c> 的 <c>data</c> 载荷。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserDailyBudgetData
{
    /// <summary>逐条结果列表（官方 <c>list</c>，<c>struct[]</c>，每条自带 <c>code</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAdvertiserDailyBudgetResultItem>? List { get; set; }

    /// <summary>失败的账户 id 集合（官方 <c>fail_id_list</c>，<c>integer[]</c>；与逐条 <c>code</c> 是两个判定面）。</summary>
    [JsonPropertyName("fail_id_list")]
    public List<long>? FailIdList { get; set; }
}

/// <summary>
/// 批量修改日预算请求体（官方请求根只有一个字段 <c>update_daily_budget_spec</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserUpdateDailyBudgetRequest
{
    /// <summary>批量规格列表（官方 <c>update_daily_budget_spec</c>，<b>必填</b>，数组最大长度 100）。</summary>
    [JsonPropertyName("update_daily_budget_spec")]
    public List<AdsUpdateDailyBudgetSpec>? UpdateDailyBudgetSpec { get; set; }
}

/// <summary>
/// <c>advertiser/update_daily_budget</c> 应答信封（闭合类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserUpdateDailyBudgetResponse : AdsResponse<AdsAdvertiserDailyBudgetData>
{
}

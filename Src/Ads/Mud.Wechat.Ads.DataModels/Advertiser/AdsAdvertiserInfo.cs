// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Advertiser;

/// <summary>
/// <c>advertiser/get</c> 的 <c>data</c> 载荷（列表 + 两种分页元信息）。
/// </summary>
/// <remarks>
/// <b>两种分页形态同时在场</b>：官方把 <c>page_info</c>（普通翻页）与 <c>cursor_page_info</c>（游标翻页）
/// 列在同一个 <c>data</c> 下，各自「按请求的 <c>pagination_mode</c> 返回」⇒ 两者均建为可空，
/// 调用方按自己请求的模式取对应一支。应答示例里 <c>cursor_page_info</c> 甚至只出现
/// <c>{page_size, total_number}</c>（无 <c>has_more</c> / <c>cursor</c>）⇒ 全部字段可空。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserListData
{
    /// <summary>客户信息列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAdvertiserInfo>? List { get; set; }

    /// <summary>普通翻页模式的分页信息（官方 <c>page_info</c>；游标模式不返回）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }

    /// <summary>游标翻页模式的分页信息（官方 <c>cursor_page_info</c>；普通模式不返回）。</summary>
    [JsonPropertyName("cursor_page_info")]
    public AdsAdvertiserCursorPageInfo? CursorPageInfo { get; set; }
}

/// <summary>
/// <c>advertiser/get</c> 应答信封（闭合类型，满足 <c>AdsResponse&lt;T&gt;</c> 不得直接作返回类型的约束）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserGetResponse : AdsResponse<AdsAdvertiserListData>
{
}

/// <summary>
/// 客户（广告主账号）信息（<c>advertiser/get</c> 的 <c>data.list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 字段名与类型照官方字段表（2026-10-10 逐页核验）：官方标 <c>integer</c> 的建为 <see cref="long"/>、
/// <c>enum</c> 的以 <see cref="string"/> 承载（官方枚举集会随权限变化，不做本地枚举）。
/// </para>
/// <para>
/// <b>官方示例与其字段表自相矛盾之处（照录，SDK 不替官方「修正」）</b>：
/// ① <c>area_code</c> 字段表标 <c>integer</c>，示例却写成字符串 <c>"110100"</c> ⇒ 本类型按字段表取
/// <see cref="long"/>，但官方示例的字符串形态在 <c>System.Text.Json</c> 源生成口径下
/// 由 <c>NumberHandling</c> 决定能否读取 ⇒ 若线上出现字符串形态，属官方报文缺陷，需改型而非本地兼容层；
/// ② <c>account_id</c> 字段表标 <c>integer</c>，示例值是占位符字符串 <c>"&lt;ACCOUNT_ID&gt;"</c>；
/// ③ <c>individual_qualification</c> 字段表只有 <c>name</c> / <c>identification_number</c> 两字段，
/// 示例却额外出现 <c>identification_front_image_id</c> / <c>identification_back_image_id</c>
/// ⇒ 本类型按示例建为四字段超集（缺字段会让官方已返回的数据无处安放）。
/// </para>
/// <para>
/// <b>部分字段依主体类型出现</b>：官方逐字段标注【企业账号】/【个人账号】适用性
/// （如 <c>corporation_licence</c> / <c>certification_image*</c> / <c>area_code</c> 个人账号不使用）⇒
/// 全部可空，SDK 不做主体类型判定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserInfo
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>竞价投放账户日预算（官方 <c>daily_budget</c>，<c>integer</c>，单位为分；<c>0</c> 表示不设预算）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>账户主体类型（官方 <c>registration_type</c>，<c>enum</c>，官方「枚举详情」链接给出取值集）。</summary>
    [JsonPropertyName("registration_type")]
    public string? RegistrationType { get; set; }

    /// <summary>企业名称 / 个人姓名（官方 <c>corporation_name</c>）。</summary>
    [JsonPropertyName("corporation_name")]
    public string? CorporationName { get; set; }

    /// <summary>企业营业执照注册号（官方 <c>corporation_licence</c>；个人账号不使用）。</summary>
    [JsonPropertyName("corporation_licence")]
    public string? CorporationLicence { get; set; }

    /// <summary>营业执照 / 企业资质证明图片 id（官方 <c>certification_image_id</c>；个人账号不使用）。</summary>
    [JsonPropertyName("certification_image_id")]
    public string? CertificationImageId { get; set; }

    /// <summary>营业执照 / 企业资质证明图片 URL（官方 <c>certification_image</c>；个人账号不使用）。</summary>
    [JsonPropertyName("certification_image")]
    public string? CertificationImage { get; set; }

    /// <summary>身份证明（官方 <c>individual_qualification</c>，<c>struct</c>；企业为法人身份证明、个人为本人身份证明）。</summary>
    [JsonPropertyName("individual_qualification")]
    public AdsIndividualQualification? IndividualQualification { get; set; }

    /// <summary>所在地（官方 <c>area_code</c>，<c>integer</c>，取值见官方「地域信息」附录）。</summary>
    [JsonPropertyName("area_code")]
    public long? AreaCode { get; set; }

    /// <summary>客户主体 id（官方 <c>mdm_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("mdm_id")]
    public long? MdmId { get; set; }

    /// <summary>客户主体名称（官方 <c>mdm_name</c>）。</summary>
    [JsonPropertyName("mdm_name")]
    public string? MdmName { get; set; }

    /// <summary>行业 id（官方 <c>system_industry_id</c>，<c>integer</c>，取值见官方「行业分类」）。</summary>
    [JsonPropertyName("system_industry_id")]
    public long? SystemIndustryId { get; set; }

    /// <summary>调用方行业名称（官方 <c>customized_industry</c>；官方仅此一短语，语义照录）。</summary>
    [JsonPropertyName("customized_industry")]
    public string? CustomizedIndustry { get; set; }

    /// <summary>业务介绍页地址（官方 <c>introduction_url</c>，作为开户信息参考）。</summary>
    [JsonPropertyName("introduction_url")]
    public string? IntroductionUrl { get; set; }

    /// <summary>品牌名称（官方 <c>corporate_brand_name</c>）。</summary>
    [JsonPropertyName("corporate_brand_name")]
    public string? CorporateBrandName { get; set; }

    /// <summary>账户备注（官方 <c>memo</c>）。</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    /// <summary>客户系统状态（官方 <c>system_status</c>，<c>enum</c>，示例值 <c>CUSTOMER_STATUS_NORMAL</c>）。</summary>
    [JsonPropertyName("system_status")]
    public string? SystemStatus { get; set; }

    /// <summary>审核消息（官方 <c>reject_message</c>）。</summary>
    [JsonPropertyName("reject_message")]
    public string? RejectMessage { get; set; }

    /// <summary>是否为 ADX 程序化投放账号（官方 <c>is_adx</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_adx")]
    public bool? IsAdx { get; set; }

    /// <summary>客户工作台账号标签（官方 <c>business_alias</c>）。</summary>
    [JsonPropertyName("business_alias")]
    public string? BusinessAlias { get; set; }

    /// <summary>联系人姓名（官方 <c>contact_person</c>）。</summary>
    [JsonPropertyName("contact_person")]
    public string? ContactPerson { get; set; }

    /// <summary>联系人 email（官方 <c>contact_person_email</c>）。</summary>
    [JsonPropertyName("contact_person_email")]
    public string? ContactPersonEmail { get; set; }

    /// <summary>联系人座机（官方 <c>contact_person_telephone</c>，格式「区号-座机号」）。</summary>
    [JsonPropertyName("contact_person_telephone")]
    public string? ContactPersonTelephone { get; set; }

    /// <summary>联系人手机（官方 <c>contact_person_mobile</c>，如 <c>+8613900000000</c> 或 <c>13900000000</c>）。</summary>
    [JsonPropertyName("contact_person_mobile")]
    public string? ContactPersonMobile { get; set; }

    /// <summary>推广链接列表（官方 <c>websites</c>，<c>struct[]</c>；官方原文「当且仅当输入参数 account_id 不为空时有效」）。</summary>
    [JsonPropertyName("websites")]
    public List<AdsAdvertiserWebsite>? Websites { get; set; }

    /// <summary>代理商 account_id（官方 <c>agency_account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("agency_account_id")]
    public long? AgencyAccountId { get; set; }

    /// <summary>运营人员列表（官方 <c>operators</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("operators")]
    public List<AdsAdvertiserOperator>? Operators { get; set; }
}

/// <summary>
/// 身份证明（官方 <c>individual_qualification</c>，<c>advertiser/get</c> 与 <c>advertiser/update</c> 共用一支）。
/// </summary>
/// <remarks>
/// <b>共用而非分建的理由</b>：两页字段表的交集恰为 <c>name</c> + <c>identification_number</c> 两字段
/// （update 页另标二者「必填」），而两页的<b>示例报文</b>都出现四个字段 ⇒ 一份四字段超集可同时承载两侧，
/// 分建两支只会让「同一官方结构」在 SDK 里有两个名字。<b>必填性差异写在请求侧字段注释</b>，不做本地拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsIndividualQualification
{
    /// <summary>姓名（官方 <c>name</c>；请求侧 1–64 字节）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>身份证号码（官方 <c>identification_number</c>；请求侧 1–64 字节）。</summary>
    [JsonPropertyName("identification_number")]
    public string? IdentificationNumber { get; set; }

    /// <summary>身份证正面图片 id（官方示例中的 <c>identification_front_image_id</c>；字段表未列出，见 <see cref="AdsAdvertiserInfo"/> 的矛盾记录）。</summary>
    [JsonPropertyName("identification_front_image_id")]
    public string? IdentificationFrontImageId { get; set; }

    /// <summary>身份证反面图片 id（官方示例中的 <c>identification_back_image_id</c>；字段表未列出）。</summary>
    [JsonPropertyName("identification_back_image_id")]
    public string? IdentificationBackImageId { get; set; }
}

/// <summary>
/// 推广链接（官方 <c>websites</c> 数组元素，<c>get</c> 与 <c>update</c> 共用一支）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两页字段集不同，取并集</b>：<c>get</c> 返回
/// <c>{website_domain, icp_image_id, system_status, reject_message}</c>，
/// <c>update</c> 只接受 <c>{website_domain*, icp_image_id*}</c>（二者请求侧必填）。
/// 请求时后两字段留空即被序列化器的 <c>WhenWritingNull</c> 省略，不会造成多余字段。
/// </para>
/// <para>官方（<c>update</c>）：<c>websites</c> 数组最小长度 0、最大长度 255；
/// <c>website_domain</c> ≤255 字节且<b>无需 http 前缀</b>（原文示例 <c>www.qq.com</c>）；
/// <c>icp_image_id</c> ≤64 字节，需先经 images 模块上传取得。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserWebsite
{
    /// <summary>推广链接域名（官方 <c>website_domain</c>，无需 http 前缀）。</summary>
    [JsonPropertyName("website_domain")]
    public string? WebsiteDomain { get; set; }

    /// <summary>网站 ICP 备案证书扫描图片 id（官方 <c>icp_image_id</c>）。</summary>
    [JsonPropertyName("icp_image_id")]
    public string? IcpImageId { get; set; }

    /// <summary>推广链接系统状态（官方 <c>system_status</c>，<c>enum</c>，示例值 <c>WEBSITE_STATUS_DENIED</c>；仅 <c>get</c> 返回）。</summary>
    [JsonPropertyName("system_status")]
    public string? SystemStatus { get; set; }

    /// <summary>审核消息（官方 <c>reject_message</c>；仅 <c>get</c> 返回）。</summary>
    [JsonPropertyName("reject_message")]
    public string? RejectMessage { get; set; }
}

/// <summary>
/// 运营人员（官方 <c>operators</c> 数组元素，仅 <c>advertiser/get</c> 返回）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserOperator
{
    /// <summary>运营人员 id（官方 <c>operator_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("operator_id")]
    public long? OperatorId { get; set; }

    /// <summary>运营人员名称（官方 <c>operator_name</c>）。</summary>
    [JsonPropertyName("operator_name")]
    public string? OperatorName { get; set; }

    /// <summary>登录 QQ 号码（官方 <c>qq</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("qq")]
    public long? Qq { get; set; }

    /// <summary>微信账号 id（官方 <c>wechat_account_id</c>，官方标 <c>string</c> 而非 integer）。</summary>
    [JsonPropertyName("wechat_account_id")]
    public string? WechatAccountId { get; set; }

    /// <summary>是否主要运营人员（官方 <c>is_master</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_master")]
    public bool? IsMaster { get; set; }
}

/// <summary>
/// <c>advertiser/get</c> 的游标翻页信息（官方 <c>cursor_page_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类型为「游标形态逐域分建」规则的第一个实例</b>：官方在本页给出
/// <c>{page_size, total_number, has_more, cursor}</c> 且 <c>cursor</c> 为 <c>integer</c>，
/// 而其他 <c>*/get</c> 页的 <c>cursor_page_info</c> 字段集与之不同 ⇒ <b>不得</b>收敛成公共一支
/// （收敛会把某一页的字段名当成全页事实，是最难察觉的一类契约错误）。各域各建一支，由守卫 ADS-B2 逐页锁定。
/// </para>
/// <para>官方请求侧约束（同页）：游标有效期 24 小时，且游标模式下除分页参数外的请求参数须与上一次调用完全一致。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Advertiser")]
public class AdsAdvertiserCursorPageInfo
{
    /// <summary>一页显示的数据条数（官方 <c>page_size</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>总条数（官方 <c>total_number</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("total_number")]
    public long? TotalNumber { get; set; }

    /// <summary>是否有下一页（官方 <c>has_more</c>，<c>boolean</c>；原文「返回 false 表示已无下一页，此时务必停止拉取」）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>下一次拉取使用的游标（官方 <c>cursor</c>，<c>integer</c>，最小值 1）。</summary>
    [JsonPropertyName("cursor")]
    public long? Cursor { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>
/// 审核材料（官方 <c>audit_info</c> 对象——<c>getbizcatflowlist</c>/<c>getbizcatflowdetail</c>/，
/// <c>get_category_relation_detail</c> 三处字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <b>官方历史拼写照录</b>：本响应对象中经营平台字段拼作 <c>jingyingpingtai</c>（多一个 g），
/// 与「申请类目」请求体中的 <c>jingyingpintai</c> 拼写不一致，照抄官方原文、勿统一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAuditInfo
{
    /// <summary>获取或设置类目 id（官方 <c>level3</c>）。</summary>
    [JsonPropertyName("level3")]
    public long? Level3 { get; set; }

    /// <summary>
    /// 获取或设置资质资料（官方 <c>certificate</c>）——官方参数表仅标 array、未标注元素结构，
    /// SDK 按 file_id 字符串列表建模。
    /// </summary>
    [JsonPropertyName("certificate")]
    public List<string>? Certificate { get; set; }

    /// <summary>获取或设置品牌列表（官方 <c>brand_list</c>）。</summary>
    [JsonPropertyName("brand_list")]
    public List<ChannelsCategoryBrandRef>? BrandList { get; set; }

    /// <summary>获取或设置报备函（官方 <c>baobeihan</c>）。</summary>
    [JsonPropertyName("baobeihan")]
    public List<string>? Baobeihan { get; set; }

    /// <summary>获取或设置经营证明（官方 <c>jingyingzhengming</c>）。</summary>
    [JsonPropertyName("jingyingzhengming")]
    public List<string>? Jingyingzhengming { get; set; }

    /// <summary>获取或设置经营平台（官方 <c>jingyingpingtai</c>——官方响应侧历史拼写照录，与请求侧 <c>jingyingpintai</c> 差一个 g，勿统一）。</summary>
    [JsonPropertyName("jingyingpingtai")]
    public string? Jingyingpingtai { get; set; }

    /// <summary>获取或设置帐号名称（官方 <c>zhanghaomingcheng</c>）。</summary>
    [JsonPropertyName("zhanghaomingcheng")]
    public string? Zhanghaomingcheng { get; set; }

    /// <summary>获取或设置入驻证明（官方 <c>ruzhuzhizhi</c>）。</summary>
    [JsonPropertyName("ruzhuzhizhi")]
    public List<string>? Ruzhuzhizhi { get; set; }

    /// <summary>获取或设置带货口碑（官方 <c>daihuokoubei</c>）。</summary>
    [JsonPropertyName("daihuokoubei")]
    public List<string>? Daihuokoubei { get; set; }

    /// <summary>获取或设置经营流水（官方 <c>jingyingliushui</c>）。</summary>
    [JsonPropertyName("jingyingliushui")]
    public List<string>? Jingyingliushui { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>buchongcailiao</c>）。</summary>
    [JsonPropertyName("buchongcailiao")]
    public List<string>? Buchongcailiao { get; set; }

    /// <summary>获取或设置证照信息（官方 <c>license_group_list</c>）。</summary>
    [JsonPropertyName("license_group_list")]
    public List<ChannelsCategoryLicenseGroup>? LicenseGroupList { get; set; }

    /// <summary>获取或设置承诺函（官方 <c>commitment_letter_list</c>）。</summary>
    [JsonPropertyName("commitment_letter_list")]
    public List<string>? CommitmentLetterList { get; set; }

    /// <summary>获取或设置账号链接（官方 <c>account_link</c>）。</summary>
    [JsonPropertyName("account_link")]
    public string? AccountLink { get; set; }

    /// <summary>获取或设置是否为新版证照组申请流程（官方 <c>is_new_apply_cat</c>）。</summary>
    [JsonPropertyName("is_new_apply_cat")]
    public bool? IsNewApplyCat { get; set; }
}

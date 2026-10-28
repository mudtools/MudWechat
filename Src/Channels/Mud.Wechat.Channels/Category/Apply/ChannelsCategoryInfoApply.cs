// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>申请类目信息（官方 <c>category_info</c> 对象，<c>addcategory</c>）。</summary>
/// <remarks>
/// <b>类目树双轨</b>：设了 <c>cats_v2</c> 则优先读取（多级结构）；未设则用 <c>level1/level2/level3</c>（旧三级结构）；
/// <c>cats_v2</c> 数组下标顺序与一至 N 级类目严格一致。
/// <b>官方参数表标「certificate 必填」但同时注明已废弃</b>，须改用证照组 <c>license_group_list</c>
/// （<c>is_new_apply_cat</c> 须设为 true）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryInfoApply
{
    /// <summary>获取或设置一级类目 ID（官方 <c>level1</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level1")]
    public long Level1 { get; set; }

    /// <summary>获取或设置二级类目 ID（官方 <c>level2</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level2")]
    public long Level2 { get; set; }

    /// <summary>获取或设置三级类目 ID（官方 <c>level3</c>，必填；未设 <c>cats_v2</c> 时生效）。</summary>
    [JsonPropertyName("level3")]
    public long Level3 { get; set; }

    /// <summary>
    /// 获取或设置类目树（官方 <c>cats_v2</c>，新多级类目）——官方参数表标必填，但官方注意事项写明
    /// 「未设则用 level1/level2/level3（旧三级结构）」（文档矛盾照录，SDK 按注意事项建模为可空）。
    /// </summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsCategoryTreeNode>? CatsV2 { get; set; }

    /// <summary>
    /// 获取或设置资质材料（官方 <c>certificate</c>，官方标必填但注明「已废弃，须改用证照组」；
    /// 官方参数表仅标 array、描述为「图片fileid+类型」，SDK 按 file_id 字符串列表建模）。
    /// </summary>
    [JsonPropertyName("certificate")]
    public List<string>? Certificate { get; set; }

    /// <summary>获取或设置报备函（官方 <c>baobeihan</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("baobeihan")]
    public List<string>? Baobeihan { get; set; }

    /// <summary>获取或设置经营证明（官方 <c>jingyingzhengming</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("jingyingzhengming")]
    public List<string>? Jingyingzhengming { get; set; }

    /// <summary>获取或设置带货口碑（官方 <c>daihuokoubei</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("daihuokoubei")]
    public List<string>? Daihuokoubei { get; set; }

    /// <summary>获取或设置入驻资质（官方 <c>ruzhuzhizhi</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("ruzhuzhizhi")]
    public List<string>? Ruzhuzhizhi { get; set; }

    /// <summary>获取或设置经营流水（官方 <c>jingyingliushui</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("jingyingliushui")]
    public List<string>? Jingyingliushui { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>buchongcailiao</c>，选填，最多 10 张）。</summary>
    [JsonPropertyName("buchongcailiao")]
    public List<string>? Buchongcailiao { get; set; }

    /// <summary>
    /// 获取或设置经营平台（官方 <c>jingyingpintai</c>——官方历史拼写照录，少一个 g，
    /// 审核单详情响应中同义字段拼作 <c>jingyingpingtai</c>，两者勿统一）；
    /// 仅支持 taobao/jd/douyin/kuaishou/pdd/other。
    /// </summary>
    [JsonPropertyName("jingyingpintai")]
    public string? Jingyingpintai { get; set; }

    /// <summary>获取或设置账号名称（官方 <c>zhanghaomingcheng</c>，选填）。</summary>
    [JsonPropertyName("zhanghaomingcheng")]
    public string? Zhanghaomingcheng { get; set; }

    /// <summary>获取或设置品牌列表（官方 <c>brand_list</c>，选填；当类目 <c>attr.is_limit_brand</c> 为 true 时必传）。</summary>
    [JsonPropertyName("brand_list")]
    public List<ChannelsCategoryBrandRef>? BrandList { get; set; }

    /// <summary>获取或设置证照组列表（官方 <c>license_group_list</c>，必填）。</summary>
    [JsonPropertyName("license_group_list")]
    public List<ChannelsCategoryLicenseGroup> LicenseGroupList { get; set; } = new();

    /// <summary>获取或设置是否为新版证照组申请（官方 <c>is_new_apply_cat</c>，必填；官方要求须设为 true 并用证照组提交）。</summary>
    [JsonPropertyName("is_new_apply_cat")]
    public bool IsNewApplyCat { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Ads.DataModels.Common;
using Mud.Wechat.Ads.DataModels.DynamicCreatives;

namespace Mud.Wechat.Ads.DataModels.Components;

/// <summary>
/// 组件详情（官方 <c>component_detail/get</c> 应答的 <c>component_detail</c>，2026-10-11 L3 核验，4 键）。
/// </summary>
/// <remarks>
/// 四键的内部形状深浅不一（<c>jump_info</c> 含 20+ spec union、<c>image_list</c> / <c>video_list</c>
/// 为组件条目数组、<c>brand</c> 为品牌值结构）：数组键按组件条目同构建型
/// （<see cref="AdsCreativeComponentItem"/>，元素 <c>{component_id, value, is_deleted}</c> 形状一致），
/// 开放 union 键以字典承载（D2 裁剪）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDetail
{
    /// <summary>跳转信息（官方 <c>jump_info</c>；<c>page_type</c> + 20+ spec union 开放承载）。</summary>
    [JsonPropertyName("jump_info")]
    public Dictionary<string, JsonElement>? JumpInfo { get; set; }

    /// <summary>图片列表（官方 <c>image_list</c>，组件条目数组）。</summary>
    [JsonPropertyName("image_list")]
    public List<AdsCreativeComponentItem>? ImageList { get; set; }

    /// <summary>视频列表（官方 <c>video_list</c>，组件条目数组）。</summary>
    [JsonPropertyName("video_list")]
    public List<AdsCreativeComponentItem>? VideoList { get; set; }

    /// <summary>品牌信息（官方 <c>brand</c>）。</summary>
    [JsonPropertyName("brand")]
    public Dictionary<string, JsonElement>? Brand { get; set; }
}

/// <summary><c>component_detail/get</c> 的 <c>data.list[]</c> 元素。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDetailInfo
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>组织 id（官方 <c>organization_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    /// <summary>组件 id（官方 <c>component_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("component_id")]
    public long? ComponentId { get; set; }

    /// <summary>组件详情（官方 <c>component_detail</c>，4 键）。</summary>
    [JsonPropertyName("component_detail")]
    public AdsComponentDetail? ComponentDetail { get; set; }
}

/// <summary><c>component_detail/get</c> 的 <c>data</c> 载荷。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDetailListData
{
    /// <summary>组件详情列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsComponentDetailInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/component_detail/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Components")]
public class AdsComponentDetailGetResponse : AdsResponse<AdsComponentDetailListData>
{
}

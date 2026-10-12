// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 地图点位（官方 <c>data.item[]</c>）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：<c>pic_urls</c> / <c>card_id_list</c> 仅标注为 array、未给出元素类型
/// （SDK 以 <c>List&lt;string&gt;</c> 承载）；<c>data_supply</c> 语义未定义（仅标注「地图数据」）。
/// <b><c>sosomap_poi_uid</c> 是 <c>add_store</c> 的 <c>map_poi_id</c> 取值来源</b>（官方原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMapPoiItem
{
    /// <summary>获取或设置门店名称（官方 <c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>获取或设置地址描述（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置经度（官方 <c>longitude</c>，number）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>获取或设置纬度（官方 <c>latitude</c>，number）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置电话号码（官方 <c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>获取或设置类目（官方 <c>category</c>）。</summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>获取或设置地图点位 id（官方 <c>sosomap_poi_uid</c>；<c>add_store</c> 的 <c>map_poi_id</c> 即取此值）。</summary>
    [JsonPropertyName("sosomap_poi_uid")]
    public string? SosoMapPoiUid { get; set; }

    /// <summary>获取或设置地图数据（官方 <c>data_supply</c>；官方仅标注「地图数据」，语义未定义，照录）。</summary>
    [JsonPropertyName("data_supply")]
    public int? DataSupply { get; set; }

    /// <summary>获取或设置门店图片列表（官方 <c>pic_urls</c>；元素类型官方未给出，以字符串承载）。</summary>
    [JsonPropertyName("pic_urls")]
    public List<string>? PictureUrls { get; set; }

    /// <summary>获取或设置门店相应卡券列表（官方 <c>card_id_list</c>；元素类型官方未给出，以字符串承载）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }
}

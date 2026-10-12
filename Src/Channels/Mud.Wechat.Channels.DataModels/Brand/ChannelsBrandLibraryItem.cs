// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>品牌库品牌条目（官方 <c>brands</c> 数组元素，<c>getallbrandslogic</c>）。</summary>
/// <remarks>官方本端点 <c>brand_id</c> 类型为 <b>string</b>（与提交/详情侧 number 型不一致，照抄官方）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandLibraryItem
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，官方本端点为 string 型）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置品牌商标中文名（官方 <c>ch_name</c>）。</summary>
    [JsonPropertyName("ch_name")]
    public string? ChName { get; set; }

    /// <summary>获取或设置品牌商标英文名（官方 <c>en_name</c>）。</summary>
    [JsonPropertyName("en_name")]
    public string? EnName { get; set; }
}

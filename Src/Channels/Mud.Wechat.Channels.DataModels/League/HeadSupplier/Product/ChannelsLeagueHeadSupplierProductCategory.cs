// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 机构侧新类目树节点（官方 <c>item.product_info.cats_v2</c> 数组元素：<c>cat_id</c> + <c>cat_name</c>）。
/// </summary>
/// <remarks>
/// 商家侧 <see cref="Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory"/> 只有 <c>cat_id</c> ⇒ 字段集不同，分建 DTO。
/// <c>cat_id</c> 官方类型 number，但同页代码示例为字符串（<c>"1421"</c>）⇒ 与商家侧一致建模为 string 兼容两种形态。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierProductCategory
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }

    /// <summary>获取或设置类目名称（官方 <c>cat_name</c>）。</summary>
    [JsonPropertyName("cat_name")]
    public string? CatName { get; set; }
}

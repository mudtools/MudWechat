// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>推荐类目（官方 <c>categories</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductClassifyCategory
{
    /// <summary>获取或设置推荐的多个多级类目信息（官方 <c>cats</c>）。</summary>
    [JsonPropertyName("cats")]
    public ChannelsProductClassifyCats? Cats { get; set; }

    /// <summary>获取或设置是否有该类目的权限（官方 <c>has_permission</c>）。</summary>
    [JsonPropertyName("has_permission")]
    public bool? HasPermission { get; set; }

    /// <summary>获取或设置类目 id（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public string? CatId { get; set; }
}

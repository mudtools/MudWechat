// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>店铺主页商品分类树（官方 <c>tree</c> 对象，<c>getclassificationtree</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationTree
{
    /// <summary>获取或设置一级分类列表（官方 <c>level_1</c>）。</summary>
    [JsonPropertyName("level_1")]
    public List<ChannelsClassificationNode>? Level1 { get; set; }

    /// <summary>获取或设置分类树名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置分类树 ID（官方 <c>tree_id</c>）。</summary>
    [JsonPropertyName("tree_id")]
    public long? TreeId { get; set; }
}

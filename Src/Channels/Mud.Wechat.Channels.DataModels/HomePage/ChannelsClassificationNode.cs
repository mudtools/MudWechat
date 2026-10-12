// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 分类树节点（官方 <c>tree.level_1[]</c> / <c>tree.level_1[].level_2[]</c> 元素；两级元素结构一致 ⇒ 共用，
/// 仅 2 级树，一级节点携带 <c>level_2</c> 列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsClassificationNode
{
    /// <summary>获取或设置分类 ID（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置分类名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置二级分类列表（官方 <c>level_2</c>；仅一级分类节点携带，仅支持 2 级树）。</summary>
    [JsonPropertyName("level_2")]
    public List<ChannelsClassificationNode>? Level2 { get; set; }

    /// <summary>获取或设置分类图标（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

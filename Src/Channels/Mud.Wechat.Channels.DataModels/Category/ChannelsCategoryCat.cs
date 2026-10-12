// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>类目详情（官方 <c>cat</c> 对象；<c>leaf</c> 仅 <c>cats_v2</c> 新树版本出现）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryCat
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>）。</summary>
    [JsonPropertyName("cat_id")]
    public long? CatId { get; set; }

    /// <summary>获取或设置类目名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置父类目 ID（官方 <c>f_cat_id</c>）。</summary>
    [JsonPropertyName("f_cat_id")]
    public long? FCatId { get; set; }

    /// <summary>获取或设置类目等级（官方 <c>level</c>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置是否为叶子类目（品类）（官方 <c>leaf</c>，仅 <c>cats_v2</c> 新树版本携带）。</summary>
    [JsonPropertyName("leaf")]
    public bool? Leaf { get; set; }
}

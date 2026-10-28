// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>图片消息分支（官方 <c>images</c>；多图 + 推荐语形态，与 preview 页的单图 image 分支不同）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassImages
{
    /// <summary>获取或设置用于群发的 media_id 列表（官方 <c>media_ids</c>；官方示例 3 个）。</summary>
    [JsonPropertyName("media_ids")]
    public List<string>? MediaIds { get; set; }

    /// <summary>获取或设置推荐语（官方 <c>recommend</c>）。</summary>
    [JsonPropertyName("recommend")]
    public string? Recommend { get; set; }

    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置是否开启评论（官方 <c>need_open_comment</c>：1 开启 / 0 关闭）。</summary>
    [JsonPropertyName("need_open_comment")]
    public int? NeedOpenComment { get; set; }

    /// <summary>获取或设置是否仅粉丝可评论（官方 <c>only_fans_can_comment</c>：1 开启 / 0 关闭）。</summary>
    [JsonPropertyName("only_fans_can_comment")]
    public int? OnlyFansCanComment { get; set; }
}

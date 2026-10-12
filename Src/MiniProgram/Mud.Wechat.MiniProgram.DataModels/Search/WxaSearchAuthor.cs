// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Search;

/// <summary>搜一搜数据推送的作者信息（<c>author</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Search")]
public class WxaSearchAuthor
{
    /// <summary>名称（<c>author_name</c>，选填）。</summary>
    [JsonPropertyName("author_name")]
    public string? AuthorName { get; set; }

    /// <summary>职务（<c>author_title</c>，选填）。</summary>
    [JsonPropertyName("author_title")]
    public string? AuthorTitle { get; set; }

    /// <summary>头像 URL（<c>author_portrait</c>，选填）。</summary>
    [JsonPropertyName("author_portrait")]
    public string? AuthorPortrait { get; set; }
}

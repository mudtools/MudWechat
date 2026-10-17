// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 草稿箱列表项（<c>gettemplatedraftlist</c> 应答 <c>draft_list</c> 数组元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/getTemplatedRaftList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformTemplateDraftItem
{
    /// <summary>草稿 ID（官方字段名 <c>draft_id</c>，number）。</summary>
    [JsonPropertyName("draft_id")]
    public long DraftId { get; set; }

    /// <summary>版本号，开发者自定义字段（官方字段名 <c>user_version</c>，string）。</summary>
    [JsonPropertyName("user_version")]
    public string? UserVersion { get; set; }

    /// <summary>版本描述，开发者自定义字段（官方字段名 <c>user_desc</c>，string）。</summary>
    [JsonPropertyName("user_desc")]
    public string? UserDesc { get; set; }

    /// <summary>开发者上传草稿时间戳（官方字段名 <c>create_time</c>，number）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>开发小程序的 appid（官方字段名 <c>source_miniprogram_appid</c>，string）。</summary>
    [JsonPropertyName("source_miniprogram_appid")]
    public string? SourceMiniprogramAppid { get; set; }

    /// <summary>开发小程序的名称（官方字段名 <c>source_miniprogram</c>，string）。</summary>
    [JsonPropertyName("source_miniprogram")]
    public string? SourceMiniprogram { get; set; }

    /// <summary>操作者微信昵称（官方字段名 <c>developer</c>，string）。</summary>
    [JsonPropertyName("developer")]
    public string? Developer { get; set; }
}

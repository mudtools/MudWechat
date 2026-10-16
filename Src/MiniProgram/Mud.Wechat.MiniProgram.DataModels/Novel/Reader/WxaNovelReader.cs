// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>
/// 设置预览配置请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelSetPreviewSettingRequest
{
    /// <summary>
    /// 预览配置（官方请求体是 setting 包装的单对象，非扁平字段）
    /// </summary>
    [JsonPropertyName("setting")]
    public WxaNovelPreviewSetting? Setting { get; set; }
}

/// <summary>
/// 获取预览配置请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetPreviewSettingRequest
{
    /// <summary>
    /// 作品id
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }
}

/// <summary>
/// 获取预览配置响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelGetPreviewSettingResponse : WxaResponse
{
    /// <summary>
    /// 预览配置
    /// </summary>
    [JsonPropertyName("setting")]
    public WxaNovelPreviewSetting? Setting { get; set; }
}

/// <summary>
/// 预览配置
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelPreviewSetting
{
    /// <summary>
    /// 作品id
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>
    /// 默认预览字数
    /// </summary>
    [JsonPropertyName("default_words")]
    public long? DefaultWords { get; set; }

    /// <summary>
    /// 章节配置列表
    /// </summary>
    [JsonPropertyName("chapter_setting")]
    public List<WxaNovelChapterSetting>? ChapterSetting { get; set; }
}

/// <summary>
/// 章节配置
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelChapterSetting
{
    /// <summary>
    /// 章节索引
    /// </summary>
    [JsonPropertyName("chapter_index")]
    public long? ChapterIndex { get; set; }

    /// <summary>
    /// 章节字数
    /// </summary>
    [JsonPropertyName("words")]
    public long? Words { get; set; }
}

/// <summary>
/// 设置推荐小说请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelSetRecmdNovelRequest
{
    /// <summary>
    /// 推荐小说的类型：1-android(付费小说) 2-ios(全文免费小说)
    /// </summary>
    [JsonPropertyName("recmd_type")]
    public long? RecmdType { get; set; }

    /// <summary>
    /// 推荐小说的 book_id 数组
    /// </summary>
    [JsonPropertyName("book_id_list")]
    public List<string>? BookIdList { get; set; }
}

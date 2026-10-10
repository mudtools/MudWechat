// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 学校通知小程序消息体（miniprogram）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class SchoolMiniProgramBody
{
    /// <summary>
    /// 获取或设置小程序 appid（官方必填），必须是关联到企业的小程序应用。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息标题，最多 64 个字节，超过会自动截断（支持 id 转译）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置小程序消息封面的 mediaid（官方必填），封面图建议尺寸为 520*416。
    /// </summary>
    [JsonPropertyName("thumb_media_id")]
    public string? ThumbMediaId { get; set; }

    /// <summary>
    /// 获取或设置点击消息卡片后进入的小程序页面路径（官方必填）。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}

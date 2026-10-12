// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>小程序卡片消息体（<c>miniprogrampage</c> 分支；四字段官方均必填）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMiniProgramPageMessage
{
    /// <summary>小程序卡片标题。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>小程序 APPID。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>小程序页面路径（与 <c>app.json</c> 对齐，支持参数）。</summary>
    [JsonPropertyName("pagepath")]
    public string PagePath { get; set; } = string.Empty;

    /// <summary>卡片封面 media_id（image 类型；官方建议尺寸 520*416）。</summary>
    [JsonPropertyName("thumb_media_id")]
    public string ThumbMediaId { get; set; } = string.Empty;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议背景对象（添加/获取会议背景列表响应 <c>background_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutBackground
{
    /// <summary>获取或设置背景 ID。</summary>
    [JsonPropertyName("background_id")]
    public string? BackgroundId { get; set; }

    /// <summary>获取或设置背景图片 MD5（图片内容 MD5 的十六进制表示）。</summary>
    [JsonPropertyName("image_md5")]
    public string? ImageMd5 { get; set; }
}

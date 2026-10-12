// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Kf;

/// <summary>
/// 媒体消息内容（官方 <c>image</c> / <c>video</c> / <c>file</c> 对象，<c>sendmsg</c>）——
/// 三者字段集完全一致（仅 <c>cos_url</c>）⇒ 共用同一 DTO。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class ChannelsKfMediaContent
{
    /// <summary>获取或设置上传多媒体 cos_url（官方 <c>cos_url</c>，必填；经「上传多媒体资源」接口获取）。</summary>
    [JsonPropertyName("cos_url")]
    public string CosUrl { get; set; } = string.Empty;
}

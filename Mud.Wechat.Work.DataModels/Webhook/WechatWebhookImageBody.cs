// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人图片消息体（<c>msgtype=image</c>）。
/// <para>base64 与 md5 官方均为必填；图片大小不超过 2MB，仅支持 JPG/PNG 格式。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookImageBody
{
    /// <summary>
    /// 获取或设置图片内容的 base64 编码（官方必填）。
    /// </summary>
    [JsonPropertyName("base64")]
    public string? Base64 { get; set; }

    /// <summary>
    /// 获取或设置图片内容的 md5 值（官方必填；十六进制摘要，供官方校验图片完整性）。
    /// </summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Media;

/// <summary>
/// 上传图片响应体（<c>/cgi-bin/media/uploadimg</c>，用于获取图文消息正文中的图片 URL）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class UploadImageResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置上传后得到的图片 URL（永久有效）。
    /// <para>官方约束：该 URL 仅能用于图文消息正文中的图片展示，或者给客户发送欢迎语等；
    /// 若用于非企业微信环境下的页面，图片将被屏蔽。</para>
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

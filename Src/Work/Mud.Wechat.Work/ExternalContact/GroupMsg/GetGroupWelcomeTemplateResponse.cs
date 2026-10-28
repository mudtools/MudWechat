// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 获取入群欢迎语素材响应体（<c>/cgi-bin/externalcontact/group_welcome_template/get</c>）。
/// <para>素材字段为响应根级字段（与添加 / 编辑请求体同构）；
/// 若设置时使用异步上传临时素材获取的 media_id，则返回同类型的 media_id，图片仅返回 pic_url。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GetGroupWelcomeTemplateResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置文本消息内容。
    /// </summary>
    [JsonPropertyName("text")]
    public GroupMsgTextContent? Text { get; set; }

    /// <summary>
    /// 获取或设置图片附件（读取时返回 pic_url）。
    /// </summary>
    [JsonPropertyName("image")]
    public GroupMsgImageAttachment? Image { get; set; }

    /// <summary>
    /// 获取或设置图文（链接）附件。
    /// </summary>
    [JsonPropertyName("link")]
    public GroupMsgLinkAttachment? Link { get; set; }

    /// <summary>
    /// 获取或设置小程序附件。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public GroupMsgMiniProgramAttachment? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置文件附件。
    /// </summary>
    [JsonPropertyName("file")]
    public GroupMsgFileAttachment? File { get; set; }

    /// <summary>
    /// 获取或设置视频附件。
    /// </summary>
    [JsonPropertyName("video")]
    public GroupMsgVideoAttachment? Video { get; set; }
}

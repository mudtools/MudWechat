// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 添加入群欢迎语素材请求体（<c>/cgi-bin/externalcontact/group_welcome_template/add</c>）。
/// <para>
/// 文本 / 图片 / 图文 / 小程序 / 文件 / 视频不能全为空；文本可与其他类型同时发送（两条消息触达）；
/// 文本以外的类型只能配置一个，同时填写多个时按 image &gt; link &gt; miniprogram &gt; file &gt; video 的优先级取参；
/// 图片的 media_id 与 pic_url 填一个即可，同时填写时使用 media_id。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class AddGroupWelcomeTemplateRequest
{
    /// <summary>
    /// 获取或设置文本消息（最长 3000 字节；支持 <c>%NICKNAME%</c> 占位符，发送时替换为客户昵称）。
    /// </summary>
    [JsonPropertyName("text")]
    public GroupMsgTextContent? Text { get; set; }

    /// <summary>
    /// 获取或设置图片附件。
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

    /// <summary>
    /// 获取或设置企业应用的应用 id（仅旧的第三方多应用套件需要填写）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置是否通知成员应用该欢迎语：0 - 不通知，1 - 通知（不填则通知）。
    /// </summary>
    [JsonPropertyName("notify")]
    public int? Notify { get; set; }
}

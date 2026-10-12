// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;

/// <summary>更新服务卡片扩展信息请求体（<c>POST /wxa/set_user_notifyext</c>）。</summary>
/// <remarks>
/// 官方文档：<c>mp-message-management/subscribe-message/api_setusernotifyext.html</c>。
/// 只<b>追加/更新扩展信息</b>（如按钮、表单等交互组件数据），不改写卡片主体内容；卡片须先经
/// <c>set_user_notify</c> 激活。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscribeMessage")]
public class WxaSetUserNotifyExtRequest
{
    /// <summary>用户 <c>openid</c>（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务卡片模板 ID（<c>template_id</c>，必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>扩展信息内容（<c>data</c>，必填），见 <see cref="WxaServiceCardDataValue"/>。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, WxaServiceCardDataValue>? Data { get; set; }
}

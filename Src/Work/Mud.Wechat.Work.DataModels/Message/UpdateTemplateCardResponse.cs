// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 更新模版卡片消息响应体（<c>/cgi-bin/message/update_template_card</c>）。
/// <para>部分用户无权限或不存在时更新仍会执行，但会返回无效部分，
/// 常见原因是用户不在应用可见范围内或者不在消息的接收范围内。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class UpdateTemplateCardResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置无效用户列表（不区分大小写，统一转为小写；
    /// 注意与发送应用消息响应的竖线分隔字符串形态不同，本接口为数组）。
    /// </summary>
    [JsonPropertyName("invaliduser")]
    public List<string>? InvalidUser { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人图文消息体（<c>msgtype=news</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookNewsBody
{
    /// <summary>
    /// 获取或设置图文条目列表（官方必填），1~8 条，超出报错。
    /// </summary>
    [JsonPropertyName("articles")]
    public List<WechatWebhookNewsArticle>? Articles { get; set; }
}

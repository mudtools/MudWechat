// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 消息内容键值对，小程序通知（<c>miniprogram_notice.content_item</c>）与第三方模板消息（<c>template_msg.content_item</c>）共用。
/// <para>key 和 value 两个字段同时为空时，该键值对将被忽略。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageContentItem
{
    /// <summary>
    /// 获取或设置键值对的 key，长度 10 个汉字以内（第三方模板消息场景必填，长度 1~20 个 utf8 字符且须与 template_id 对应模板匹配）。
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 获取或设置键值对的 value，长度 30 个汉字以内，支持 id 转译（第三方模板消息场景必填，长度 1~40 个 utf8 字符）。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

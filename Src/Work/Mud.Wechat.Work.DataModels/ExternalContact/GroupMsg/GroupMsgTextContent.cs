// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 群发 / 欢迎语消息的文本内容（<c>text</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class GroupMsgTextContent
{
    /// <summary>
    /// 获取或设置消息文本内容
    /// （企业群发 / 欢迎语最长 4000 字节；入群欢迎语素材最长 3000 字节，
    /// 支持 <c>%NICKNAME%</c> 占位符（大小写敏感），发送时自动替换为客户昵称）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

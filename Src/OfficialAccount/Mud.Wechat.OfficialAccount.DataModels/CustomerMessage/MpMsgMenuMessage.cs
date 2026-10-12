// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>菜单消息体（<c>msgmenu</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMsgMenuMessage
{
    /// <summary>菜单描述（可选）。</summary>
    [JsonPropertyName("head_content")]
    public string? HeadContent { get; set; }

    /// <summary>菜单内容（必填）。</summary>
    [JsonPropertyName("list")]
    public List<MpMsgMenuItem> List { get; set; } = new();

    /// <summary>菜单结尾（可选）。</summary>
    [JsonPropertyName("tail_content")]
    public string? TailContent { get; set; }
}

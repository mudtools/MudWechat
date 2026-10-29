// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 按钮更新内容（更新模版卡片消息接口的 button 节点，用于将按钮更新为不可点击状态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class UpdateTemplateCardButton
{
    /// <summary>
    /// 获取或设置需要更新的按钮的文案（官方必填）。
    /// </summary>
    [JsonPropertyName("replace_name")]
    public string? ReplaceName { get; set; }
}

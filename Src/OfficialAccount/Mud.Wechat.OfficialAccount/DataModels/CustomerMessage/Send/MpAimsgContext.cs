// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>AI 消息上下文（<c>aimsgcontext</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpAimsgContext
{
    /// <summary>消息下方是否增加灰色标记「内容由第三方 AI 生成」（<c>0</c> 不增加 / <c>1</c> 增加）。</summary>
    [JsonPropertyName("is_ai_msg")]
    public int? IsAiMsg { get; set; }
}

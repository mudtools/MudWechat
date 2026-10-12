// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 修改卡券（<c>POST /card/update</c>）应答。
/// </summary>
/// <remarks>
/// <b>本向应答不回传 <c>card_id</c></b>（与建卡不同），只有是否需要送审的 <c>send_check</c>。
/// 名称取 <see cref="RequireSendCheck"/> 以贴合官方语义「是否需要提交审核」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardUpdateResponse : MpResponse
{
    /// <summary>获取或设置是否需要提交审核（官方 <c>send_check</c>）。</summary>
    [JsonPropertyName("send_check")]
    public bool RequireSendCheck { get; set; }
}

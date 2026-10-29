// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 卡券消息入口（官方 <c>modify_msg_operation</c> / <c>activate_msg_operation</c>，仅修改会员卡方向）。
/// </summary>
/// <remarks>
/// <b>为何只出现在修改方向</b>：该对象配置「修改信息 / 领卡后」推送给用户的入口按钮，属运营期设置，
/// 建卡页字段表不含（对齐基准：SKIT <c>CardUpdateRequest.Types.MembershipCard.Types.Operation</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMessageOperation
{
    /// <summary>获取或设置入口卡片（官方 <c>url_cell</c>）。</summary>
    [JsonPropertyName("url_cell")]
    public MpCardUrlCell? UrlCell { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 二维码动作信息（官方 <c>action_info</c>）—— <c>card</c>（单卡）与 <c>multiple_card</c>（多卡）<b>互斥</b>。
/// </summary>
/// <remarks>由 <c>action_name</c> 判别取哪一支；SDK <b>不做本地互斥校验</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeActionInfo
{
    /// <summary>获取或设置单卡信息（官方 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardQrcodeCardInfo? Card { get; set; }

    /// <summary>获取或设置多卡信息（官方 <c>multiple_card</c>）。</summary>
    [JsonPropertyName("multiple_card")]
    public MpCardQrcodeMultipleCardInfo? MultipleCard { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 会员卡明细统计单日数据项（官方 <c>getcardmembercarddetail</c> 的 <c>list</c> 项；在会员卡统计字段上追加券与商户标识）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpCardMemberCardDetailData : MpCardMemberCardData
{
    /// <summary>获取或设置会员卡 ID（官方 <c>cardid</c>；官方拼写无下划线，照抄）。</summary>
    [JsonPropertyName("cardid")]
    public string? CardId { get; set; }

    /// <summary>获取或设置商户类别（官方 <c>merchanttype</c>，可选）。</summary>
    [JsonPropertyName("merchanttype")]
    public int? SubMerchantType { get; set; }

    /// <summary>获取或设置子商户 ID（官方 <c>submerchantid</c>，可选）。</summary>
    [JsonPropertyName("submerchantid")]
    public string? SubMerchantId { get; set; }

    /// <summary>新增会员人数（官方 <c>new_user</c>）。</summary>
    [JsonPropertyName("new_user")]
    public int NewUserCount { get; set; }

    /// <summary>会员卡原来实收金额（官方 <c>payOriginalFee</c>；官方驼峰拼写，照抄）。</summary>
    [JsonPropertyName("payOriginalFee")]
    public int PayOriginalFee { get; set; }

    /// <summary>会员卡实收金额（官方 <c>fee</c>）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }
}

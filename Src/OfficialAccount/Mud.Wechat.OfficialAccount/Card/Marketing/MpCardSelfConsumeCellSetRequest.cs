// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 开通或关闭「自助核销」组件（<c>POST /card/selfconsumecell/set</c>）请求体。
/// </summary>
/// <remarks>
/// <b>官方键名照录 <c>need_verify_cod</c></b>（SKIT 对齐基准如此）—— 该拼写<b>缺尾部 <c>e</c></b>，
/// 与常见 <c>need_verify_code</c> 不同。此为<b>待官方逐页核验</b>的高风险点：若官方页确为
/// <c>need_verify_code</c>，守卫 CD6 与本片段的「照录而非修拼写」判断需同步更正。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSelfConsumeCellSetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置是否开通自助核销（官方 <c>is_open</c>，必填）。</summary>
    [JsonPropertyName("is_open")]
    public bool IsOpen { get; set; }

    /// <summary>获取或设置核销时是否需要验证券码（官方 <c>need_verify_cod</c>，选填；拼写<b>待逐页核验</b>）。</summary>
    [JsonPropertyName("need_verify_cod")]
    public bool? NeedVerifyCode { get; set; }

    /// <summary>获取或设置核销时是否需要录入金额（官方 <c>need_remark_amount</c>，选填）。</summary>
    [JsonPropertyName("need_remark_amount")]
    public bool? NeedRemarkAmount { get; set; }
}

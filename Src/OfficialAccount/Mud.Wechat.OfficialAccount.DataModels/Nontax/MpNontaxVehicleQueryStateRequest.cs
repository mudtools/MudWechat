// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询车辆状态」请求（官方 <c>vehicle/querystate</c>；<c>plate_number</c> 与 <c>openid</c> 二选一）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleQueryStateRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置银行 ID（官方 <c>bank_id</c>，可选）。</summary>
    [JsonPropertyName("bank_id")]
    public string? BankId { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_account</c>，可选）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置地区编码（官方 <c>region_code</c>，必填）。</summary>
    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>获取或设置交易场景（官方 <c>trade_scene</c>，必填）。</summary>
    [JsonPropertyName("trade_scene")]
    public string TradeScene { get; set; } = string.Empty;

    /// <summary>获取或设置跳转场景（官方 <c>jump_scene</c>，可选）。</summary>
    [JsonPropertyName("jump_scene")]
    public string? JumpScene { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选；与车牌二选一）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置车牌号（官方 <c>plate_number</c>，可选；与 OpenId 二选一）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>获取或设置进区材料信息（官方 <c>material_info</c>，可选）。</summary>
    [JsonPropertyName("material_info")]
    public string? MaterialInfo { get; set; }

    /// <summary>获取或设置渠道类型（官方 <c>channel_type</c>，可选）。</summary>
    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }
}

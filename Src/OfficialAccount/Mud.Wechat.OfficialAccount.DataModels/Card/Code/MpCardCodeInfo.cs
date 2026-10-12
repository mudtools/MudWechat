// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>券码与卡券信息（官方 <c>code/get</c> 应答的 <c>card</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置卡券编号（官方 <c>card_number</c>）。</summary>
    [JsonPropertyName("card_number")]
    public string? CardNumber { get; set; }

    /// <summary>获取或设置生效时间戳（官方 <c>begin_time</c>，秒；<b>本向键名为 <c>begin_time</c></b>）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置失效时间戳（官方 <c>end_time</c>，秒；<b>本向键名为 <c>end_time</c></b>）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置该券码携带的积分（官方 <c>bonus</c>，与顶层会员积分同名不同义）。</summary>
    [JsonPropertyName("bonus")]
    public int? Bonus { get; set; }

    /// <summary>获取或设置该券码携带的余额（官方 <c>balance</c>，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int? Balance { get; set; }
}

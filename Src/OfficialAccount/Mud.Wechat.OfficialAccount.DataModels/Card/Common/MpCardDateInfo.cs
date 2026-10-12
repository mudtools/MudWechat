// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 使用日期（官方 <c>base_info.date_info</c>）—— 建卡 / 查询 / 修改<b>三向同表</b>，共用一型。
/// </summary>
/// <remarks>
/// <b>字段组互斥</b>：<c>fixed_term</c> / <c>fixed_begin_term</c> 与 <c>begin_timestamp</c> /
/// <c>end_timestamp</c> 由 <c>type</c> 判别取哪一组（官方 <c>type</c> 取值表<b>待逐页核验</b>）。
/// SDK <b>不做本地互斥校验</b>，越界由官方错误码表达。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardDateInfo
{
    /// <summary>获取或设置使用时间类型（官方 <c>type</c>，判别取哪一组日期字段；取值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置起用时间戳（官方 <c>begin_timestamp</c>，秒）。</summary>
    [JsonPropertyName("begin_timestamp")]
    public long? BeginTimestamp { get; set; }

    /// <summary>获取或设置过期时间戳（官方 <c>end_timestamp</c>，秒）。</summary>
    [JsonPropertyName("end_timestamp")]
    public long? EndTimestamp { get; set; }

    /// <summary>获取或设置自领取后有效天数（官方 <c>fixed_term</c>）。</summary>
    [JsonPropertyName("fixed_term")]
    public int? FixedTerm { get; set; }

    /// <summary>获取或设置自领取后生效天数（官方 <c>fixed_begin_term</c>）。</summary>
    [JsonPropertyName("fixed_begin_term")]
    public int? FixedBeginTerm { get; set; }
}

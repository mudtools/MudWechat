// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取直播观看明细响应体（<c>/cgi-bin/living/get_watch_stat</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：是否拉完只能依据 <see cref="Ending"/> 字段判断（0 还有更多数据、1 已拉完），
/// 以 <see cref="NextKey"/> 实现分页拉取。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetLivingWatchStatResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否结束：0 表示还有更多数据需继续拉取，1 表示已拉完。只能依据该字段判断是否拉完。</summary>
    [JsonPropertyName("ending")]
    public int? Ending { get; set; }

    /// <summary>获取或设置当前数据最后一个 key，下次调用带上该值可实现分页拉取。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置统计信息列表（详见 <see cref="LivingWatchStatInfo"/>）。</summary>
    [JsonPropertyName("stat_info")]
    public LivingWatchStatInfo? StatInfo { get; set; }
}

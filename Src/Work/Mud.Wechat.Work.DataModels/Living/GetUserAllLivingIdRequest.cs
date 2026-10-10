// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取成员直播 ID 列表请求体（<c>/cgi-bin/living/get_user_all_livingid</c>；三种应用类型请求形态一致）。
/// </summary>
/// <remarks>
/// <para>官方限制：limit 建议填 20，默认值和最大值都为 100；
/// 以 next_cursor 分页，第一次拉取 cursor 可不填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetUserAllLivingIdRequest
{
    /// <summary>获取或设置企业成员的 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置上一次调用时返回的 next_cursor（第一次拉取可以不填）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置每次拉取的数据量（建议填 20，默认值和最大值都为 100）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

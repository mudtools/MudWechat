// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取直播观看明细请求体（<c>/cgi-bin/living/get_watch_stat</c>；三种应用类型请求形态一致）。
/// </summary>
/// <remarks>
/// <para>官方限制：以响应 ending 字段判断是否拉完（0 表示还有更多数据需继续拉取，1 表示已拉完），
/// next_key 分页，初次调用可填 "0"。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetLivingWatchStatRequest
{
    /// <summary>获取或设置直播的 id（官方必填）。</summary>
    [JsonPropertyName("livingid")]
    public string? Livingid { get; set; }

    /// <summary>获取或设置上一次调用返回的 next_key（初次调用可填 "0"）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

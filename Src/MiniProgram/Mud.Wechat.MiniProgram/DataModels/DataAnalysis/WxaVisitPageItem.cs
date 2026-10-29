// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>页面明细条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitPageItem
{
    /// <summary>页面路径（<c>page_path</c>）。</summary>
    [JsonPropertyName("page_path")]
    public string? PagePath { get; set; }

    /// <summary>访问次数（<c>page_visit_pv</c>）。</summary>
    [JsonPropertyName("page_visit_pv")]
    public long? PageVisitPv { get; set; }

    /// <summary>访问人数（<c>page_visit_uv</c>）。</summary>
    [JsonPropertyName("page_visit_uv")]
    public long? PageVisitUv { get; set; }

    /// <summary>次均停留时长（<c>page_staytime_pv</c>，<b>浮点</b>）。</summary>
    [JsonPropertyName("page_staytime_pv")]
    public double? PageStayTimePv { get; set; }

    /// <summary>进入页次数（<c>entrypage_pv</c>）。</summary>
    [JsonPropertyName("entrypage_pv")]
    public long? EntryPagePv { get; set; }

    /// <summary>退出页次数（<c>exitpage_pv</c>）。</summary>
    [JsonPropertyName("exitpage_pv")]
    public long? ExitPagePv { get; set; }

    /// <summary>转发次数（<c>page_share_pv</c>）。</summary>
    [JsonPropertyName("page_share_pv")]
    public long? PageSharePv { get; set; }

    /// <summary>转发人数（<c>page_share_uv</c>）。</summary>
    [JsonPropertyName("page_share_uv")]
    public long? PageShareUv { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 激活码分配信息（<c>share_info</c>，<c>/cgi-bin/license/get_active_info_by_code</c> 响应嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseActiveShareInfo
{
    /// <summary>
    /// 获取或设置下游企业 corpid。
    /// <para>官方口径：当激活码通过上游分配给下游时，获取上游企业该激活码详情时返回该字段，
    /// 表示被分配给了哪个下游企业。</para>
    /// </summary>
    [JsonPropertyName("to_corpid")]
    public string? ToCorpid { get; set; }

    /// <summary>
    /// 获取或设置上游企业 corpid。
    /// <para>官方口径：当激活码通过上游分配给下游时，获取下游企业该激活码详情时返回该字段，
    /// 表示从哪个上游企业分配过来。</para>
    /// </summary>
    [JsonPropertyName("from_corpid")]
    public string? FromCorpid { get; set; }
}

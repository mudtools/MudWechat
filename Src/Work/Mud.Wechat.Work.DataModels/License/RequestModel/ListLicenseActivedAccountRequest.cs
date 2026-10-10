// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 获取企业的账号列表请求体（<c>/cgi-bin/license/list_actived_account</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档对本端点路由的原文拼写为 <c>list_actived_account</c>（非 activated），本 SDK 照抄原文。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class ListLicenseActivedAccountRequest
{
    /// <summary>
    /// 获取或设置企业 corpid（官方必填）。
    /// <para>官方口径：若为上下游场景，为上游企业 corpid。</para>
    /// </summary>
    [JsonPropertyName("corpid")]
    public string Corpid { get; set; } = string.Empty;

    /// <summary>获取或设置返回的最大记录数（官方可选，整型，最大值 1000，默认值 500）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置用于分页查询的游标（官方可选，字符串类型，由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

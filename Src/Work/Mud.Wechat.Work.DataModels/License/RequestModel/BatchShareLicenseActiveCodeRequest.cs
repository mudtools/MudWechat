// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 分配激活码给下游/下级企业请求体（<c>/cgi-bin/license/batch_share_active_code</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class BatchShareLicenseActiveCodeRequest
{
    /// <summary>获取或设置上游/上级企业 corpid（官方必填）。</summary>
    [JsonPropertyName("from_corpid")]
    public string FromCorpid { get; set; } = string.Empty;

    /// <summary>获取或设置下游/下级企业 corpid（官方必填）。</summary>
    [JsonPropertyName("to_corpid")]
    public string ToCorpid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置分配的接口许可列表（官方必填）。
    /// <para>官方约束：每次分配激活码不可超过 1000 个，且每次分配给下游/下级企业的激活码数
    /// 不可超过上下游/企业互联通讯录中该下游企业人数的 2 倍。</para>
    /// </summary>
    [JsonPropertyName("share_list")]
    public List<LicenseShareActiveCode> ShareList { get; set; } = new List<LicenseShareActiveCode>();

    /// <summary>
    /// 获取或设置分配的场景（官方可选，不填默认为 0）：<c>0</c>-上下游，<c>1</c>-企业互联。
    /// </summary>
    [JsonPropertyName("corp_link_type")]
    public int? CorpLinkType { get; set; }
}

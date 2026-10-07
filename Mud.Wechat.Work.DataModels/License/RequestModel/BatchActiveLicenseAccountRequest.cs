// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 批量激活账号请求体（<c>/cgi-bin/license/batch_active_account</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class BatchActiveLicenseAccountRequest
{
    /// <summary>获取或设置激活码所属企业 corpid（官方必填）。</summary>
    [JsonPropertyName("corpid")]
    public string Corpid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置需要激活的账号列表（官方必填）。
    /// <para>官方约束：单次激活的员工数量不超过 1000。</para>
    /// </summary>
    [JsonPropertyName("active_list")]
    public List<LicenseActiveAccountItem> ActiveList { get; set; } = new List<LicenseActiveAccountItem>();
}

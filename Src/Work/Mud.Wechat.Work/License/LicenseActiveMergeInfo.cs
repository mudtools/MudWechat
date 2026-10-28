// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 激活码合并信息（<c>merge_info</c>，<c>/cgi-bin/license/get_active_info_by_code</c> 响应嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseActiveMergeInfo
{
    /// <summary>获取或设置该激活码合并到的新激活码信息。</summary>
    [JsonPropertyName("to_active_code")]
    public string? ToActiveCode { get; set; }

    /// <summary>
    /// 获取或设置被合并的旧激活码。
    /// <para>官方口径：激活码激活 userid 时，若 userid 原来已经绑定了一个激活码，则会返回该字段。</para>
    /// </summary>
    [JsonPropertyName("from_active_code")]
    public string? FromActiveCode { get; set; }
}

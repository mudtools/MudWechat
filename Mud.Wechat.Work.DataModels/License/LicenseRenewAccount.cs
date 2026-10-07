// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 续期的账号列表项（<c>account_list</c> 元素，<c>/cgi-bin/license/create_renew_order_job</c> 请求体）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseRenewAccount
{
    /// <summary>获取或设置续期企业的成员 userid（官方必填）。</summary>
    [JsonPropertyName("userid")]
    public string Userid { get; set; } = string.Empty;

    /// <summary>获取或设置续期账号类型（官方必填）：<c>1</c>-基础账号，<c>2</c>-互通账号。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

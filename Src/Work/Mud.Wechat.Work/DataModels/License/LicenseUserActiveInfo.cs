// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 成员账号激活列表项（<c>active_info_list</c> 元素，<c>/cgi-bin/license/get_active_info_by_user</c> 响应）。
/// </summary>
/// <remarks>
/// <para>官方口径：同一个 userid 同类账号最多只能有一个。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseUserActiveInfo
{
    /// <summary>获取或设置账号激活码。</summary>
    [JsonPropertyName("active_code")]
    public string? ActiveCode { get; set; }

    /// <summary>获取或设置账号类型：<c>1</c>-基础账号，<c>2</c>-互通账号。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置账号绑定激活的成员 userid。
    /// <para>官方口径：返回加密的 userid。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置激活时间（unix 时间戳）。</summary>
    [JsonPropertyName("active_time")]
    public long? ActiveTime { get; set; }

    /// <summary>获取或设置过期时间（unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

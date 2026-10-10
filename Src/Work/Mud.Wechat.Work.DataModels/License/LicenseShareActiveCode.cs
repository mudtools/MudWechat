// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 分配的接口许可列表项（<c>share_list</c> 元素，<c>/cgi-bin/license/batch_share_active_code</c> 请求体）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseShareActiveCode
{
    /// <summary>
    /// 获取或设置分配的激活码（官方必填）。
    /// <para>官方约束：每次分配激活码不可超过 1000 个，且每次分配给下游/下级企业的激活码数
    /// 不可超过上下游/企业互联通讯录中该下游企业人数的 2 倍。</para>
    /// </summary>
    [JsonPropertyName("active_code")]
    public string ActiveCode { get; set; } = string.Empty;
}

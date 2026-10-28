// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 获取高级功能账号列表请求体（<c>/cgi-bin/security/vip/list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ListVipAccountsRequest
{
    /// <summary>
    /// 获取或设置分页游标（由上一次调用返回，首次可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置每次请求返回数据上限（默认 100，最大 200）。
    /// <para>官方提示不保证每次返回数据正好等于 limit，需以 has_more 判断是否继续请求。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

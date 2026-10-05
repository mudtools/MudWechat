// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取高级功能账号列表请求体（<c>/cgi-bin/meeting/vip/list</c>；查询企业已分配高级功能且在应用可见范围的账号列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListPremiumAccountsRequest
{
    /// <summary>获取或设置分页查询游标（由上一次调用返回，首次调用可不填）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页大小（每次请求返回的数据上限，默认 100，最大 200）。
    /// <para>官方说明：不保证每次返回的数据刚好为指定 limit，必须用返回的 <c>has_more</c> 判断是否继续请求。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

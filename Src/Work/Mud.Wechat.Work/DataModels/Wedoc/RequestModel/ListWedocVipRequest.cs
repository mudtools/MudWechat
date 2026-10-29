// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取高级功能账号列表请求体（<c>/cgi-bin/wedoc/vip/list</c>）。
/// </summary>
/// <remarks>
/// <para>cursor + limit 分页形态，两字段官方均为选填；官方续页口径是「无更多数据时 next_cursor 返回空」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class ListWedocVipRequest
{
    /// <summary>获取或设置分页每页数量（官方选填，取值范围 1~1000，默认 100；即「每次返回条数最大值」而非游标）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置翻页标记（官方选填，字符串；第一次请求传入空，之后填入上一次调用返回的 next_cursor）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

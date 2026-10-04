// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 获取巡查上报事件列表请求体（<c>/cgi-bin/report/patrol/get_order_list</c>，政民沟通巡查上报族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovPatrolGetOrderListRequest
{
    /// <summary>获取或设置起始创建时间戳。筛选返回 begin_create_time 之后新创建的上报。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>获取或设置起始修改时间戳。筛选返回 begin_modify_time 之后新修改的上报。</summary>
    [JsonPropertyName("begin_modify_time")]
    public long? BeginModifyTime { get; set; }

    /// <summary>获取或设置翻页参数。首次查询为空；如果查询条件有变更，需要将这个字段置空。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置单页单数。如果不填，默认 20 条，最大 50。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

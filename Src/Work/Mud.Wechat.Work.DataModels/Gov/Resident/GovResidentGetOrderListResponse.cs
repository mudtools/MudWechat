// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 获取居民上报事件列表响应体（<c>/cgi-bin/report/resident/get_order_list</c>，政民沟通居民上报族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovResidentGetOrderListResponse : WechatWorkResponse
{
    /// <summary>获取或设置获取下一页数据的凭据。为空字符串代表是最后一页。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置工单列表。</summary>
    [JsonPropertyName("order_list")]
    public List<GovResidentOrder>? OrderList { get; set; }
}

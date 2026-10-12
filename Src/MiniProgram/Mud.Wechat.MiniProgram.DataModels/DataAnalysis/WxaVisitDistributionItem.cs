// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>分布数据条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitDistributionItem
{
    /// <summary>
    /// 分布类型（<c>index</c>）：<c>access_source_session_cnt</c>（访问来源）/
    /// <c>access_staytime_info</c>（访问时长）/ <c>access_depth_info</c>（访问深度）。
    /// </summary>
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    /// <summary>分布明细（<c>item_list</c>，<c>{key,value}</c> 数组），见 <see cref="WxaKeyValueItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaKeyValueItem>? ItemList { get; set; }
}

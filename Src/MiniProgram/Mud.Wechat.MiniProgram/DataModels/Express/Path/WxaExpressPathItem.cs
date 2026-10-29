// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>物流轨迹节点（<c>path_item_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathItem
{
    /// <summary>轨迹发生时间（<c>action_time</c>，Unix 时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，枚举值同附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，展示在快递轨迹详情页）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }
}

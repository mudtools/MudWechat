// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>待推送的物流轨迹（<c>path</c> 内嵌对象，<c>pathnotify</c> 专用）。</summary>
/// <remarks>
/// <c>action_type</c> 为官方枚举（与普通单一致）：<c>90001</c> 网点接单、<c>90002</c> 分配业务员、
/// <c>100001</c> 揽件成功、<c>200001</c> 更新运输轨迹、<c>300002</c> 派送中、<c>300003</c> 签收成功、
/// <c>400001</c> 订单取消、<c>500001</c> 兜底状态等。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyPath
{
    /// <summary>轨迹变化 Unix 时间戳（<c>action_time</c>，必填；单位：秒）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；官方枚举值，见类说明）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填；展示在快递轨迹详情页中）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>取件员姓名（<c>pickup_courier_name</c>，可选；分配取件员成功时返回）。</summary>
    [JsonPropertyName("pickup_courier_name")]
    public string? PickupCourierName { get; set; }

    /// <summary>取件员电话（<c>pickup_courier_phone</c>，可选；分配取件员成功时返回）。</summary>
    [JsonPropertyName("pickup_courier_phone")]
    public string? PickupCourierPhoneNumber { get; set; }

    /// <summary>派件员姓名（<c>delivery_courier_name</c>，可选；分配派件员成功时返回）。</summary>
    [JsonPropertyName("delivery_courier_name")]
    public string? DeliveryCourierName { get; set; }

    /// <summary>派件员电话（<c>delivery_courier_phone</c>，可选；分配派件员成功时返回）。</summary>
    [JsonPropertyName("delivery_courier_phone")]
    public string? DeliveryCourierPhoneNumber { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>礼物单结构（官方 <c>present</c> 对象，<c>present/get</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：<c>is_b2c_free_present</c> 官方声明 boolean 类型但描述为
/// 「0：已付款 1：无需付款」——SDK 按声明类型 boolean 建模，0/1 描述照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStorePresent
{
    /// <summary>获取或设置礼物单 ID（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置礼物单详情（官方 <c>present_detail</c>）。</summary>
    [JsonPropertyName("present_detail")]
    public ChannelsMiniStorePresentDetail? PresentDetail { get; set; }

    /// <summary>获取或设置礼物单中的子单列表（官方 <c>sub_order_list</c>）。</summary>
    [JsonPropertyName("sub_order_list")]
    public List<ChannelsMiniStorePresentSubOrderDetail>? SubOrderList { get; set; }

    /// <summary>获取或设置是否无需付款（官方 <c>is_b2c_free_present</c>；官方描述照录「0：已付款 1：无需付款」）。</summary>
    [JsonPropertyName("is_b2c_free_present")]
    public bool? IsB2cFreePresent { get; set; }

    /// <summary>获取或设置礼物单关联的活动 ID（官方 <c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }
}

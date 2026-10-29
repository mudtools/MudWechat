// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>橱窗商品抢购活动信息（官方 <c>limit_discount_info</c> 对象，<c>getwindowproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsWindowLimitDiscountInfo
{
    /// <summary>获取或设置是否有生效中的抢购活动（官方 <c>is_effect</c>）。</summary>
    [JsonPropertyName("is_effect")]
    public bool? IsEffect { get; set; }

    /// <summary>获取或设置抢购价（官方 <c>discount_price</c>，单位：分）。</summary>
    [JsonPropertyName("discount_price")]
    public long? DiscountPrice { get; set; }

    /// <summary>
    /// 获取或设置抢购活动结束时间（官方 <c>end_time_ms</c>，毫秒时间戳；
    /// 官方参数表标注 string、返回示例为数字 1666086005500——矛盾照录，按参数表建模为字符串）。
    /// </summary>
    [JsonPropertyName("end_time_ms")]
    public string? EndTimeMs { get; set; }

    /// <summary>获取或设置抢购剩余库存（官方 <c>stock</c>）。</summary>
    [JsonPropertyName("stock")]
    public int? Stock { get; set; }
}

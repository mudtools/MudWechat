// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 分享员直播间条目（<c>lives[]</c> 元素，<c>get_sharer_live_summary_list</c> 应答）。
/// </summary>
/// <remarks>
/// <para><b>时间口径</b>：<c>live_start_time</c> / <c>live_end_time</c> 官方标 number（unix 时间戳），
/// 且<b>直播中时 <c>live_end_time</c> 恒为 0</b>（官方原文）⇒ 判「直播中」须用 <c>live_status==1</c> 或
/// <c>live_end_time==0</c> 两个信号，不得把 0 当 1970 年解析。</para>
/// <para><b><c>live_status</c> 官方枚举</b>：<c>1</c> 直播中、<c>0</c> 直播结束。</para>
/// <para><b>与订单侧 <see cref="WxaShopSharerPromotionInfo"/> 的 <c>live_start_time</c> 同名不同型</b>
/// （本类 number、订单侧 string）⇒ 两类型不共用（守卫 SHOPSH5 锁定）。</para>
/// <para><b>敏感面</b>：<c>gmv</c>（单位分）等经营数据禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerLiveInfo
{
    /// <summary>直播间 export_id（<c>live_export_id</c>，string）：<c>get_sharer_live_order_list</c> 的必填入参来源。</summary>
    [JsonPropertyName("live_export_id")]
    public string? LiveExportId { get; set; }

    /// <summary>直播间昵称（<c>live_nickname</c>，string）。</summary>
    [JsonPropertyName("live_nickname")]
    public string? LiveNickname { get; set; }

    /// <summary>直播开始时间（<c>live_start_time</c>，number，unix 时间戳；与订单侧同名 string 字段不同型）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>直播结束时间（<c>live_end_time</c>，number；<b>直播中为 0</b>，官方原文）。</summary>
    [JsonPropertyName("live_end_time")]
    public long? LiveEndTime { get; set; }

    /// <summary>直播状态（<c>live_status</c>，number）：<c>1</c> 直播中、<c>0</c> 直播结束。</summary>
    [JsonPropertyName("live_status")]
    public int? LiveStatus { get; set; }

    /// <summary>分享员有效带货额（<c>gmv</c>，number，<b>单位分</b>）。禁落日志。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>订单数（<c>order_cnt</c>，number）。</summary>
    [JsonPropertyName("order_cnt")]
    public long? OrderCnt { get; set; }

    /// <summary>下单用户数（<c>user_cnt</c>，number）。</summary>
    [JsonPropertyName("user_cnt")]
    public long? UserCnt { get; set; }
}

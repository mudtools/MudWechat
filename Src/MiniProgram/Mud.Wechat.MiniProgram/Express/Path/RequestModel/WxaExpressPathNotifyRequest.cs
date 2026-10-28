// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>推送已绑定物流轨迹信息请求体（<c>POST /cgi-bin/express/delivery/pathnotify</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/msgpush/api_deliverypathnotify"/>
/// （接口英文名 <c>deliveryPathNotify</c>）。运力方调用本接口向已绑定用户推送物流轨迹消息。
/// </para>
/// <para>
/// 响应 <c>exist</c> 表示<b>该手机号是否已绑定</b>（<c>0</c> 未绑定 / <c>1</c> 已绑定）；
/// 未绑定时轨迹消息<b>不会</b>送达，需先引导用户完成绑定。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPathNotifyRequest
{
    /// <summary>寄件人（<c>sender</c>，必填），见 <see cref="WxaExpressPathNotifyContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressPathNotifyContact? Sender { get; set; }

    /// <summary>收件人（<c>receiver</c>，必填），见 <see cref="WxaExpressPathNotifyContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressPathNotifyContact? Receiver { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>当前需要推送消息的轨迹（<c>path</c>，必填），见 <see cref="WxaExpressPathNotifyPath"/>。</summary>
    [JsonPropertyName("path")]
    public WxaExpressPathNotifyPath? Path { get; set; }

    /// <summary>运单创建时间（<c>create_time</c>，必填；Unix 时间戳，单位：秒）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 自定义菜单事件载荷（<c>MsgType = event</c>，官方自定义菜单事件推送页 9 键合并登记）。
/// </summary>
/// <remarks>
/// <para>
/// <b>标量字段</b>：<c>EventKey</c>（菜单 KEY / 跳转 URL / 小程序路径）与 <c>MenuId</c>（跳转类事件携带）。
/// </para>
/// <para>
/// <b>嵌套结构已类型化</b>（F18 已逐页核验）：<c>scancode_*</c> → <see cref="ScanCodeInfo"/>（<c>Object</c> 通道）、
/// <c>pic_*</c> → <see cref="SendPicsInfo"/>（<c>Object</c> + <c>ItemsObject</c> 三层，项元素名官方固定 <c>item</c>）、
/// <c>location_select</c> → <see cref="SendLocationInfo"/>（<c>Object</c> 通道）。未携带对应节点的报文 ⇒ 相应属性为 <c>null</c>。
/// </para>
/// <para>
/// <b>不登记</b> <c>media_id</c>/<c>view_limited</c>：本轮官方菜单事件页未列出，须逐页核验后增量补入（方案 §12 V6）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCallbackEventTypes.Click,
    MpCallbackEventTypes.View,
    MpCallbackEventTypes.ScanCodePush,
    MpCallbackEventTypes.ScanCodeWaitMsg,
    MpCallbackEventTypes.PicSysPhoto,
    MpCallbackEventTypes.PicPhotoOrAlbum,
    MpCallbackEventTypes.PicWeixin,
    MpCallbackEventTypes.LocationSelect,
    MpCallbackEventTypes.ViewMiniProgram,
})]
public sealed partial class MpMenuEventPayload : MpCallbackPayload
{
    /// <summary>
    /// 事件 KEY 值：<c>CLICK</c> 为菜单 KEY；<c>VIEW</c> 为跳转 URL；<c>view_miniprogram</c> 为小程序路径；
    /// <c>scancode_*</c> / <c>pic_*</c> / <c>location_select</c> 为菜单配置的 KEY。
    /// </summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>菜单 ID（跳转类事件 <c>VIEW</c> / <c>view_miniprogram</c> 携带）。</summary>
    [PayloadField("MenuId")]
    public string? MenuId { get; set; }

    /// <summary>扫码信息（<c>scancode_push</c> / <c>scancode_waitmsg</c> 携带；其余事件为 <c>null</c>）。</summary>
    [PayloadField("ScanCodeInfo")]
    public MpScanCodeInfo? ScanCodeInfo { get; set; }

    /// <summary>发送的图片信息（<c>pic_sysphoto</c> / <c>pic_photo_or_album</c> / <c>pic_weixin</c> 携带；其余为 <c>null</c>）。</summary>
    [PayloadField("SendPicsInfo")]
    public MpSendPicsInfo? SendPicsInfo { get; set; }

    /// <summary>地理位置信息（<c>location_select</c> 携带；其余事件为 <c>null</c>）。</summary>
    [PayloadField("SendLocationInfo")]
    public MpSendLocationInfo? SendLocationInfo { get; set; }
}

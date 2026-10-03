// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 发图菜单事件载荷（<c>pic_sysphoto</c> / <c>pic_photo_or_album</c> / <c>pic_weixin</c> 三键；官方 path 90240）。
/// </summary>
/// <remarks>
/// <b>结构族</b>：三事件的官方报文字段集合一致（信封 + <c>EventKey</c> + <c>SendPicsInfo</c> + <c>AgentID</c>），
/// 差异仅在触发入口（系统拍照 / 拍照或相册 / 微信相册），不体现在报文上。
/// <see cref="SendPicsInfo"/> 为三层嵌套结构（<c>SendPicsInfo/PicList/item/PicMd5Sum</c>），
/// 经 G-ADR-17 <c>Object</c> + <c>ItemsObject</c> 通道声明化；节点缺失 ⇒ <c>null</c>。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.PicSysPhoto,
        WechatCallbackEventTypes.PicPhotoOrAlbum,
        WechatCallbackEventTypes.PicWeixin })]
public sealed partial class MenuPicPayload : WechatCallbackPayload
{
    /// <summary>事件 KEY 值（官方 <c>EventKey</c>，与自定义菜单接口中 KEY 值对应）。</summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>发送的图片信息（官方 <c>SendPicsInfo</c>：<c>Count</c> + 图片项列表）。</summary>
    [PayloadField("SendPicsInfo")]
    public WechatCallbackSendPicsInfo? SendPicsInfo { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}

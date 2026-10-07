// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 公众号消息加解密方式（服务器配置「消息加解密方式」三值）。
/// </summary>
/// <remarks>
/// 官方原文：明文模式「不加密，明文发送，安全系数较低，不建议使用」；
/// 兼容模式「明文、密文共存，不建议使用」；安全模式「纯密文，安全系数高，强烈推荐使用」。
/// </remarks>
public enum MpCallbackSecurityMode
{
    /// <summary>明文模式：不加密（无 <c>Encrypt</c> 节点），拒收密文。</summary>
    Plain = 0,

    /// <summary>兼容模式：明文与密文共存，按请求实际形态分支。</summary>
    Compatible = 1,

    /// <summary>安全模式（默认）：纯密文，明文一律拒收。</summary>
    Safe = 2,
}

/// <summary>
/// 接收普通消息的 <c>MsgType</c> 取值（官方接收普通消息页）。
/// </summary>
/// <remarks>
/// 解码示例：text（文本）/ image（图片）/ voice（语音）/ video（视频）/ shortvideo（小视频）/
/// location（地理位置）/ link（链接）。事件消息的 <c>MsgType</c> 恒为 <see cref="Event"/>。
/// </remarks>
public static class MpCallbackMessageTypes
{
    /// <summary>事件消息类型（<c>MsgType = event</c>，与 <c>Event</c> 节点配合）。</summary>
    public const string Event = "event";

    /// <summary>文本消息。</summary>
    public const string Text = "text";

    /// <summary>图片消息。</summary>
    public const string Image = "image";

    /// <summary>语音消息。</summary>
    public const string Voice = "voice";

    /// <summary>视频消息。</summary>
    public const string Video = "video";

    /// <summary>小视频消息。</summary>
    public const string ShortVideo = "shortvideo";

    /// <summary>地理位置消息。</summary>
    public const string Location = "location";

    /// <summary>链接消息。</summary>
    public const string Link = "link";
}

/// <summary>
/// 自定义菜单事件推送的 <c>Event</c> 取值（官方自定义菜单事件推送页）。
/// </summary>
/// <remarks>
/// 官方补充约束：点击菜单弹出子菜单<b>不产生上报</b>；第 3~8 个事件
/// （<c>scancode_push</c>～<c>location_select</c>）仅支持 iOS 微信 5.4.1+ / Android 微信 5.4+。
/// <b>不得凭记忆补入</b> <c>media_id</c>/<c>view_limited</c>（本轮官方页面未列出，须逐页核验后增量补）。
/// </remarks>
public static class MpCallbackEventTypes
{
    /// <summary>点击菜单拉取消息时的事件推送（EventKey 为菜单 KEY 值）。</summary>
    public const string Click = "CLICK";

    /// <summary>点击菜单跳转链接时的事件推送（EventKey 为跳转 URL，附 MenuId）。</summary>
    public const string View = "VIEW";

    /// <summary>扫码推事件（EventKey + ScanCodeInfo）。</summary>
    public const string ScanCodePush = "scancode_push";

    /// <summary>扫码推事件且弹出「消息接收中」提示框（EventKey + ScanCodeInfo）。</summary>
    public const string ScanCodeWaitMsg = "scancode_waitmsg";

    /// <summary>弹出系统拍照发图的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicSysPhoto = "pic_sysphoto";

    /// <summary>弹出拍照或者相册发图的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicPhotoOrAlbum = "pic_photo_or_album";

    /// <summary>弹出微信相册发图器的事件推送（EventKey + SendPicsInfo）。</summary>
    public const string PicWeixin = "pic_weixin";

    /// <summary>弹出地理位置选择器的事件推送（EventKey + SendLocationInfo）。</summary>
    public const string LocationSelect = "location_select";

    /// <summary>点击菜单跳转小程序的事件推送（EventKey 为小程序路径，附 MenuId）。</summary>
    public const string ViewMiniProgram = "view_miniprogram";
}

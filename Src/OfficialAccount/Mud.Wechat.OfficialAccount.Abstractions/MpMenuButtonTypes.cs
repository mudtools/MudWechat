// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 自定义菜单按钮类型常量（官方 <c>type</c> 枚举 12 项，逐项核验官方文档）。
/// </summary>
/// <remarks>
/// <para>
/// <b>客户端版本门槛（官方原文）</b>：下列 3~8 类事件（扫码 / 拍照 / 相册 / 地理位置）
/// <b>仅支持 iPhone 5.4.1 以上与 Android 5.4 以上</b>版本微信；旧版本用户点击后<b>没有回应</b>，
/// 开发者也收不到事件推送。
/// </para>
/// <para>
/// <b>9~11 类的适用范围（易误用）</b>：<see cref="MediaId"/> / <see cref="ArticleId"/> /
/// <see cref="ArticleViewLimited"/> 是官方「专门给第三方平台旗下<b>未微信认证</b>公众号准备」的事件类型，
/// <b>没有事件推送</b>、能力受限；<b>其他类型的公众号 / 服务号不必使用</b>。
/// </para>
/// <para>
/// <b>官方接口变更</b>：草稿接口灰度完成后<b>不再支持图文信息类型的 <c>media_id</c> 与 <c>view_limited</c></b>，
/// 有需要的应改用 <see cref="ArticleId"/> 与 <see cref="ArticleViewLimited"/>。
/// </para>
/// </remarks>
public static class MpMenuButtonTypes
{
    /// <summary>click：点击推事件——微信服务器按消息接口推送 <c>event</c> 结构并带上按钮 <c>key</c>。</summary>
    public const string Click = "click";

    /// <summary>view：跳转 URL——客户端打开按钮 <c>url</c>（可结合网页授权获取用户基本信息）。</summary>
    public const string View = "view";

    /// <summary>scancode_push：扫码推事件（仅 iPhone 5.4.1+ / Android 5.4+）。</summary>
    public const string ScanCodePush = "scancode_push";

    /// <summary>scancode_waitmsg：扫码推事件且弹出「消息接收中」提示框（版本门槛同上）。</summary>
    public const string ScanCodeWaitMsg = "scancode_waitmsg";

    /// <summary>pic_sysphoto：弹出系统拍照发图（版本门槛同上）。</summary>
    public const string PicSysPhoto = "pic_sysphoto";

    /// <summary>pic_photo_or_album：弹出拍照或相册发图（版本门槛同上）。</summary>
    public const string PicPhotoOrAlbum = "pic_photo_or_album";

    /// <summary>pic_weixin：弹出微信相册发图器（版本门槛同上）。</summary>
    public const string PicWeixin = "pic_weixin";

    /// <summary>location_select：弹出地理位置选择器（版本门槛同上）。</summary>
    public const string LocationSelect = "location_select";

    /// <summary>media_id：下发永久素材（图片 / 音频 / 视频）；<b>无事件推送</b>，面向未认证的第三方旗下公众号。</summary>
    public const string MediaId = "media_id";

    /// <summary>article_id：以卡片形式下发 <c>article_id</c> 对应的图文消息（官方推荐替代 <c>media_id</c>）。</summary>
    public const string ArticleId = "article_id";

    /// <summary>article_view_limited：类似 view_limited，但使用 <c>article_id</c> 而非 <c>media_id</c>。</summary>
    public const string ArticleViewLimited = "article_view_limited";

    /// <summary>miniprogram：跳转小程序（<c>appid</c> 仅认证账号可配置且必须小写；<c>pagepath</c> 必填）。</summary>
    public const string MiniProgram = "miniprogram";
}

/// <summary>
/// 个性化菜单「客户端系统型号」常量（官方 <c>matchrule.client_platform_type</c>，取值为字符串）。
/// </summary>
public static class MpMenuClientPlatformTypes
{
    /// <summary>iOS（<c>1</c>）。</summary>
    public const string Ios = "1";

    /// <summary>Android（<c>2</c>）。</summary>
    public const string Android = "2";

    /// <summary>其他（<c>3</c>）。</summary>
    public const string Others = "3";
}

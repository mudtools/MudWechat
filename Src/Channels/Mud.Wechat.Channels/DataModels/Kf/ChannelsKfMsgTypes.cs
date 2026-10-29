// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Kf;

/// <summary>商家客服消息类型常量（官方 <c>msg_type</c> 枚举值，<c>sendmsg</c>）。</summary>
public static class ChannelsKfMsgTypes
{
    /// <summary>文本消息。</summary>
    public const string Text = "text";

    /// <summary>图片消息。</summary>
    public const string Image = "image";

    /// <summary>视频消息。</summary>
    public const string Video = "video";

    /// <summary>商品卡片消息。</summary>
    public const string ProductShare = "product_share";

    /// <summary>订单卡片消息。</summary>
    public const string OrderShare = "order_share";

    /// <summary>文件消息。</summary>
    public const string File = "file";
}

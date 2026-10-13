// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间类型取值（<c>type</c>，<see cref="WxaLiveCreateRoomRequest.Type"/>；
/// 应答侧对应 <c>live_type</c>，<see cref="WxaLiveRoomInfoItem.LiveType"/>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_createroom.html"/>。
/// 官方原文「直播间类型 【1: 推流，0：手机直播】」⇒ 取值仅两值，且<b>只能创建时指定</b>
/// （<c>editroom</c> 参数表不含该字段）。应答侧字段名换为 <c>live_type</c> 而取值口径相同。
/// </remarks>
public static class WxaLiveRoomType
{
    /// <summary>手机直播（<c>0</c>）。</summary>
    public const int Mobile = 0;

    /// <summary>推流（<c>1</c>）——需另行取推流地址，见 <c>getpushurl</c>。</summary>
    public const int Push = 1;
}

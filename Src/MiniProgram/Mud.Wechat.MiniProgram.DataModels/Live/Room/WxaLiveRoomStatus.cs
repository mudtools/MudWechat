// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播间状态取值（<c>live_status</c>，<see cref="WxaLiveRoomInfoItem.LiveStatus"/>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// 官方原文「101：直播中，102：未开始，103已结束，104禁播，105：暂停，106：异常，107：已过期」
/// —— 官方 <c>103</c> 一栏<b>缺冒号</b>，属原文排版差异，取值本身连续为 101~107 七值。
/// 房间状态即官方多处 <c>300023</c>「房间状态拦截」的判据来源（删除 / 上下架 / 推送等操作各有允许状态）。
/// </remarks>
public static class WxaLiveRoomStatus
{
    /// <summary>直播中（<c>101</c>）。</summary>
    public const int Living = 101;

    /// <summary>未开始（<c>102</c>）。</summary>
    public const int NotStarted = 102;

    /// <summary>已结束（<c>103</c>）。</summary>
    public const int Ended = 103;

    /// <summary>禁播（<c>104</c>）。</summary>
    public const int Banned = 104;

    /// <summary>暂停（<c>105</c>）。</summary>
    public const int Paused = 105;

    /// <summary>异常（<c>106</c>）。</summary>
    public const int Abnormal = 106;

    /// <summary>已过期（<c>107</c>）。</summary>
    public const int Expired = 107;
}

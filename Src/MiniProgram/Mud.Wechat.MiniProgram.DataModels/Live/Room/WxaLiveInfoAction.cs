// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// <c>getliveinfo</c> 的 <c>action</c> 取值（官方原文「只能填 <c>"get_replay"</c>，表示获取回放」）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
/// 官方只给出<b>唯一</b>取值 ⇒ 本常量类只登记该值；缺省（不传）为「拉取直播间列表」形态，
/// 传本值则应答改为 <c>live_replay</c> 数组且<b>不返回</b> <c>room_info</c>。不得凭空造其他动作值。
/// </remarks>
public static class WxaLiveInfoAction
{
    /// <summary>获取指定直播间的回放列表（官方原文唯一取值 <c>"get_replay"</c>）。</summary>
    public const string GetReplay = "get_replay";
}

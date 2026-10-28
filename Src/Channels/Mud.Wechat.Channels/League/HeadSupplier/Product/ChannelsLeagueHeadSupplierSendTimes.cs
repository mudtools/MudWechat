// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 发货时间期限常量（官方 <c>freight_info.send_time</c>「枚举信息」章节，2026-03-09 新增）。
/// </summary>
/// <remarks>官方枚举字面量 <c>SendTime_FOUTYEIGHT_HOUR</c> 为原文拼写（非 <c>FORTY</c>）⇒ 照抄不改，避免与网关实际回包不匹配。</remarks>
public static class ChannelsLeagueHeadSupplierSendTimes
{
    /// <summary>24 小时内发货。</summary>
    public const string TwentyFourHour = "SendTime_TWENTYFOUR_HOUR";

    /// <summary>48 小时内发货（官方原文拼写为 <c>FOUTYEIGHT</c>）。</summary>
    public const string FortyEightHour = "SendTime_FOUTYEIGHT_HOUR";

    /// <summary>今日发。</summary>
    public const string ShipToday = "SendTime_SHIP_TODAY";
}

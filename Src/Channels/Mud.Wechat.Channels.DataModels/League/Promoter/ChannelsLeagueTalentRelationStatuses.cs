// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>带货者推广状态常量（官方 <c>talent_relation_status</c> 枚举值，<c>promotion/talent/list</c>）。</summary>
public static class ChannelsLeagueTalentRelationStatuses
{
    /// <summary>推广中。</summary>
    public const int Promoting = 1;

    /// <summary>已移除。</summary>
    public const int Removed = 4;

    /// <summary>已被拉黑。</summary>
    public const int Blacklisted = 10;
}

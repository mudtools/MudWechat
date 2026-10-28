// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>提现状态常量（官方 <c>status</c> 枚举值）。</summary>
public static class ChannelsWithdrawStatuses
{
    /// <summary>业务单已创建。</summary>
    public const string Init = "INIT";

    /// <summary>受理成功。</summary>
    public const string CreateSuccess = "CREATE_SUCCESS";

    /// <summary>提现成功。</summary>
    public const string Success = "SUCCESS";

    /// <summary>提现失败。</summary>
    public const string Fail = "FAIL";

    /// <summary>提现退票。</summary>
    public const string Refund = "REFUND";

    /// <summary>关单。</summary>
    public const string Close = "CLOSE";
}

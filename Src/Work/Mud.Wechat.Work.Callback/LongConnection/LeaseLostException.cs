// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 长连接<b>租约已易主</b>（续租被拒）—— 预期控制流，不是故障。
/// </summary>
/// <remarks>
/// 由 <see cref="WechatBotLeaseRenewer"/> 在续租返回 <c>false</c> 时抛出，经
/// <c>WechatBotConnection.RunAsync</c> 的专门 catch 收敛为「退避后重走夺取流程」并按 Information 记录
/// （不进异常告警路径）。官方 101463「每个机器人同一时间仅一条有效连接」：备实例夺租建连即踢旧连接，
/// 现实例收到 <c>disconnected_event</c> 后同样走退避重夺 —— 乒乓是官方切换机制，本异常只是它的程序内表达。
/// </remarks>
internal sealed class LeaseLostException : Exception
{
    /// <summary>创建租约易主异常。</summary>
    /// <param name="botKey">机器人键（租约分槽键）。</param>
    public LeaseLostException(string botKey)
        : base($"智能机器人 {botKey} 的长连接租约已易主（续租被拒）：放弃当前连接，退避后重走夺取流程。")
    {
        BotKey = botKey;
    }

    /// <summary>机器人键（租约分槽键）。</summary>
    public string BotKey { get; }
}

#endif

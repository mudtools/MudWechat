// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Demo.Shared;

/// <summary>
/// 写操作闸：创建成员、发消息、群发、退款等会对真实企业产生副作用的端点，
/// 默认<b>不映射</b>（请求得到 404），仅在 <c>MUDWECHAT_DEMO_ALLOW_WRITE=1</c> 时启用。
/// </summary>
public static class DemoWriteGate
{
    /// <summary>开关变量名（不含前缀）。</summary>
    public const string EnvName = "ALLOW_WRITE";

    /// <summary>写操作是否放行。</summary>
    public static bool Enabled => DemoEnv.Flag(EnvName);

    /// <summary>
    /// 启动期播报一次闸状态：启用时按 Warning 级别输出（提醒不得指向生产账号），关闭时按 Information 说明放行方式。
    /// </summary>
    public static void Announce(ILogger logger)
    {
        if (Enabled)
        {
            logger.LogWarning(
                "写操作端点已启用（{Env}=1）：本 Demo 会对所配置的企业产生真实副作用，请确认指向测试账号。",
                DemoEnv.FullName(EnvName));
            return;
        }

        logger.LogInformation(
            "写操作端点未映射（默认）：如需执行创建/发送类端点，设置 {Env}=1 后重启。",
            DemoEnv.FullName(EnvName));
    }
}

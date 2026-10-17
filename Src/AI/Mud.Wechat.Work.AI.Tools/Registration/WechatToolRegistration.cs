// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Registration;

/// <summary>
/// 注册表写入的单源入口：生成物的域注册器只经本类型登记执行入口。
/// </summary>
/// <remarks>
/// <b>为什么不让生成物直接调 <see cref="WechatToolRegistry.Register"/> 之外的东西</b>：登记是「一个工具一条」的
/// 机械动作，重复实现会引入两种口径（有的带绑定、有的不带），进而让运行期取不到应用归属。收口到本类型后，
/// 生成物的调用形态由引擎固定，产品侧改注册表结构不会波及产物。
/// </remarks>
public static class WechatToolRegistration
{
    /// <summary>登记一个工具的执行入口。</summary>
    /// <param name="registry">工具注册表（不得为 <see langword="null"/>）。</param>
    /// <param name="toolName">工具名（生成物传入的名字契约常量）。</param>
    /// <param name="binding">执行绑定（应用/企业归属）。</param>
    /// <param name="executeAsync">执行委托（执行器方法）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="registry"/> 为 <see langword="null"/>。</exception>
    public static void RegisterExecution(
        WechatToolRegistry registry,
        string toolName,
        WechatToolBinding binding,
        Func<IReadOnlyDictionary<string, object?>, CancellationToken, Task<ToolResult>> executeAsync)
    {
        if (registry is null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(new WechatToolExecution(toolName, binding, executeAsync));
    }
}

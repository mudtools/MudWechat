// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.AIFunctions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;

namespace Mud.Wechat.Work.AI.Tools.Extensions;

/// <summary>
/// 企微 AI 工具面的 DI 装配入口：<c>AddWechatWorkTools()</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>注册清单（全部 <c>TryAdd</c>/<c>Singleton</c>）</b>：
/// <list type="number">
/// <item>执行绑定 <see cref="WechatToolBinding"/>（应用/企业归属的唯一来源）；</item>
/// <item>逐执行器的工厂（生成物 <c>Add*Core</c> 方法，按域聚合；依赖缺席 ⇒ 该域执行器为 <see langword="null"/>）；</item>
/// <item>工具注册表 <see cref="WechatToolRegistry"/>（首次解析时把可用域的注册器汇总进来）。</item>
/// </list>
/// </para>
/// <para>
/// <b>前置条件</b>：宿主已调用 <c>AddWechatApp(...)</c> 并启用相应业务域端点（如 <c>AddContactApi()</c>）——
/// 未启用即该域工具缺席（软缺席，不抛）；工具面与业务域注册顺序无关。
/// </para>
/// <para>
/// <b>与 Agent 装配的关系</b>：本入口只装「工具面」，不注册 <c>IChatClient</c> / 会话 / 通道；Agent 侧
/// （<c>AddWechatWorkAgent()</c>）可在本方法之前或之后调用。
/// </para>
/// </remarks>
public static class WechatWorkToolServiceCollectionExtensions
{
    /// <summary>注册企微工具面（执行绑定 + 执行器 + 注册表）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="binding">执行绑定（不得为 <see langword="null"/>）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="binding"/> 为 <see langword="null"/>。</exception>
    public static IServiceCollection AddWechatWorkTools(this IServiceCollection services, WechatToolBinding binding)
        => AddWechatWorkTools(services, binding, configurePolicy: null);

    /// <summary>注册企微工具面（可配置白名单策略）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="binding">执行绑定（不得为 <see langword="null"/>）。</param>
    /// <param name="configurePolicy">白名单策略配置（可空 = 默认只读全开、写类全关、强制授权）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="binding"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="InvalidOperationException">策略非法（未知工具名 / 写工具未开授权）。</exception>
    public static IServiceCollection AddWechatWorkTools(
        this IServiceCollection services,
        WechatToolBinding binding,
        Action<WechatToolPolicy>? configurePolicy)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (binding is null)
        {
            throw new ArgumentNullException(nameof(binding));
        }

        services.TryAddSingleton(binding);

        // 生成物：逐执行器的「依赖缺席 ⇒ null 执行器」工厂 + 域注册器工厂。
        services.AddWechatApprovalToolHandlersCore();
        services.AddWechatCheckinToolHandlersCore();
        services.AddWechatContactsDepartmentToolHandlersCore();
        services.AddWechatContactsUserToolHandlersCore();
        services.AddWechatContactsTagToolHandlersCore();
        services.AddWechatExternalContactToolHandlersCore();
        services.AddWechatExternalContactGroupChatToolHandlersCore();
        services.AddWechatExternalContactMomentToolHandlersCore();
        services.AddWechatExternalContactStatisticsToolHandlersCore();
        services.AddWechatExternalContactGroupMsgToolHandlersCore();
        services.AddWechatExternalContactProductAlbumToolHandlersCore();
        services.AddWechatExternalContactInheritanceToolHandlersCore();
        services.AddWechatJournalToolHandlersCore();
        services.AddWechatKfToolHandlersCore();
        services.AddWechatLivingToolHandlersCore();
        services.AddWechatMeetingToolHandlersCore();
        services.AddWechatMessageToolHandlersCore();
        services.AddWechatWedocToolHandlersCore();

        services.TryAddSingleton(static provider => WechatToolDomainRegistrars.Build(provider));

        // 白名单在装配期冻结（运行期不可改），授权器由宿主前置注册（缺失 ⇒ 强制授权时一律拒绝）。
        var policy = new WechatToolPolicy();
        configurePolicy?.Invoke(policy);
        policy.Validate();
        services.TryAddSingleton(policy);
        services.TryAddSingleton<WechatToolDispatcher>();

        // 模型可见面 = 白名单投影（看不见即不可试）；Agent 侧按 IReadOnlyList<AIFunction> 注入。
        services.TryAddSingleton(static provider => WechatToolFunctions.Create(provider.GetRequiredService<WechatToolDispatcher>()));

        return services;
    }
}

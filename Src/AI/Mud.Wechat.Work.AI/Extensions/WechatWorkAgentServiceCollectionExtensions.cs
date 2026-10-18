// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Mud.AI.Abstractions.Observability;
using Mud.AI.Agent.Events;
using Mud.AI.Agent.Extensions;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Configuration;
using Mud.Wechat.Work.AI.Events;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Agent.Conversations;

namespace Mud.Wechat.Work.AI.Extensions;

/// <summary>
/// 企微 AI Agent 的 DI 装配入口：<c>AddWechatWorkAgent()</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>注册清单（顺序即依赖顺序，全部 <c>TryAdd</c> ⇒ 先注册者生效）</b>：
/// <list type="number">
/// <item>配置面 + 校验器（<see cref="WechatAgentOptions"/> / <see cref="WechatAgentOptionsValidator"/>）；</item>
/// <item>会话存储：<b>产品 TTL 先行注册</b>（<see cref="ISessionStore"/> → <c>MemorySessionStore(TTL)</c>），
/// 使中立骨架 <c>AddAgentRuntimeCore</c> 的默认 TTL 不生效 —— 产品薄壳必须显式给出自己的单一阈值源；</item>
/// <item>中立运行期默认件（遥测空实现 / 待确认快照），由 <c>AddAgentRuntimeCore</c> 补齐；</item>
/// <item>企微侧端口实现：工具身份上下文 / <see cref="ITurnScopeFactory"/> 作用域工厂 / 后台回合调度器；</item>
/// <item><see cref="WechatAgent"/> 单例（延迟解析 <see cref="IChatClient"/>，缺失时给出可操作错误）；</item>
/// <item><see cref="WechatBotAgentHandler"/>（transient；宿主经
/// <c>AddWechatBotCallback().AddHandler&lt;WechatBotAgentHandler&gt;()</c> 挂到机器人路由）。</item>
/// </list>
/// </para>
/// <para>
/// <b>宿主前置条件</b>：① 已注册 <see cref="IChatClient"/>（如 <c>Microsoft.Extensions.AI</c> 的客户端）；
/// ② 已调用 <c>AddWechatApp(...)</c>（多应用注册表 + <c>IWechatAppContextSwitcher</c>）与
/// <c>AddWechatBotCallback(...)</c>；③ 已注册智能机器人端点（<c>AddAibotApi()</c> 或 <c>AddAllApis()</c>），
/// 否则 <see cref="WechatBotAgentHandler"/> 激活时因缺少 <c>IWechatWorkInternalAibotService</c> 而 fail-fast。
/// </para>
/// <para>
/// <b>与多应用/回调注册不冲突</b>：本入口只注册 AI 域自有类型 + 复用中立骨架的 <c>TryAdd</c>；不触碰
/// <c>WechatModule</c> 枚举、不注册任何业务域端点，故与 <c>AddWechatApp</c> / <c>AddWechatCallback</c> /
/// <c>AddWechatBotCallback</c> 可任意顺序共存（唯一顺序约束是中立骨架的 <c>TryAdd</c> 语义：宿主若要覆盖
/// <see cref="ISessionStore"/>，须在调用本方法<b>之前</b>注册自己的实现）。
/// </para>
/// </remarks>
public static class WechatWorkAgentServiceCollectionExtensions
{
    /// <summary>注册企微 AI Agent（委托配置，不经配置文件）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托（可空 = 全部默认值）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <see langword="null"/>。</exception>
    public static IServiceCollection AddWechatWorkAgent(
        this IServiceCollection services,
        Action<WechatAgentOptions>? configure = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure is not null)
        {
            services.Configure(configure);
        }

        return AddWechatWorkAgentCore(services);
    }

    /// <summary>注册企微 AI Agent（配置节绑定：<c>WechatAi</c>）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置根（源生成绑定，走 <c>section.Bind(o)</c>）。</param>
    /// <param name="sectionName">配置节名（默认 <see cref="WechatAgentOptions.DefaultSectionName"/>）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="configuration"/> 为 <see langword="null"/>。</exception>
    public static IServiceCollection AddWechatWorkAgent(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = WechatAgentOptions.DefaultSectionName)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // 源生成绑定：必须走 Configure<T>(o => section.Bind(o))，不得用 Configure<T>(IConfiguration) 重载
        // （反射绑定破坏 AOT/IL2026 净零，见 AGENTS.md §3）。
        services.Configure<WechatAgentOptions>(options => configuration.GetSection(sectionName).Bind(options));
        return AddWechatWorkAgentCore(services);
    }

    /// <summary>公共注册体（两个重载共用；幂等）。</summary>
    private static IServiceCollection AddWechatWorkAgentCore(IServiceCollection services)
    {
        // ① 配置面 + 校验器（规则单一来源在 WechatAgentOptions.Validate）。
        services.AddOptions<WechatAgentOptions>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<WechatAgentOptions>, WechatAgentOptionsValidator>());

        // ② 会话存储：产品 TTL 先行注册（先注册者生效，压过 AddAgentRuntimeCore 的默认 TTL）。
        services.TryAddSingleton<ISessionStore>(static sp =>
        {
            var seconds = sp.GetRequiredService<IOptions<WechatAgentOptions>>().Value.SessionTtlSeconds;
            return new MemorySessionStore(TimeSpan.FromSeconds(seconds));
        });

        // ③ 中立运行期默认件（遥测空实现 / 待确认快照；全部 TryAdd）。
        services.AddAgentRuntimeCore();

        // ④ 企微侧端口实现。
        services.TryAddSingleton<IWechatAiCorpContextAccessor, WechatAiCorpContextAccessor>();
        services.TryAddSingleton<ITurnScopeFactory, WechatWorkAiScopeFactory>();
        services.TryAddSingleton<IWechatAgentTurnScheduler, ThreadPoolAgentTurnScheduler>();

        // ⑤ Agent 单例（延迟解析 IChatClient ⇒ 缺失时错误可操作）。
        services.TryAddSingleton(CreateAgent);

        // ⑥ 回调处理器（宿主负责挂到机器人路由）：按事件族显式注册，共用 WechatAgentTurnRunner。
        //    新增族 = 加一行 TryAddTransient + 继承 WechatBotEventHandlerBase（不触碰既有族）。
        services.TryAddTransient<WechatBotAgentHandler>();
        services.TryAddTransient<WechatBotMixedMessageHandler>();
        services.TryAddTransient<WechatBotVoiceMessageHandler>();

        // ⑦ 事件装配器（中立兜底；宿主可前置注册自己的 IContextAssembler 覆写某族的装配策略）。
        services.TryAddSingleton<IContextAssembler, DefaultContextAssembler>();

        // ⑧ 回合事实装配器（对齐中立 Mud.AI.Abstractions.Context.IContextAssembler 槽位）：
        //    时间锚点 + 运行身份。它不占工具预算、不经白名单，也不是「元工具」——事实恒定注入，能力才需模型调用。
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<Mud.AI.Abstractions.Context.IContextAssembler, WechatTurnContextAssembler>());

        return services;
    }

    /// <summary>构造 Agent（解析 <see cref="IChatClient"/> 并给出可操作错误）。</summary>
    private static WechatAgent CreateAgent(IServiceProvider services)
    {
        var chatClient = services.GetService<IChatClient>();
        if (chatClient is null)
        {
            throw new InvalidOperationException(
                "AddWechatWorkAgent() 需要宿主先注册模型客户端 IChatClient"
                + "（如 services.AddChatClient(new OpenAIClient(apiKey).GetChatClient(model))）。"
                + "注册顺序无关，但容器内必须存在一个 IChatClient 实现。");
        }

        // 工具面由 AddWechatWorkTools() 注册（可选）：未装配工具面时该解析为 null，Agent 照常工作。
        return new WechatAgent(
            chatClient,
            services.GetRequiredService<IOptions<WechatAgentOptions>>(),
            services.GetRequiredService<ISessionStore>(),
            services.GetService<IAgentTelemetry>(),
            services.GetService<ITurnScopeFactory>(),
            services.GetService<IReadOnlyList<AIFunction>>(),
            services.GetService<ILogger<WechatAgent>>(),
            services.GetServices<Mud.AI.Abstractions.Context.IContextAssembler>());
    }
}

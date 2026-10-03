// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Payloads;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 官方事件键契约表（41 键）：事件键 → 上游映射表 + 本仓库两级开放面声明。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何映射表在这里被「具体类型静态成员访问」</b>：C# 禁止在泛型上下文访问类型参数的静态成员
/// （<c>TPayload.PayloadFieldMap</c> 是 CS0712），而 <c>static abstract</c> 需 net7+ 运行时支持、
/// 本仓库含 <c>netstandard2.0</c>/<c>net6.0</c> ⇒ 注册入口只能在<b>具体类型</b>处书写。
/// 这正是上游 <see cref="IPayloadContractAccessor"/>（非泛型桥）存在的理由。
/// </para>
/// <para>
/// <b>族前置条件（B4）</b>：每条契约都显式声明 <c>RequiredEvent</c> 与 <c>RequiredFamily</c>，
/// 防止「同名 <c>ChangeType</c>」跨族串门（如 <c>Event=change_contact</c> 却带 <c>ChangeType=create_chain</c>
/// 的报文不得进入上下游载荷）。
/// </para>
/// <para>
/// <b>事件键级开放面（ADR-15）</b>：每条契约<b>显式</b>声明 <c>SupportedAppTypes</c>/<c>RequiredChannel</c>
/// （守卫 CB22 断言不得隐式继承）。当前官方开放面恰好与族粒度重合，显式声明的作用是
/// 把未来的「改闸 + 改守卫」降级为「改一行声明」。
/// </para>
/// <para>
/// <b>逐条对照官方文档</b>（守卫 <c>OfficialPayloadContractsOpenSurfaceTests</c>）：
/// 通讯录 90967/90970/90971/90972、异步任务 90973/95797、上下游 95796、
/// 消息与事件 90240（企业内部开发）/ 90376（第三方）/ 96468（服务商代开发，正文逐字一致）。
/// 授权族 7 键<b>不登记契约</b>（走信封，ADR-8）。
/// </para>
/// <para>
/// <b>三模式说明</b>：90240/90376/96468 三份文档正文一致 ⇒ 载荷与字段结构三模式同一（ADR-14），
/// 差异仅体现在个别事件的<b>开放面</b>声明上（<c>open_approval_change</c> 不含代开发、
/// <c>share_agent_change</c>/<c>share_chain_change</c> 仅自建），该差异由本表承载，<b>不</b>下沉重荷类型。
/// </para>
/// </remarks>
public static class OfficialPayloadContracts
{
    /// <summary>登记全部官方事件键契约（组合根期调用一次）。</summary>
    /// <param name="registry">契约注册表。</param>
    public static void RegisterAll(IWechatPayloadContractRegistry registry)
    {
        if (registry == null)
            throw new ArgumentNullException(nameof(registry));

        // ——— 通讯录变更族：三类应用 + 应用数据通道（官方 90967/90970/90971/90972）———
        RegisterAll(registry,
            WechatCallbackEventTypes.ChangeContact,
            WechatCallbackEventFamily.ContactChange,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            ContactUserChangedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.CreateUser,
            WechatCallbackEventTypes.UpdateUser,
            WechatCallbackEventTypes.DeleteUser);

        RegisterAll(registry,
            WechatCallbackEventTypes.ChangeContact,
            WechatCallbackEventFamily.ContactChange,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            ContactPartyChangedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.CreateParty,
            WechatCallbackEventTypes.UpdateParty,
            WechatCallbackEventTypes.DeleteParty);

        RegisterAll(registry,
            WechatCallbackEventTypes.ChangeContact,
            WechatCallbackEventFamily.ContactChange,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            ContactTagChangedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.UpdateTag);

        // ——— 异步任务族：三类应用 + 应用数据通道（官方 90973 / 95797，双布局经 ScopeFallback）———
        RegisterAll(registry,
            WechatCallbackEventTypes.BatchJobResult,
            WechatCallbackEventFamily.BatchJob,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            BatchJobCompletedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.BatchJobResult);

        // ——— 上下游变更族：仅企业自建 + 应用数据通道（官方 95796）———
        RegisterAll(registry,
            WechatCallbackEventTypes.ChangeChain,
            WechatCallbackEventFamily.ChainChange,
            WechatAppTypeSet.Internal,
            WechatCallbackChannel.App,
            ChainChangedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.CreateChain,
            WechatCallbackEventTypes.UpdateChain,
            WechatCallbackEventTypes.DeleteChain,
            WechatCallbackEventTypes.CreateGroup,
            WechatCallbackEventTypes.UpdateGroup,
            WechatCallbackEventTypes.DeleteGroup,
            WechatCallbackEventTypes.CorpJoin,
            WechatCallbackEventTypes.UpdateCorp,
            WechatCallbackEventTypes.RemoveCorp);

        // ——— 官方 90240 消息与事件（Event 即事件键本身，无外层分组信封值；族为 Unknown）———

        // 平铺型事件：三类应用 + 应用数据通道（关注 / 进入应用 / 菜单 KEY 类 / 应用状态类）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            PlainEventPayload.PayloadFieldMap,
            WechatCallbackEventTypes.Subscribe,
            WechatCallbackEventTypes.Unsubscribe,
            WechatCallbackEventTypes.EnterAgent,
            WechatCallbackEventTypes.Click,
            WechatCallbackEventTypes.View,
            WechatCallbackEventTypes.ViewMiniProgram,
            WechatCallbackEventTypes.CloseInactiveAgent,
            WechatCallbackEventTypes.ReopenInactiveAgent,
            WechatCallbackEventTypes.LowActive,
            WechatCallbackEventTypes.ActiveRestored);

        // 共享应用：官方触发时机为「共享自建应用」⇒ 仅企业自建（与上一组共用同一载荷，开放面不同）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.Internal,
            WechatCallbackChannel.App,
            PlainEventPayload.PayloadFieldMap,
            WechatCallbackEventTypes.ShareAgentChange,
            WechatCallbackEventTypes.ShareChainChange);

        // 应用预警：三类应用（停用预警 / 低活跃预警，携带 EffectTime）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            AgentAlertPayload.PayloadFieldMap,
            WechatCallbackEventTypes.InactiveAlert,
            WechatCallbackEventTypes.LowActiveAlert);

        // 菜单事件：扫码 / 发图 / 位置选择（三类应用）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            MenuScanCodePayload.PayloadFieldMap,
            WechatCallbackEventTypes.ScanCodePush,
            WechatCallbackEventTypes.ScanCodeWaitMsg);

        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            MenuPicPayload.PayloadFieldMap,
            WechatCallbackEventTypes.PicSysPhoto,
            WechatCallbackEventTypes.PicPhotoOrAlbum,
            WechatCallbackEventTypes.PicWeixin);

        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            MenuLocationSelectPayload.PayloadFieldMap,
            WechatCallbackEventTypes.LocationSelect);

        // 上报地理位置（官方键值大写 LOCATION）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            LocationReportedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.Location);

        // 审批状态通知：官方触发时机为「自建/第三方应用调用审批流程引擎」⇒ 不含代开发。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.Internal | WechatAppTypeSet.ThirdParty,
            WechatCallbackChannel.App,
            ApprovalStatusChangedPayload.PayloadFieldMap,
            WechatCallbackEventTypes.OpenApprovalChange);

        // 模板卡片：按钮点击 + 右上角菜单（三类应用，同一载荷）。
        RegisterKeys(registry,
            WechatCallbackEventFamily.Unknown,
            WechatAppTypeSet.All,
            WechatCallbackChannel.App,
            TemplateCardEventPayload.PayloadFieldMap,
            WechatCallbackEventTypes.TemplateCardEvent,
            WechatCallbackEventTypes.TemplateCardMenuEvent);
    }

    /// <summary>
    /// 按「事件键自身即 <c>RequiredEvent</c>」登记一组同载荷契约。
    /// </summary>
    /// <remarks>
    /// 官方 path 90240 的事件<b>没有</b> <c>change_contact</c>/<c>change_chain</c> 那样的外层分组信封值 ——
    /// <c>Event</c> 节点本身就承载事件键（如 <c>click</c>、<c>LOCATION</c>）。
    /// 故 <c>RequiredEvent = 事件键</c>：既满足 B4 的族前置条件（<c>Unknown</c> 族按 <c>Event</c> 精确比对），
    /// 又避免 24 行逐键重复书写。
    /// </remarks>
    private static void RegisterKeys<TPayload>(
        IWechatPayloadContractRegistry registry,
        WechatCallbackEventFamily requiredFamily,
        WechatAppTypeSet supportedAppTypes,
        WechatCallbackChannel requiredChannel,
        IPayloadFieldMap<TPayload> map,
        params string[] eventTypeKeys)
        where TPayload : class
    {
        var accessor = AsAccessor(map);

        for (var i = 0; i < eventTypeKeys.Length; i++)
        {
            registry.Register(WechatPayloadContract.CreateWithOpenSurface(
                eventTypeKeys[i], accessor, supportedAppTypes, requiredChannel,
                requiredEvent: eventTypeKeys[i], requiredFamily: requiredFamily));
        }
    }

    private static void RegisterAll<TPayload>(
        IWechatPayloadContractRegistry registry,
        string requiredEvent,
        WechatCallbackEventFamily requiredFamily,
        WechatAppTypeSet supportedAppTypes,
        WechatCallbackChannel requiredChannel,
        IPayloadFieldMap<TPayload> map,
        params string[] eventTypeKeys)
        where TPayload : class
    {
        var accessor = AsAccessor(map);

        for (var i = 0; i < eventTypeKeys.Length; i++)
        {
            registry.Register(WechatPayloadContract.CreateWithOpenSurface(
                eventTypeKeys[i], accessor, supportedAppTypes, requiredChannel, requiredEvent, requiredFamily));
        }
    }

    /// <summary>
    /// 取映射表的非泛型视图。
    /// </summary>
    /// <remarks>
    /// <b>为何需要一次显式转换</b>：泛型接口 <see cref="IPayloadFieldMap{T}"/> 以 <c>in T</c> 声明（可逆变），
    /// 故<b>不能</b>继承含 <c>CreateInstance()</c>（返回位置）的 <see cref="IPayloadContractAccessor"/>；
    /// 实现两条接口的是<b>具体类</b> <c>PayloadFieldMap&lt;T&gt;</c>。
    /// 生成物与手写链的静态类型都是 <see cref="IPayloadFieldMap{T}"/>，故在此一次性转出非泛型视图。
    /// 手写链若使用非 <c>PayloadFieldMap&lt;T&gt;</c> 的实现，将在<b>组合根期</b>以本方法的信息明确的异常失败
    /// —— 优于在读取路径上抛裸 <c>InvalidCastException</c>。
    /// </remarks>
    /// <exception cref="ArgumentException">映射表未实现 <see cref="IPayloadContractAccessor"/>。</exception>
    private static IPayloadContractAccessor AsAccessor<TPayload>(IPayloadFieldMap<TPayload> map)
        where TPayload : class
    {
        if (map == null)
            throw new ArgumentNullException(nameof(map));

        if (map is IPayloadContractAccessor accessor)
            return accessor;

        throw new ArgumentException(
            "映射表 " + map.GetType().FullName + " 未实现 " + nameof(IPayloadContractAccessor) +
            "；请使用上游 PayloadFieldMap<T> 构建映射表（生成链与手写链均满足该要求）。", nameof(map));
    }
}

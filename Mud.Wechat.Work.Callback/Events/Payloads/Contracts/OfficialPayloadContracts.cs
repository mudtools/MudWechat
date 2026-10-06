// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 官方事件键契约表（53 键）：事件键 → 上游映射表 + 本仓库两级开放面声明。
/// </summary>
/// <remarks>
/// <para>
/// <b>登记方法体由生成器发射</b>：事件键、族前置条件与开放面的权威声明是载荷类上的
/// <c>[WechatCallbackContract]</c> 特性（与 <c>[PayloadContract]</c> 并存 —— 载荷类是唯一契约源），
/// 本仓 <c>WechatCallbackContractRegistrationGenerator</c> 依据特性发射 <see cref="RegisterAll"/>
/// 方法体（生成物 <c>OfficialPayloadContracts.RegisterAll.g.cs</c>，勿手改）。
/// </para>
/// <para>
/// <b>为何映射表在生成物中被「具体类型静态成员访问」</b>：C# 禁止在泛型上下文访问类型参数的静态成员
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
/// <b>事件键级开放面（ADR-15）</b>：每条契约<b>显式</b>声明 <c>OpenSurfaces</c>（守卫 CB4c 断言不得隐式继承）。
/// 大多数族的官方开放面与族粒度重合；客户联系/获客族的接入方式按应用模式分通道
/// （自建·代开发×应用数据通道 + 第三方×套件指令通道），由<b>同键多特性声明合并</b>的多组开放面承载。
/// </para>
/// <para>
/// <b>逐条对照官方文档（核对字段以此为准）</b>（守卫 CB4b/CB4c，Tests/**/ContractGuards/WechatCallbackContractGuards.cs）：
/// 通讯录 <see href="https://developer.work.weixin.qq.com/document/path/90967">path 90967 通讯录回调概述</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90970">path 90970 成员变更</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90971">path 90971 部门变更</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90972">path 90972 标签成员变更</see>；
/// 异步任务 <see href="https://developer.work.weixin.qq.com/document/path/90973">path 90973 异步任务完成通知</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797 异步任务完成通知（上下游，BatchJob 包装）</see>
/// （结果文件调取见 <see href="https://developer.work.weixin.qq.com/document/path/92125">path 92125 获取异步任务结果</see>）；
/// 上下游 <see href="https://developer.work.weixin.qq.com/document/path/95796">path 95796 上下游变更回调</see>
/// （配套异步任务见 <see href="https://developer.work.weixin.qq.com/document/path/95797">path 95797 异步任务完成通知</see>）；
/// 安全管理 <see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080 企业微信域名IP变更事件</see>
/// （仅自建应用可配置接收；第三方/代开发暂不支持）；
/// 客户联系 <see href="https://developer.work.weixin.qq.com/document/path/92130">path 92130 事件回调（自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/92277">path 92277 事件回调（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96361">path 96361 事件回调（服务商代开发）</see>；
/// 获客助手 <see href="https://developer.work.weixin.qq.com/document/path/97299">path 97299 事件通知（自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97402">path 97402 事件通知（第三方，套件信封）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/99485">path 99485 获客助手组件（第三方组件形态）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/98958">path 98958 事件通知（代开发）</see>；
/// 消息与事件 <see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 接收消息与事件（企业内部开发）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90376">path 90376 接收消息与事件（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96468">path 96468 接收消息与事件（服务商代开发，正文逐字一致）</see>。
/// 授权族 7 键<b>不登记契约</b>（走信封，ADR-8）。
/// 收银台·应用版本付费订单回调族 91929~91933 / 99353（6 键，套件信封 InfoType，
/// 第三方 × 套件指令通道；与授权族同属 <c>InfoType</c> 信封但确有业务载荷，故登记强类型载荷）。
/// </para>
/// <para>
/// <b>三模式说明</b>：90240/90376/96468 三份文档正文一致 ⇒ 载荷与字段结构三模式同一（ADR-14），
/// 差异仅体现在个别事件的<b>开放面</b>声明上（<c>open_approval_change</c> 不含代开发、
/// <c>share_agent_change</c>/<c>share_chain_change</c> 仅自建），该差异由
/// <c>PlainEventPayload</c>/<c>ApprovalStatusChangedPayload</c> 的特性声明承载，<b>不</b>下沉重荷类型。
/// </para>
/// </remarks>
public static partial class OfficialPayloadContracts
{
    /// <summary>登记全部官方事件键契约（组合根期调用一次）。</summary>
    /// <param name="registry">契约注册表。</param>
    /// <remarks>方法体由 <c>WechatCallbackContractRegistrationGenerator</c> 依据
    /// <c>[WechatCallbackContract]</c> 特性声明发射；重复键在注册表处 fail-fast（接线错误）。</remarks>
    public static partial void RegisterAll(IWechatPayloadContractRegistry registry);
}

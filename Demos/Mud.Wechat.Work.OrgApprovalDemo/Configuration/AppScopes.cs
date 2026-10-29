// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.OrgApprovalDemo.Configuration;

/// <summary>
/// 两个自建应用的 AppKey（方案 §0「凭据形态」/ §8.1）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要两个应用</b>：通讯录写入端点的官方权限口径是「仅通讯录同步助手或第三方通讯录应用可调用」
/// ——<c>access_token</c> 本身不区分权限，但换令牌用的 <c>corpsecret</c> 必须是通讯录同步助手那一个；
/// 审批端点则要求应用被配置进「审批 - 可调用接口的应用」。两套凭据不能共用，故注册两个
/// <see cref="Mud.Wechat.Work.Abstractions.Configuration.WechatAppConfig"/>，各自持有独立的
/// <c>WechatAppContext</c> 与令牌管理器（令牌键含 AppKey，互不串）。
/// </para>
/// <para>
/// <b>注册形态</b>：<c>AddWechatApp(Action&lt;WechatAppConfig&gt;)</c> <b>不能调用两次</b>
/// （第二次会以新列表覆盖注册表并直接抛错，见 <c>AddWechatTokenInfrastructure</c> 的重复调用防护），
/// 因此 <c>Program.cs</c> 用 <c>AddWechatApp(List&lt;WechatAppConfig&gt;)</c> 一次性注册两个应用。
/// </para>
/// <para>
/// <b>作用域不变量</b>：任何 SDK 调用都必须包在显式应用作用域里（<c>WechatCallScope</c>），
/// 不依赖「默认应用」——名单里第一个应用会被自动置为默认，依赖它等于把身份绑到注册顺序上。
/// </para>
/// </remarks>
public static class AppScopes
{
    /// <summary>通讯录同步助手应用（读写部门/成员、<c>user/list_id</c> 游标）。</summary>
    public const string Contacts = "contacts";

    /// <summary>审批应用（模板详情/创建/更新、审批单回捞）。</summary>
    public const string Approval = "oa";
}

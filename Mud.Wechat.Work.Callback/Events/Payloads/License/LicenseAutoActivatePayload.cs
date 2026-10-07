// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 接口调用许可「自动激活回调通知」事件载荷（<c>auto_activate</c>，官方 path 97198，
/// 仅服务商代开发文档树提供——自建/第三方应用开发无对应事件回调，套件信封推送）。
/// </summary>
/// <remarks>
/// <para>
/// <b>事件语义</b>：成员满足自动激活条件并触发自动激活后，由企业微信后台推送；
/// 携带激活时机（<see cref="Scene"/>）与激活的许可账号列表（<see cref="AccountItems"/>）。
/// </para>
/// <para>
/// <b>对象列表形态</b>：<see cref="AccountItems"/> 的官方 <c>AccountList</c> 是<b>根下重复同名兄弟元素</b>
/// （无包装容器，多账号同时自动激活时重复出现），经根层同名兄弟合并投影 +
/// <see cref="WechatPayloadConverter.RepeatLicenseAccountItems"/> 读取（单元素报文同样覆盖）。
/// </para>
/// <para>
/// <b>不入载荷的字段</b>：<c>TimeStamp</c> 属信封字段（<c>WechatCallbackEvent.TimeStamp</c>，与 URL 验签
/// 时间戳同源），按 ADR-9 不在载荷中重复；<c>AuthCorpId</c>（客户企业 CorpID）由信封
/// <c>WechatCallbackEvent.AuthCorpId</c> 承载；<c>InfoType</c> 即事件键。
/// <c>ServiceCorpId</c>（服务商 CorpID）<b>须</b>由载荷承载：信封无对应字段。
/// </para>
/// <para>
/// <b>开放面</b>：本族走套件信封（<c>InfoType</c> 非空）⇒ 事件族为
/// <see cref="WechatCallbackEventFamily.Authorization"/>；官方仅在服务商代开发文档树提供，
/// 开放面声明为「代开发 × 套件指令通道」（不宽于族默认，组合根期 fail-fast 校验）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97198">path 97198 自动激活回调通知（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Authorization,
    SupportedAppTypes = WechatAppTypeSet.Provider,
    RequiredChannel = WechatCallbackChannel.Suite,
    EventTypes = new[] { WechatCallbackEventTypes.AutoActivate })]
public sealed partial class LicenseAutoActivatePayload : WechatCallbackPayload
{
    /// <summary>
    /// 服务商 CorpID（官方 <c>ServiceCorpId</c>）：信封无对应字段（本族报文亦无 <c>SuiteId</c> 节点），
    /// 服务商身份只能取自本字段。
    /// </summary>
    [PayloadField("ServiceCorpId")]
    public string? ServiceCorpId { get; set; }

    /// <summary>
    /// 自动激活时机（官方 <c>Scene</c>）：1 = 企业成员主动访问应用，2 = 服务商调用消息推送接口，
    /// 3 = 服务商调用互通接口。
    /// </summary>
    [PayloadField("Scene")]
    public long? Scene { get; set; }

    /// <summary>自动激活的许可账号列表（官方 <c>AccountList</c>，根下重复同名兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    [PayloadField("AccountList", Method = nameof(WechatPayloadConverter.RepeatLicenseAccountItems))]
    public List<WechatCallbackLicenseAccountItem> AccountItems { get; set; } =
        new List<WechatCallbackLicenseAccountItem>();
}

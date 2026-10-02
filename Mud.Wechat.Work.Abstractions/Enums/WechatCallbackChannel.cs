// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Enums;

/// <summary>
/// 企业微信回调通道（区别于 <see cref="Mud.Wechat.Work.Abstractions.Enums.WechatAppType"/> 的应用部署形态，
/// 描述一条回调 URL 承载的<b>回调类别</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 企业微信把回调拆成两条独立 URL：<b>指令回调 URL</b>（Suite 通道，服务商后台/「接收消息」里配置的
/// suite_ticket / 授权通知 / 取消授权等套件指令与票据事件）与<b>数据回调 URL</b>（App 通道，应用级
/// change_contact / batch_job_result / change_chain 等业务事件）。同一应用类型可能同时占有两类回调：
/// 第三方/代开发既有套件指令回调，又有面向授权企业的应用数据回调；自建应用只有应用数据回调。
/// </para>
/// <para>
/// 通道决定 <c>receiveid</c> 的校验语义（见 <c>WechatCallbackReceiver.ValidateReceiveId</c>）：
/// Suite 通道的 receiveid 恒为静态 <c>SuiteId</c>；App 通道的 receiveid 对企业自建为静态 <c>CorpId</c>，
/// 对第三方/代开发则为<b>动态的授权企业 CorpId</b>（随授权企业变化，只能比对解密明文外层 <c>ToUserName</c>）。
/// </para>
/// </remarks>
public enum WechatCallbackChannel
{
    /// <summary>应用数据/事件回调通道（receiveid：自建 = CorpId，第三方/代开发 = 授权企业 CorpId）。</summary>
    App = 1,

    /// <summary>套件指令/票据回调通道（receiveid = SuiteId）。</summary>
    Suite = 2,
}
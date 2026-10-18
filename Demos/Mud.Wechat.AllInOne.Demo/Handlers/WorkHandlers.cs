// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.AllInOne.Demo.Handlers;

/// <summary>
/// 企业微信 XML 通道的兜底记录器（信封级，<c>SupportedEventType</c> = 空串）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本 Demo 只挂兜底、不挂业务处理器</b>：共存证明要的是「报文穿透了这条线的整条接收链
/// （验签 → 时效 → 解密 → receiveid → 指纹 → 分发）」，而不是重复 DM-02 的通讯录语义。
/// 兜底键恰好是最省事的证据 —— 任何事件都能记账，事件键本身就是「线选对了」的判据。
/// </para>
/// <para>
/// <b>代价与 DM-02 相同</b>：注册兜底后分发结局恒为 <c>Handled</c>，
/// 分发器那句「未找到处理器」的 Warning 不再出现 ⇒ 漏接线只能看台账里的事件键分布。
/// </para>
/// <para>
/// <b>不读 <c>DecryptedXml</c></b>：该属性是解密后的明文 XML（含 UserId、openid 级字段），
/// 属红线字段，既不落台账也不进日志。
/// </para>
/// </remarks>
internal sealed class WorkFallbackRecorder(AllInOneLedger ledger, ILogger<WorkFallbackRecorder> logger)
    : IWechatCallbackEventHandler
{
    /// <summary>空串 = 兜底处理器（不是「支持一个叫空字符串的事件」）。</summary>
    public string SupportedEventType => string.Empty;

    public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
    {
        // ChangeType 是事件族判别字段（类别由它承载），与事件键一并记账即可定位「哪条链接住了」。
        ledger.Record("work", eventData.AppKey, eventData.EventTypeKey, $"(兜底记录：ChangeType={eventData.ChangeType ?? "-"})");
        logger.LogInformation("企微回调事件已记账：{EventKey}", eventData.EventTypeKey);

        return Task.CompletedTask;
    }
}

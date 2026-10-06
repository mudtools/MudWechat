// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「智能机器人」模块企业自建应用 SDK：官方仅向企业自建应用开放（凭证在企业侧后台配置），
/// 本域全部端点声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// <b>无令牌</b>：本接口<b>不声明 <c>[Token]</c></b> —— 端点以 URL 上的 <c>response_code</c> 为一次性凭据
/// （官方 101138），不属于 <c>access_token</c> / <c>suite_access_token</c> / <c>provider_access_token</c>
/// 任一令牌链路；故也不进入通用守卫 G5（Query 令牌注入白名单）。组件分析器据此场景发出的
/// HTTPCLIENT018（建议补 <c>[Token]</c>）为不适用告警，已在下方声明处局部 <c>#pragma</c> 豁免；
/// <c>TokenManage</c> 与父接口保持一致（全仓父接口恒为 <c>nameof(IWechatAppManager)</c>），
/// 防止生成器以 <c>new</c> 隐藏基类切换成员（HTTPCLIENT028）。
/// </para>
/// <para>
/// 应用类型开放面：官方 9 篇智能机器人文档正文零提及第三方应用 / 服务商代开发，且机器人本体与其凭证
/// 均在「企业微信管理后台 → 智能机器人 → API 设置」配置，故无第三方 / 代开发子接口。
/// </para>
/// </remarks>
// HTTPCLIENT018 豁免理由：同父接口 IWechatWorkAibotService——本域为「无令牌端点」既存例外
//（官方 101138 以 response_code 一次性凭据鉴权，守卫 AI3 锁定不得声明 [Token]），
// 分析器唯一消警路径与本域契约冲突；此处仅本接口局部豁免，不影响全仓其余接口的 018 检查。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(RegistryGroupName = "Aibot",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAibotService))]
public interface IWechatWorkInternalAibotService : IWechatWorkAibotService
{
#pragma warning restore HTTPCLIENT018
    /// <summary>
    /// 主动回复消息
    /// <para>
    /// 用户与智能机器人交互时，企微会把交互事件回调到开发者配置的回调 URL，回调中返回一个
    /// <c>response_url</c>；开发者待业务逻辑处理完后使用该 url 主动调用本接口回复消息。
    /// </para>
    /// <para>
    /// 官方支持返回 <c>response_url</c> 的场景：① 用户向智能机器人发送消息；② 用户点击模板卡片相关按钮等。
    /// </para>
    /// <para>
    /// <b>官方硬约束（不可重试）</b>：每个 <c>response_url</c> <b>仅可调用一次</b>，有效期 <b>1 小时</b>，
    /// 超过有效期无法使用。故本 SDK <b>不做</b> errcode 驱动的重试包装；失败后只能改走被动回复 /
    /// 长连接通道，errcode 直出给宿主决策。
    /// </para>
    /// <para>
    /// 官方支持的 <c>msgtype</c> 仅 <c>markdown</c> 与 <c>template_card</c>（<c>stream</c> / 媒体消息
    /// <b>不在</b>本接口支持面内，属长连接能力）；调用前请经 <c>WechatBotReplySupport</c> 校验形态。
    /// </para>
    /// </summary>
    /// <param name="responseCode">
    /// 一次性应答凭据（取自回调报文的 <c>response_url</c> 查询参数 <c>response_code</c>）。
    /// </param>
    /// <param name="message">
    /// 回复消息体（<see cref="AibotMessage"/> 可空超集；本接口仅 <c>markdown</c> / <c>template_card</c> 两个分支有效）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答结果（errcode / errmsg）；官方文档未列出响应字段，按统一响应基底承载。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101138"/></para>
    /// <para>
    /// 官方频控与业务约束：同一会话（回复 + 主动推送合计）<b>30 条/分钟、1000 条/小时</b>；
    /// 群聊中主动回复会引用触发回调的用户消息（模板卡片消息不支持引用，官方默认生成一条空消息引用）；
    /// 回复 <c>template_card</c> 时可携带 <c>template_card.feedback.id</c> 以启用用户反馈事件。
    /// </para>
    /// <para>官方 101138 <b>未提供</b>响应体说明，故返回值可能为 <c>null</c>（空响应体）。</para>
    /// </remarks>
    [Post("/cgi-bin/aibot/response")]
    Task<AibotReplyResponse?> ReplyAsync(
        [Query("response_code")] string responseCode,
        [Body] AibotMessage message,
        CancellationToken cancellationToken = default);
}

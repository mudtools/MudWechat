// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 申请接入」域 SDK（4 端点：接入申请 / 场景接入申请 / 获取接入状态 /
/// 完成接入任务；官方 minishopopencomponent2 分区 → enter 子目录「申请接入接口」，含场景接入申请）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 4 页均无第二把凭据参数、无权限集编号、无第三方代调用与云调用声明（官方本页空缺，留档不得补数）。
/// </para>
/// <para>
/// <b>本域是「账号级开通动作 + 任务申报」，全部非幂等</b>：<see cref="ApplyAccessAsync"/> 会
/// 「发送法务协议确认到管理员微信」（官方原文）；<see cref="ApplySceneAsync"/> 是场景准入申请；
/// <see cref="FinishAccessInfoAsync"/> 是向微信申报某项接入任务完成 ⇒ 三支都不得放进自动重试管道，
/// 只有 <see cref="CheckAccessAsync"/> 是只读回读。
/// </para>
/// <para>
/// <b>「未接入」与「接入未完成」是两件事</b>：<c>/shop/register/check</c> 在账户未接入时返回错误码
/// <c>1040003</c>（官方原文两处都写了这条），而已接入但任务未完成时是 <c>errcode = 0</c> + 一串 0 值位 ⇒
/// 判定顺序必须先 <c>errcode</c> 后 <c>data</c>。
/// </para>
/// <para>
/// <b>申报侧与回读侧两套字段名</b>：<see cref="FinishAccessInfoAsync"/> 用枚举整数
/// （6/7/19/8/9/20/10/11，官方编号<b>不连续</b>），<see cref="CheckAccessAsync"/> 回读
/// <c>*_success</c> / <c>*_finished</c> 具名位 ⇒ 两个方向不共用模型。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopRegisterService
{
    /// <summary>
    /// 接入申请（<c>POST /shop/register/apply</c>，无请求体；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/enter/enter_apply.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方回包即此两支）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，请求示例为 <c>{}</c>（本页连「请求参数说明」小节都没有）⇒ 本端点不声明请求 DTO；
    /// 官方本页无端点级频率数字。</para>
    /// <para><b>本端点开通的是「自定义版交易组件」本身</b>（官方正文原文「通过此接口开通自定义版交易组件，
    /// 将发送法务协议确认到管理员微信，请服务商以<b>组件开通回调 / 轮询开通状态</b>作为开通判断」）⇒
    /// <c>errcode = 0</c> 只表示受理，<b>不代表已开通</b>；结论要接「账户接入回调」或轮询 <see cref="CheckAccessAsync"/>。</para>
    /// <para><b>与标准版组件互斥</b>（官方原文）：「如果账户已接入标准版组件，则无法开通，
    /// 请到微信公众平台取消标准组件的开通」⇒ 该失败是运营侧动作，SDK 无法自愈。</para>
    /// <para><b><c>action_type</c> 已废弃</b>（官方原文「action_type 已经废弃，自定义版交易组件不再区分校验打开状态」）
    /// ⇒ 本仓请求侧不提供该键，宿主不得自行补回。</para>
    /// </remarks>
    [Post("/shop/register/apply")]
    Task<WxaResponse> ApplyAccessAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 场景接入申请（<c>POST /shop/register/apply_scene</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/enter/scene_apply.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有场景组 id 一支，必填），见 <see cref="WxaShopSceneApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b>请求键名官方两处不一致</b>：请求示例是 <c>scene_group_id</c>、参数表列名是 <c>scene_group</c> ⇒
    /// 本仓按<b>请求示例</b>落地 <c>scene_group_id</c>（守卫 SHOPA5 锁定该选择），差异在
    /// <see cref="WxaShopSceneApplyRequest"/> 留档。</para>
    /// <para><b>申请结果不在应答里</b>：本端点只回 <c>ok</c>，场景审核状态须由 <see cref="CheckAccessAsync"/> 的
    /// <c>data.scene_group_list[]</c> 回读（含 <c>reason</c> 驳回理由）。</para>
    /// <para><b>官方取值面本页只写 <c>1</c>: 视频号</b>，而两支提审端点的 <c>scene_group_list</c> 另列 <c>3</c>: 订单中心
    /// ⇒ 本页不因此扩值。</para>
    /// </remarks>
    [Post("/shop/register/apply_scene")]
    Task<WxaResponse> ApplySceneAsync(
        [Body] WxaShopSceneApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取接入状态（<c>POST /shop/register/check</c>，无请求体；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/enter/enter_check.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接入任务位与场景审核列表（<c>data</c> 对象包装），见 <see cref="WxaShopAccessCheckResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，「请求参数说明」为<b>无</b>（示例体 <c>{}</c>）⇒ 不声明请求 DTO；官方本页无端点级频率数字。</para>
    /// <para><b>未接入是错误码 <c>1040003</c></b>（官方原文两处：页首「如果账户未接入，将返回错误码 1040003」
    /// 与回包表「错误码，未开通将返回 1040003（该小程序还没接入）」）⇒ 本端点<b>不能</b>当作「是否已接入」的布尔探针，
    /// 须按码分支。</para>
    /// <para><b>11 支 <c>access_info</c> 位分成「success（真做过）」与「finished（已申报）」两套</b>⇒
    /// 只有 success 位不足以让微信认为接入完成，还须 <see cref="FinishAccessInfoAsync"/> 逐项申报。</para>
    /// <para><b>场景状态有官方明示的历史脏值</b>（原文「由于历史原因可能出现 0 和 1 的 status，视为已接入即可，
    /// 后续会自动流转到 2」）⇒ 该层的「已接入」判据不得只认 <c>1</c>，且与本域 <c>0/1/9</c> 审核表不同源。</para>
    /// </remarks>
    [Post("/shop/register/check")]
    Task<WxaShopAccessCheckResponse> CheckAccessAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成接入任务（<c>POST /shop/register/finish_access_info</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/enter/finish_access_info.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>access_info_item</c> 一支，必填），见 <see cref="WxaShopFinishAccessRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b>本端点是「申报」而非「校验」</b>：官方不提供任何前置校验说明，一次调用只标记一个任务项 ⇒
    /// 八支枚举值须逐项调用（编号 <c>6 / 7 / 19 / 8 / 9 / 20 / 10 / 11</c> <b>不连续</b>，
    /// 二级商户号分支占用 19、20，守卫 SHOPA6 锁定不得按连续序号生成）。</para>
    /// <para><b>申报后回读位是 <c>*_finished</c></b>（<see cref="CheckAccessAsync"/>），而非 <c>*_success</c> ⇒
    /// 用 success 位判断申报是否生效会得到错误结论。</para>
    /// </remarks>
    [Post("/shop/register/finish_access_info")]
    Task<WxaResponse> FinishAccessInfoAsync(
        [Body] WxaShopFinishAccessRequest request,
        CancellationToken cancellationToken = default);
}

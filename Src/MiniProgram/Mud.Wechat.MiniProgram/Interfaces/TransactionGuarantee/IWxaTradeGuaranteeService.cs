// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「交易保障 · 基础」域 SDK（2 端点：交易保障标状态 + 交易体验分违规记录，
/// 官方「交易保障」分组 → 基础接口）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域归属（官方目录 2026-10-13 逐页核验）</b>：本域 2 端点位于官方服务端 API 的
/// <c>transaction-guarantee/basic</c> 章节，路由前缀 <c>/wxaapi/wxamptrade/</c>，
/// 适用范围「小程序」账号类型 ⇒ 与同分区的「评价管理」（<c>/wxaapi/comment/*</c>，9 端点）
/// 与「投诉管理」（<c>/wxaapi/minishop/complaint*</c> 等 5 端点）合为本线 <b>TransactionGuarantee</b> 域，
/// 共用注册组 <c>TransactionGuarantee</c>；三张接口的分工见各自 XML 注释。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>，不新增令牌类型常量（新增会让 <c>MpTokenManagerRegistry</c> 单槽
/// <c>Resolve</c> 返回 <c>null</c> ⇒ errcode 令牌自愈静默失效）。
/// </para>
/// <para>
/// <b>第三方平台代调用</b>：官方逐页明示支持第三方平台代商家调用 ——
/// <c>get_guarantee_status</c> 权限集 id <b>30、151</b>（获得其中之一即可），
/// <c>get_penalty_list</c> 权限集 id <b>151</b>，均以 <c>authorizer_access_token</c> 代调用；
/// 本 SDK 只声明小程序应用级 <c>access_token</c>（Query），第三方代调用形态由宿主以小程序应用配置供给令牌。
/// </para>
/// <para>
/// <b>调用边界</b>：本域端点均为服务器端 HTTP 调用，官方未提供云调用形态。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "TransactionGuarantee", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaTradeGuaranteeService
{
    /// <summary>
    /// 获取交易保障标状态（<c>GET /wxaapi/wxamptrade/get_guarantee_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getguaranteestatus.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>保障标状态（<c>isActived</c> / <c>msg</c> / <c>reasons</c>），见 <see cref="DataModels.TransactionGuarantee.WxaTradeGuaranteeStatusResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>，无请求体、无业务 Query 参数；Query 仅携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>语义（官方两个应答示例）</b>：<c>isActived</c> 为「是否激活交易保障标」；未激活时
    /// <c>reasons</c> 逐条给出未开通原因（如「经营时长小于 3 年」「保证金余额不足」），
    /// <c>msg</c> 为状态描述（<b>已激活也可能带提示</b>，官方示例「已开通但未引入展示组件」）。
    /// </para>
    /// <para>官方错误码：<c>200002</c>（参数错误）。</para>
    /// </remarks>
    [Get("/wxaapi/wxamptrade/get_guarantee_status")]
    Task<DataModels.TransactionGuarantee.WxaTradeGuaranteeStatusResponse> GetGuaranteeStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序交易体验分违规记录（<c>GET /wxaapi/wxamptrade/get_penalty_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/basic/api_getpenaltylist.html"/>。
    /// </summary>
    /// <param name="offset">偏移量（官方原文「获取从第 offset 条开始的 limit 条记录（序号从 0 开始），最大不超过总记录数」）。</param>
    /// <param name="limit">本次读取条数（官方原文「最大不超过 100」）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>违规记录列表（<c>appealList</c> / <c>currentScore</c> / <c>totalNum</c>），见 <see cref="DataModels.TransactionGuarantee.WxaTradePenaltyListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；Query 携带 <c>access_token</c> 与 <c>offset</c>、<c>limit</c>。</para>
    /// <para>
    /// <b>GET 带「请求体」参数表的建模裁决</b>：官方本页在 GET 端点下给出「请求体 Request Payload」表
    /// （<c>offset</c> / <c>limit</c> 均必填），但官方<b>代码示例</b>实际以查询串发送
    /// （<c>?access_token=xxx&amp;offset=0&amp;limit=10</c>）⇒ 本 SDK 按示例落地为 <c>[Query]</c>，
    /// 不为 GET 声明请求体（全仓零 GET + <c>[Body]</c> 先例，声明后组件行为不可预期）。
    /// </para>
    /// <para>
    /// <b>记录状态</b>：<c>appealList[].status</c> 官方枚举 <c>2</c> 扣分审批通过 / <c>4</c> 申诉中 /
    /// <c>5</c> 申述驳回（原文如此）/ <c>6</c> 申诉成功。
    /// </para>
    /// <para>官方错误码：<c>20002</c>（参数非法）/ <c>200002</c>（参数错误）。</para>
    /// </remarks>
    [Get("/wxaapi/wxamptrade/get_penalty_list")]
    Task<DataModels.TransactionGuarantee.WxaTradePenaltyListResponse> GetPenaltyListAsync(
        [Query("offset")] long offset,
        [Query("limit")] long limit,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopFunds;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 资金流水」域 SDK（2 端点：获取资金流水列表 / 获取资金流水详情；
/// 官方「小程序支付管理服务 → 资金接口」分区的流水两支）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 域级事实（前缀两线分持、强制校验来源 IP、频率数字空缺、敏感面）见
/// <see cref="IWxaShopFundsAccountService"/> 的类型注释。
/// </para>
/// <para>
/// <b>两支的合成即官方翻页范式</b>：列表支按「记账日期 + 页码」换 <c>funds_flow_ids</c>，详情支按单个流水号换明细，
/// 官方<b>没有</b>「列表直接返回明细」的形态，也<b>没有</b>按时间区间或金额筛选的入参 ⇒ 宿主须自行逐条取详情（N+1 次调用）。
/// </para>
/// <para><b>路由名与字段名不同族（照录，不得「顺手规整」）</b>：路由为 <c>scanorderflow</c> / <c>getorderflow</c>（orderflow），字段却全是 <c>funds_flow*</c>；且列表支动词是 POST + JSON 请求体（官方正文如此，不是 GET + Query）。</para>
/// <para><b>金额单位与余额面不同</b>：本域 <c>amount</c> / <c>balance</c> 官方类型为 <c>string</c>、示例为「0.10」「115.12」（元、两位小数）⇒ 本仓照录 <c>string</c>、<b>不转 <c>decimal</c>、不做单位换算</b>，与提现族「单位：分」的 number 口径互不相通。</para>
/// <para><b>敏感面（AGENTS §8）</b>：收支金额、账户余额与业务单号（<c>request_no</c> 可为提现单号 / 订单号）属资金与个人金融信息 ⇒ <b>禁止写入日志、遥测或异常消息</b>；本域两支均为只读查询、可安全重试（官方无「仅一次」语义），但逐条取详情是 N+1 调用，须自行控制并发。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopFunds", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopFundsFlowService
{
    /// <summary>
    /// 获取资金流水列表（<c>POST /shop/funds/scanorderflow</c>，官方页面标题「获取资金流水列表」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/scanorderflow.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_num</c> / <c>page_size</c> / <c>date</c> 三支全必填），见 <see cref="WxaShopFundsFlowListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水单号列表（<c>funds_flow_ids</c>）与总数（<c>total_num</c>），见 <see cref="WxaShopFundsFlowListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；返回码 <c>-1</c> 系统异常、<c>9710001</c> 暂无数据。</para>
    /// <para><b>按「记账日期」整日查询</b>：官方 <c>date</c> 必填、格式为 <c>20210101</c> 形式的 8 位字符串（示例 <c>20211120</c>），无起止区间 ⇒ 跨日对账须宿主逐日循环。</para>
    /// <para><b><c>page_size</c> 官方明写「必须小于等于 100」</b>（本域唯一的规模上限，属参数校验而非频率限制）；<c>page_num</c> 官方未写起始值（同族提现列表写「从 1 开始」）⇒ 照录不做推断。</para>
    /// <para><b>应答只有单号</b>：明细须逐条调 <see cref="GetFundsFlowDetailAsync"/>。</para>
    /// </remarks>
    [Post("/shop/funds/scanorderflow")]
    Task<WxaShopFundsFlowListResponse> GetFundsFlowListAsync(
        [Body] WxaShopFundsFlowListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取资金流水详情（<c>POST /shop/funds/getorderflow</c>，官方页面标题「获取资金流水详情」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/getorderflow.html"/>。
    /// </summary>
    /// <param name="request">请求体（唯一字段 <c>funds_flow_no</c>，必填），见 <see cref="WxaShopFundsFlowDetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水详情（<c>funds_flow</c>，单对象非列表），见 <see cref="WxaShopFundsFlowDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；返回码 <c>-1</c> 系统异常、<c>9710001</c> 暂无数据。</para>
    /// <para>
    /// <b>请求体字段名以官方参数表为准（本页自相矛盾）</b>：参数表写 <c>funds_flow_no</c>（必填，「流水单号」），
    /// 而同页请求示例却写 <c>funds_flow_id</c>；应答侧字段名是 <c>funds_flow_id</c>（「流水id」）⇒
    /// 本仓请求取参数表 <c>funds_flow_no</c>、应答取回包 <c>funds_flow_id</c>，两字段各归其位、<b>不得</b>合并成一个（守卫 SHOPF5 锁定）。
    /// </para>
    /// <para><b>回包是单对象 <c>funds_flow</c></b>（官方建为「Object FundsFlow」，非列表）；<c>request_no</c> 是「业务单号」，其含义随 <c>busi_type</c>（订单 / 售后 / 提现）变化，官方不提供订单详情端点 ⇒ 不得据它反查订单。</para>
    /// <para><b>金额与余额为字符串</b>：<c>amount</c> / <c>balance</c> 官方类型 <c>string</c>、示例两位小数（元），<c>balance</c> 语义官方写「账户余额(可提现+待结算)」⇒ 与 <c>getbalance</c> 的两个 number 字段口径不同，不得互校。</para>
    /// </remarks>
    [Post("/shop/funds/getorderflow")]
    Task<WxaShopFundsFlowDetailResponse> GetFundsFlowDetailAsync(
        [Body] WxaShopFundsFlowDetailRequest request,
        CancellationToken cancellationToken = default);
}

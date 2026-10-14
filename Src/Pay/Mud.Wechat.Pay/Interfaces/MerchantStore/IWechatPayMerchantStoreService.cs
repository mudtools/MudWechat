// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MerchantStore;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商户门店管理」域 SDK（<b>5 端点</b>：创建 + 更新 + 查询 + 绑定收款号 + 解绑收款号）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-14 十页逐字段核验，商户面与服务商面成对）：创建
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948546"/>、更新
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085768"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948551"/>、查询
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085769"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948577"/>、绑定收款商户号
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085771"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948579"/>、解绑收款商户号
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085773"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948580"/>。</para>
/// <para><b>十个文档页 ⇒ 五个端点</b>：本域商户面与服务商面<b>路由逐字符相同</b>，字段表只差一个
/// <c>sub_mchid</c>（创建 / 更新 / 绑定 / 解绑在包体、查询在 query），应答表零差异
/// ⇒ 按「表相同则共用」归并为一套方法与 DTO（守卫 MST-B6 断言两支共用同一类型、且本域无
/// <c>Partner</c> 前缀分身）。这与 <see cref="IWechatPayMerchantRiskManageService"/> 的「同路由分建两形态」
/// 正好相反，判据是官方字段表是否相同，不是页面数量。</para>
/// <para><b>本域是支付线第一支 PATCH</b>：<b>更新门店</b>走 <c>PATCH /v3/merchant-store/stores/{store_id}</c>
/// 且成功应答是 <b>204 无包体</b> ⇒ 返回裸 <see cref="Task"/>；同域的创建 / 查询 / 绑定 / 解绑四支都是 200 + JSON。</para>
/// <para><b>「204 / 空失败列表」都不等于生效</b>：本域是<b>三方确认流</b>——收款商户号的超级管理员须在微信下发的
/// 「收款关系确认消息」里确认，地址改动还会触发核实流程。官方状态机为此设了
/// <see cref="MerchantStoreStateValues.RecipientVerifying"/> / <see cref="MerchantStoreStateValues.AddressVerifying"/>
/// / <see cref="MerchantStoreStateValues.RecipientRefused"/> 三档 ⇒ <b>唯一闭环路径是查询支读
/// <c>store_state</c></b>，把写支的成功码当部门可用会把未生效门店放进收款链路。</para>
/// <para><b><c>store_id</c> 官方是 integer</b>，本 SDK 的 path 参数按线内既有惯例声明为 <c>string</c>
/// （宿主传 <c>id.ToString()</c>）⇒ 守卫 MST-B1 锁定的是路由与参数位置，不是 CLR 类型。</para>
/// <para><b>规模与格式限制</b>（官方散文口径，SDK 不校验）：一个门店<b>最多绑定五个</b>收款商户号，
/// 门店名 / 品牌名 ≤32 字符、分店名 ≤20、<c>address_code</c> 仅数字、<c>address_detail</c> 表上 string(100)
/// 而散文要求 4-64 字符且不重复省市区、<c>business_hours</c> 最多两段以竖线分隔。</para>
/// <para><b>本域十页均未把 <c>Wechatpay-Serial</c> 列入参数表、也均未出现在示例里</b>
/// ⇒ 门店面<b>无加密字段</b>；十页亦<b>无任何端点级频率数字</b>（两轮复核一致）。</para>
/// <para><b>业务错误码</b>：十页一致只列 4 行 —— <c>PARAM_ERROR</c>(400) / <c>INVALID_REQUEST</c>(400) /
/// <c>SIGN_ERROR</c>(401) / <c>SYSTEM_ERROR</c>(500)，原样透传至
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
/// <para><b>本章不含门店列表查询与删除</b>：官方十页内<b>没有</b>「分页查门店」与「删除门店」两支 HTTP 面
/// （<c>STORE_STATE_CLOSED</c> 的删除动作官方写明是「登录商户平台删除」）⇒ 本域零该端点（守卫 MST-B2 带正对照）。</para>
/// <para><b>客户端</b>：走支付专用命名客户端 <c>wechat-pay</c>（BaseAddress 恒为
/// <c>https://api.mch.weixin.qq.com</c>）；商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，
/// <c>sub_mchid</c> 是<b>报文参数</b>而非商户作用域；本域<b>零 <c>[Token]</c></b>（守卫 PAY-B1）。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{"code","message"}</c>，
/// 加上后错误体原样落到 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/> 与
/// <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Message"/>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MerchantStore", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMerchantStoreService
{
    /// <summary>
    /// 「创建品牌门店」：一次提交门店基础 / 地址 / 经营 / 收款号四棵子树，返回微信支付分配的门店 ID。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948546"/>）。
    /// </summary>
    /// <param name="request">请求体，字段见 <see cref="MerchantStoreCreateRequest"/>（服务商调用须填 <c>sub_mchid</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只回 <c>store_id</c>，见 <see cref="MerchantStoreCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-store/stores</c>；无 path / query 参数。</para>
    /// <para><b>品牌名须先完成微信支付品牌认证</b>（官方原文），<c>store_name</c> 须与门头照一致且不含分店信息。</para>
    /// <para><b>受理 ≠ 可用</b>：本支返回 <c>store_id</c> 后，收款关系仍待对方超管确认
    /// （<c>STORE_STATE_RECIPIENT_VERIFYING</c>），须轮询 <see cref="QueryStoreAsync"/>。</para>
    /// </remarks>
    [Post("/v3/merchant-store/stores")]
    Task<MerchantStoreCreateResponse> CreateStoreAsync(
        MerchantStoreCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新品牌门店」：<b>PATCH</b> 三棵子树（基础 / 地址 / 经营），成功应答为 <b>204 No Content（无包体）</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085768"/>（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948551"/>）。
    /// </summary>
    /// <param name="storeId">微信支付门店 ID（官方 path <c>store_id</c>，官方类型 integer，必填）。</param>
    /// <param name="request">请求体，字段见 <see cref="MerchantStoreUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>无返回内容（204）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/merchant-store/stores/{store_id}</c>（支付线<b>第一支 PATCH</b>）。</para>
    /// <para><b>204 无包体 ⇒ 返回裸 <see cref="Task"/></b>，且地址改动只进入核实流程、结论须经查询支读
    /// <c>store_state</c>（守卫 MST-B1 断言返回类型）。</para>
    /// <para><b>本支请求体没有 <c>store_recipient</c> 字段</b>：收款号增删走独立的 bind / unbind 两支
    /// ⇒ 照抄创建请求类型会多出一个官方不接受的字段。</para>
    /// <para><b>三棵子树选填 = 不传即不改</b>，但传入后其内部叶子（<c>brand_name</c> / <c>store_name</c> /
    /// <c>address_code</c> / <c>address_detail</c>）官方仍标<b>必填</b> ⇒ 只传半棵树会被拒。</para>
    /// </remarks>
    [Patch("/v3/merchant-store/stores/{storeId}")]
    Task UpdateStoreAsync(
        [Path] string storeId,
        MerchantStoreUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询品牌门店」：回门店四棵子树加<b>只在本应答出现的 <c>store_state</c></b>（本域的状态闭环入口）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085769"/>（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948577"/>）。
    /// </summary>
    /// <param name="storeId">微信支付门店 ID（官方 path <c>store_id</c>，官方类型 integer，必填）。</param>
    /// <param name="subMchId">特约商户号（官方 query <c>sub_mchid</c>，选填 string(16)）：<b>仅服务商面有此参数</b>，服务商调用须传。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>门店全字段，见 <see cref="MerchantStoreQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/merchant-store/stores/{store_id}</c>。</para>
    /// <para><b>服务商面与商户面应答表零差异</b>：差异只在请求面多一个 <b>query</b> 参数（不在包体）
    /// ⇒ 两支共用本方法与同一应答类型。</para>
    /// <para><b><c>store_state</c> 只在查询应答里</b>：更新支是 204、创建支只回 <c>store_id</c>
    /// ⇒ 本支是唯一的生效确认路径，含地址核实失败原因 <c>address_failed_reason</c>。</para>
    /// </remarks>
    [Get("/v3/merchant-store/stores/{storeId}")]
    Task<MerchantStoreQueryResponse> QueryStoreAsync(
        [Path] string storeId,
        [Query("sub_mchid")] string? subMchId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「绑定品牌门店收款商户号」：向门店追加收款商户号（最多五个），应答只列失败项。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085771"/>（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948579"/>）。
    /// </summary>
    /// <param name="storeId">微信支付门店 ID（官方 path <c>store_id</c>，官方类型 integer，必填）。</param>
    /// <param name="request">请求体，字段见 <see cref="MerchantStoreRecipientBindRequest"/>（<c>store_recipient</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>失败项列表，见 <see cref="MerchantStoreRecipientOperateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-store/stores/{store_id}/recipients/bind</c>
    /// （写操作走 POST + 动词段，而不是 PATCH）。</para>
    /// <para><b>应答缺字段 = 全部受理成功</b>：官方 <c>failed_store_recipient</c> 标选填且「仅失败时返回」
    /// ⇒ 不得按「没返回即没生效」处置。</para>
    /// <para><b>绑定后仍待对方确认</b>：成功受理只是把门店推进 <c>STORE_STATE_RECIPIENT_VERIFYING</c>，
    /// 超管拒绝或超 10 天未确认会变成 <c>STORE_STATE_RECIPIENT_REFUSED</c>（门店创建失败）。</para>
    /// </remarks>
    [Post("/v3/merchant-store/stores/{storeId}/recipients/bind")]
    Task<MerchantStoreRecipientOperateResponse> BindStoreRecipientsAsync(
        [Path] string storeId,
        MerchantStoreRecipientBindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「解绑品牌门店收款商户号」：与 <see cref="BindStoreRecipientsAsync"/> 同路由族、同应答类型，仅 <c>store_recipient</c> 的必填性相反。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085773"/>（服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948580"/>）。
    /// </summary>
    /// <param name="storeId">微信支付门店 ID（官方 path <c>store_id</c>，官方类型 integer，必填）。</param>
    /// <param name="request">请求体，字段见 <see cref="MerchantStoreRecipientUnbindRequest"/>（<c>store_recipient</c> 官方标<b>选填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>与绑定支<b>同一应答类型</b>，见 <see cref="MerchantStoreRecipientOperateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/merchant-store/stores/{store_id}/recipients/unbind</c>。</para>
    /// <para><b><c>store_recipient</c> 官方标选填</b>（bind 为必填）：官方未说明「不传」的语义 ⇒
    /// SDK 不校验、不代填，也不把它当「解绑全部」的简写（守卫 MST-B4 锁定两支必填性相反）。</para>
    /// <para><b>两支应答表逐字段相同</b> ⇒ 共用 <see cref="MerchantStoreRecipientOperateResponse"/>。</para>
    /// </remarks>
    [Post("/v3/merchant-store/stores/{storeId}/recipients/unbind")]
    Task<MerchantStoreRecipientOperateResponse> UnbindStoreRecipientsAsync(
        [Path] string storeId,
        MerchantStoreRecipientUnbindRequest request,
        CancellationToken cancellationToken = default);
}

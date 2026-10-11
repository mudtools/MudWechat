// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Locallife;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「本地生活-团购券」域 SDK（6 端点：上传 code / 用户券详情 /
/// 用户券列表 / 核销 / 撤销核销 / 券账单，路由前缀 <c>/channels/ec/voucher/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>文档面与路由（视频号清单）</b>：本域端点官方文档位于视频号本地生活 API 面
/// （<c>doc/channels/api/locallife/*</c>），路由统一走 <c>/channels/ec/voucher/*</c>，
/// 适用范围为「微信小店」账号类型。
/// </para>
/// <para>
/// <b>与「营销管理-优惠券」（<see cref="IChannelsMarketingService"/>，<c>/channels/ec/coupon/*</c>）
/// 概念严格区分</b>：本域是本地生活<b>团购券</b>（到店核销凭证，code 核销 + 分账结算），
/// <b>不是</b>电商优惠券（满减 / 折扣 / 直减）。
/// </para>
/// <para>
/// <b>限频</b>：查询 / 核销 / 撤销 / 账单类接口官方限频「单商户 200QPS」。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Locallife", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsLocallifeService
{
    /// <summary>
    /// 上传团购优惠 code（<c>uploadvouchercode</c>，官方路由 <c>/channels/ec/voucher/codes/upload</c>）。
    /// </summary>
    /// <param name="request">上传请求（<c>product_id</c>/<c>sku_id</c>/<c>codes</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上传结果（成功 / 失败 code 明细与计数，失败明细 <c>fail_codes</c> 修改后可重试）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/voucher/api_uploadvouchercode"/></para>
    /// <para>
    /// 官方业务限制：①仅「商家预存 code 模式」需要（添加团购优惠时 <c>code_source_type=2</c>，
    /// 用户支付成功后平台从预存 code 库中分配）；②一次最多 200 个 code；③单个 code 不超过 48 位，
    /// 仅支持数字和字母组成。
    /// </para>
    /// <para>官方错误码（节选）：10027004（上传的 code 数量超过最大数量）/ 10027005（product_id 不存在或填写错误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/codes/upload")]
    Task<ChannelsVoucherCodesUploadResponse> UploadVoucherCodesAsync(
        [Body] ChannelsVoucherCodesUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户的团购优惠详情（<c>getvoucher</c>，官方路由 <c>/channels/ec/voucher/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>code</c> 与 <c>encrypted_code</c> 二者选其一；使用 <c>encrypted_code</c> 查询时不传 <c>sku_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户券详情（<c>voucher</c>：code / 状态 / 发放与核销时间等，状态见 <see cref="ChannelsVoucherStatuses"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_getvoucher"/></para>
    /// <para>
    /// 官方业务限制：①<b>单商户限频 200QPS</b>；②官方说明原文「code 和 encrypted_data 二者选其一」
    /// 与参数表字段名 <c>encrypted_code</c> 不一致（照录，SDK 按参数表字段名建模）；
    /// ③券状态枚举见 <see cref="ChannelsVoucherStatuses"/>（官方 2026-06-22 新增「已预约」）。
    /// </para>
    /// <para>官方错误码（节选）：10027013（查询的 code 错误）/ 10027026（code 和 encrypted_code 都未设置）/ 10027036（券不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>164</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/get")]
    Task<ChannelsGetVoucherResponse> GetVoucherAsync(
        [Body] ChannelsGetVoucherRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户的团购优惠列表（<c>findvoucher</c>，官方路由 <c>/channels/ec/voucher/get_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c>/<c>page_size</c>/<c>page_ctx</c> 必填；<c>status</c> 选填，不填查所有券）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户券列表（<c>voucher_list</c>）、用户 openid 与翻页上下文（<c>page_ctx</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_findvoucher"/></para>
    /// <para>
    /// 官方业务限制：①<b>单商户限频 200QPS</b>；②页大小限制 <c>1 ≤ page_size ≤ 50</c>；
    /// ③<c>page_ctx</c> 第一次请求填空，后续请求填上次返回值；④状态筛选值与券状态同轴
    /// （官方 findvoucher 枚举名 VOUCHER_STATUS_*，见 <see cref="ChannelsVoucherStatuses"/>）。
    /// </para>
    /// <para>官方业务限制：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>164</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/get_list")]
    Task<ChannelsGetVoucherListResponse> GetVoucherListAsync(
        [Body] ChannelsGetVoucherListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 核销用户的团购优惠（<c>consumevoucher</c>，官方路由 <c>/channels/ec/voucher/consume</c>）。
    /// </summary>
    /// <param name="request">核销请求（<c>consume_request_no</c>/<c>consume_channel</c> 必填；<c>encrypted_codes</c> 与 <c>codes+sku_id</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>核销后的券详情（<c>voucher_list</c>）与失败明细（<c>fail_voucher_list</c>，返回 10027061 时返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_consumevoucher"/></para>
    /// <para>
    /// 官方业务限制：①<b>单商户限频 200QPS</b>；②<c>consume_request_no</c> 为幂等串（商户自定义、
    /// 小店下唯一、最长 128 字节）；③加密 code 与明文 code 二选一、单次最多 10 张、整体同成功或同失败
    /// （次卡传多次相同值代表核销多次）；④系统失败时不要换单重试，应以幂等方式重试或先反查券状态；
    /// ⑤每张券最多可反复操作「核销-撤销核销」10 次；⑥官方参数表字段名为 <c>consume_store_name</c>，
    /// 请求示例写作 <c>store_name</c>（矛盾照录，按参数表建模）；⑦需先在视频号小店网页端开通核销方式
    /// （仅面向中高品牌力商家）。
    /// </para>
    /// <para>官方错误码（节选）：10027022（consume_channel 不合法）/ 10027061（券已被其他操作凭证锁定）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>164</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/consume")]
    Task<ChannelsConsumeVoucherResponse> ConsumeVoucherAsync(
        [Body] ChannelsConsumeVoucherRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销用户已核销团购优惠（<c>withdrawconsume</c>，官方路由 <c>/channels/ec/voucher/revoke</c>）。
    /// </summary>
    /// <param name="request">撤销请求（<c>revoke_request_no</c>/<c>reovke_vouchers</c> 必填；撤销核销次卡时须传 <c>consume_request_no</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>撤销后的券详情（<c>voucher_list</c>）与失败明细（<c>fail_voucher_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/cancel/api_withdrawconsume"/></para>
    /// <para>
    /// 官方业务限制：①<b>单商户限频 200QPS</b>；②核销成功后、资金结算前可撤销（资金已结算后不支持）；
    /// 撤销时券在有效期内则变「待使用」，已过有效期则变「已过期」并触发过期自动退款；
    /// ③待撤回券最多 10 张且需属于同一订单，整体同成功或同失败；
    /// ④<b>官方参数表字段名 <c>reovke_vouchers</c> 为官方历史拼写（照抄原文，勿「顺手修正」为 revoke）</b>；
    /// ⑤返回「存在券无法执行此操作」表示券处于锁定状态，可间隔 1 秒重试一次。
    /// </para>
    /// <para>官方错误码：<c>10027061</c>（存在券无法执行此操作——仅支持核销待使用券、仅支持撤销已核销券）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>164</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/revoke")]
    Task<ChannelsRevokeVoucherResponse> RevokeVoucherAsync(
        [Body] ChannelsRevokeVoucherRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询券账单（<c>getvoucherbilllist</c>，官方路由 <c>/channels/ec/voucher/get_bill_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>bill_date</c>/<c>page_size</c>/<c>page_ctx</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>券账单列表（<c>bill_list</c>：售卖/支付/优惠/结算金额（分）、核销与退款信息）与翻页上下文。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/funds/api_getvoucherbilllist"/></para>
    /// <para>
    /// 官方业务限制：①<b>单商户限频 200QPS</b>；②页大小限制 <c>1 ≤ page_size ≤ 100</c>；
    /// ③<c>page_ctx</c> 为空说明没有下一页；④金额字段单位均为<b>分</b>（次卡为单次金额）；
    /// ⑤退款账户枚举见 <see cref="ChannelsVoucherRefundAccounts"/>。
    /// </para>
    /// <para>官方业务限制：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>164</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/voucher/get_bill_list")]
    Task<ChannelsGetVoucherBillListResponse> GetVoucherBillListAsync(
        [Body] ChannelsGetVoucherBillListRequest request,
        CancellationToken cancellationToken = default);
}

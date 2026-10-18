// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopFunds;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 资金结算（余额与结算账户）」域 SDK（3 端点：查询商户余额 /
/// 查询结算账户 / 修改结算账户；官方「小程序支付管理服务 → 资金接口」分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b><c>/shop/funds/*</c> 前缀由两线分持（本域最易误读的一处）</b>：官方同一前缀下另有一批页面已被
/// 小店线 <c>Mud.Wechat.Channels</c> 的 <c>IChannelsFundsService</c> 声明（银行与地区查询 5 支 +
/// 提款二维码取图 / 查扫码状态 2 支，走小店 <c>ChannelsTokenTypes.AccessToken</c>）。本域只声明
/// 小程序令牌面的 9 条（守卫 SHOPF7 逐条锁定两线路由集不交叉），<b>不得</b>按前缀整族认领。
/// </para>
/// <para>
/// <b>官方逐页强制校验来源 IP</b>：9 页正文均在「接口调用请求说明」首行写「接口强制校验来源IP」⇒
/// 私有化 / 多出口部署须先把出口 IP 报备到小程序后台，否则得到的是 IP 校验失败而非业务错误码。
/// </para>
/// <para>
/// <b>端点级频率数字官方空缺</b>：9 页均未给「次/秒 / 次/日」类数字；唯二的量化限制是
/// <see cref="SetBankAccountAsync"/> 的「每日 5 次」与提现的「每日 1 次」，两者是<b>业务上限</b>、不得当频率解读。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：<c>account_number</c>（银行账号）、<c>bank_num</c>、<c>account_bank</c> /
/// <c>bank_name</c> 与户名类字段、<c>verify_fail_reason</c>、提现金额均属<b>个人金融信息</b> ⇒
/// <b>禁止写入日志、遥测或异常消息</b>；宿主侧持久化与截图同样受此约束。
/// </para>
/// <para>
/// <b>第三方平台代调用与权限集</b>：9 页均未标注权限集编号与第三方代调用支持面 ⇒ 本仓只按自建
/// <c>access_token</c> 面落地，注释留档该空缺，不得凭第三方 SDK 或记忆补数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopFunds", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopFundsAccountService
{
    /// <summary>
    /// 查询商户余额（<c>POST /shop/funds/getbalance</c>，官方页面标题「查询商户余额」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/getbalance.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>余额信息（<c>balance_info.available_amount</c> 可提现余额 / <c>pending_amount</c> 待结算余额），见 <see cref="WxaShopFundsBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <b>空请求体</b>（请求示例即 <c>{}</c>，参数表无请求侧字段）⇒ 本方法<b>零</b> <c>[Body]</c> 参数，不得人为造 DTO。</para>
    /// <para>官方本页无端点级频率数字；返回码表只有通用 <c>-1</c> 系统异常。</para>
    /// <para>
    /// <b>金额单位官方未标注</b>：本页两个字段类型为 <c>number</c>、示例值 45331 / 7549，正文未写「分 / 元」⇒
    /// 本仓照录 <c>long</c> 口径、<b>不做单位换算</b>；宿主若要展示金额，须以提现族官方明写「单位：分」的端点为口径基准自行判断。
    /// </para>
    /// </remarks>
    [Post("/shop/funds/getbalance")]
    Task<WxaShopFundsBalanceResponse> GetBalanceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询结算账户（<c>POST /shop/funds/getbankaccount</c>，官方页面标题「查询结算账户」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/getbankaccount.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>结算账户信息（<c>account_info</c>）与绑卡失败原因（<c>verify_fail_reason</c>），见 <see cref="WxaShopFundsBankAccountResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + <b>空请求体</b>（请求示例 <c>{}</c>）⇒ 本方法<b>零</b> <c>[Body]</c> 参数。</para>
    /// <para>
    /// <b>本页回包子字段未展开</b>：官方 <c>account_info</c> 只写「Object AccountInfo」，<b>没有</b>子字段表
    /// ⇒ 本仓的 <see cref="WxaShopFundsBankAccountInfo"/> 字段集取同前缀「修改结算账户」页的 AccountInfo 参数表
    /// （该表为官方唯一一份 AccountInfo 定义），两端点共用一支类型（守卫 SHOPF5 锁定）；
    /// 本页示例仅出现 4 字段，故 <c>bank_branch_id</c> / <c>bank_name</c> 在本应答里可缺省。
    /// </para>
    /// <para><b><c>verify_fail_reason</c> 是绑卡审核失败原因</b>（官方回包参数表）：与 <c>errcode=0</c> 并存 ⇒ 查询成功不代表账户已生效，宿主须自行判该字段。</para>
    /// <para><b>敏感面</b>：<c>account_number</c> 为完整银行账号，属个人金融信息，禁入日志、遥测与异常消息。</para>
    /// <para>官方本页无端点级频率数字；返回码表只有通用 <c>-1</c> 系统异常。</para>
    /// </remarks>
    [Post("/shop/funds/getbankaccount")]
    Task<WxaShopFundsBankAccountResponse> GetBankAccountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改结算账户（<c>POST /shop/funds/setbankaccount</c>，官方页面标题「修改结算账户」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/setbankaccount.html"/>。
    /// </summary>
    /// <param name="request">请求体（唯一字段 <c>account_info</c>，必填），见 <see cref="WxaShopFundsSetBankAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（故返回 <see cref="WxaResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；应答无业务字段。</para>
    /// <para>
    /// <b>账号级唯一值 + 覆盖式写入</b>：结算账户是商户唯一配置，重复调用即整体覆盖 ⇒ 本端点<b>不是</b>幂等新增，
    /// 宿主必须先 <see cref="GetBankAccountAsync"/> 取当前值再决定是否改写。
    /// </para>
    /// <para><b>官方量化限制（本页原文，非频率数字）</b>：「每天允许修改结算账户的次数为 5 次」，且「结算账户需要与开店主体一致」
    /// （对公为公司名称、对私为经营者本人银行卡，枚举见 <see cref="WxaShopFundsBankAccountTypes"/>）。</para>
    /// <para>
    /// <b>必填集照官方 AccountInfo 参数表</b>：<c>bank_account_type</c> / <c>account_bank</c> /
    /// <c>bank_address_code</c> / <c>account_number</c> 必填；<c>bank_branch_id</c>（联行号）与
    /// <c>bank_name</c>（开户银行全称）选填，且官方注明开户银行为「其他银行」时二者<b>需二选一</b>填写。
    /// </para>
    /// <para><b>成功应答不代表绑卡通过</b>：审核结果经 <see cref="GetBankAccountAsync"/> 的 <c>verify_fail_reason</c> 反映 ⇒ 不得把 <c>errcode=0</c> 当作落地证据。</para>
    /// <para>官方返回码：<c>-1</c> 系统异常、<c>400</c> 参数错误（官方本页即列这两个业务码）。</para>
    /// <para><b>敏感面</b>：请求体含完整银行账号与户名类字段，属个人金融信息，禁入日志、遥测与异常消息；本端点亦<b>不得</b>放进自动重试管道（每日 5 次上限 + 覆盖式语义）。</para>
    /// </remarks>
    [Post("/shop/funds/setbankaccount")]
    Task<WxaResponse> SetBankAccountAsync(
        [Body] WxaShopFundsSetBankAccountRequest request,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「卡券 · 券码核销」域 SDK（3 端点，与 <see cref="IMpCardService"/> 同注册组 <c>Card</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何独立成接口而非并入 <see cref="IMpCardService"/></b>：模板生命周期（<c>/card/{create,get,update,…}</c>）
/// 与券码操作（<c>/card/code/*</c>）在官方文档树中是<b>两个不同小节</b>，且主体不同
/// （前者针对 <c>card_id</c> 模板、后者针对 <c>code</c> 实例）⇒ 分接口让「只接核销」的宿主不必依赖建卡面。
/// 同注册组由同一条生成注册入口 <c>AddCardWebApiHttpClient()</c> 装载（形态先例：openApi 管理域双接口同组）。
/// </para>
/// <para>
/// <b>对齐基准（勿弱化）</b>：官方文档站本批实施期（2026-10-10）正文不可达 ⇒ 路由与字段面以本地 SKIT
/// <c>SKIT.FlurlHttpClient.Wechat.Api/Models/Card/Code/*.cs</c> 为对齐依据，官方 URL 只给到服务端 API 索引页
/// （<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理 → 券码管理）。
/// 逐页深链、频率上限与错误码表<b>待官方逐页核验</b>。
/// </para>
/// <para>
/// <b>扫码核销链路（SDK 只提供端点、不编排两步）</b>：用户侧扫码得到的是<b>加密券码</b> ⇒
/// 必须先 <see cref="DecryptCardCodeAsync"/>（<c>encrypt_code</c> → <c>code</c>），再
/// <see cref="GetCardCodeAsync"/> 验状态、<see cref="ConsumeCardCodeAsync"/> 核销。
/// 把两步合并成「一个方法」会隐藏一次网络往返与失败点，故<b>刻意不编排</b>。
/// </para>
/// <para>
/// <b>域级约束</b>：
/// </para>
/// <list type="bullet">
/// <item><b>3 端点全部为 POST + JSON 请求体</b>（官方数据统计类接口多为 POST，本域同样）。</item>
/// <item><b><c>card_id</c> 在核销与查询两侧均为选填</b>：官方允许仅凭 <c>code</c> 定位 ⇒ SDK 不强制。</item>
/// <item><b>核销幂等性</b>：同一 <c>code</c> 重复核销由官方返回状态类错误码表达，SDK <b>不做</b>本地去重
/// （若需 at-most-once，宿主须自备幂等键）。</item>
/// <item><b>本域不引入 api_ticket</b>：三端点均只消费应用级 <see cref="MpTokenTypes.AccessToken"/>；
/// 卡券前端取卡所需的 <c>api_ticket</c> 由 Abstractions 的票据接口承载，与本核销面无关。</item>
/// </list>
/// <para>
/// <b>未建模的券码运维面（守卫 CD8 留档）</b>：官方另有 <c>/card/code/checkcode</c>、<c>/card/code/deposit</c>、
/// <c>/card/code/getdepositcount</c>、<c>/card/code/update</c>、<c>/card/code/unavailable</c> 五条
/// （自定义码「预入库」运维链）。该链的前置是 <c>use_custom_code = true</c> 的建卡形态 + 码池批量导入，
/// 属独立运营面 ⇒ 本批不塞进同一接口，新增时须同批改 CD1/CD8 与 <c>MpRouteCountGuard</c>。
/// </para>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险，白名单由 <c>MpQueryTokenWhitelistGuard</c> QT1 锁定）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Card", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpCardCodeService
{
    /// <summary>
    /// 核销卡券。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 券码管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">核销请求（<c>code</c> 必填；<c>card_id</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>核销命中的 <c>card.card_id</c> 与持券用户 <c>openid</c>。</returns>
    /// <remarks>
    /// <para><b>入参必须是解密后的原始码</b>：加密码须先走 <see cref="DecryptCardCodeAsync"/>。</para>
    /// <para><b>应答不含 <c>code</c></b>（官方仅回 <c>card_id</c> + <c>openid</c>）⇒ 宿主需自行留存本次核销的码。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/card/code/consume")]
    Task<MpCardCodeConsumeResponse> ConsumeCardCodeAsync(
        [Body] MpCardCodeConsumeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询券码状态。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 券码管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>code</c> 必填；<c>card_id</c> 选填；<c>check_consume</c> 决定是否回带可核销性）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>券码与卡券信息、状态、持券用户及其会员维度字段（积分 / 余额 / <c>user_info</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>字段名三处陷阱</b>（守卫 CD7 锁定）：有效期键名为 <c>begin_time</c> / <c>end_time</c>
    /// （<b>非</b>建卡侧的 <c>begin_timestamp</c> / <c>end_timestamp</c>）；<c>bonus</c> / <c>balance</c>
    /// 在<b>顶层</b>（会员维度）与 <c>card</c> 内（券码维度）<b>同名且同时存在</b>，SDK 以
    /// <see cref="MpCardCodeGetResponse.MemberBonus"/> 与 <see cref="MpCardCodeInfo.Bonus"/> 分面承载。
    /// </para>
    /// <para><c>user_card_status</c> 的枚举表<b>待官方逐页核验</b> ⇒ SDK 取 <c>string</c>、不建常量、不做状态机校验。</para>
    /// </remarks>
    [Post("/card/code/get")]
    Task<MpCardCodeGetResponse> GetCardCodeAsync(
        [Body] MpCardCodeGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密券码。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 券码管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">解密请求（<c>encrypt_code</c>，来自扫码链路）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>解密后的原始券码（<c>code</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>入参键名为 <c>encrypt_code</c> 而非 <c>code</c></b>；应答只有 <c>code</c> 一个业务字段 ⇒
    /// 复用本方法所得 <c>code</c> 才能进 <see cref="ConsumeCardCodeAsync"/> / <see cref="GetCardCodeAsync"/>。
    /// </para>
    /// <para><b>解密算法不在 SDK 侧</b>：官方由服务端完成，SDK 不做本地解密、也不缓存加密码↔原始码映射。</para>
    /// </remarks>
    [Post("/card/code/decrypt")]
    Task<MpCardCodeDecryptResponse> DecryptCardCodeAsync(
        [Body] MpCardCodeDecryptRequest request,
        CancellationToken cancellationToken = default);
}

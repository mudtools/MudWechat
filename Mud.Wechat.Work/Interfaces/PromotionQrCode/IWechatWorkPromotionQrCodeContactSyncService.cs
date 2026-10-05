// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.PromotionQrCode;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「推广二维码」域「通讯录迁移」域第三方应用 SDK
/// （设置授权应用可见范围 + 设置通讯录同步完成）。
/// <para>
/// 官方仅在第三方应用开发文档树下开放本族 2 个端点，且二者消费的是
/// <b>「查询注册状态」返回的通讯录迁移 <c>access_token</c></b>（或「注册完成回调事件」的 <c>AccessToken</c>），
/// <b>不是</b>应用自身 <c>access_token</c>、<b>也不是</b>服务商 <c>provider_access_token</c>
/// ——官方两处均明文标注「请注意与 provider_access_token 的区别」，且明确不可用 provider_access_token 代替。
/// </para>
/// <para>
/// 因此本族按「显式传令牌参数」形态落位（形态对齐 <see cref="IWechatWorkProviderAuthenticationUrl"/>：
/// <b>不带 <c>[Token]</c></b>、以 <c>[Query("access_token")]</c> 显式传入），且因无 SDK 管理的令牌而
/// <b>没有父接口、没有应用类型子接口</b>——不进入契约守卫 G5（Query 令牌注入白名单）。
/// </para>
/// <para>走服务商 provider_access_token 的「获取注册码」「查询注册状态」两端点见
/// <see cref="IWechatWorkThirdPartyServicePromotionQrCodeService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// <b>SDK 刻意不缓存该通讯录迁移凭证</b>：官方契约下它有 30 分钟有效期、且被「设置通讯录同步完成」立即作废，
/// 同时具备<b>全部通讯录读写权限</b>。调用方须自行从
/// <see cref="IWechatWorkThirdPartyServicePromotionQrCodeService.GetRegisterInfoAsync"/> 的响应
/// （<c>contact_sync.access_token</c>）或「注册完成回调事件」中取得并显式传入。
/// </para>
/// <para>官方调用前提：须<b>开启通讯录迁移</b>、收到<b>授权成功通知</b>后才可调用；企业注册初始化安装应用后，
/// 应用默认可见范围为<b>根部门</b>，如需修改才调本接口。</para>
/// <para><b>危险操作约束</b>：调用「设置通讯录同步完成」后、或 <c>access_token</c> 超过 30 分钟失效
/// （即解除通讯录锁定状态）后，<b>不能继续调用本接口</b>；且「设置授权应用可见范围」的三个范围参数
/// <b>未填即清空</b>对应列表。</para>
/// <para>MUD005 相关说明：本族令牌为官方契约的 <c>access_token</c> Query 参数（MUD005 已知接受风险面），
/// 但因其非 SDK 管理的 per-注册码 凭证，一律以显式 Query 参数传入，不进入 <c>[Token]</c> 声明式注入。</para>
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "PromotionQrCode")]
public interface IWechatWorkPromotionQrCodeContactSyncService
{
    /// <summary>
    /// 设置授权应用可见范围
    /// <para>企业注册初始化安装应用后，应用默认可见范围为根部门；如需修改应用可见范围，
    /// 服务商可以调用本接口设置授权应用的可见范围。</para>
    /// <para><b>官方调用前提</b>：调用本接口前提是开启通讯录迁移、收到授权成功通知后可调用。</para>
    /// <para><b>危险操作约束</b>：allow_user / allow_party / allow_tag 三者<b>未填该字段即清空</b>对应
    /// 成员 / 部门 / 标签列表（非「保持不变」）。</para>
    /// <para><b>令牌来源受限</b>：只能使用「注册完成回调事件」返回的 <c>AccessToken</c>，或「查询注册状态」
    /// 接口返回的 <c>access_token</c>；调用「设置通讯录同步完成」后、或 <c>access_token</c> 超过 30 分钟失效
    /// （即解除通讯录锁定状态）后则不能继续调用本接口。</para>
    /// </summary>
    /// <param name="accessToken">通讯录迁移 access_token（<c>access_token</c>，Query 显式传入）：
    /// 「注册完成回调事件」返回的 <c>AccessToken</c> 或「查询注册状态」返回的 <c>access_token</c>。
    /// 官方特别提示：请注意与 <c>provider_access_token</c> 的区别，且不可用后者代替。</param>
    /// <param name="request">请求体（<see cref="SetAuthorizedAppScopeRequest"/>：agentid 授权方应用 id /
    /// allow_user 可见范围成员 / allow_party 可见范围部门 / allow_tag 可见范围标签）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非法项列表（invaliduser 非法成员 / invalidparty 非法部门 / invalidtag 非法标签）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90583"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/agent/set_scope")]
    Task<SetAuthorizedAppScopeResponse> SetAuthorizedAppScopeAsync(
        [Query("access_token")] string accessToken,
        [Body] SetAuthorizedAppScopeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置通讯录同步完成
    /// <para>该 API 用于设置通讯录同步完成，解除通讯录锁定状态，同时使通讯录迁移 <c>access_token</c> 失效。</para>
    /// <para><b>令牌来源受限</b>：只能使用「注册完成回调事件」返回的 <c>AccessToken</c>，或「查询注册状态」
    /// 接口返回的 <c>access_token</c>；官方特别提示：请注意与 <c>provider_access_token</c> 的区别。</para>
    /// <para><b>危险操作约束（不可逆）</b>：本端点官方即 <b>GET</b> 却执行状态变更
    /// （设置同步完成 + 解除通讯录锁定 + 使迁移 access_token 失效）；
    /// 调用成功后不能再调用「设置授权应用可见范围」，务必避免由重试 / 重复请求机制无条件重复调用。</para>
    /// </summary>
    /// <param name="accessToken">通讯录迁移 access_token（<c>access_token</c>，Query 显式传入）：
    /// 「注册完成回调事件」返回的 <c>AccessToken</c> 或「查询注册状态」返回的 <c>access_token</c>。
    /// 官方特别提示：请注意与 <c>provider_access_token</c> 的区别。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg（官方无业务负载，故不新建空响应 DTO）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90584"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Get("/cgi-bin/sync/contact_sync_success")]
    Task<WechatWorkResponse> SetContactSyncSuccessAsync(
        [Query("access_token")] string accessToken,
        CancellationToken cancellationToken = default);
}

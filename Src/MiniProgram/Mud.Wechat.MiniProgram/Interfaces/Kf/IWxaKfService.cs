// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「客服」域 SDK（9 端点：客服角色 2 + 客服子商户 4 + 微信客服 3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 客服消息 / 客服子商户 / 微信客服，2026-10-10 依据官方清单核验）：
/// 客服消息 <c>customer-service/</c>、客服子商户 <c>business/</c> 族、微信客服 <c>customservice/work/</c> 族。
/// </para>
/// <para>
/// <b>为何只有 9 端点（MP-X1）</b>：客服账号的「增删改/头像/邀请绑定」（<c>/customservice/kfaccount/{add,update,del,uploadheadimg,inviteworker}</c>）、
/// 客服列表（<c>/cgi-bin/customservice/getkflist</c>、<c>getonlinekflist</c>）、消息发送（<c>/cgi-bin/message/custom/send</c>）、
/// 素材（<c>/cgi-bin/media/{upload,get}</c>）均已被公众号线占据；本域仅补公众号线<b>未覆盖</b>的
/// 客服角色管理、客服子商户与微信客服（<c>work</c>）三族端点。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）；客服账号格式为
/// <c>账号前缀@公众号微信号</c>（官方原文），子商户与微信客服族以 <c>mch_id</c> / <c>kf_openid</c> 定位。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaKfService
{
    /// <summary>
    /// 设置客服账号角色（管理员）。官方路由：<c>POST /customservice/kfaccount/setadmin</c>。
    /// </summary>
    /// <param name="kfAccount">完整客服账号（<c>kfaccount</c>，必填；格式 <c>账号前缀@公众号微信号</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/customservice/kfaccount/setadmin</c> ＋ 请求体 <c>{"kfaccount":…}</c>；Query 携带 <c>access_token</c>。</para>
    /// <para><b>角色语义</b>：设置后该客服账号具备管理员权限（可配置其他客服/角色），与
    /// <see cref="CancelAdminAsync"/> 互为逆操作。公众号线未覆盖该端点（MP-X1 下仅本线提供）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/setadmin")]
    Task<WxaResponse> SetAdminAsync(
        [Body] WxaKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消客服账号角色（管理员）。官方路由：<c>POST /customservice/kfaccount/canceladmin</c>。
    /// </summary>
    /// <param name="kfAccount">完整客服账号（<c>kfaccount</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/customservice/kfaccount/canceladmin</c> ＋ 请求体 <c>{"kfaccount":…}</c>；Query 携带 <c>access_token</c>。</remarks>
    [Post("/customservice/kfaccount/canceladmin")]
    Task<WxaResponse> CancelAdminAsync(
        [Body] WxaKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册客服子商户。官方路由：<c>POST /cgi-bin/business/register</c>。
    /// </summary>
    /// <param name="request">商户号与品牌信息，见 <see cref="WxaBusinessRegisterRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/business/register</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>子商户用途（官方原文）</b>：客服消息的「子商户」形态（多品牌号共享客服能力），
    /// 注册后以 <c>mch_id</c> 作为唯一标识参与 <see cref="GetBusinessInfoAsync"/> / <see cref="ListBusinessAsync"/> 等操作。</para>
    /// </remarks>
    [Post("/cgi-bin/business/register")]
    Task<WxaResponse> RegisterBusinessAsync(
        [Body] WxaBusinessRegisterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客服子商户信息。官方路由：<c>POST /cgi-bin/business/update</c>。
    /// </summary>
    /// <param name="request">商户号与待更新品牌信息，见 <see cref="WxaBusinessUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/cgi-bin/business/update</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/business/update")]
    Task<WxaResponse> UpdateBusinessAsync(
        [Body] WxaBusinessUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服子商户信息。官方路由：<c>POST /cgi-bin/business/get</c>。
    /// </summary>
    /// <param name="request">商户号（<c>mch_id</c>），见 <see cref="WxaBusinessGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户品牌信息，见 <see cref="WxaBusinessGetResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/cgi-bin/business/get</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/business/get")]
    Task<WxaBusinessGetResponse> GetBusinessInfoAsync(
        [Body] WxaBusinessGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服子商户列表。官方路由：<c>POST /cgi-bin/business/list</c>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户品牌列表，见 <see cref="WxaBusinessListResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/cgi-bin/business/list</c>（请求体可为空对象）；Query 携带 <c>access_token</c>。</remarks>
    [Post("/cgi-bin/business/list")]
    Task<WxaBusinessListResponse> ListBusinessAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取微信客服账号信息。官方路由：<c>POST /customservice/work/get</c>。
    /// </summary>
    /// <param name="request">微信客服账号标识（<c>kf_openid</c>），见 <see cref="WxaKfWorkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信客服账号信息，见 <see cref="WxaKfWorkGetResponse"/>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/customservice/work/get</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/customservice/work/get")]
    Task<WxaKfWorkGetResponse> GetWorkAccountAsync(
        [Body] WxaKfWorkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 绑定微信客服。官方路由：<c>POST /customservice/work/bind</c>。
    /// </summary>
    /// <param name="request">微信客服账号标识，见 <see cref="WxaKfWorkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/customservice/work/bind</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/customservice/work/bind")]
    Task<WxaResponse> BindWorkAccountAsync(
        [Body] WxaKfWorkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解绑微信客服。官方路由：<c>POST /customservice/work/unbind</c>。
    /// </summary>
    /// <param name="request">微信客服账号标识，见 <see cref="WxaKfWorkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方契约：<b>POST</b> <c>/customservice/work/unbind</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</remarks>
    [Post("/customservice/work/unbind")]
    Task<WxaResponse> UnbindWorkAccountAsync(
        [Body] WxaKfWorkRequest request,
        CancellationToken cancellationToken = default);
}
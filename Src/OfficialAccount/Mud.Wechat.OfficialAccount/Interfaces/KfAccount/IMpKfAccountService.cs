// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「客服消息 → 客服管理」子分组 SDK（7 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/subscription/api/"/>
/// → 客服消息 → 客服管理（文档路径段 <c>customer/servicermanage/</c>）。深链均为逐页核验过的真实页面。
/// </para>
/// <para>
/// <b>本子分组写入 SDK 的业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>数量上限</b>：每个账号最多添加 <b>100</b> 个客服账号（超限 <c>65405</c>）。</item>
/// <item><b>账号形状</b>：<c>kf_account</c> = 「账号前缀@公众号微信号」——前缀 ≤10 字符且须为英文 / 数字 / 下划线，
/// 后缀为公众号微信号且 ≤30 字符；昵称 ≤16 字。</item>
/// <item><b>前置条件</b>：公众号<b>须先在公众平台官网设置微信号</b>才能使用客服账号能力（否则无可用后缀）。</item>
/// <item><b>账号可用性</b>：客服账号<b>尚未绑定微信号不能投入使用</b>（<c>65402</c>）⇒ 添加后须走
/// <see cref="InviteKfWorkerAsync"/> 完成绑定。</item>
/// <item><b>头像上传为 multipart</b>：文件字段名必须为 <c>media</c>（≤5M），且 <c>kf_account</c> 在
/// <b>Query</b> 而非表单里。</item>
/// <item><b>「新版客服功能」开关</b>：未开通 / 未升级时本子分组多数端点返回 <c>65400</c>（属配置态问题，非调用错误）。</item>
/// </list>
/// <para>
/// <b>账号适用性</b>：官方本子分组各页适用范围为「小程序 ✔ / 公众号 仅认证 / 服务号 仅认证 / 小游戏 ✔」；
/// 本 SDK 仅覆盖公众号与服务号形态（<b>非服务号专属</b>，故不设账号类型本地闸）。
/// </para>
/// <para>
/// <b>令牌路由</b>：7 端点均消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "KfAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpKfAccountService
{
    /// <summary>
    /// 获取所有客服账号。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_getkflist.html"/>
    /// （官方接口英文名 <c>getkflist</c>）。
    /// </summary>
    /// <param name="businessId">客服子商户的 business_id（<b>普通账号不需要填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服列表（<c>kf_account</c>/<c>kf_nick</c>/<c>kf_headimgurl</c>/<c>kf_id</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET 且无请求体</b>（本页 <c>kf_id</c> 为<b>字符串</b>，与在线列表的数值形态不同）。</para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/customservice/getkflist")]
    Task<MpGetKfListResponse> GetKfListAsync(
        [Query("business_id")] string? businessId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取在线客服列表。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_getonlinekflist.html"/>
    /// （官方接口英文名 <c>getonlinekflist</c>）。
    /// </summary>
    /// <param name="businessId">客服子商户的 business_id（<b>普通账号不需要填</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>在线客服列表（含 <c>status</c>/<c>accepted_case</c>/<c>kf_openid</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET 且无请求体</b>；<c>status</c> 取值见 <c>MpKfOnlineStatus</c>（<c>0</c> 不在线 / <c>1</c> web 在线）。</para>
    /// <para><c>accepted_case</c> 为客服当前正在接待的会话数 ⇒ 可用于「按负载分配客服」。</para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/customservice/getonlinekflist")]
    Task<MpGetOnlineKfListResponse> GetOnlineKfListAsync(
        [Query("business_id")] string? businessId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加客服账号。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_addkfaccount.html"/>
    /// （官方接口英文名 <c>addkfaccount</c>）。
    /// </summary>
    /// <param name="request">账号信息（<c>kf_account</c> 与 <c>nickname</c> 必填；官方<b>无 password 字段</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>上限与形状</b>：每账号最多 <b>100</b> 个客服账号（<c>65405</c>）；账号前缀 ≤10 字符（英文 / 数字 / 下划线）、
    /// 后缀（公众号微信号）≤30 字符；昵称 ≤16 字。
    /// </para>
    /// <para><b>前置条件</b>：公众号须先在公众平台官网<b>设置微信号</b>后才能使用该能力。</para>
    /// <para>
    /// 官方错误码：<c>0</c> / <c>40003</c> / <c>40005</c> / <c>40009</c> / <c>65400</c> / <c>65401</c> /
    /// <c>65402</c> / <c>65403</c> / <c>65404</c> / <c>65405</c> / <c>65406</c>。
    /// </para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/add")]
    Task<MpResponse> AddKfAccountAsync(
        [Body] MpAddKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改客服账号。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_updatekfaccount.html"/>
    /// （官方接口英文名 <c>updatekfaccount</c>）。
    /// </summary>
    /// <param name="request">账号与昵称（<b>官方无 <c>business_id</c></b>，与添加页形状不同）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方错误码：<c>0</c> / <c>65400</c> / <c>65401</c>（无效客服账号）/ <c>65403</c>（昵称不合法）。</para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/update")]
    Task<MpResponse> UpdateKfAccountAsync(
        [Body] MpUpdateKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客服账号。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_delkfaccount.html"/>
    /// （官方接口英文名 <c>delkfaccount</c>）。
    /// </summary>
    /// <param name="request">待删除账号（<c>kf_account</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 删除后该客服账号不再可用；官方<b>未声明</b>「删除是否会解除已有会话 / 标签」的联动语义
    /// ⇒ SDK 不推断，调用方如需保证一致性应在删除前用会话控制端点收尾。
    /// </para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/del")]
    Task<MpResponse> DelKfAccountAsync(
        [Body] MpDelKfAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置客服头像。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_uploadkfheadimg.html"/>
    /// （官方接口英文名 <c>uploadkfheadimg</c>）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（<b>文件字段名必须为 <c>media</c></b>，文件 ≤5M；
    /// 由调用方构建 <see cref="IFormContent"/> 实现，须标 <c>[MultipartForm]</c>）。</param>
    /// <param name="kfAccount">完整客服账号（<b>官方置于 Query</b>，非表单字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>参数位置易错点</b>：<c>kf_account</c> 在官方契约里是 <b>Query</b> 参数
    /// （<c>…/uploadheadimg?access_token=…&amp;kf_account=KF_ACCOUNT</c>），文件字段名为 <c>media</c>。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40005</c>（invalid file type，文件格式不对）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/uploadheadimg")]
    Task<MpResponse> UploadKfHeadImgAsync(
        [MultipartForm] IFormContent formData,
        [Query("kf_account")] string kfAccount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 邀请绑定客服账号。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/customer/servicermanage/api_invitekfworker.html"/>
    /// （官方接口英文名 <c>invitekfworker</c>）。
    /// </summary>
    /// <param name="request">邀请信息（<c>kf_account</c> + <c>invite_wx</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>业务位置</b>：客服账号「尚未绑定微信号不能投入使用」（<c>65402</c>）⇒ 添加账号后必须经本接口
    /// 邀请微信号完成绑定，或用该微信号在公众平台侧确认邀请。
    /// </para>
    /// <para>
    /// 官方错误码（全为本业务专有码）：<c>65407</c>（已是本账号客服）/ <c>65408</c>（已发送邀请）/
    /// <c>65409</c>（无效微信号）/ <c>65410</c>（绑定数量超限）/ <c>65411</c>（存在待确认邀请）/
    /// <c>65412</c>（该客服账号已绑定微信号）。
    /// </para>
    /// <para>官方「注意事项」原文为「本接口无特殊注意事项」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/customservice/kfaccount/inviteworker")]
    Task<MpResponse> InviteKfWorkerAsync(
        [Body] MpInviteKfWorkerRequest request,
        CancellationToken cancellationToken = default);
}

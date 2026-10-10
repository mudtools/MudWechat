// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 公众号接口调用凭据签发接口（令牌管理器直调）。
/// </summary>
/// <remarks>
/// <para>
/// 公共约定（对齐企微 <c>IWechatWorkInternalAppAuthentication</c> 与 <c>IFeishuAuthentication</c>）：
/// 令牌签发接口位于 Abstractions、<b>不带 <c>[Token]</c></b>（令牌管理器直调，自递归约束——刷新请求
/// 自身不得进入恢复链路）、不硬编码 BaseAddress（由 per-app <c>MpAppConfig.BaseUrl</c> 经
/// <c>MpHttpClientFactory</c> 运行时解析）。
/// </para>
/// <para>
/// <b>官方明确：两个接口均不支持云调用与第三方平台调用</b>（故无从本接口签发
/// <c>component_access_token</c> / <c>authorizer_access_token</c> 的形态）。
/// </para>
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "Authentication")]
public interface IMpAuthentication
{
    /// <summary>
    /// 获取接口调用凭据（普通通道）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/base/api_getaccesstoken.html"/>
    /// </summary>
    /// <param name="appid">账号唯一凭证（AppID），对应 <see cref="Configuration.MpAppConfig.AppId"/>。</param>
    /// <param name="secret">账号密钥（AppSecret），对应 <see cref="Configuration.MpAppConfig.AppSecret"/>。</param>
    /// <param name="grantType">凭证类型，固定 <c>client_credential</c>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>令牌响应（成功时不带 <c>errcode</c>）。</returns>
    /// <remarks>
    /// <para>官方推荐改用稳定版接口（<see cref="GetStableTokenAsync"/>）；两个通道的凭据<b>互相隔离</b>。</para>
    /// <para>成功响应：<c>access_token</c> + <c>expires_in</c>（7200 秒之内的值）。</para>
    /// <para>
    /// 错误码：<c>-1</c> / <c>40001</c>（AppSecret 错误）/ <c>40002</c> / <c>40013</c> / <c>40125</c> /
    /// <c>40164</c> / <c>40243</c>（AppSecret 已冻结）/ <c>41004</c> / <c>50004</c>（禁止使用 token 接口）/
    /// <c>50007</c>（账号已冻结）。
    /// </para>
    /// </remarks>
    [Get("/cgi-bin/token")]
    Task<MpGetTokenResponse?> GetTokenAsync(
        [Query("appid")] string appid,
        [Query("secret")] string secret,
        [Query("grant_type")] string grantType = "client_credential",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取稳定版接口调用凭据（官方推荐）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/base/api_getstableaccesstoken.html"/>
    /// </summary>
    /// <param name="request">请求体（<c>grant_type</c> / <c>appid</c> / <c>secret</c> / <c>force_refresh</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>令牌响应（普通模式复用旧 token 时 <c>expires_in</c> 为剩余秒数）。</returns>
    /// <remarks>
    /// <para><b>仅支持 POST</b>（非 POST 返回 <c>43002</c>）；<b>与 <c>getAccessToken</c> 的凭据完全隔离，互不影响</b>。</para>
    /// <para>官方注意事项：调用频率限制 1 万次/分钟、每天 50 万次；<c>access_token</c> 存储空间至少保留 512 字符；
    /// <b>强制刷新模式每天限用 20 次且需间隔 30 秒</b>；<b>普通模式下平台会提前 5 分钟更新 <c>access_token</c></b>。</para>
    /// <para>
    /// 错误码：<c>-1</c> / <c>0</c> / <c>40002</c> / <c>40013</c> / <c>40125</c> / <c>40164</c> / <c>41002</c> /
    /// <c>41004</c> / <c>43002</c> / <c>45009</c> / <c>45011</c> / <c>89503</c> / <c>89506</c> / <c>89507</c>。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/stable_token")]
    Task<MpGetTokenResponse?> GetStableTokenAsync(
        [Body] MpStableTokenRequest request,
        CancellationToken cancellationToken = default);
}

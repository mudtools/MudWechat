// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Account;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台（第三方平台）「授权账号管理」域 SDK
/// （基本信息 / 头像 / 功能简介 / 公众号快速注册小程序——即官方「授权账号管理」分组下
/// 消费<b>授权方令牌</b>的四个端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌语义（官方契约，已逐端点核验）</b>：本组端点管理「授权账号」自身资料，
/// 令牌为<b>授权方令牌</b>（query 参数名 <c>access_token</c>，值 <c>authorizer_access_token</c>）。
/// 调用前须经 <see cref="IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)"/>
/// 进入目标授权方作用域，否则运行期 fail-fast。
/// </para>
/// <para>
/// MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
/// URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <see cref="WechatOpenPlatformException"/> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Account", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatOpenPlatformAccountService
{
    /// <summary>
    /// 获取授权账号的基本信息。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账号资料（主体 / 认证 / 昵称头像简介的修改额度等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getAccountBasicInfo.html"/></para>
    /// <para>官方契约：<b>本域唯一 GET 端点</b>、无请求体（操作对象即当前授权方上下文自身）。</para>
    /// </remarks>
    [Get("/cgi-bin/account/getaccountbasicinfo")]
    Task<OpenPlatformAccountBasicInfoResponse> GetAccountBasicInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改授权账号头像。
    /// </summary>
    /// <param name="request">修改请求（临时素材 <c>media_id</c> + 裁剪坐标，均官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setHeadImage.html"/></para>
    /// <para>官方约束：头像图片须先经临时素材接口上传（<c>media_id</c> 3 天有效）；裁剪坐标取值 (0,1]；
    /// <b>本年度修改次数有上限</b>（额度见 <see cref="OpenPlatformAccountHeadImageInfo.ModifyQuota"/>），用尽即终态。</para>
    /// </remarks>
    [Post("/cgi-bin/account/modifyheadimage")]
    Task<OpenPlatformResponse> ModifyAccountHeadImageAsync(
        [Body] OpenPlatformModifyAccountHeadImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改授权账号功能简介。
    /// </summary>
    /// <param name="request">修改请求（<c>signature</c> 必填，4-120 字）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setSignature.html"/></para>
    /// <para>官方约束：<b>一个月内可申请修改 5 次</b>（额度见 <see cref="OpenPlatformAccountSignatureInfo.ModifyQuota"/>）。</para>
    /// </remarks>
    [Post("/cgi-bin/account/modifysignature")]
    Task<OpenPlatformResponse> ModifyAccountSignatureAsync(
        [Body] OpenPlatformModifyAccountSignatureRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用公众号快速注册小程序（第三方平台代注册链路的确认端点）。
    /// </summary>
    /// <param name="request">确认请求（<c>ticket</c> 必填；来自公众号快速注册任务票据）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>注册结果（新小程序 <c>appid</c> / 授权码 / 认证与绑定结果——授权码为<b>敏感凭据勿落日志</b>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-officalaccount/registerMiniprogramByOffiaccount.html"/></para>
    /// <para>
    /// 官方约束：<c>ticket</c> 由公众号侧快速注册任务产生（管理后台/公众号接口），本端点为第三方平台侧确认；
    /// 布尔结果官方以字符串 <c>"true"/"false"</c> 承载（DTO 原样字符串，不做转换）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/account/fastregister")]
    Task<OpenPlatformAccountFastRegisterResponse> FastRegisterAccountAsync(
        [Body] OpenPlatformAccountFastRegisterRequest request,
        CancellationToken cancellationToken = default);
}

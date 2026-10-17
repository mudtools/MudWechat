// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代码模板库管理」域 SDK（<b>4 端点</b>：获取草稿箱列表 / 获取模板列表 / 将草稿添加到模板库 / 删除代码模板）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域恒 <c>component_access_token</c>（Query 参数名 <c>access_token</c>），第三方平台以自身令牌操作模板库，非授权方令牌。</para>
/// <para><b>动词</b>：<c>gettemplatedraftlist</c>/<c>gettemplatelist</c> 官方为 <b>GET</b>（gettemplatelist 官方特别提示「请求方式是 get，不是 post」，误用 POST 报 <c>43001</c>）；
/// <c>addtotemplate</c>/<c>deletetemplate</c> 为 POST。</para>
/// <para><b>与 <c>MiniProgramManage</c>（授权方令牌面）区分</b>：本域操作服务商自身的代码模板库，故独立注册组 <c>MiniProgramTemplate</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramTemplate", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformTemplateService
{
    /// <summary>
    /// 获取草稿箱列表（<c>gettemplatedraftlist</c>，GET，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/getTemplatedRaftList.html"/>。
    /// </summary>
    [Get("/wxa/gettemplatedraftlist")]
    Task<OpenPlatformTemplateDraftListResponse> GetTemplateDraftListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取模板列表（<c>gettemplatelist</c>，GET；<c>template_type</c> 作 Query 过滤，不填返回全部）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/getTemplateList.html"/>。
    /// </summary>
    /// <param name="templateType">官方选填 <c>template_type</c>：0 普通模板 / 1 标准模板，<c>null</c> 表示不过滤。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    [Get("/wxa/gettemplatelist")]
    Task<OpenPlatformTemplateListResponse> GetTemplateListAsync(
        [Query("template_type")] long? templateType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将草稿添加到模板库（<c>addtotemplate</c>，POST）。模板库满报 <c>85065</c>，找不到草稿报 <c>85064</c>。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/addToTemplate.html"/>。
    /// </summary>
    [Post("/wxa/addtotemplate")]
    Task<OpenPlatformResponse> AddToTemplateAsync(
        [Body] OpenPlatformAddToTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除代码模板（<c>deletetemplate</c>，POST）。找不到模板报 <c>85064</c>。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/deleteTemplate.html"/>。
    /// </summary>
    [Post("/wxa/deletetemplate")]
    Task<OpenPlatformResponse> DeleteTemplateAsync(
        [Body] OpenPlatformDeleteTemplateRequest request,
        CancellationToken cancellationToken = default);
}
